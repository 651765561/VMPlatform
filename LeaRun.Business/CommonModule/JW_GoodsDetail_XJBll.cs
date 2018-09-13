using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using LeaRun.Utilities;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace LeaRun.Business
{
    public partial class JW_GoodsDetail_XJBll : RepositoryFactory<JW_GoodsDetail>
    {
        /// <summary>
        /// 加载从表的信息
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageGoodsJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where gm.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select gd.* from JW_GoodsMain gm join JW_GoodsDetail gd on gm.goodsmain_id=gd.goodsmain_id {0}", sqlWhere);

                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by name desc) rowNumber 
,gd.* from JW_GoodsMain gm
join JW_GoodsDetail gd on gm.goodsmain_id=gd.goodsmain_id  {4}
) as a  
where rowNumber between {0} and {1}  
order by {2} {3} "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , sqlWhere
                        );
                DataTable dt = Repository().FindTableBySql(sqlLoad);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dtAll.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                    page = jqgridparam.page, //当前页码
                    records = dtAll.Rows.Count, //总记录数
                    costtime = CommonHelper.TimerEnd(watch), //查询消耗的毫秒数
                    rows = dt
                };
                return JsonData.ToJson();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Form提交
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="submitType"></param>
        /// <param name="jwGoodsDetail"></param>
        /// <returns></returns>
        public int SubmitDetailGoodsForm(string apply_id, string goodsdetail_id, string user_id, string submitType, JW_GoodsDetail jwGoodsDetail)
        {
            //先检测主表中有没有对应从表数据
            string sqlCheckMain = string.Format(@" select * from JW_GoodsMain where apply_id='{0}' ", apply_id);
            DataTable dtCheckMain = SqlHelper.DataTable(sqlCheckMain, CommandType.Text);
            string goodsMainId = Guid.NewGuid().ToString();
            if (dtCheckMain.Rows.Count <= 0)
            {
                //主表当中没有数据
                string sqlInsertMain =
                    string.Format(@" insert into JW_GoodsMain(goodsmain_id,apply_id,getuser_id) values('{0}','{1}','{2}') ", goodsMainId,
                        apply_id, user_id);
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlInsertMain, CommandType.Text);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                goodsMainId = dtCheckMain.Rows[0]["goodsmain_id"].ToString();
            }

            if (submitType == "add")
            {
                //新增
                string sqlInsertDetail = string.Format(@" insert into JW_GoodsDetail(goodsdetail_id,goodsmain_id,name,num,treatment,adduser_id,adddate) values(@goodsdetail_id,@goodsmain_id,@name,@num,@treatment,@adduser_id,@adddate) ");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@goodsdetail_id",goodsdetail_id), 
                    new SqlParameter("@goodsmain_id",goodsMainId), 
                    new SqlParameter("@name",jwGoodsDetail.name),
                    new SqlParameter("@num",jwGoodsDetail.num),
                    new SqlParameter("@treatment",jwGoodsDetail.treatment),
                    new SqlParameter("@adduser_id",jwGoodsDetail.adduser_id),
                    new SqlParameter("@adddate",jwGoodsDetail.adddate)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlInsertDetail, CommandType.Text, pars);
                    return r;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                string sqlCheckEdit = string.Format(@"select * from JW_GoodsDetail where goodsdetail_id='{0}' and adduser_id='{1}'", goodsdetail_id, user_id);
                int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                if (count <= 0)
                {
                    return -1;
                }

                //编辑
                string sqlUpdateDetail = string.Format(@"update JW_GoodsDetail set goodsmain_id=@goodsmain_id,name=@name,num=@num,treatment=@treatment,adduser_id=@adduser_id,adddate=@adddate where goodsdetail_id=@goodsdetail_id");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@goodsmain_id",goodsMainId), 
                    new SqlParameter("@name",jwGoodsDetail.name),
                    new SqlParameter("@num",jwGoodsDetail.num),
                    new SqlParameter("@treatment",jwGoodsDetail.treatment),
                    new SqlParameter("@goodsdetail_id",goodsdetail_id),
                    new SqlParameter("@adduser_id",jwGoodsDetail.adduser_id),
                    new SqlParameter("@adddate",jwGoodsDetail.adddate) 
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlUpdateDetail, CommandType.Text, pars);
                    return r;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 加载从表Form数据
        /// </summary>
        /// <param name="goodsdetail_id"></param>
        /// <returns></returns>
        public DataTable SetGoodsDetailForm(string goodsdetail_id)
        {
            string sql = string.Format(@"select * from JW_GoodsDetail where goodsdetail_id='{0}'", goodsdetail_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 删除从表中的数据
        /// </summary>
        /// <param name="goodsdetail_id"></param>
        /// <returns></returns>
        public int DeleteGoodsDetail(string goodsdetail_id,string user_id)
        {
            //string sqlCheckEdit = string.Format(@"select * from JW_GoodsDetail where goodsdetail_id='{0}' and adduser_id='{1}'", goodsdetail_id, user_id);
            //int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return -1;
            //}

            string sqlDel = string.Format(@"delete JW_GoodsDetail where goodsdetail_id='{0}'", goodsdetail_id);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="upload_id"></param>
        /// <returns></returns>
        public string DelImgs(string upload_id, string goodsdetail_id, string user_id)
        {
            //string sqlCheckDel = string.Format(@"  select * from JW_Upload where uploaduser_id='{0}' and type=6 and Object_id='{1}' and upload_id='{2}' ", user_id, goodsdetail_id, upload_id);
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return "-1";
            //}

            string sql = string.Format(@" delete JW_Upload where upload_id='{0}' "
                , upload_id
                );
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r <= 0)
                {
                    return string.Empty;
                }
                else
                {
                    return "delSuccess";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 加载所有的图片信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string LoadImgs(string type, string goodsdetail_id)
        {
            string sql =
                string.Format(@"select * from JW_Upload where type='{0}' and Object_id='{1}' order by uploadDate"
                    , type
                    , goodsdetail_id
                    );
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt == null || dt.Rows.Count <= 0 || dt.Rows[0]["upload_id"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    //有数据
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendFormat("<li id='{0}'><a href='{1}' target='_blank'>{2}</a>&nbsp;&nbsp;" + "<a href='javascript:void(0);' title='删除' onclick='deleteOwner(\"{0}\")' name='rmlink'>删除</a></li>"
                            , row["upload_id"]
                            , row["load"]
                            , row["realName"]
                            );
                    }
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 上传文件，向数据库插入数据
        /// </summary>
        /// <param name="unit_id"></param>
        /// <param name="user_id"></param>
        /// <param name="uploadDate"></param>
        /// <param name="type"></param>
        /// <param name="object_id"></param>
        /// <param name="location"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public string Uploader(string upload_id, string unit_id, string user_id, string uploadDate, string type, string object_id,
            string location, string realName)
        {
            string sql = string.Format(@"
                    insert into JW_Upload(upload_id,unit_id,uploaduser_id,uploadDate,type,Object_id,load,realName) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')
                    "
                , upload_id
                , unit_id
                , user_id
                , uploadDate
                , type
                , object_id
                , location
                , realName
                );
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r <= 0)
                {
                    return "error";
                }
                else
                {
                    return "success";
                }
            }
            catch (Exception)
            {
                return "error";
            }
        }
    }
}

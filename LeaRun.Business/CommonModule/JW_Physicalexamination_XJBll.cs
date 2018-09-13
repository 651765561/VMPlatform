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
    public partial class JW_Physicalexamination_XJBll : RepositoryFactory<JW_Physicalexamination>
    {
        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPagePhysicalJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where jp.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_Physicalexamination jp {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by examDate desc) rowNumber
, jp.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName from JW_Physicalexamination jp
join Base_Unit u on jp.unit_id=u.Base_Unit_id 
join Base_PoliceArea pa on jp.PoliceArea_id=pa.PoliceArea_id
join Base_User bu on jp.adduser_id=bu.UserId  {4}
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

        /// <summary>
        /// 加载所有的图片信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string LoadImgs(string type, string exam_id)
        {
            string sql =
                string.Format(@"select * from JW_Upload where type='{0}' and Object_id='{1}' order by uploadDate"
                    , type
                    , exam_id
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
        /// 删除图片
        /// </summary>
        /// <param name="upload_id"></param>
        /// <returns></returns>
        public string DelImgs(string upload_id, string apply_detail_id, string user_id)
        {
            //start 取消互相删除的权限
            //string sqlCheckDel = string.Format(@" select * from JW_Upload where upload_id='{0}' and uploaduser_id='{1}' and type=2 and Object_id='{2}' ", upload_id, user_id, apply_detail_id);
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return "-1";
            //}
            //end 取消互相删除的权限

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
        /// 表单提交
        /// </summary>
        /// <param name="submitType"></param>
        /// <param name="jwPhysicalexamination"></param>
        /// <returns></returns>
        public int SubmitPhyForm(string submitType, JW_Physicalexamination jwPhysicalexamination)
        {
            //先获取相关信息
            string sql = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwPhysicalexamination.apply_id);
            DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
            jwPhysicalexamination.unit_id = dt.Rows[0]["unit_id"].ToString();
            jwPhysicalexamination.PoliceArea_id = dt.Rows[0]["PoliceArea_id"].ToString();
            if (submitType == "add")
            {
                //新增
                string sqlAdd = string.Format(@"insert into JW_Physicalexamination(
                                    exam_id
                                    ,unit_id
                                    ,PoliceArea_id
                                    ,adduser_id
                                    ,addDate
                                    ,apply_id
                                    ,examusername
                                    ,examDate
                                    ,examplace
                                    ,examreason
                                    ,medicalhistory
                                    ,temperature
                                    ,blood
                                    ,heartrate
                                    ,breathing
                                    ,examdetail
                                    ) values(@exam_id,@unit_id,@PoliceArea_id,@adduser_id,@addDate,@apply_id,@examusername,@examDate,@examplace,@examreason,@medicalhistory,@temperature,@blood,@heartrate,@breathing,@examdetail)"
                    );
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@exam_id",jwPhysicalexamination.exam_id),
                    new SqlParameter("@unit_id",jwPhysicalexamination.unit_id),
                    new SqlParameter("@PoliceArea_id",jwPhysicalexamination.PoliceArea_id),
                    new SqlParameter("@adduser_id",jwPhysicalexamination.adduser_id),
                    new SqlParameter("@addDate",jwPhysicalexamination.addDate==null?(object)DBNull.Value:jwPhysicalexamination.addDate),
                    new SqlParameter("@apply_id",jwPhysicalexamination.apply_id),
                    new SqlParameter("@examusername",jwPhysicalexamination.examusername),
                    new SqlParameter("@examDate",jwPhysicalexamination.examDate==null?(object)DBNull.Value:jwPhysicalexamination.examDate),
                    new SqlParameter("@examplace",jwPhysicalexamination.examplace),
                    new SqlParameter("@examreason",jwPhysicalexamination.examreason),
                    new SqlParameter("@medicalhistory",jwPhysicalexamination.medicalhistory),
                    new SqlParameter("@temperature",jwPhysicalexamination.temperature),
                    new SqlParameter("@blood",jwPhysicalexamination.blood),
                    new SqlParameter("@heartrate",jwPhysicalexamination.heartrate),
                    new SqlParameter("@breathing",jwPhysicalexamination.breathing),
                    new SqlParameter("@examdetail",jwPhysicalexamination.examdetail)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlAdd, CommandType.Text, pars);
                    return r;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                //string sqlCheckEdit = string.Format(@" select * from JW_Physicalexamination where adduser_id='{0}' and exam_id='{1}' ", jwPhysicalexamination.adduser_id, jwPhysicalexamination.exam_id);
                //int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                //if (count <= 0)
                //{
                //    return -1;
                //}

                //编辑
                string sqlEdit = string.Format(@"update JW_Physicalexamination set 
                                unit_id=@unit_id
                                ,PoliceArea_id=@PoliceArea_id
                                ,adduser_id=@adduser_id
                                ,addDate=@addDate
                                ,apply_id=@apply_id
                                ,examusername=@examusername
                                ,examDate=@examDate
                                ,examplace=@examplace
                                ,examreason=@examreason
                                ,medicalhistory=@medicalhistory
                                ,temperature=@temperature
                                ,blood=@blood
                                ,heartrate=@heartrate
                                ,breathing=@breathing
                                ,examdetail=@examdetail
                        where 
                                exam_id=@exam_id"
                    );
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@exam_id",jwPhysicalexamination.exam_id),
                    new SqlParameter("@unit_id",jwPhysicalexamination.unit_id),
                    new SqlParameter("@PoliceArea_id",jwPhysicalexamination.PoliceArea_id),
                    new SqlParameter("@adduser_id",jwPhysicalexamination.adduser_id),
                    new SqlParameter("@addDate",jwPhysicalexamination.addDate==null?(object)DBNull.Value:jwPhysicalexamination.addDate),
                    new SqlParameter("@apply_id",jwPhysicalexamination.apply_id),
                    new SqlParameter("@examusername",jwPhysicalexamination.examusername),
                    new SqlParameter("@examDate",jwPhysicalexamination.examDate==null?(object)DBNull.Value:jwPhysicalexamination.examDate),
                    new SqlParameter("@examplace",jwPhysicalexamination.examplace),
                    new SqlParameter("@examreason",jwPhysicalexamination.examreason),
                    new SqlParameter("@medicalhistory",jwPhysicalexamination.medicalhistory),
                    new SqlParameter("@temperature",jwPhysicalexamination.temperature),
                    new SqlParameter("@blood",jwPhysicalexamination.blood),
                    new SqlParameter("@heartrate",jwPhysicalexamination.heartrate),
                    new SqlParameter("@breathing",jwPhysicalexamination.breathing),
                    new SqlParameter("@examdetail",jwPhysicalexamination.examdetail)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlEdit, CommandType.Text, pars);
                    return r;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 删除列表中的数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int DeletePhy(string exam_id, string user_id)
        {
            //start 取消删除权限
            //string sqlCheckEdit = string.Format(@" select * from JW_Physicalexamination where adduser_id='{0}' and exam_id='{1}' ", user_id, exam_id);
            //int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return -1;
            //}
            //end 取消互相删除的权限

            string sql = string.Format(@" 
                        delete JW_Physicalexamination where exam_id='{0}';
                        delete JW_Upload where type='2' and Object_id='{0}' 
                        "
                , exam_id
                );
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}

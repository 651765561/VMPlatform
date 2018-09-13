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
    public partial class JW_Physicalexamination_ZDJSBll : RepositoryFactory<JW_Apply_detail>
    {
        /// <summary>
        /// 就医体检的时候，绑定房间的列表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable RoomListJson(string apply_id, string user_id, string unit_id)
        {
            string sqlCheck = string.Format(@"select br.* from Base_Room  br
join Base_RoomType brt on br.RoomType_id=brt.RoomType_id 
join JW_Apply ja on ja.PoliceArea_id=br.PoliceArea_id
where brt.Name='监居医务室' and ja.apply_id='{0}'", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sqlCheck, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

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
                    sqlWhere = string.Format(" where jar.apply_id='{0}' and jad.record_type=3 ", apply_id);
                }
                string sqlTotal = string.Format(@"
 select ROW_NUMBER() over(order by Apply_detail_id) rowNumber, jad.*,br.RoomName Apply_room_name,jar.state from JW_Apply_detail jad 
        join JW_Apply_room jar on jad.Apply_room_id=jar.apply_room_id 
        join Base_Room br on jar.Room_id=br.Room_id  {0}

", sqlWhere);

                //string sqlLoadAll = string.Format(" select * from JW_Physicalexamination jp {0}", sqlWhere);
                //DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" 
select * from ( 
       {4}
) as a  
where rowNumber between {0} and {1}  
order by {2} {3} "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , sqlTotal
                        );
                DataTable dt = Repository().FindTableBySql(sqlLoad);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                    page = jqgridparam.page, //当前页码
                    records = dt.Rows.Count, //总记录数
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
        public string Uploader(string upload_id, string unit_id, string user_id, string uploadDate, string type, string object_id, string location, string realName)
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
        public string LoadImgs(string type, string apply_detail_id)
        {
            string sql =
                string.Format(@"select * from JW_Upload where type='{0}' and Object_id='{1}' order by uploadDate"
                    , type
                    , apply_detail_id
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
        public int SubmitPhyForm(string submitType, JW_Apply_detail jwApplyDetail, string apply_id)
        {

            if (submitType == "add")
            {
                //新增
                //1.获取Apply_room_id
                string sqlApply_room_id = string.Format(@"select jar.apply_room_id from JW_Apply ja 
join JW_Apply_room jar on ja.apply_id=jar.apply_id
where ja.apply_id='{0}' and jar.state=1 and jar.enddate is NULL", apply_id);
                DataTable dtApply_room_id = SqlHelper.DataTable(sqlApply_room_id, CommandType.Text);
                string apply_room_id = dtApply_room_id.Rows[0]["apply_room_id"].ToString();

                string sqlAdd = string.Format(@"insert into JW_Apply_detail(Apply_detail_id,Apply_room_id,Room_id,outtime,intime,record_type,cmaker,detail,adduser_id,addate,examreason,medicalhistory,temperature,blood,heartrate,breathing) values(@Apply_detail_id,@Apply_room_id,@Room_id,@outtime,@intime,@record_type,@cmaker,@detail,@adduser_id,@addate,@examreason,@medicalhistory,@temperature,@blood,@heartrate,@breathing)");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@Apply_detail_id",jwApplyDetail.Apply_detail_id),
                    new SqlParameter("@Apply_room_id",apply_room_id),
                    new SqlParameter("@Room_id",jwApplyDetail.Room_id),
                    new SqlParameter("@outtime",jwApplyDetail.outtime==null?(object)DBNull.Value:jwApplyDetail.outtime),
                    new SqlParameter("@intime",jwApplyDetail.intime==null?(object)DBNull.Value:jwApplyDetail.intime),
                    new SqlParameter("@record_type",jwApplyDetail.record_type),
                    new SqlParameter("@cmaker",jwApplyDetail.cmaker),
                    new SqlParameter("@detail",jwApplyDetail.detail),
                    new SqlParameter("@adduser_id",jwApplyDetail.adduser_id),
                    new SqlParameter("@addate",jwApplyDetail.addate==null?(object)DBNull.Value:jwApplyDetail.addate),
                    new SqlParameter("@examreason",jwApplyDetail.examreason),
                    new SqlParameter("@medicalhistory",jwApplyDetail.medicalhistory),
                    new SqlParameter("@temperature",jwApplyDetail.temperature),
                    new SqlParameter("@blood",jwApplyDetail.blood),
                    new SqlParameter("@heartrate",jwApplyDetail.heartrate),
                    new SqlParameter("@breathing",jwApplyDetail.breathing)
                };

                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlAdd, CommandType.Text, pars);
                    string sqlUpdateRoom = string.Format(@"update JW_Apply_room set state=3 where apply_id='{0}' and enddate is NULL ", apply_id);
                    int r1 = SqlHelper.ExecuteNonQuery(sqlUpdateRoom, CommandType.Text);
                    return r;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                //编辑
                //1.获取Apply_room_id
                //                string sqlApply_room_id = string.Format(@"select jar.apply_room_id from JW_Apply ja 
                //join JW_Apply_room jar on ja.apply_id=jar.apply_id
                //where ja.apply_id='{0}' and jar.state=3 and jar.enddate is NULL", apply_id);
                //                DataTable dtApply_room_id = SqlHelper.DataTable(sqlApply_room_id, CommandType.Text);
                //                string apply_room_id = dtApply_room_id.Rows[0]["apply_room_id"].ToString();

                //string sqlCheckEdit = string.Format(@"select * from JW_Apply_detail where adduser_id='{0}' and Apply_detail_id='{1}'", jwApplyDetail.adduser_id, jwApplyDetail.Apply_detail_id);
                //int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                //if (count <= 0)
                //{
                //    return -1;
                //}



                string sqlEdit = string.Format(@"update JW_Apply_detail set 

--Apply_room_id=@Apply_room_id,

Room_id=@Room_id,outtime=@outtime,intime=@intime,record_type=@record_type,cmaker=@cmaker,detail=@detail,adduser_id=@adduser_id,addate=@addate,examreason=@examreason,medicalhistory=@medicalhistory,temperature=@temperature,blood=@blood,heartrate=@heartrate,breathing=@breathing where Apply_detail_id=@Apply_detail_id");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@Apply_detail_id",jwApplyDetail.Apply_detail_id),
                    //new SqlParameter("@Apply_room_id",apply_room_id),
                    new SqlParameter("@Room_id",jwApplyDetail.Room_id),
                    new SqlParameter("@outtime",jwApplyDetail.outtime==null?(object)DBNull.Value:jwApplyDetail.outtime),
                    new SqlParameter("@intime",jwApplyDetail.intime==null?(object)DBNull.Value:jwApplyDetail.intime),
                    new SqlParameter("@record_type",jwApplyDetail.record_type),
                    new SqlParameter("@cmaker",jwApplyDetail.cmaker),
                    new SqlParameter("@detail",jwApplyDetail.detail),
                    new SqlParameter("@adduser_id",jwApplyDetail.adduser_id),
                    new SqlParameter("@addate",jwApplyDetail.addate==null?(object)DBNull.Value:jwApplyDetail.addate),
                    new SqlParameter("@examreason",jwApplyDetail.examreason),
                    new SqlParameter("@medicalhistory",jwApplyDetail.medicalhistory),
                    new SqlParameter("@temperature",jwApplyDetail.temperature),
                    new SqlParameter("@blood",jwApplyDetail.blood),
                    new SqlParameter("@heartrate",jwApplyDetail.heartrate),
                    new SqlParameter("@breathing",jwApplyDetail.breathing)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlEdit, CommandType.Text, pars);
                    if (jwApplyDetail.intime != null)
                    {
                        string sqlFindApply_room_id = string.Format(@" select * from JW_Apply_detail where Apply_detail_id='{0}' ", jwApplyDetail.Apply_detail_id);
                        string apply_roomid = SqlHelper.DataTable(sqlFindApply_room_id, CommandType.Text).Rows[0]["Apply_room_id"].ToString();
                        string sqlUpdateRoom = string.Format(@"update JW_Apply_room set state=1 where apply_room_id='{0}' ", apply_roomid);
                        int r1 = SqlHelper.ExecuteNonQuery(sqlUpdateRoom, CommandType.Text);
                    }
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
        public int DeletePhy(string apply_detail_id, string user_id)
        {
            //start 取消互相删除的权限
            //string sqlCheckDel = string.Format(@"select * from JW_Apply_detail where adduser_id='{0}' and Apply_detail_id='{1}'", user_id, apply_detail_id);
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return -1;
            //}
            //end 取消互相删除的权限

            string sql = string.Format(@" 
                       delete JW_Apply_detail where Apply_detail_id='{0}' 
                        "
                , apply_detail_id
                );
            try
            {
                string sqlFindApply_room_id = string.Format(@" select * from JW_Apply_detail where Apply_detail_id='{0}' ", apply_detail_id);
                string apply_roomid = SqlHelper.DataTable(sqlFindApply_room_id, CommandType.Text).Rows[0]["Apply_room_id"].ToString();
                string sqlUpdateRoom = string.Format(@"update JW_Apply_room set state=1 where apply_room_id='{0}' ", apply_roomid);
                int r1 = SqlHelper.ExecuteNonQuery(sqlUpdateRoom, CommandType.Text);

                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// 给窗体赋值
        /// </summary>
        /// <param name="apply_detail_id"></param>
        /// <returns></returns>
        public DataTable SetApplyDetailForm(string apply_detail_id)
        {
            string sql = string.Format(@"select * from JW_Apply_detail where Apply_detail_id='{0}'", apply_detail_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// 新增的时候，判断状态
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public DataTable CheckState(string apply_id)
        {
            string sqlApply_room_id = string.Format(@"select jar.* from JW_Apply ja 
join JW_Apply_room jar on ja.apply_id=jar.apply_id
where ja.apply_id='{0}' and jar.enddate is NULL", apply_id);
            DataTable dtApply_room_id = SqlHelper.DataTable(sqlApply_room_id, CommandType.Text);
            return dtApply_room_id;
        }

        /// <summary>
        /// 加载签名
        /// </summary>
        /// <param name="exam_id"></param>
        /// <returns></returns>
        public DataTable LoadQM(string apply_detail_id)
        {
            string sql = string.Format(@"select * from JW_Qm where detail_id='{0}' and type=2", apply_detail_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }
    }
}

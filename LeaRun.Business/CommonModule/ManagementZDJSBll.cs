using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Utilities;
using System.Diagnostics;
using System.Data;
using LeaRun.Repository;
using LeaRun.Entity;
using System.Data.SqlClient;
using LeaRun.Entity.CommonModule;

namespace LeaRun.Business
{
    public partial class ManagementZDJSBll
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, string user_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal = string.Format(@"
select ROW_NUMBER() over(order by ja.apply_id) rowNumber,ja.*,js.SendDate from JW_Apply ja 
	                    join JW_OnDuty jo on ja.PoliceArea_id=jo.PoliceArea_id 
	                    join JW_SendPolice js on jo.OnDuty_id=js.Object_id
	                    where ja.state=3
	                    and js.state=1 
	                    and ja.type=3 
	                    and js.user_id='{0}'

                        union

	                    select ROW_NUMBER() over(order by jpa.apply_id) rowNumber,ja.*,jpa.usedate as SendDate from JW_Policeapply jpa 
	                    join JW_SendPolice jsp on jpa.apply_id=jsp.Object_id and  (jsp.type=4 or jsp.type=5) 
	                    join JW_apply ja on  ja.type=3 and ja.state in (3) and ((ja.apply_id=jpa.Object_id_9  and jpa.tasktype_id=9)
	                    or (ja.apply_id=jpa.Object_id_4  and jpa.tasktype_id=4))
	                    where jpa.state =3  and jsp.state in (1,2) --已确认待实施，已实施
	                    and jsp.user_id='{0}'
                ", user_id);

                //                string sql =
                //                    string.Format(
                //                        @" select * from ( 
                //                select ROW_NUMBER() over(order by ja.apply_id) rowNumber, ja.*,jsp.SendDate from JW_SendPolice jsp 
                //                join JW_Apply ja on jsp.Object_id=ja.apply_id 
                //                where 
                //                jsp.type=3              --派警表的类型是 指定居所
                //                and jsp.state=1         --派警表中的状态是 已确认待实施
                //                and jsp.user_id='{4}'   --派警表中的法警
                //                and ja.state=3          --申请表中的状态是待实施
                //                ) as a  
                //                where rowNumber between {0} and {1}  
                //                order by {2} {3} "
                //                        , (pageIndex - 1) * pageSize + 1
                //                        , pageIndex * pageSize
                //                        , jqgridparam.sidx
                //                        , jqgridparam.sord
                //                        , user_id
                //                        );

                string sql = string.Format(@"
                    select * from (
	                    {4}
                    )as a  
                    where rowNumber between {0} and {1}  
                    order by {2}  {3}  
                    "
                  , (pageIndex - 1) * pageSize + 1
                  , pageIndex * pageSize
                  , jqgridparam.sidx
                  , jqgridparam.sord
                  , sqlTotal
                 );

                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)),  //总页数
                    page = jqgridparam.page,                                                        //当前页码
                    records = dt.Rows.Count,                                                        //总记录数
                    costtime = CommonHelper.TimerEnd(watch),                                        //查询消耗的毫秒数
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
        /// 获取房间的状态
        /// </summary>
        /// <param name="unitId">当前登录用户的单位主键</param>
        /// <param name="roomType">加载除了指定的其他全部的房间类型</param>
        /// <returns></returns>
        public string GetRoomState(string user_id, string unit_id)
        {
            #region MyRegion
            //            string sql = string.Format(@"
            //                    select bpa.AreaName,br.*,a.apply_id from (
            //                        select top 1 ja.* from JW_SendPolice js
            //                        --join JW_Apply ja on js.Object_id=ja.apply_id
            //                        join JW_OnDuty jo on js.Object_id=jo.OnDuty_id 
            //                        join JW_Apply ja on ja.PoliceArea_id=jo.PoliceArea_id
            //                        where 
            //                        js.type=3				        --派警表的类型是 办案区
            //                        and js.user_id='{0}'	        --派警表中的法警
            //                        and (js.state=1 or js.state=2)--已确认待实施，已实施
            //                        order by js.SendDate
            //                        ) a 
            //                        join Base_Room br on a.PoliceArea_id=br.PoliceArea_id 
            //                        join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id 
            //                        where br.state=1
            //                                    ", user_id);



            //            try
            //            {
            //                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);      //当前办案区中的所有房间
            //                string returnHtml = string.Empty;
            //                int flag = 0;
            //                for (int i = 0; i <= dt.Rows.Count - 1; i++)
            //                {
            //                    if (flag == 0)
            //                    {
            //                        //returnHtml += "<tr>";
            //                        flag += 1;
            //                    }
            //                    else if (flag == 4)
            //                    {
            //                        //returnHtml += "</tr>";
            //                        //returnHtml += "<tr>";
            //                        flag = 1;
            //                    }
            //                    else
            //                    {
            //                        flag += 1;
            //                    }

            //                    //DataTable dtcase =
            //                    //   SqlHelper.DataTable(
            //                    //       string.Format("select * from JW_Usedetail where isend=0 and room_id='{0}'  ", dt.Rows[i]["Room_id"]),
            //                    //       CommandType.Text);

            //                    DataTable dtcase =
            //                       SqlHelper.DataTable(
            //                           string.Format(" select * from JW_Apply_room where Room_id='{0}' and enddate is null   ", dt.Rows[i]["Room_id"]),
            //                           CommandType.Text);

            //                    if (dtcase.Rows.Count > 0)
            //                    {
            //                        string apply_id = dt.Rows[i]["apply_id"].ToString();
            //                        //DataTable dtCurrentCase = SqlHelper.DataTable(string.Format(@"select * from JW_Usedetail where isend=0 and room_id='{0}' and apply_id='{1}'", dt.Rows[i]["Room_id"], dt.Rows[i]["apply_id"]), CommandType.Text);
            //                        DataTable dtCurrentCase = SqlHelper.DataTable(string.Format(@" select * from JW_Apply_room where Room_id='{0}' and enddate is null ", dt.Rows[i]["Room_id"]), CommandType.Text);
            //                        if (dtCurrentCase.Rows.Count > 0)
            //                        {
            //                            returnHtml +=
            //                            "<div  align='left'  style=\"cursor:hand;background-color:#842B00;color:white; width:120px; height:100px; float:left; margin-right:5px; margin-bottom:2px;\"><div style=\"height:20px;\"> &nbsp;" + dt.Rows[i]["RoomName"].ToString() +
            //                            "</div><div style=\"height:60px;\">办案中……</div><div><input type='button' value='点击进入房间' onclick=\"intodirect('" + apply_id + "','" + user_id + "','" + unit_id + "')\" style=\"width:120px;\" /></div>";
            //                        }
            //                        else
            //                        {
            //                            returnHtml +=
            //                            "<div  align='left'  style=\"cursor:hand;background-color:#842B00;color:white; width:120px; height:100px; float:left; margin-right:5px; margin-bottom:2px;\"><div style=\"height:20px;\"> &nbsp;" + dt.Rows[i]["RoomName"].ToString() +
            //                            "</div><div>办案中……</div>";
            //                        }
            //                    }
            //                    else
            //                    {
            //                        returnHtml += "<div  align='left' style=\"cursor:hand;background-color:Green;color:white; width:120px; height:100px; float:left; margin-right:5px; margin-bottom:2px;\"><div style=\"height:20px;\"> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div>空闲中……</div>";
            //                    }
            //                    returnHtml += "</div>";
            //                    dtcase.Dispose();
            //                }
            //                return returnHtml;
            //            }
            //            catch (Exception)
            //            {
            //                return string.Empty;
            //            } 
            #endregion


            string sql = string.Format(@"
                    select br.* from JW_SendPolice jsp
                    join JW_OnDuty jo on jsp.Object_id=jo.OnDuty_id 
                    join Base_Room br on jo.PoliceArea_id=br.PoliceArea_id
                    where jsp.type=3   
                    and jsp.user_id='{0}'
                    and jsp.state in ('1','2')
                    and br.state=1

                    union
                    
                    select br.* from  JW_Policeapply jpa 
                    join JW_SendPolice jsp on jpa.apply_id=jsp.Object_id and  jsp.type in (4,5) --4用警申请 5直接派警
                    join JW_apply ja on  ja.type=3 and ja.state in (4) and ((ja.apply_id=jpa.Object_id_9  and jpa.tasktype_id=9) 
                    or (ja.apply_id=jpa.Object_id_4  and jpa.tasktype_id=4))
                    join JW_Apply_room jar on jar.apply_id=ja.apply_id 
                    join Base_Room br on br.Room_id=jar.room_id   --从房间找，只显示能看到的房间 
                    where jpa.state =3 and jsp.state in (1,2)  --已确认待实施，已实施
                    and jsp.user_id='{0}'
                                    ", user_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);      //当前办案区中的所有房间
                string returnHtml = string.Empty;
                int flag = 0;
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (flag == 0)
                    {
                        flag += 1;
                    }
                    else if (flag == 4)
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag += 1;
                    }

                    DataTable dtcase =
                       SqlHelper.DataTable(
                           string.Format(" select * from JW_Apply_room where Room_id='{0}' and enddate is null   ", dt.Rows[i]["Room_id"]),
                           CommandType.Text);
                    if (dtcase.Rows.Count > 0)
                    {
                        returnHtml +=
                        "<div  align='left'  style=\"cursor:hand;background-color:#842B00;color:white; width:120px; height:100px; float:left; margin-right:5px; margin-bottom:2px;\"><div style=\"height:20px;\"> &nbsp;" + dt.Rows[i]["RoomName"].ToString() +
                        "</div><div style=\"height:60px;\">办案中……</div><div><input type='button' value='点击进入房间' onclick=\"intodirect('" + dtcase.Rows[0]["apply_id"] + "','" + user_id + "','" + unit_id + "')\" style=\"width:120px;\" /></div>";

                    }
                    else
                    {
                        returnHtml += "<div  align='left' style=\"cursor:hand;background-color:Green;color:white; width:120px; height:100px; float:left; margin-right:5px; margin-bottom:2px;\"><div style=\"height:20px;\"> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div>空闲中……</div>";
                    }
                    returnHtml += "</div>";
                    dtcase.Dispose();
                }
                return returnHtml;
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

        /// <summary>
        /// 加载所有的图片信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string LoadImgs(string type, string apply_id)
        {
            string sql =
                string.Format(@"select * from JW_Upload where type='{0}' and Object_id='{1}' order by uploadDate"
                    , type
                    , apply_id
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
                        sb.AppendFormat(
                            "<li id='{0}'><a href='{1}' target='_blank'><img  class='imgstyle'  src='{1}'/></a><a href='javascript:void(0);' onclick='deleteOwner(\"{0}\")' name='rmlink' title='删除'>X</a></li>"
                            , row["upload_id"]
                            , row["load"]
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
        public string DelImgs(string upload_id, string user_id)
        {
            //start 去除 不同法警互相删除照片的权限
            //string sqlCheckDel = string.Format(@" select * from JW_Upload where upload_id='{0}' and uploaduser_id='{1}' ", upload_id, user_id);
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return "-1";
            //}
            //end 去除 不同法警互相删除照片的权限

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
        /// 人员进入的时候进行房间绑定
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable RoomListJson(string apply_id, string user_id, string unit_id)
        {
            string sqlCheck = string.Format(@"select top 1 br.* from JW_Apply_room jar 
join Base_Room br on jar.room_id=br.Room_id
where jar.apply_id='{0}' and br.state=1 order by jar.addDate desc", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sqlCheck, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
            }
            catch (Exception)
            {
                return null;
            }

            string sql = string.Format(@" select br.* from JW_Apply ja
join Base_Room br on ja.PoliceArea_id=br.PoliceArea_id
where ja.apply_id='{0}' and Room_id not in (select room_id from JW_Apply_room where enddate is null) and br.state=1", apply_id);
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
        /// 2017-04-19 from su      2016 zy update(解决区院法警无法使用市院医务室、会见室的问题----现在绑定监居室所在单位的医务室、医务室)
        /// 加载当前可用的医务室
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable RoomListJsonPhyZDJS(string apply_id, string user_id, string unit_id)
        {
//            string sql = string.Format(@"
//                            select * from Base_Room br
//                            join Base_RoomType brt on br.RoomType_id=brt.RoomType_id
//                            join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id
//                            where brt.Name='监居医务室'
//                            and br.Room_id  not in (select Room_id from JW_Apply_room where state<>6)
//                            and br.Unit_id='{0}'
//                            and AreaType='6'
//            ", unit_id);
            string sql = string.Format(@"
                            select * from Base_Room br
                            join Base_RoomType brt on br.RoomType_id=brt.RoomType_id
                            join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id
                            where brt.Name='监居医务室'
                            and br.Room_id  not in (select Room_id from JW_Apply_room where state<>6)
                            and br.Unit_id in (select unit_id from base_room where  room_id  in (select room_id from jw_apply_room  where apply_id='{0}'))
                            and AreaType='6'
            ", apply_id);
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
        /// 人员进入 保存
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public int SubmitCheckInForm(JW_Apply_room jwApplyRoom, string unit_id, JW_Apply jwApply)
        {
            //先判断JW_Apply_room表中是否有该apply_id的记录
            string checksql = string.Format(@"select * from JW_Apply_room where apply_id='{0}'", jwApplyRoom.apply_id);
            try
            {
                int r = SqlHelper.DataTable(checksql, CommandType.Text).Rows.Count;
                if (r > 0)
                {
                    //有数据
                    return -1;
                }
            }
            catch (Exception)
            {
                //数据异常
                return -2;
            }

            //获取JW_Apply相关数据信息
            //string getApplySql = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwApplyRoom.apply_id);
            //string apply_unit_id = string.Empty;
            //string apply_policeArea = string.Empty;
            //try
            //{
            //    DataTable dtApply = SqlHelper.DataTable(getApplySql, CommandType.Text);
            //    if (dtApply.Rows.Count > 0)
            //    {
            //        apply_unit_id = dtApply.Rows[0]["unit_id"].ToString();
            //        apply_policeArea = dtApply.Rows[0]["PoliceArea_id"].ToString();
            //    }
            //    else
            //    {
            //        //数据异常
            //        return -2;
            //    }
            //}
            //catch (Exception)
            //{
            //    //数据异常
            //    return -2;
            //}

            //获取监居区所在单位的主键
            string sqlGetAreaUnit = string.Format(@"select bu.*,ja.PoliceArea_id from Base_Unit bu
                                    join JW_Apply ja on bu.Base_Unit_id=ja.unit_id 
                                    where ja.apply_id='{0}'", jwApplyRoom.apply_id);
            string policeAreaId = SqlHelper.DataTable(sqlGetAreaUnit, CommandType.Text).Rows[0]["PoliceArea_id"].ToString();
            string unitId = SqlHelper.DataTable(sqlGetAreaUnit, CommandType.Text).Rows[0]["Base_Unit_id"].ToString();



            //插入JW_Usedetail，更新JW_Apply状态，更新JW_SendPolice的状态
            //            string sql = string.Format(@"insert into JW_Usedetail(Usedetail_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,room_id,startdate,isend) values(NEWID(),@unit_id,@PoliceArea_id,@apply_id,@adduser_id,@addDate,@room_id,@startdate,@isend);
            //
            //update JW_Apply set state=4,fact_indate=@startdate where apply_id=@apply_id;
            //update JW_SendPolice set state=2 where type=3 and Object_id=@apply_id and user_id=@adduser_id and state=1;
            //
            //insert into JW_SafetyCheck(SafetyCheck_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,room_id,checkdate,detail)
            //values(NEWID(),@unit_id,@PoliceArea_id,@apply_id,@adduser_id,GETDATE(),@room_id,GETDATE(),'安全')
            //");
            //            SqlParameter[] pars = new SqlParameter[]
            //            {
            //                new SqlParameter("@unit_id",apply_unit_id), 
            //                new SqlParameter("@PoliceArea_id",apply_policeArea),
            //                new SqlParameter("@apply_id",jwUsedetail.apply_id),
            //                new SqlParameter("@adduser_id",jwUsedetail.adduser_id),
            //                new SqlParameter("@addDate",jwUsedetail.addDate),
            //                new SqlParameter("@room_id",jwUsedetail.room_id),
            //                new SqlParameter("@startdate",jwUsedetail.startdate==null?(object)DBNull.Value:jwUsedetail.startdate),
            //                new SqlParameter("@isend",jwUsedetail.isend)
            //            };


            //插入JW_Apply_room，更新JW_Apply状态，更新JW_SendPolice的状态

            string sql = string.Format(@"insert into JW_Apply_room(apply_room_id,unit_id,apply_id,adduser_id,adddate,Room_id,startdate,state) 
values(NEWID(),@unit_id,@apply_id,@adduser_id,@adddate,@Room_id ,@startdate,@state);
update JW_Apply set state=4,fact_indate=@startdate, xyr_name=@xyr_name, xyr_sex=@xyr_sex, xyr_sfz_id=@xyr_sfz_id, xyr_address=@xyr_address where apply_id=@apply_id;
update JW_SendPolice set state=2 where type=3 and Object_id=@apply_id and user_id=@adduser_id and state=1;
insert into JW_SafetyCheck(SafetyCheck_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,room_id,checkdate,detail) 
values(NEWID(),@unit_id,@PoliceArea_id,@apply_id,@adduser_id,GETDATE(),@Room_id,GETDATE(),'安全')
");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@unit_id",unitId), 
                new SqlParameter("@apply_id",jwApplyRoom.apply_id),
                new SqlParameter("@adduser_id",jwApplyRoom.adduser_id),
                new SqlParameter("@adddate",jwApplyRoom.adddate),
                new SqlParameter("@Room_id",jwApplyRoom.Room_id),
                new SqlParameter("@startdate",jwApplyRoom.startdate), 
                new SqlParameter("@state",jwApplyRoom.state),
                new SqlParameter("@PoliceArea_id",policeAreaId) ,
                new SqlParameter("@xyr_name",jwApply.xyr_name==null?"":jwApply.xyr_name),
                new SqlParameter("@xyr_sex",jwApply.xyr_sex==null?"":jwApply.xyr_sex),
                new SqlParameter("@xyr_sfz_id",jwApply.xyr_sfz_id==null?"":jwApply.xyr_sfz_id),
                new SqlParameter("@xyr_address",jwApply.xyr_address==null?"":jwApply.xyr_address)
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                if (r > 0)
                {
                    return r;
                }
                else
                {
                    return -2;
                }
            }
            catch (Exception)
            {
                return -2;
            }

        }

        /// <summary>
        /// 加载 人员进入 信息
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public DataTable SetCheckInForm(string apply_id)
        {
            string sql = string.Format(@"select top 1 * from JW_Apply_room where apply_id='{0}' order by addDate desc", apply_id);
            try
            {
                return SqlHelper.DataTable(sql, CommandType.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 人员离开
        /// </summary>
        /// <param name="jwSecurityCheck"></param>
        /// <returns></returns>
        public int SubmitLeaveForm(JW_Apply jwApply, string user_id, string unit_id)
        {
            string sql = string.Format(@"update JW_Apply set fact_outdate=@fact_outdate,Whereabouts=@Whereabouts,state=@state where apply_id=@apply_id;
--update JW_Usedetail set isend=1 where apply_id=@apply_id;
update JW_Apply_room set enddate=@fact_outdate , state = 6 where apply_id=@apply_id;
--update JW_SendPolice set state=3 where type=3 and Object_id=@apply_id --and user_id=@user_id
                                            ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@apply_id",jwApply.apply_id),
                new SqlParameter("@fact_outdate",jwApply.fact_outdate==null?(object)DBNull.Value:jwApply.fact_outdate),
                new SqlParameter("@Whereabouts",jwApply.Whereabouts),
                new SqlParameter("@state",jwApply.state),
                new SqlParameter("@user_id",user_id)
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 获得身份证信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetCardId(string apply_id)
        {
            string sql = string.Format(@" 
                select 
                xyr_name,xyr_sex,xyr_sfz_id,xyr_address
                from jw_apply
                WHERE apply_id='{0}' 
                "
                , apply_id
                );
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
    }
}

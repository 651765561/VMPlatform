using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Utilities;
using System.Diagnostics;
using System.Data;
using LeaRun.Repository;
using LeaRun.Entity.CommonModule;
using System.Data.SqlClient;

namespace LeaRun.Business.CommonModule
{
    public partial class SendPoliceBll
    {
        /// <summary>
        /// 加载警务派警列表
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="user_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageJson(string ParameterJson, string user_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by SendPolice_id) rowNumber, sp.*,unit.unit Unit_name,us.RealName user_name,userr.RealName SendUser_name from JW_SendPolice sp
left join Base_Unit unit on sp.Unit_id=unit.Base_Unit_id
left join Base_User us on sp.user_id=us.UserId 
left join Base_User userr on sp.SendUser_id=userr.UserId
where sp.SendUser_id='{0}' and sp.state<>3 and sp.type<>8
) as a   "
                       
                        , user_id
                        );

                string sql =
                    string.Format(
                        @" select * from ( 
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
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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
        /// 确定修改
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public string SubmitJW(JW_Apply jwApply, string sendPolice_id)
        {
            #region 审批：现在对以前的申请表不做任何的处理；此段代码已注销
            //string sql = string.Format(@"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userType=@userType,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,leaderuser_id=@leaderuser_id,leaderdate=@leaderdate,leaderdetail=@leaderdetail,state=@state where apply_id=@apply_id");

            //SqlParameter[] pars = new SqlParameter[]
            //    {
            //        new SqlParameter("@type",jwApply.type),
            //        new SqlParameter("@case_id",jwApply.case_id),
            //        new SqlParameter("@unit_id",jwApply.unit_id),
            //        new SqlParameter("@unitname",jwApply.unitname),
            //        new SqlParameter("@dep_id",jwApply.dep_id),
            //        new SqlParameter("@depname",jwApply.depname),
            //        new SqlParameter("@asker_id",jwApply.asker_id),
            //        new SqlParameter("@PoliceArea_id",jwApply.PoliceArea_id),
            //        new SqlParameter("@adduser_id",jwApply.adduser_id),
            //        new SqlParameter("@adddate",jwApply.adddate==null?(object)DBNull.Value:jwApply.adddate),
            //        new SqlParameter("@startdate",jwApply.startdate==null?(object)DBNull.Value:jwApply.startdate),
            //        new SqlParameter("@enddate",jwApply.enddate==null?(object)DBNull.Value:jwApply.enddate),
            //        new SqlParameter("@docCode",jwApply.docCode),
            //        new SqlParameter("@roomdetail",jwApply.roomdetail),
            //        new SqlParameter("@userName",jwApply.userName),
            //        new SqlParameter("@userSex",jwApply.userSex==null?(object)DBNull.Value:jwApply.userSex),
            //        new SqlParameter("@userAge",jwApply.userAge),
            //        new SqlParameter("@userCode",jwApply.userCode),
            //        new SqlParameter("@userNation",jwApply.userNation),
            //        new SqlParameter("@userType",jwApply.userType),
            //        //new SqlParameter("@userbiref",jwApply.userbiref),
            //        new SqlParameter("@userPoliticalstatus",jwApply.userPoliticalstatus),
            //        new SqlParameter("@userIsNPCmember",jwApply.userIsNPCmember),
            //        new SqlParameter("@userEducation",jwApply.userEducation),
            //        new SqlParameter("@userWork",jwApply.userWork),
            //        new SqlParameter("@userDuty",jwApply.userDuty),
            //        new SqlParameter("@userHome",jwApply.userHome),
            //        new SqlParameter("@userHealthy",jwApply.userHealthy),
            //        //new SqlParameter("@remark",jwApply.remark),
            //        //new SqlParameter("@usercardcode",jwApply.usercardcode),
            //        new SqlParameter("@detail",jwApply.detail),
            //        new SqlParameter("@ApprovalDetail",jwApply.ApprovalDetail),
            //        new SqlParameter("@fjuser_id",jwApply.fjuser_id),
            //        new SqlParameter("@fjdate",jwApply.fjdate==null?(object)DBNull.Value:jwApply.fjdate),
            //        new SqlParameter("@fjdetail",jwApply.fjdetail),
            //        new SqlParameter("@leaderuser_id",jwApply.leaderuser_id),
            //        new SqlParameter("@leaderdate",jwApply.leaderdate==null?(object)DBNull.Value:jwApply.leaderdate),
            //        new SqlParameter("@leaderdetail",jwApply.leaderdetail),
            //        new SqlParameter("@state",jwApply.state),
            //        new SqlParameter("@apply_id",jwApply.apply_id)
            //    };
            //try
            //{
            //    int r1 = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
            //}
            //catch (Exception)
            //{
            //    return "数据异常，审批失败";
            //}
            #endregion

            if (sendPolice_id != "" && sendPolice_id != "&nbsp;")
            {
                //对派警的处理

                #region 删除；此段代码已注销
                //                //1.先删除原来的数据
                //                string sqlDel = string.Format(@"
                //                    delete JW_SendPolice where Unit_id='{0}' and SendUser_id='{1}'  and type='{2}' and Object_id='{3}';
                //                    "
                //                    , jwApply.unit_id
                //                    , jwApply.adduser_id
                //                    , "1"
                //                    , jwApply.apply_id
                //                    );
                //                sqlDel = string.Format(@"
                //                                    declare @errorNumber int=0 
                //                                    begin tran 
                //                                    {0} 
                //                                    set @errorNumber+=@@ERROR 
                //                                    if(@errorNumber>0) 
                //	                                    begin 
                //		                                    rollback tran 
                //	                                    end 
                //                                    else 
                //	                                    begin 
                //		                                    commit tran 
                //	                                    end 
                //                                    "
                //                    , sqlDel.ToString()
                //                    );

                //                try
                //                {
                //                    int r2 = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                //                }
                //                catch (Exception)
                //                {
                //                    return "数据异常，派警失败";
                //                }

                //                //2.先拿到当前单位的简称
                //                string sqlGetUnit = string.Format(@" select * from Base_Unit where Base_Unit_id='{0}' ", jwApply.unit_id);
                //                string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

                //                //3.拿到数据库中当前单位的最大的流水号
                //                string code = string.Empty;
                //                string sqlGetCode =
                //                    string.Format(
                //                        @" select MAX(CONVERT(int,SUBSTRING(SendCode,6,7))) code from JW_SendPolice where Unit_id='{0}' ",
                //                        jwApply.unit_id);
                //                DataTable dtCode = SqlHelper.DataTable(sqlGetCode, CommandType.Text);
                //                if (dtCode == null || dtCode.Rows.Count <= 0 || dtCode.Rows[0]["code"].ToString() == string.Empty)
                //                {
                //                    code = DateTime.Now.Year.ToString() + "001";
                //                }
                //                else
                //                {
                //                    string year = dtCode.Rows[0]["code"].ToString().Substring(0, 4);
                //                    string co = dtCode.Rows[0]["code"].ToString().Substring(4, 3);
                //                    if (year == DateTime.Now.Year.ToString())
                //                    {
                //                        code = (Convert.ToInt32(year + co) + 1).ToString();
                //                    }
                //                    else
                //                    {
                //                        code = DateTime.Now.Year.ToString() + "001";
                //                    }
                //                }
                #endregion

                #region 添加；此段代码已注销
                //                StringBuilder sb = new StringBuilder();
                //                //再添加新的数据
                //                if (!sendPolice_id.Contains(","))
                //                {
                //                    //只选择了一个人
                //                    sb.AppendFormat(
                //                        @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                //                        , jwApply.unit_id
                //                        , unitShotName + "检警派【" + code + "】号"
                //                        , jwApply.adduser_id
                //                        , jwApply.adddate
                //                        , "1"
                //                        , jwApply.apply_id
                //                        , sendPolice_id
                //                        , "0"
                //                        );

                //                }
                //                else
                //                {
                //                    //选择了多个人
                //                    string[] ids = sendPolice_id.Split(',');

                //                    for (int i = 0; i < ids.Length; i++)
                //                    {
                //                        sb.AppendFormat(
                //                          @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                //                          , jwApply.unit_id
                //                          , unitShotName + "检警派【" + (Convert.ToInt32(code) + i) + "】号"
                //                          , jwApply.adduser_id
                //                          , jwApply.adddate
                //                          , "1"
                //                          , jwApply.apply_id
                //                          , ids[i]
                //                          , "0"
                //                          );
                //                    }
                //                }

                //                string sqlInsert = string.Format(@"
                //                                    declare @errorNumber int=0 
                //                                    begin tran 
                //                                    {0} 
                //                                    set @errorNumber+=@@ERROR 
                //                                    if(@errorNumber>0) 
                //	                                    begin 
                //		                                    rollback tran 
                //	                                    end 
                //                                    else 
                //	                                    begin 
                //		                                    commit tran 
                //	                                    end 
                //                                    "
                //                    , sb.ToString()
                //                    );

                //                try
                //                {
                //                    int r3 = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
                //                }
                //                catch (Exception)
                //                {
                //                    return "数据异常，派警失败";
                //                }
                #endregion

                #region 新的逻辑处理代码

                try
                {
                    //1.先获取数据库中已经派警的数据记录(法警的主键,派警的状态)
                    //List<string> oldPoliceList = new List<string>();
                    Dictionary<string, string> oldPoliceDic = new Dictionary<string, string>();
                    string sqlOldPolice = string.Format(@"select * from JW_SendPolice where Object_id='{0}'", jwApply.apply_id);
                    DataTable dtOldPolice = SqlHelper.DataTable(sqlOldPolice, CommandType.Text);
                    foreach (DataRow row in dtOldPolice.Rows)
                    {
                        //oldPoliceList.Add(row["user_id"] + "," + row["state"]);
                        oldPoliceDic.Add(row["user_id"].ToString(), row["state"].ToString());
                    }

                    //2.获取新传递过来的派警的数据(法警的主键)
                    List<string> newPoliceList = new List<string>();
                    if (!sendPolice_id.Contains(","))
                    {
                        newPoliceList.Add(sendPolice_id);
                    }
                    else
                    {
                        string[] ids = sendPolice_id.Split(',');
                        newPoliceList = ids.ToList();
                    }

                    //逻辑处理：新旧进行对比
                    foreach (string oldPolice in oldPoliceDic.Keys)
                    {
                        //1.数据库中的人不在新的选择的人中，需要检测当前数据库中这个人的状态
                        //如果这个人的状态是 已实施 的话，那么状态变更成 实施完成，其他的状态
                        if (!newPoliceList.Contains(oldPolice))
                        {
                            //选择的人不在数据库中，对数据库中的人进行的处理
                            string sqlProOldPolice = string.Empty;
                            if (oldPoliceDic[oldPolice].ToString() == "2") //已实施 → 已完成
                            {
                                sqlProOldPolice =
                                    string.Format(
                                        @"update JW_SendPolice set state = 3 where Object_id='{0}' and user_id='{1}' ",
                                        jwApply.apply_id, oldPolice);
                                SqlHelper.ExecuteNonQuery(sqlProOldPolice, CommandType.Text);
                            }
                            else if (oldPoliceDic[oldPolice].ToString() == "1") //已确认待实施 → 取消任务
                            {
                                sqlProOldPolice =
                                    string.Format(
                                        @"update JW_SendPolice set state = -2 where Object_id='{0}' and user_id='{1}' ",
                                        jwApply.apply_id, oldPolice);
                                SqlHelper.ExecuteNonQuery(sqlProOldPolice, CommandType.Text);
                            }
                            else if (oldPoliceDic[oldPolice].ToString() == "0") //待确认 → 删除
                            {
                                sqlProOldPolice =
                                    string.Format(@"delete JW_SendPolice where Object_id='{0}' and user_id='{1}'",
                                        jwApply.apply_id, oldPolice);
                                SqlHelper.ExecuteNonQuery(sqlProOldPolice, CommandType.Text);
                            }
                        }
                        else
                        {
                            //如果选择的人，同时也是数据库中
                            string sqlProOldPolice = string.Empty;
                            if (oldPoliceDic[oldPolice].ToString() == "3" || oldPoliceDic[oldPolice].ToString() == "-1" || oldPoliceDic[oldPolice].ToString() == "-2")      //实施完成、确认退回、取消任务 → 待确认
                            {
                                sqlProOldPolice =
                                   string.Format(
                                       @"update JW_SendPolice set state = 0 where Object_id='{0}' and user_id='{1}' ",
                                       jwApply.apply_id, oldPolice);
                                SqlHelper.ExecuteNonQuery(sqlProOldPolice, CommandType.Text);
                            }
                        }
                    }

                    foreach (string newPolice in newPoliceList)
                    {
                        //2.新选择的人不在数据库中,插入数据库中
                        if (!oldPoliceDic.Keys.Contains(newPolice))
                        {
                            #region MyRegion
                            //先拿到当前单位的简称
                            string sqlGetUnit = string.Format(@" select * from Base_Unit where Base_Unit_id='{0}' ", jwApply.unit_id);
                            string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

                            //拿到数据库中当前单位的最大的流水号
                            string code = string.Empty;
                            string sqlGetCode =
                                string.Format(
                                    @" select MAX(CONVERT(int,SUBSTRING(SendCode,6,7))) code from JW_SendPolice where Unit_id='{0}' ",
                                    jwApply.unit_id);
                            DataTable dtCode = SqlHelper.DataTable(sqlGetCode, CommandType.Text);
                            if (dtCode == null || dtCode.Rows.Count <= 0 || dtCode.Rows[0]["code"].ToString() == string.Empty)
                            {
                                code = DateTime.Now.Year.ToString() + "001";
                            }
                            else
                            {
                                string year = dtCode.Rows[0]["code"].ToString().Substring(0, 4);
                                string co = dtCode.Rows[0]["code"].ToString().Substring(4, 3);
                                if (year == DateTime.Now.Year.ToString())
                                {
                                    code = (Convert.ToInt32(year + co) + 1).ToString();
                                }
                                else
                                {
                                    code = DateTime.Now.Year.ToString() + "001";
                                }
                            }
                            #endregion

                            string sqlInsertNewPolice = string.Format(@"insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');", jwApply.unit_id, unitShotName + "检警派【" + code + "】号", jwApply.adduser_id, jwApply.adddate, jwApply.type, jwApply.apply_id, newPolice, "0");
                            SqlHelper.ExecuteNonQuery(sqlInsertNewPolice, CommandType.Text);
                        }
                    }
                    return "1";
                }
                catch (Exception)
                {
                    //失败
                    return "0";
                }

                #endregion
            }
            else
            {
                //失败
                return "0";
            }
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="sendPolice_id"></param>
        /// <returns></returns>
        public string CancelSendPolice(string sendPolice_id)
        {
            string sql = string.Format(@" update JW_SendPolice set state=-2 where SendPolice_id='{0}' ", sendPolice_id);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r > 0)
                {
                    return "取消任务成功";
                }
                else
                {
                    return "数据异常，取消任务失败";
                }
            }
            catch (Exception)
            {
                return "数据异常，取消任务失败";
            }
        }

        public string LoadSendPolice(string keyValue)
        {
            string actdate = string.Empty;
            string enddate = string.Empty;
            string actdetail = string.Empty;
            string sql = string.Format(@" select * from JW_SendPolice where  SendPolice_id='{0}' ", keyValue);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt == null || dt.Rows.Count <= 0 || dt.Rows[0]["SendPolice_id"].ToString() == string.Empty)
                {
                    return "|";
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        actdate += row["actdate"].ToString() + ",";
                        enddate += row["enddate"].ToString() + ",";
                        actdetail += row["actdetail"].ToString() + ",";
                       
                    }
                    return actdate.Substring(0, actdate.Length - 1) + "|" + enddate.Substring(0, enddate.Length - 1) + "|" + actdetail.Substring(0, actdetail.Length - 1);
                }
            }
            catch (Exception)
            {
                return "|";
            }
        }
    }
}

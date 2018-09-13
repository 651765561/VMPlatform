using LeaRun.Entity.CommonModule;
using LeaRun.Repository;
using LeaRun.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace LeaRun.Business
{
    public partial class CaseApplyListFJBll
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal = string.Format(@"

select ROW_NUMBER() over(order by apply_id) rowNumber, '办案区使用申请' applyType, ja.apply_id,ja.case_id,ja.unitname,ja.depname,ja.adddate,ja.detail,ja.state,u.RealName from JW_Apply  ja
left join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id 
left join Base_Unit bu on bpa.unit_id=bu.Base_Unit_id 
left join base_user u on  ja.adduser_id=u.userid
 where bu.Base_Unit_id='{0}' and ja.type=1 and (ja.state=1 or ja.state=2 or ja.state=3 or ja.state=4 or ja.state=-1 or ja.state=-2) 
union 
select ROW_NUMBER() over(order by apply_id) rowNumber, '刑检办案区使用申请' applyType, ja.apply_id,ja.case_id,ja.unitname,ja.depname,ja.adddate,ja.detail,ja.state,u.RealName from JW_Apply  ja
left join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id 
left join Base_Unit bu on bpa.unit_id=bu.Base_Unit_id 
left join base_user u on  ja.adduser_id=u.userid
 where bu.Base_Unit_id='{0}' and ja.type=2 and (ja.state=1 or ja.state=2 or ja.state=3 or ja.state=4 or ja.state=-1 or ja.state=-2) 
union 
select ROW_NUMBER() over(order by apply_id) rowNumber, '指定居所使用申请' applyType, ja.apply_id,ja.case_id,ja.unitname,ja.depname,ja.adddate,ja.detail,ja.state,u.RealName from JW_Apply  ja
left join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id 
left join Base_Unit bu on bpa.unit_id=bu.Base_Unit_id 
left join base_user u on  ja.adduser_id=u.userid
 where bu.Base_Unit_id='{0}' and ja.type=3 and (ja.state=1 or ja.state=2 or ja.state=3 or ja.state=4 or ja.state=-1 or ja.state=-2)  
union 
select ROW_NUMBER() over(order by apply_id) rowNumber, '用警申请'+'-'+case tasktype_id when 1 then '保护犯罪现场' when 2 then '执行传唤' when 3 then '执行拘传' when 4 then '协助执行指定居所监视居住' when 5 then '协助执行拘留、逮捕' when 6 then '参与追捕在逃或者逃脱的犯罪嫌疑人' when 7 then '参与搜查任务' when 8 then '提押犯罪嫌疑人被告人或罪犯' when 9 then '看管犯罪嫌疑人被告人或罪犯'  when 10 then '添加送达法律文书信息' when 11 then '保护检察人员' when 12 then '办公、办案、控申接待场所执勤' when 13 then '参与处置突发事件任务' when 14 then '完成其他任务' end   as applyType, apply_id,'' AS case_id,unitname,depname,adddate ,applydetail AS detail,state,u.RealName  from JW_PoliceApply
left join base_user u on  JW_PoliceApply.adduser_id=u.userid
where (unit_id='{0}' and (state =1 or state=2 or state=3 or state=4 or state=-1 or state=-2)) or  ( unit_id='{0}' and state in (1 ,2,3,4,-1,-2,7) and apply_id not in (select object_id from jw_sendpolice ))
union 
select ROW_NUMBER() over(order by callpolice_id) rowNumber, '调警申请' applyType, callpolice_id,'' AS case_id,unit as unitname,'' as depname,addDate ,'' AS detail,JW_AssignPolice.state,u.RealName  from JW_AssignPolice 
left join base_unit on from_unit_id=base_unit_id
left join base_user u on  JW_AssignPolice.adduser=u.userid
where from_unit_id='{0}' and (JW_AssignPolice.state in (1 ,2,3,4,-1,-2,7))
union 
select ROW_NUMBER() over(order by orderpolice_id) rowNumber, '调警令' applyType, orderpolice_id,'' AS case_id,unit as unitname,'' as depname,addDate ,'' AS detail,JW_OrderPolice.state,u.RealName  from JW_OrderPolice
left join base_unit on to_unit_id=base_unit_id  
left join base_user u on  JW_OrderPolice.adduser=u.userid
where from_unit_id='{0}' and JW_OrderPolice.state in (1 ,2,3,4,-1,-2,7)

                ", unit_id);
                ////                string sql =
                ////                    string.Format(
                ////                        @" select * from ( 
                ////select ROW_NUMBER() over(order by apply_id) rowNumber, '办案区使用申请' applyType, apply_id,case_id,unitname,depname,adddate,detail,state from JW_Apply  where unit_id='{4}' and type=1 and state=1 
                ////union 
                ////select ROW_NUMBER() over(order by apply_id) rowNumber, '刑检办案区使用申请' applyType, apply_id,case_id,unitname,depname,adddate,detail,state from JW_Apply  where unit_id='{4}' and type=2 and state=1 
                ////union 
                ////select ROW_NUMBER() over(order by apply_id) rowNumber, '指定居所使用申请' applyType, apply_id,case_id,unitname,depname,adddate,detail,state from JW_Apply  where unit_id='{4}' and type=3 and state=1 
                ////union 
                ////select ROW_NUMBER() over(order by apply_id) rowNumber, '用警申请' applyType, apply_id,'' AS case_id,unitname,depname,adddate ,applydetail AS detail,state  from JW_PoliceApply   where unit_id='{4}' and state=1
                ////union 
                ////select ROW_NUMBER() over(order by callpolice_id) rowNumber, '调警申请' applyType, callpolice_id,'' AS case_id,unit as unitname,'' as depname,addDate ,'' AS detail,JW_AssignPolice.state  from JW_AssignPolice left join base_unit on from_unit_id=base_unit_id  where from_unit_id='{4}' and JW_AssignPolice.state=1 
                ////union 
                ////select ROW_NUMBER() over(order by orderpolice_id) rowNumber, '调警令' applyType, orderpolice_id,'' AS case_id,unit as unitname,'' as depname,addDate ,'' AS detail,JW_OrderPolice.state  from JW_OrderPolice left join base_unit on from_unit_id=base_unit_id  where from_unit_id='{4}' and JW_OrderPolice.state=1
                ////) as a  
                ////where rowNumber between {0} and {1}  
                ////order by {2} {3} "
                ////                        , (pageIndex - 1) * pageSize + 1
                ////                        , pageIndex * pageSize
                ////                        , jqgridparam.sidx
                ////                        , jqgridparam.sord
                ////                        , unit_id
                ////                        );
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


                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);




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
        /// 确定审批
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public string SubmitJW(JW_Apply jwApply, string sendPolice_id)
        {
            #region 审批

            if (jwApply.type == 1 || jwApply.type == 2)
            {
                //string sql =
                //    string.Format(
                //        @"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userType=@userType,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,leaderuser_id=@leaderuser_id,leaderdate=@leaderdate,leaderdetail=@leaderdetail,state=@state where apply_id=@apply_id");

                string sql =
                    string.Format(
                        @"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userType=@userType,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,state=@state where apply_id=@apply_id");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@type", jwApply.type),
                    new SqlParameter("@case_id", jwApply.case_id),
                    new SqlParameter("@unit_id", jwApply.unit_id),
                    new SqlParameter("@unitname", jwApply.unitname),
                    new SqlParameter("@dep_id", jwApply.dep_id),
                    new SqlParameter("@depname", jwApply.depname),
                    new SqlParameter("@asker_id", jwApply.asker_id),
                    new SqlParameter("@PoliceArea_id", jwApply.PoliceArea_id),
                    new SqlParameter("@adduser_id", jwApply.adduser_id),
                    new SqlParameter("@adddate", jwApply.adddate == null ? (object) DBNull.Value : jwApply.adddate),
                    new SqlParameter("@startdate", jwApply.startdate == null ? (object) DBNull.Value : jwApply.startdate),
                    new SqlParameter("@enddate", jwApply.enddate == null ? (object) DBNull.Value : jwApply.enddate),
                    new SqlParameter("@docCode", jwApply.docCode),
                    new SqlParameter("@roomdetail", jwApply.roomdetail),
                    new SqlParameter("@userName", jwApply.userName),
                    new SqlParameter("@userSex", jwApply.userSex == null ? (object) DBNull.Value : jwApply.userSex),
                    new SqlParameter("@userAge", jwApply.userAge),
                    new SqlParameter("@userCode", jwApply.userCode),
                    new SqlParameter("@userNation", jwApply.userNation),
                    new SqlParameter("@userType", jwApply.userType),
                    new SqlParameter("@userPoliticalstatus", jwApply.userPoliticalstatus),
                    new SqlParameter("@userIsNPCmember", jwApply.userIsNPCmember),
                    new SqlParameter("@userEducation", jwApply.userEducation),
                    new SqlParameter("@userWork", jwApply.userWork),
                    new SqlParameter("@userDuty", jwApply.userDuty),
                    new SqlParameter("@userHome", jwApply.userHome),
                    new SqlParameter("@userHealthy", jwApply.userHealthy),
                    new SqlParameter("@detail", jwApply.detail),
                    new SqlParameter("@ApprovalDetail", jwApply.ApprovalDetail),
                    new SqlParameter("@fjuser_id", jwApply.fjuser_id),
                    new SqlParameter("@fjdate", jwApply.fjdate == null ? (object) DBNull.Value : jwApply.fjdate),
                    new SqlParameter("@fjdetail", jwApply.fjdetail),
                    //new SqlParameter("@leaderuser_id", jwApply.leaderuser_id),
                    //new SqlParameter("@leaderdate",jwApply.leaderdate == null ? (object) DBNull.Value : jwApply.leaderdate),
                    //new SqlParameter("@leaderdetail", jwApply.leaderdetail),
                    new SqlParameter("@state", jwApply.state),
                    new SqlParameter("@apply_id", jwApply.apply_id)
                };
                try
                {
                    int r1 = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                }
                catch (Exception)
                {
                    return "数据异常，审批失败";
                }
            }
            else
            {
                //string sql = string.Format(@"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userbiref=@userbiref,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,remark=@remark,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,leaderuser_id=@leaderuser_id,leaderdate=@leaderdate,leaderdetail=@leaderdetail,state=@state where apply_id=@apply_id");
                string sql = string.Format(@"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userbiref=@userbiref,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,remark=@remark,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,state=@state where apply_id=@apply_id");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@type",jwApply.type),
                    new SqlParameter("@case_id",jwApply.case_id),
                    new SqlParameter("@unit_id",jwApply.unit_id),
                    new SqlParameter("@unitname",jwApply.unitname),
                    new SqlParameter("@dep_id",jwApply.dep_id),
                    new SqlParameter("@depname",jwApply.depname),
                    new SqlParameter("@asker_id",jwApply.asker_id),
                    new SqlParameter("@PoliceArea_id",jwApply.PoliceArea_id),
                    new SqlParameter("@adduser_id",jwApply.adduser_id),
                    new SqlParameter("@adddate",jwApply.adddate==null?(object)DBNull.Value:jwApply.adddate),
                    new SqlParameter("@startdate",jwApply.startdate==null?(object)DBNull.Value:jwApply.startdate),
                    new SqlParameter("@enddate",jwApply.enddate==null?(object)DBNull.Value:jwApply.enddate),
                    new SqlParameter("@docCode",jwApply.docCode),
                    new SqlParameter("@roomdetail",jwApply.roomdetail),
                    new SqlParameter("@userName",jwApply.userName),
                    new SqlParameter("@userSex",jwApply.userSex==null?(object)DBNull.Value:jwApply.userSex),
                    new SqlParameter("@userAge",jwApply.userAge),
                    new SqlParameter("@userCode",jwApply.userCode),
                    new SqlParameter("@userNation",jwApply.userNation),
                    new SqlParameter("@userbiref",jwApply.userbiref),
                    new SqlParameter("@userPoliticalstatus",jwApply.userPoliticalstatus),
                    new SqlParameter("@userIsNPCmember",jwApply.userIsNPCmember),
                    new SqlParameter("@userEducation",jwApply.userEducation),
                    new SqlParameter("@userWork",jwApply.userWork),
                    new SqlParameter("@userDuty",jwApply.userDuty),
                    new SqlParameter("@userHome",jwApply.userHome),
                    new SqlParameter("@userHealthy",jwApply.userHealthy),
                    new SqlParameter("@remark",jwApply.remark),
                    new SqlParameter("@detail",jwApply.detail),
                    new SqlParameter("@ApprovalDetail",jwApply.ApprovalDetail),
                    new SqlParameter("@fjuser_id",jwApply.fjuser_id),
                    new SqlParameter("@fjdate",jwApply.fjdate==null?(object)DBNull.Value:jwApply.fjdate),
                    new SqlParameter("@fjdetail",jwApply.fjdetail),
                    //new SqlParameter("@leaderuser_id",jwApply.leaderuser_id),
                    //new SqlParameter("@leaderdate",jwApply.leaderdate==null?(object)DBNull.Value:jwApply.leaderdate),
                    //new SqlParameter("@leaderdetail",jwApply.leaderdetail),
                    new SqlParameter("@state",jwApply.state),
                    new SqlParameter("@apply_id",jwApply.apply_id)
                };
                try
                {
                    int r1 = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                }
                catch (Exception)
                {
                    return "数据异常，审批失败";
                }
            }

            #endregion

            if (sendPolice_id != "" && sendPolice_id != "&nbsp;")
            {
                //对派警的处理
                #region 删除
                //1.先删除原来的数据
                //                string sqlDel = string.Format(@"
                //                    delete JW_SendPolice where Unit_id='{0}' and SendUser_id='{1}'  and type='{2}' and Object_id='{3}';
                //                    "
                //                    , jwApply.unit_id
                //                    , jwApply.fjuser_id
                //                    , jwApply.type
                //                    , jwApply.apply_id
                //                    );
                string sqlDel = string.Format(@"
                    delete JW_SendPolice where Unit_id='{0}' and type='{2}' and Object_id='{3}';
                    "
                    , jwApply.unit_id
                    , jwApply.fjuser_id
                    , jwApply.type
                    , jwApply.apply_id
                    );

                //2017.2.28---jwApply.addUser_id改为jwApply.fjuser_id
                sqlDel = string.Format(@"
                                    declare @errorNumber int=0 
                                    begin tran 
                                    {0} 
                                    set @errorNumber+=@@ERROR 
                                    if(@errorNumber>0) 
	                                    begin 
		                                    rollback tran 
	                                    end 
                                    else 
	                                    begin 
		                                    commit tran 
	                                    end 
                                    "
                    , sqlDel.ToString()
                    );

                try
                {
                    int r2 = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                }
                catch (Exception)
                {
                    return "数据异常，派警失败";
                }

                //2.先拿到当前单位的简称
                string sqlGetUnit = string.Format(@" select * from Base_Unit where Base_Unit_id='{0}' ", jwApply.unit_id);
                string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

                //3.拿到数据库中当前单位的最大的流水号
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

                #region 添加

                StringBuilder sb = new StringBuilder();
                //再添加新的数据
                if (!sendPolice_id.Contains(","))
                {
                    //只选择了一个人
                    sb.AppendFormat(
                        @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                        , jwApply.unit_id
                        , unitShotName + "检警派【" + code + "】号"
                        , jwApply.fjuser_id
                        , jwApply.adddate
                        , jwApply.type
                        , jwApply.apply_id
                        , sendPolice_id
                        , "0"
                        );
                    //2017.2.28---jwApply.addUser_id改为jwApply.fjuser_id
                }
                else
                {
                    //选择了多个人
                    string[] ids = sendPolice_id.Split(',');

                    for (int i = 0; i < ids.Length; i++)
                    {
                        sb.AppendFormat(
                          @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                          , jwApply.unit_id
                          , unitShotName + "检警派【" + (Convert.ToInt32(code) + i) + "】号"
                          , jwApply.fjuser_id
                          , jwApply.adddate
                          , jwApply.type
                          , jwApply.apply_id
                          , ids[i]
                          , "0"
                          );
                        //2017.2.28---jwApply.addUser_id改为jwApply.fjuser_id
                    }
                }

                string sqlInsert = string.Format(@"
                                    declare @errorNumber int=0 
                                    begin tran 
                                    {0} 
                                    set @errorNumber+=@@ERROR 
                                    if(@errorNumber>0) 
	                                    begin 
		                                    rollback tran 
	                                    end 
                                    else 
	                                    begin 
		                                    commit tran 
	                                    end 
                                    "
                    , sb.ToString()
                    );
                try
                {
                    int r3 = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
                }
                catch (Exception)
                {
                    return "数据异常，派警失败";
                }
                #endregion
            }
            return "审批成功";
        }

        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public string BackApplyForm(JW_Apply jwApply, string sendPolice_id)
        {
            #region 回退
            //string sql = string.Format(@"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userType=@userType,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,leaderuser_id=@leaderuser_id,leaderdate=@leaderdate,leaderdetail=@leaderdetail,state=@state where apply_id=@apply_id");

            string sql = string.Format(@"update JW_Apply set   depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,detail=@detail,ApprovalDetail=@ApprovalDetail,fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,state=@state where apply_id=@apply_id");

            SqlParameter[] pars = new SqlParameter[]
                {
                    //new SqlParameter("@type",jwApply.type),
                    //new SqlParameter("@case_id",jwApply.case_id),
                    //new SqlParameter("@unit_id",jwApply.unit_id),
                    //new SqlParameter("@unitname",jwApply.unitname),
                    new SqlParameter("@dep_id",jwApply.dep_id),
                    new SqlParameter("@depname",jwApply.depname),
                    new SqlParameter("@asker_id",jwApply.asker_id),
                    new SqlParameter("@PoliceArea_id",jwApply.PoliceArea_id),
                    new SqlParameter("@adduser_id",jwApply.adduser_id),
                    new SqlParameter("@adddate",jwApply.adddate==null?(object)DBNull.Value:jwApply.adddate),
                    new SqlParameter("@startdate",jwApply.startdate==null?(object)DBNull.Value:jwApply.startdate),
                    new SqlParameter("@enddate",jwApply.enddate==null?(object)DBNull.Value:jwApply.enddate),
                    new SqlParameter("@docCode",jwApply.docCode),
                    new SqlParameter("@roomdetail",jwApply.roomdetail),
                    new SqlParameter("@userName",jwApply.userName),
                    new SqlParameter("@userSex",jwApply.userSex==null?(object)DBNull.Value:jwApply.userSex),
                    new SqlParameter("@userAge",jwApply.userAge),
                    new SqlParameter("@userCode",jwApply.userCode),
                    new SqlParameter("@userNation",jwApply.userNation),
                    //new SqlParameter("@userType",jwApply.userType),
                    new SqlParameter("@userPoliticalstatus",jwApply.userPoliticalstatus),
                    new SqlParameter("@userIsNPCmember",jwApply.userIsNPCmember),
                    new SqlParameter("@userEducation",jwApply.userEducation),
                    new SqlParameter("@userWork",jwApply.userWork),
                    new SqlParameter("@userDuty",jwApply.userDuty),
                    new SqlParameter("@userHome",jwApply.userHome),
                    new SqlParameter("@userHealthy",jwApply.userHealthy),
                    new SqlParameter("@detail",jwApply.detail),
                    new SqlParameter("@ApprovalDetail",jwApply.ApprovalDetail),
                    new SqlParameter("@fjuser_id",jwApply.fjuser_id),
                    new SqlParameter("@fjdate",jwApply.fjdate==null?(object)DBNull.Value:jwApply.fjdate),
                    new SqlParameter("@fjdetail",jwApply.fjdetail),
                    //new SqlParameter("@leaderuser_id",jwApply.leaderuser_id),
                    //new SqlParameter("@leaderdate",jwApply.leaderdate==null?(object)DBNull.Value:jwApply.leaderdate),
                    //new SqlParameter("@leaderdetail",jwApply.leaderdetail),
                    new SqlParameter("@state","-1"),
                    new SqlParameter("@apply_id",jwApply.apply_id)
                };
            try
            {
                int r1 = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
            }
            catch (Exception)
            {
                return "数据异常，回退失败";
            }
            #endregion
            return "回退成功";
        }

        /// <summary>
        /// 加载派警信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public string LoadSelectPolice(string keyValue)
        {
            string id = string.Empty;
            string name = string.Empty;
            string sql = string.Format(@" select * from JW_SendPolice where  Object_id='{0}'", keyValue);
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
                        id += row["user_id"].ToString() + ",";
                        string sqlGetPoliceName = string.Format(@" select * from Base_User where UserId='{0}' ", row["user_id"].ToString());
                        name += SqlHelper.DataTable(sqlGetPoliceName, CommandType.Text).Rows[0]["RealName"].ToString() +
                               ",";
                    }
                    return id.Substring(0, id.Length - 1) + "|" + name.Substring(0, name.Length - 1);
                }
            }
            catch (Exception)
            {
                return "|";
            }
        }
    }
}

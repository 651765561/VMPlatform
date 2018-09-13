using LeaRun.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Entity.CommonModule;
using LeaRun.Utilities;
using System.Diagnostics;

namespace LeaRun.Business
{
    public partial class JW_ApplyBll : RepositoryFactory<JW_Apply>
    {
        /// <summary>
        /// 根据当前单位的主键获取单位的相关信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetCurrentUserUnit(string unitId)
        {
            string sql = string.Format(@" 
                select 
                * 
                from Base_Unit 
                where 
                Base_Unit_id='{0}' 
                "
                , unitId
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

        /// <summary>
        ///  查询案件信息
        /// </summary>
        /// <returns></returns>
        public DataTable CaseInfoListJson(string user_id)
        {
            string sql = string.Format(@" select * from Case_caseinfo cci 
left join Case_CasePower ccp on cci.case_id=ccp.case_id 
where ccp.powerlevel='1' and State = 1 and powertype='1'  and ccp.user_id='{0}' order by adddate desc ", user_id);
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
        ///  查询案件信息
        /// </summary>
        /// <returns></returns>
        public DataTable ShowCaseInfoListJson(string applyId)
        {
            string sql = string.Format(@"
            
select cci.* from JW_Apply ja 
join Case_caseinfo cci on ja.case_id=cci.case_id 
where ja.apply_id='{0}'

            ", applyId);
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
        ///  查询案件信息 法警审批
        /// </summary>
        /// <returns></returns>
        public DataTable CaseInfoListJson_FJ(string KeyValue)
        {
            string sql = string.Format(@" select cca.case_id,name from Case_caseinfo cca join JW_Apply ja on ja.case_id=cca.case_id where ja.apply_id='" + KeyValue + "'");
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
        /// 根据caseid获取caseinfo数据
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public DataTable GetCaseByCaseId(string caseId)
        {
            string sql = string.Format(@" select * from Case_caseinfo  where case_id='{0}' ", caseId);
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
        /// 根据unitID获取办案区的相关信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable PoliceAreaListJson(string unitId)
        {
            string sql = string.Format(@" 
                select 
                * 
                from Base_PoliceArea 
                where 
                unit_id='{0}' 
                "
                , unitId
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

        /// <summary>
        /// 绑定当前单位中的所有的人员
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable AksIdList(string unitId)
        {
            string sql = string.Format(@" select UserId id,RealName value from Base_User where CompanyId='{0}'  ", unitId);
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
        /// 添加，编辑
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public int AddApply(JW_Apply jwApply, string submitType)
        {
            if (submitType == "add")
            {
                string sql =
                     string.Format(@"insert into JW_Apply(
apply_id,type,case_id,unit_id,unitname,dep_id,depname,asker_id,PoliceArea_id,adduser_id,adddate,startdate,enddate,docCode,roomdetail,userName,userSex,userAge,userCode,userNation,userType,userPoliticalstatus,userIsNPCmember,userEducation,userWork,userDuty,userHome,userHealthy,detail,ApprovalDetail,state,tasktype_id
)values(
@apply_id,@type,@case_id,@unit_id,@unitname,@dep_id,@depname,@asker_id,@PoliceArea_id,@adduser_id,@adddate,@startdate,@enddate,@docCode,@roomdetail,@userName,@userSex,@userAge,@userCode,@userNation,@userType,@userPoliticalstatus,@userIsNPCmember,@userEducation,@userWork,@userDuty,@userHome,@userHealthy,@detail,@ApprovalDetail,@state,@tasktype_id)");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@apply_id",jwApply.apply_id),
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
                    new SqlParameter("@userType",jwApply.userType),
                    new SqlParameter("@userPoliticalstatus",jwApply.userPoliticalstatus),
                    new SqlParameter("@userIsNPCmember",jwApply.userIsNPCmember),
                    new SqlParameter("@userEducation",jwApply.userEducation),
                    new SqlParameter("@userWork",jwApply.userWork),
                    new SqlParameter("@userDuty",jwApply.userDuty),
                    new SqlParameter("@userHome",jwApply.userHome),
                    new SqlParameter("@userHealthy",jwApply.userHealthy),
                    new SqlParameter("@detail",jwApply.detail),
                    new SqlParameter("@ApprovalDetail",jwApply.ApprovalDetail),
                    //new SqlParameter("@fjuser_id",jwApply.fjuser_id),
                    //new SqlParameter("@fjdate",jwApply.fjdate==null?(object)DBNull.Value:jwApply.fjdate),
                    //new SqlParameter("@fjdetail",jwApply.fjdetail),
                    //new SqlParameter("@leaderuser_id",jwApply.leaderuser_id),
                    //new SqlParameter("@leaderdate",jwApply.leaderdate==null?(object)DBNull.Value:jwApply.leaderdate),
                    //new SqlParameter("@leaderdetail",jwApply.leaderdetail),
                    new SqlParameter("@state",jwApply.state),
                    new SqlParameter("@tasktype_id",jwApply.tasktype_id)
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
            else
            {
                string sql = string.Format(@"update JW_Apply set type=@type,case_id=@case_id,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,asker_id=@asker_id,PoliceArea_id=@PoliceArea_id,adduser_id=@adduser_id,adddate=@adddate,startdate=@startdate,enddate=@enddate,docCode=@docCode,roomdetail=@roomdetail,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userType=@userType,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy,detail=@detail,ApprovalDetail=@ApprovalDetail,state=@state  where apply_id=@apply_id");

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
                    new SqlParameter("@userType",jwApply.userType),
                    new SqlParameter("@userPoliticalstatus",jwApply.userPoliticalstatus),
                    new SqlParameter("@userIsNPCmember",jwApply.userIsNPCmember),
                    new SqlParameter("@userEducation",jwApply.userEducation),
                    new SqlParameter("@userWork",jwApply.userWork),
                    new SqlParameter("@userDuty",jwApply.userDuty),
                    new SqlParameter("@userHome",jwApply.userHome),
                    new SqlParameter("@userHealthy",jwApply.userHealthy),
                    new SqlParameter("@detail",jwApply.detail),
                    new SqlParameter("@ApprovalDetail",jwApply.ApprovalDetail),
                    //new SqlParameter("@fjuser_id",jwApply.fjuser_id),
                    //new SqlParameter("@fjdate",jwApply.fjdate==null?(object)DBNull.Value:jwApply.fjdate),
                    //new SqlParameter("@fjdetail",jwApply.fjdetail),
                    //new SqlParameter("@leaderuser_id",jwApply.leaderuser_id),
                    //new SqlParameter("@leaderdate",jwApply.leaderdate==null?(object)DBNull.Value:jwApply.leaderdate),
                    //new SqlParameter("@leaderdetail",jwApply.leaderdetail),
                    new SqlParameter("@state",jwApply.state),
                    new SqlParameter("@apply_id",jwApply.apply_id)
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
        }

        /// <summary>
        /// 编辑涉案人
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public int EdutApply(JW_Apply jwApply, string submitType)
        {

            string sql = string.Format(@"update JW_Apply set type=@type,userName=@userName,userSex=@userSex,userAge=@userAge,userCode=@userCode,userNation=@userNation,userType=@userType,userPoliticalstatus=@userPoliticalstatus,userIsNPCmember=@userIsNPCmember,userEducation=@userEducation,userWork=@userWork,userDuty=@userDuty,userHome=@userHome,userHealthy=@userHealthy  where apply_id=@apply_id");

            SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@type",jwApply.type),
                 
                    new SqlParameter("@userName",jwApply.userName),
                    new SqlParameter("@userSex",jwApply.userSex==null?(object)DBNull.Value:jwApply.userSex),
                    new SqlParameter("@userAge",jwApply.userAge),
                    new SqlParameter("@userCode",jwApply.userCode),
                    new SqlParameter("@userNation",jwApply.userNation),
                    new SqlParameter("@userType",jwApply.userType),
                    new SqlParameter("@userPoliticalstatus",jwApply.userPoliticalstatus),
                    new SqlParameter("@userIsNPCmember",jwApply.userIsNPCmember),
                    new SqlParameter("@userEducation",jwApply.userEducation),
                    new SqlParameter("@userWork",jwApply.userWork),
                    new SqlParameter("@userDuty",jwApply.userDuty),
                    new SqlParameter("@userHome",jwApply.userHome),
                    new SqlParameter("@userHealthy",jwApply.userHealthy),
                 
                    
                  
                    new SqlParameter("@apply_id",jwApply.apply_id)
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
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, string unit_id, JqGridParam jqgridparam, string adduser_id)
        {
            //2017.2.28增加string adduser_id，只有自己能看自己的申请单
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal = string.Format(@"
                                        select 
                                            ROW_NUMBER() over(order by apply_id) rowNumber
                                            , * 
                                        from JW_Apply 
                                            where unit_id='{0}' and type=1 and state <> 5 and adduser_id='{1}'
                                        ", unit_id, adduser_id);

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
        /// 删除审批表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteJWApply(string apply_id)
        {
            ////先判断申请表是否审核
            string sqlCheck = string.Format(@" select * from JW_Apply where apply_id ='{0}' ", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sqlCheck, CommandType.Text);
                string state = dt.Rows[0]["state"].ToString();
                List<string> list = new List<string>() { "4", "5" };
                if (list.Contains(state))
                {
                    //该申请单已审批不能删除
                    return -1;
                }
            }
            catch (Exception)
            {
                //数据异常
                return 0;
            }

            //这里删除申请表后，还对用警表进行了相应数据的删除
            string sql = string.Format(@" delete JW_Apply where apply_id ='{0}';delete JW_SendPolice where Object_id='{0}' ", apply_id);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                //数据异常
                return 0;
            }
        }

        /// <summary>
        /// 获取当前审批表单的承办人的相关信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public string LoadSelectUsers(string keyValue)
        {
            string sql = string.Format(@" select * from JW_Apply where apply_id='{0}' ", keyValue);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string asker_ids = dt.Rows[0]["asker_id"].ToString();
                    if (asker_ids.Contains(','))
                    {
                        //多人
                        string[] ids = asker_ids.Split(',');
                        List<string> nameList = new List<string>();

                        foreach (string id in ids)
                        {
                            string sqlGetName = string.Format(@" select * from Base_User where UserId='{0}' ", id);
                            DataTable dtName = SqlHelper.DataTable(sqlGetName, CommandType.Text);
                            nameList.Add(dtName.Rows[0]["UserId"].ToString() + "," + dtName.Rows[0]["RealName"].ToString());
                        }
                        string strids = string.Empty;
                        string strnames = string.Empty;
                        foreach (string n in nameList)
                        {
                            strids += n.Split(',')[0] + ",";
                            strnames += n.Split(',')[1] + ",";
                        }
                        return strids.Substring(0, strids.Length - 1) + "|" + strnames.Substring(0, strnames.Length - 1);
                    }
                    else
                    {
                        //一个人
                        string sqlGetName = string.Format(@" select * from Base_User where UserId='{0}' ", asker_ids);
                        DataTable dtName = SqlHelper.DataTable(sqlGetName, CommandType.Text);
                        return dtName.Rows[0]["UserId"].ToString() + "|" + dtName.Rows[0]["RealName"];
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前审批表单的办案区的相关信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public string LoadSelectAreas(string keyValue)
        {
            string sql = string.Format(@" select jwa.PoliceArea_id,pa.AreaName from JW_Apply jwa
join Base_PoliceArea pa on jwa.PoliceArea_id=pa.PoliceArea_id where jwa.apply_id='{0}' ", keyValue);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["PoliceArea_id"].ToString() + "|" + dt.Rows[0]["AreaName"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
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
update JW_Usedetail set isend=1 where apply_id=@apply_id;
update JW_SendPolice set state=3 where type=1 and Object_id=@apply_id and user_id=@user_id
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
        /// 新增的时候 默认加载办案区信息
        /// </summary>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable LoadPoliceArea(string unit_id, string policeAreaType)
        {
            string where = string.Empty;
            if (policeAreaType != "0")
            {
                if (policeAreaType.Contains(","))
                {
                    //需要加载多个办案区
                    string[] areasType = policeAreaType.Split(',');
                    where += string.Format(@" and ( AreaType= '{0}' ", areasType[0]);
                    for (int i = 1; i < areasType.Length; i++)
                    {
                        where += string.Format(@" or AreaType = '{0}' ", areasType[i]);
                    }
                    where += " ) ";
                }
                else
                {
                    //只需要加载一个办案区
                    where = string.Format(@" and AreaType='{0}' ", policeAreaType);
                }
            }
            string sql = string.Format(@"select top 1 * from Base_PoliceArea where unit_id='{0}' {1} ", unit_id, where);
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
        /// 查找到申请单中法警部门负责人审批的名字
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string Loadfjuser_id(string apply_id)
        {
            string sql = string.Format(@"select bu.* from JW_Apply ja
join Base_User bu on ja.fjuser_id=bu.UserId where ja.apply_id='{0}'", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["RealName"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 查找到申请单中法警部门负责人审批的名字
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string Loadleaderuser_id(string apply_id)
        {
            string sql = string.Format(@"select bu.* from JW_Apply ja
join Base_User bu on ja.leaderuser_id=bu.UserId where ja.apply_id='{0}'", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["RealName"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 判断该申请单是否提交给分管领导审批
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string CheckPostJW(string apply_id)
        {
            string sql =
                string.Format(
                    @"select * from JW_Apply where (state=2 or (leaderuser_id<>'&nbsp;' and leaderdate is not NULL and leaderdetail<>'&nbsp;')) and apply_id='{0}'",
                    apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception)
            {
                return "0";
            }
        }

        /// <summary>
        /// 文件上传，对数据库的处理
        /// </summary>
        /// <returns></returns>
        public int Uploader(string unit_id, string user_id, string time, string type, string keyValue, string loaction, string name, string fileName)
        {
            string sql =
                string.Format(
                    @"insert JW_Upload(upload_id,unit_id,uploaduser_id,uploadDate,type,Object_id,load,realName) values(@upload_id,@unit_id,@uploaduser_id,@uploadDate,@type,@Object_id,@load,@realName)");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@upload_id",fileName), 
                new SqlParameter("@unit_id",unit_id), 
                new SqlParameter("@uploaduser_id",user_id), 
                new SqlParameter("@uploadDate",time), 
                new SqlParameter("@type",type), 
                new SqlParameter("@Object_id",keyValue), 
                new SqlParameter("@load",loaction), 
                new SqlParameter("@realName",name)
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
        /// 绑定已经加载的法律文书
        /// </summary>
        /// <param name="object_id"></param>
        /// <returns></returns>
        public string BindLawFiles(string object_id)
        {
            string sql =
               string.Format(@"select * from JW_Upload where type='{0}' and Object_id='{1}' order by uploadDate"
                   , "1"
                   , object_id
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
        /// 删除法律文书
        /// </summary>
        /// <param name="upload_id"></param>
        /// <returns></returns>
        public string DelLawFiles(string upload_id)
        {
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
        /// 获取当前涉案人信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public string GetCurrentMsg(string keyValue)
        {
            string sql = string.Format(@" select * from JW_Apply where apply_id='{0}' ", keyValue);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["userName"] + "|" + dt.Rows[0]["userSex"];
                }
                else
                {
                    return "|";
                }
            }
            catch (Exception)
            {
                return "|";
            }
        }
    }
}

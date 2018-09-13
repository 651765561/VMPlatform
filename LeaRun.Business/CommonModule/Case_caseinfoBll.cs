using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using System.Data;
using System.Data.SqlClient;
using LeaRun.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LeaRun.Business
{
    public class Case_caseinfoBll : RepositoryFactory<Case_caseinfo>
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
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageCaseJson(string ParameterJson, string unit_id, string user_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                       select ROW_NUMBER() over(order by ci.case_id) rowNumber, ci.*,cb.CaseBrief_id BriefId,cb.Name BriefName from Case_caseinfo ci
                       left join Case_CasePower cp on ci.case_id=cp.case_id 
                       left join Case_CaseBrief cb on ci.Brief=cb.CaseBrief_id
                        where ci.State=1 and cp.powerlevel=1 and cp.user_id='{0}'
                                        ) as a  
                                      "
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
        /// 绑定当前单位下的部门信息
        /// </summary>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable DepListJson(string unit_id)
        {
            string sql = string.Format(@" select * from Base_Department where unit_id='{0}' ", unit_id);
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
        /// 表单提交
        /// </summary>
        /// <param name="caseCaseinfo"></param>
        /// <param name="submitType"></param>
        /// <param name="caseman_id"></param>
        /// <returns></returns>
        public int SubmitCaseinfoForm(Case_caseinfo caseCaseinfo, string submitType, string caseman_id)
        {
            if (submitType == "add")
            {
                //新增
                #region 新增案件
                string sqlAddCase = string.Format(@" insert into Case_caseinfo(case_id,name,CaseCode,Brief,acceptnumber,acceptdate,casetype,unit_id,unitname,dep_id,depname,adduser_id,adddate,State) values(@case_id,@name,@CaseCode,@Brief,@acceptnumber,@acceptdate,@casetype,@unit_id,@unitname,@dep_id,@depname,@adduser_id,@adddate,@State)");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@case_id",caseCaseinfo.case_id), 
                    new SqlParameter("@name",caseCaseinfo.name), 
                    new SqlParameter("@CaseCode",caseCaseinfo.CaseCode), 
                    new SqlParameter("@Brief",caseCaseinfo.Brief), 
                    new SqlParameter("@acceptnumber",caseCaseinfo.acceptnumber), 
                    new SqlParameter("@acceptdate",caseCaseinfo.acceptdate==null?(object)DBNull.Value:caseCaseinfo.acceptdate), 
                    new SqlParameter("@casetype",caseCaseinfo.casetype), 
                    new SqlParameter("@unit_id",caseCaseinfo.unit_id), 
                    new SqlParameter("@unitname",caseCaseinfo.unitname), 
                    new SqlParameter("@dep_id",caseCaseinfo.dep_id), 
                    new SqlParameter("@depname",caseCaseinfo.depname), 
                    new SqlParameter("@adduser_id",caseCaseinfo.adduser_id), 
                    new SqlParameter("@adddate",caseCaseinfo.adddate), 
                    new SqlParameter("@State",caseCaseinfo.State)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlAddCase, CommandType.Text, pars);
                }
                catch (Exception)
                {
                    return 0;
                }
                #endregion
                #region 新增承办人
                if (!caseman_id.Contains(","))
                {
                    //一个人
                    string sqlAddMan = string.Format(@"insert into Case_CasePower(CasePower_id,unit_id,dep_id,case_id,user_id,powerlevel,powertype)
 values(NEWID(),@unit_id,@dep_id,@case_id,@user_id,1,1)");
                    SqlParameter[] parsMan = new SqlParameter[]
                    {
                        new SqlParameter("@unit_id",caseCaseinfo.unit_id), 
                        new SqlParameter("@dep_id",caseCaseinfo.dep_id), 
                        new SqlParameter("@case_id",caseCaseinfo.case_id), 
                        new SqlParameter("@user_id",caseman_id) 
                    };
                    try
                    {
                        int r = SqlHelper.ExecuteNonQuery(sqlAddMan, CommandType.Text, parsMan);
                        return r;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }

                }
                else
                {
                    //多人
                    string[] ids = caseman_id.Split(',');
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        sb.AppendFormat(@"insert into Case_CasePower(CasePower_id,unit_id,dep_id,case_id,user_id,powerlevel,powertype)
 values(NEWID(),'{0}','{1}','{2}','{3}',1,1);", caseCaseinfo.unit_id, caseCaseinfo.dep_id, caseCaseinfo.case_id, ids[i]);
                    }

                    try
                    {
                        int r = SqlHelper.ExecuteNonQuery(sb.ToString(), CommandType.Text);
                        return r;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                #endregion
            }
            else
            {
                //编辑
                #region 编辑案件
                string sqlEditCase = String.Format(@"update Case_caseinfo set name=@name,CaseCode=@CaseCode,Brief=@Brief,acceptnumber=@acceptnumber,acceptdate=@acceptdate,casetype=@casetype,unit_id=@unit_id,unitname=@unitname,dep_id=@dep_id,depname=@depname,adduser_id=@adduser_id,adddate=@adddate,State=@State where case_id=@case_id");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@case_id",caseCaseinfo.case_id), 
                    new SqlParameter("@name",caseCaseinfo.name), 
                    new SqlParameter("@CaseCode",caseCaseinfo.CaseCode), 
                    new SqlParameter("@Brief",caseCaseinfo.Brief), 
                    new SqlParameter("@acceptnumber",caseCaseinfo.acceptnumber), 
                    new SqlParameter("@acceptdate",caseCaseinfo.acceptdate==null?(object)DBNull.Value:caseCaseinfo.acceptdate), 
                    new SqlParameter("@casetype",caseCaseinfo.casetype), 
                    new SqlParameter("@unit_id",caseCaseinfo.unit_id), 
                    new SqlParameter("@unitname",caseCaseinfo.unitname), 
                    new SqlParameter("@dep_id",caseCaseinfo.dep_id), 
                    new SqlParameter("@depname",caseCaseinfo.depname), 
                    new SqlParameter("@adduser_id",caseCaseinfo.adduser_id), 
                    new SqlParameter("@adddate",caseCaseinfo.adddate), 
                    new SqlParameter("@State",caseCaseinfo.State)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlEditCase, CommandType.Text, pars);
                }
                catch (Exception)
                {
                    return 0;
                }
                #endregion

                #region 更新涉案人信息
                string sqlEditByinquest = string.Format(@"update Case_Byinquest set adddate=@adddate,dep_id=@dep_id where case_id=@case_id");
                SqlParameter[] parsByinquest = new SqlParameter[]
                {
                    new SqlParameter("@adddate",caseCaseinfo.adddate),
                    new SqlParameter("@dep_id",caseCaseinfo.dep_id),
                    new SqlParameter("@case_id",caseCaseinfo.case_id)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlEditByinquest, CommandType.Text, parsByinquest);
                }
                catch (Exception)
                {
                    return 0;
                }
                #endregion

                #region 编辑承办人
                //1.删除当前案件的承办人信息
                string sqlDelMan = string.Format(@"delete Case_CasePower where unit_id=@unit_id and case_id=@case_id");
                SqlParameter[] parsDelMan = new SqlParameter[]
                {
                    new SqlParameter("@unit_id",caseCaseinfo.unit_id),
                    //new SqlParameter("@dep_id",caseCaseinfo.dep_id), 
                    new SqlParameter("@case_id",caseCaseinfo.case_id)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlDelMan, CommandType.Text, parsDelMan);
                }
                catch (Exception)
                {
                    return 0;
                }
                //.插入新的数据
                if (!caseman_id.Contains(","))
                {
                    //一个人
                    string sqlAddMan = string.Format(@"insert into Case_CasePower(CasePower_id,unit_id,dep_id,case_id,user_id,powerlevel,powertype)
 values(NEWID(),@unit_id,@dep_id,@case_id,@user_id,1,1)");
                    SqlParameter[] parsMan = new SqlParameter[]
                    {
                        new SqlParameter("@unit_id",caseCaseinfo.unit_id), 
                        new SqlParameter("@dep_id",caseCaseinfo.dep_id), 
                        new SqlParameter("@case_id",caseCaseinfo.case_id), 
                        new SqlParameter("@user_id",caseman_id) 
                    };
                    try
                    {
                        int r = SqlHelper.ExecuteNonQuery(sqlAddMan, CommandType.Text, parsMan);
                        return r;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }

                }
                else
                {
                    //多人
                    string[] ids = caseman_id.Split(',');
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        sb.AppendFormat(@"insert into Case_CasePower(CasePower_id,unit_id,dep_id,case_id,user_id,powerlevel,powertype)
 values(NEWID(),'{0}','{1}','{2}','{3}',1,1);", caseCaseinfo.unit_id, caseCaseinfo.dep_id, caseCaseinfo.case_id, ids[i]);
                    }

                    try
                    {
                        int r = SqlHelper.ExecuteNonQuery(sb.ToString(), CommandType.Text);
                        return r;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 加载已选择的承办人信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public string LoadSelectUsers(string keyValue)
        {
            string sqlCase = string.Format(@" select * from Case_caseinfo where case_id='{0}' ", keyValue);
            try
            {
                DataTable dtCase = SqlHelper.DataTable(sqlCase, CommandType.Text);
                if (dtCase.Rows.Count > 0)
                {
                    string unit_id = dtCase.Rows[0]["unit_id"].ToString();
                    string dep_id = dtCase.Rows[0]["dep_id"].ToString();
                    string sqlMan = string.Format(@"select cp.*,bu.RealName from Case_CasePower cp 
join Base_User bu on cp.user_id=bu.UserId 
where unit_id='{0}' and cp.dep_id='{1}' and case_id='{2}' ", unit_id, dep_id, keyValue);
                    DataTable dtMan = SqlHelper.DataTable(sqlMan, CommandType.Text);
                    if (dtMan.Rows.Count == 1)
                    {
                        //一个人
                        return dtMan.Rows[0]["user_id"].ToString() + "|" + dtMan.Rows[0]["RealName"];
                    }
                    else if (dtMan.Rows.Count > 1)
                    {
                        //多个人
                        string ids = string.Empty;
                        string names = string.Empty;
                        for (int i = 0; i < dtMan.Rows.Count; i++)
                        {
                            ids += dtMan.Rows[i]["user_id"] + ",";
                            names += dtMan.Rows[i]["RealName"] + ",";
                        }
                        return ids.Substring(0, ids.Length - 1) + "|" + names.Substring(0, names.Length - 1);
                    }
                    else
                    {
                        return string.Empty;
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
        /// 删除一个数据
        /// </summary>
        /// <param name="case_id"></param>
        /// <returns></returns>
        public int DeleteCaseinfo(string case_id)
        {
            //删除案件管理的数据，先判断JW_Apply 或者JW_OnDuty中是否有正在使用的案件
            string sqlCheckPoliceArea = string.Format(@"select count(*) from JW_Apply where case_id='{0}' and state not in (0,5)", case_id);
            string sqlCheckPoliceAreas = string.Format(@" select count(*) from JW_PoliceApply where '{0}' in(object_id_1,object_id_2,object_id_3,object_id_5,object_id_6,object_id_7,object_id_8,object_id_10,object_id_11,object_id_13,object_id_14) and state  not in (0,5)", case_id);

            int Count = Repository().FindCountBySql(sqlCheckPoliceArea) + Repository().FindCountBySql(sqlCheckPoliceAreas);
            if (Count > 0)
            {
                return -100;//表示当前警务区正在使用，不予以删除 
            }
            string sqlCase = string.Format(@"select * from Case_caseinfo where case_id='{0}'", case_id);
            try
            {
                DataTable dtCase = SqlHelper.DataTable(sqlCase, CommandType.Text);
                if (dtCase.Rows.Count <= 0)
                {
                    return 0;
                }
                else
                {
                    string unit_id = dtCase.Rows[0]["unit_id"].ToString();
                    string dep_id = dtCase.Rows[0]["dep_id"].ToString();

                    string sqlDel = string.Format(@"delete Case_caseinfo where case_id='{0}';delete Case_CasePower where unit_id='{1}' and dep_id='{2}' and case_id='{0}'", case_id, unit_id, dep_id);
                    int r = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                    return r;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 加载已经选择的案由
        /// </summary>
        /// <param name="case_id"></param>
        /// <returns></returns>
        public string LoadSelectBrief(string case_id)
        {
            string sql = string.Format(@"
select cb.* from Case_caseinfo ci 
left join Case_CaseBrief cb on ci.Brief=cb.CaseBrief_id 
where ci.case_id='{0}'", case_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count <= 0)
                {
                    return "|";
                }
                else
                {
                    return dt.Rows[0]["CaseBrief_id"] + "|" + dt.Rows[0]["Name"];
                }
            }
            catch (Exception)
            {
                return "|";
            }
        }
    }
}

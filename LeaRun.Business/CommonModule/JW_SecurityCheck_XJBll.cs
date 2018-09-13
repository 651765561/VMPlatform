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
    public class JW_SecurityCheck_XJBll : RepositoryFactory<JW_SecurityCheck>
    {
        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageSecurityCheck_XJJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where js.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_SecurityCheck js {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by checkDate desc) rowNumber
, js.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName from JW_SecurityCheck js
join Base_Unit u on js.unit_id=u.Base_Unit_id 
join Base_PoliceArea pa on js.PoliceArea_id=pa.PoliceArea_id
join Base_User bu on js.checkuser_id=bu.UserId  {4}
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
        /// 表单提交
        /// </summary>
        /// <param name="submitType"></param>
        /// <param name="jwPhysicalexamination"></param>
        /// <returns></returns>
        public int SubmitSecurityCheck_XJForm(string submitType, JW_SecurityCheck jwSecurityCheck)
        {
            //先获取相关信息
            string sqlSelectApply = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwSecurityCheck.apply_id);
            try
            {
                DataTable dtSelectApply = SqlHelper.DataTable(sqlSelectApply, CommandType.Text);
                if (dtSelectApply.Rows.Count <= 0)
                {
                    //没有数据
                    return 0;
                }
                else
                {
                    //有数据
                    if (submitType == "add")
                    {
                        //新增
                        string sqlInsert = string.Format(@"
insert into JW_SecurityCheck(SecurityCheck_id,unit_id,PoliceArea_id,apply_id,checkuser_id,checkDate,checkplace,cardcode,checkmethod,checkdetail)
values(NEWID(),@unit_id,@PoliceArea_id,@apply_id,@checkuser_id,@checkDate,@checkplace,@cardcode,@checkmethod,@checkdetail)
");
                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwSecurityCheck.apply_id), 
                            new SqlParameter("@checkuser_id",jwSecurityCheck.checkuser_id), 
                            new SqlParameter("@checkDate",jwSecurityCheck.checkDate==null?(object)DBNull.Value:jwSecurityCheck.checkDate), 
                            new SqlParameter("@checkplace",jwSecurityCheck.checkplace),
                            new SqlParameter("@cardcode",jwSecurityCheck.cardcode),
                            new SqlParameter("@checkmethod",jwSecurityCheck.checkmethod),
                            new SqlParameter("@checkdetail",jwSecurityCheck.checkdetail)
                        };
                        int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                        return r;
                    }
                    else
                    {
                        string sqlCheckEdit = string.Format(@" select * from JW_SecurityCheck where checkuser_id='{0}' and  SecurityCheck_id='{1}' ", jwSecurityCheck.checkuser_id, jwSecurityCheck.SecurityCheck_id);
                        int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                        if (count <= 0)
                        {
                            return -1;
                        }

                        //编辑
                        string sqlUpdate = string.Format(@"
                                update JW_SecurityCheck set 
                                     unit_id=@unit_id
                                    ,PoliceArea_id=@PoliceArea_id
                                    ,apply_id=@apply_id
                                    ,checkuser_id=@checkuser_id
                                    ,checkDate=@checkDate
                                    ,checkplace=@checkplace
                                    ,cardcode=@cardcode
                                    ,checkmethod=@checkmethod
                                    ,checkdetail=@checkdetail   
                                where 
                                    SecurityCheck_id=@SecurityCheck_id
                        ");

                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwSecurityCheck.apply_id), 
                            new SqlParameter("@checkuser_id",jwSecurityCheck.checkuser_id), 
                            new SqlParameter("@checkDate",jwSecurityCheck.checkDate==null?(object)DBNull.Value:jwSecurityCheck.checkDate), 
                            new SqlParameter("@checkplace",jwSecurityCheck.checkplace),
                            new SqlParameter("@cardcode",jwSecurityCheck.cardcode),
                            new SqlParameter("@checkmethod",jwSecurityCheck.checkmethod),
                            new SqlParameter("@checkdetail",jwSecurityCheck.checkdetail),
                            new SqlParameter("@SecurityCheck_id",jwSecurityCheck.SecurityCheck_id)
                        };
                        int r = SqlHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, pars);
                        return r;
                    }
                }

            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// 删除列表中的数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int DeleteSecurityCheck_XJ(string securitycheck_id,string user_id)
        {
            // start 取消不同法警互相删除的权限
            //string sqlCheckEdit = string.Format(@" select * from JW_SecurityCheck where SecurityCheck_id='{0}' and  checkuser_id='{1}'  ", securitycheck_id, user_id);
            //int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return -1;
            //}
            //end  取消不同法警互相删除的权限

            string sql = string.Format(@" 
                        delete JW_SecurityCheck where SecurityCheck_id='{0}'
                        "
                , securitycheck_id
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

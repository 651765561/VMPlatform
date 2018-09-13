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
    public class JW_LeaveBll : RepositoryFactory<JW_Leave>
    {
        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageLeaveJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where jl.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_Leave jl {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by addDate desc) rowNumber
, jl.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName from JW_Leave jl
join Base_Unit u on jl.unit_id=u.Base_Unit_id 
join Base_PoliceArea pa on jl.PoliceArea_id=pa.PoliceArea_id
join Base_User bu on jl.adduser_id=bu.UserId  {4}
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
                    total = Convert.ToInt32(Math.Ceiling(dtAll.Rows.Count * 1.0 / jqgridparam.rows)),   //总页数
                    page = jqgridparam.page,                                                            //当前页码
                    records = dtAll.Rows.Count,                                                         //总记录数
                    costtime = CommonHelper.TimerEnd(watch),                                            //查询消耗的毫秒数
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
        /// <param name="jwWatchRecord"></param>
        /// <returns></returns>
        public int SubmitLeaveForm(string submitType, JW_Leave jwLeave)
        {
            //先获取相关信息
            string sqlSelectApply = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwLeave.apply_id);
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
                        string sqlInsert = string.Format(@"insert into JW_Leave(
                                leave_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,startdate,enddate,item,detail
                                ) values(
                                @leave_id,@unit_id,@PoliceArea_id,@apply_id,@adduser_id,@addDate,@startdate,@enddate,@item,@detail
                                )");
                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@leave_id",jwLeave.leave_id),
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]),
                            new SqlParameter("@apply_id",jwLeave.apply_id),
                            new SqlParameter("@adduser_id",jwLeave.adduser_id),
                            new SqlParameter("@addDate",jwLeave.addDate==null?(object)DBNull.Value:jwLeave.addDate),
                            new SqlParameter("@startdate",jwLeave.startdate==null?(object)DBNull.Value:jwLeave.startdate),
                            new SqlParameter("@enddate",jwLeave.enddate==null?(object)DBNull.Value:jwLeave.enddate),
                            new SqlParameter("@item",jwLeave.item),
                            new SqlParameter("@detail",jwLeave.detail)
                        };
                        int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                        return r;
                    }
                    else
                    {
                        string sqlCheckEdit =
                            string.Format(@" select * from JW_Leave where leave_id='{0}' and adduser_id='{1}' "
                                , jwLeave.leave_id
                                , jwLeave.adduser_id
                                );
                        int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                        if (count == 0)
                        {
                            return -1;
                        }

                        //编辑
                        string sqlUpdate = string.Format(@"update JW_Leave set 
                                    unit_id=@unit_id
                                    ,PoliceArea_id=@PoliceArea_id
                                    ,apply_id=@apply_id
                                    ,adduser_id=@adduser_id
                                    ,addDate=@addDate
                                    ,startdate=@startdate
                                    ,enddate=@enddate
                                    ,item=@item
                                    ,detail=@detail 
                            where 
                                    leave_id=@leave_id
                            ");

                        SqlParameter[] pars = new SqlParameter[]
                        {
                           new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]),
                            new SqlParameter("@apply_id",jwLeave.apply_id),
                            new SqlParameter("@adduser_id",jwLeave.adduser_id),
                            new SqlParameter("@addDate",jwLeave.addDate==null?(object)DBNull.Value:jwLeave.addDate),
                            new SqlParameter("@startdate",jwLeave.startdate==null?(object)DBNull.Value:jwLeave.startdate),
                            new SqlParameter("@enddate",jwLeave.enddate==null?(object)DBNull.Value:jwLeave.enddate),
                            new SqlParameter("@item",jwLeave.item),
                            new SqlParameter("@detail",jwLeave.detail),
                            new SqlParameter("@leave_id",jwLeave.leave_id)
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
        public int DeleteLeave(string leave_id, string user_id)
        {
            //string sqlCheckDel =
            //                string.Format(@" select * from JW_Leave where leave_id='{0}' and adduser_id='{1}' "
            //                    , leave_id
            //                    , user_id
            //                    );
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count == 0)
            //{
            //    return -1;
            //}

            string sql = string.Format(@" 
                        delete JW_Leave where leave_id='{0}'
                        "
                , leave_id
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

        /// <summary>
        /// 加载签名
        /// </summary>
        /// <param name="leave_id"></param>
        /// <returns></returns>
        public DataTable LoadQM(string leave_id)
        {
            string sql = string.Format(@"select * from JW_Qm where detail_id='{0}' and type=4", leave_id);
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

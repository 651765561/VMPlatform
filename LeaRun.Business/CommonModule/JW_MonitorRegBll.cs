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

namespace LeaRun.Business.CommonModule
{
    public partial class JW_MonitorRegBll : RepositoryFactory<JW_WatchRecord>
    {
        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageWatchRecordJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where jw.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_WatchRecord jw {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by addDate desc) rowNumber
, jw.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName from JW_WatchRecord jw
join Base_Unit u on jw.unit_id=u.Base_Unit_id 
join Base_PoliceArea pa on jw.PoliceArea_id=pa.PoliceArea_id
join Base_User bu on jw.adduser_id=bu.UserId  {4}
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
        /// <param name="jwWatchRecord"></param>
        /// <returns></returns>
        public int SubmitWatchRecordForm(string submitType, JW_WatchRecord jwWatchRecord)
        {
            //先获取相关信息
            string sqlSelectApply = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwWatchRecord.apply_id);
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
                        string sqlInsert = string.Format(@"insert into JW_WatchRecord(
watchrecord_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,watchuser,depname,watchplace,startdate,enddate,dutydetail,attention,successor,depname2
) values(
@watchrecord_id,@unit_id,@PoliceArea_id,@apply_id,@adduser_id,@addDate,@watchuser,@depname,@watchplace,@startdate,@enddate,@dutydetail,@attention,@successor,@depname2)");
                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@watchrecord_id",jwWatchRecord.watchrecord_id), 
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwWatchRecord.apply_id), 
                            new SqlParameter("@adduser_id",jwWatchRecord.adduser_id), 
                            new SqlParameter("@addDate",jwWatchRecord.addDate==null?(object)DBNull.Value:jwWatchRecord.addDate), 
                            new SqlParameter("@watchuser",jwWatchRecord.watchuser), 
                            new SqlParameter("@depname",jwWatchRecord.depname), 
                            new SqlParameter("@watchplace",jwWatchRecord.watchplace),
                            new SqlParameter("@startdate",jwWatchRecord.startdate==null?(object)DBNull.Value:jwWatchRecord.startdate), 
                            new SqlParameter("@enddate",jwWatchRecord.enddate==null?(object)DBNull.Value:jwWatchRecord.enddate), 
                            new SqlParameter("@dutydetail",jwWatchRecord.dutydetail),
                            new SqlParameter("@attention",jwWatchRecord.attention),
                            new SqlParameter("@successor",jwWatchRecord.successor),
                            new SqlParameter("@depname2",jwWatchRecord.depname2)
                        };
                        int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                        return r;
                    }
                    else
                    {
                        string sqlCheckEidt = String.Format(@"select * from JW_WatchRecord where watchrecord_id='{0}' and adduser_id='{1}'", jwWatchRecord.watchrecord_id, jwWatchRecord.adduser_id);
                        int count = SqlHelper.DataTable(sqlCheckEidt, CommandType.Text).Rows.Count;
                        if (count <= 0)
                        {
                            return -1;
                        }

                        //编辑
                        string sqlUpdate = string.Format(@"update JW_WatchRecord set
                            unit_id=@unit_id
                            ,PoliceArea_id=@PoliceArea_id
                            ,apply_id=@apply_id
                            ,adduser_id=@adduser_id
                            ,addDate=@addDate
                            ,watchuser=@watchuser
                            ,depname=@depname
                            ,watchplace=@watchplace
                            ,startdate=@startdate
                            ,enddate=@enddate
                            ,dutydetail=@dutydetail
                            ,attention=@attention
                            ,successor=@successor
                            ,depname2=@depname2     
                        where 
                            watchrecord_id=@watchrecord_id
                        ");

                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwWatchRecord.apply_id), 
                            new SqlParameter("@adduser_id",jwWatchRecord.adduser_id), 
                            new SqlParameter("@addDate",jwWatchRecord.addDate==null?(object)DBNull.Value:jwWatchRecord.addDate), 
                            new SqlParameter("@watchuser",jwWatchRecord.watchuser), 
                            new SqlParameter("@depname",jwWatchRecord.depname), 
                            new SqlParameter("@watchplace",jwWatchRecord.watchplace),
                            new SqlParameter("@startdate",jwWatchRecord.startdate==null?(object)DBNull.Value:jwWatchRecord.startdate), 
                            new SqlParameter("@enddate",jwWatchRecord.enddate==null?(object)DBNull.Value:jwWatchRecord.enddate), 
                            new SqlParameter("@dutydetail",jwWatchRecord.dutydetail),
                            new SqlParameter("@attention",jwWatchRecord.attention),
                            new SqlParameter("@successor",jwWatchRecord.successor),
                            new SqlParameter("@depname2",jwWatchRecord.depname2),
                            new SqlParameter("@watchrecord_id",jwWatchRecord.watchrecord_id)
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
        public int DeleteWatchRecord(string watchrecord_id, string user_id)
        {
            //string sqlCheckDel = String.Format(@"select * from JW_WatchRecord where watchrecord_id='{0}' and adduser_id='{1}'", watchrecord_id, user_id);
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return -1;
            //}

            string sql = string.Format(@" 
                        delete JW_WatchRecord where watchrecord_id='{0}'
                        "
                , watchrecord_id
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
        /// <param name="watchrecord_id"></param>
        /// <returns></returns>
        public DataTable LoadQM(string watchrecord_id)
        {
            string sql = string.Format(@"select * from JW_Qm where detail_id='{0}' and type=3", watchrecord_id);
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
        /// 拿到上一条用于衔接的数据
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string GetPrevData(string apply_id)
        {
            string sql = string.Format(@"select top 1 * from JW_WatchRecord where apply_id='{0}' order by addDate desc", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                string successor = dt.Rows[0]["successor"].ToString();  //赋值给看管人
                string depname = dt.Rows[0]["depname"].ToString();      //赋值给部门
                string watchplace = dt.Rows[0]["watchplace"].ToString();//赋值给看管地点
                return successor + "|" + depname + "|" + watchplace;
            }
            catch (Exception)
            {
                return "||";
            }
        }
    }
}

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
    public class JW_DutyRecordBll : RepositoryFactory<JW_DutyRecord>
    {
        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageDutyJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where jd.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_DutyRecord jd {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by addDate desc) rowNumber
, jd.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName from JW_DutyRecord jd
join Base_Unit u on jd.unit_id=u.Base_Unit_id 
join Base_PoliceArea pa on jd.PoliceArea_id=pa.PoliceArea_id
join Base_User bu on jd.adduser_id=bu.UserId  {4}
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
        public int SubmitDutyForm(string submitType, JW_DutyRecord jwDutyRecord)
        {
            //先获取相关信息
            string sqlSelectApply = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwDutyRecord.apply_id);
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
                        string sqlInsert = string.Format(@"insert into JW_DutyRecord(
                            dutyrecord_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,dutyuser,startdate,enddate,dutydetail
                        ) values(
                            NEWID(),@unit_id,@PoliceArea_id,@apply_id,@adduser_id,@addDate,@dutyuser,@startdate,@enddate,@dutydetail
                        )");
                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwDutyRecord.apply_id), 
                            new SqlParameter("@adduser_id",jwDutyRecord.adduser_id), 
                            new SqlParameter("@addDate",jwDutyRecord.addDate==null?(object)DBNull.Value:jwDutyRecord.addDate), 
                            new SqlParameter("@dutyuser",jwDutyRecord.dutyuser),
                            new SqlParameter("@startdate",jwDutyRecord.startdate==null?(object)DBNull.Value:jwDutyRecord.startdate), 
                            new SqlParameter("@enddate",jwDutyRecord.enddate==null?(object)DBNull.Value:jwDutyRecord.enddate), 
                            new SqlParameter("@dutydetail",jwDutyRecord.dutydetail)
                        };
                        int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                        return r;
                    }
                    else
                    {
                        string sqlCheckEdit = string.Format(@" select * from JW_DutyRecord where dutyrecord_id='{0}' and adduser_id='{1}' "
                            , jwDutyRecord.dutyrecord_id
                            , jwDutyRecord.adduser_id
                            );
                        int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                        if (count == 0)
                        {
                            return -1;      //表示当前查看的这个记录，不是当前人添加的，当前人不能进行编辑
                        }


                        //编辑
                        string sqlUpdate = string.Format(@"update JW_DutyRecord set 
                            unit_id=@unit_id
                            ,PoliceArea_id=@PoliceArea_id
                            ,apply_id=@apply_id
                            ,adduser_id=@adduser_id
                            ,addDate=@addDate
                            ,dutyuser=@dutyuser
                            ,startdate=@startdate
                            ,enddate=@enddate
                            ,dutydetail=@dutydetail     
                        where 
                            dutyrecord_id=@dutyrecord_id
                        ");

                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwDutyRecord.apply_id), 
                            new SqlParameter("@adduser_id",jwDutyRecord.adduser_id), 
                            new SqlParameter("@addDate",jwDutyRecord.addDate==null?(object)DBNull.Value:jwDutyRecord.addDate), 
                            new SqlParameter("@dutyuser",jwDutyRecord.dutyuser),
                            new SqlParameter("@startdate",jwDutyRecord.startdate==null?(object)DBNull.Value:jwDutyRecord.startdate), 
                            new SqlParameter("@enddate",jwDutyRecord.enddate==null?(object)DBNull.Value:jwDutyRecord.enddate), 
                            new SqlParameter("@dutydetail",jwDutyRecord.dutydetail),
                            new SqlParameter("@dutyrecord_id",jwDutyRecord.dutyrecord_id)
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
        public int DeleteDuty(string dutyrecord_id, string user_id)
        {
            //string sqlCheckDel = string.Format(@" select * from JW_DutyRecord where dutyrecord_id='{0}' and adduser_id='{1}' ", dutyrecord_id, user_id);
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count <= 0)
            //{
            //    return -1;
            //}

            string sql = string.Format(@" 
                        delete JW_DutyRecord where dutyrecord_id='{0}'
                        "
                , dutyrecord_id
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
        /// 新增时，加载默认信息
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public string SetAddForm(string user_id)
        {
            string sql = string.Format(@"select * from Base_User where  UserId='{0}'", user_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                return dt.Rows[0]["RealName"].ToString() + "|" + DateTime.Now.ToString();
            }
            catch (Exception)
            {
                return "|";
            }
        }
    }
}

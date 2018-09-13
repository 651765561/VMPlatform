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
    public class JW_BreakRolesBll : RepositoryFactory<JW_BreakRoles>
    {
        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="apply_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageBreakRolesJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where jb.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_BreakRoles jb {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over (order by aa.breakroles_id) rowNumber,* from (
select jb.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName from JW_BreakRoles jb
left join Base_Unit u on jb.unit_id=u.Base_Unit_id 
left join Base_PoliceArea pa on jb.PoliceArea_id=pa.PoliceArea_id

left join Base_User bu on jb.adduser_id=bu.UserId {4}
union all
select Usedetail_id breakroles, ju.unit_id,ju.PoliceArea_id,apply_id,adduser_id,GETDATE() addDate,bu.realname watchuser,ju.room_id,GETDATE() startdate,'办案超时' detail,'提醒办案人员' treatement,u.unit unitName,bp.AreaName PoliceAreaName,bu.RealName adduserName from JW_Usedetail  ju 
left join Base_Unit u on ju.unit_id=u.Base_Unit_id 
left join Base_PoliceArea bp on ju.PoliceArea_id=bp.PoliceArea_id 

left join Base_User bu on ju.adduser_id=bu.UserId
where apply_id='{5}' and DATEDIFF(HOUR,startdate,GETDATE())>=12 and isend=0) aa
) as a  
where rowNumber between {0} and {1}  
order by {2} {3} "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , sqlWhere
                        , apply_id
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


//        public string GridPageBreakRolesJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
//        {
//            try
//            {
//                int pageIndex = jqgridparam.page;
//                int pageSize = jqgridparam.rows;
//                Stopwatch watch = CommonHelper.TimerStart();
//                string sqlWhere = string.Empty;
//                if (!string.IsNullOrEmpty(apply_id))
//                {
//                    sqlWhere = string.Format(" where jb.apply_id='{0}'", apply_id);
//                }

//                string sqlLoadAll = string.Format(" select * from JW_BreakRoles jb {0}", sqlWhere);
//                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
//                string sqlLoad =
//                    string.Format(
//                        @" select * from ( 
//select ROW_NUMBER() over (order by aa.breakroles_id) rowNumber,* from (
//select jb.*,u.unit unitName,pa.AreaName PoliceAreaName,bu.RealName adduserName,br.RoomName roomname from JW_BreakRoles jb
//left join Base_Unit u on jb.unit_id=u.Base_Unit_id 
//left join Base_PoliceArea pa on jb.PoliceArea_id=pa.PoliceArea_id
//left join Base_Room br on jb.room_id=br.Room_id
//left join Base_User bu on jb.adduser_id=bu.UserId {4}
//union all
//select Usedetail_id breakroles, ju.unit_id,ju.PoliceArea_id,apply_id,adduser_id,GETDATE() addDate,bu.realname watchuser,ju.room_id,GETDATE() startdate,'办案超时' detail,'提醒办案人员' treatement,u.unit unitName,bp.AreaName PoliceAreaName,bu.RealName adduserName,br.RoomName roomname  from JW_Usedetail  ju 
//left join Base_Unit u on ju.unit_id=u.Base_Unit_id 
//left join Base_PoliceArea bp on ju.PoliceArea_id=bp.PoliceArea_id 
//left join Base_Room br on ju.room_id=br.Room_id 
//left join Base_User bu on ju.adduser_id=bu.UserId
//where apply_id='{5}' and DATEDIFF(HOUR,startdate,GETDATE())>=12 and isend=0) aa
//) as a  
//where rowNumber between {0} and {1}  
//order by {2} {3} "
//                        , (pageIndex - 1) * pageSize + 1
//                        , pageIndex * pageSize
//                        , jqgridparam.sidx
//                        , jqgridparam.sord
//                        , sqlWhere
//                        , apply_id
//                        );
//                DataTable dt = Repository().FindTableBySql(sqlLoad);

//                var JsonData = new
//                {
//                    total = Convert.ToInt32(Math.Ceiling(dtAll.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
//                    page = jqgridparam.page, //当前页码
//                    records = dtAll.Rows.Count, //总记录数
//                    costtime = CommonHelper.TimerEnd(watch), //查询消耗的毫秒数
//                    rows = dt
//                };
//                return JsonData.ToJson();
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }

        /// <summary>
        /// 表单提交
        /// </summary>
        /// <param name="submitType"></param>
        /// <param name="jwWatchRecord"></param>
        /// <returns></returns>
        public int SubmitBreakRolesForm(string submitType, JW_BreakRoles jwBreakRoles)
        {
            //先获取相关信息
            string sqlSelectApply = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwBreakRoles.apply_id);
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
                        string sqlInsert = string.Format(@"insert into JW_BreakRoles(
breakroles_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,watchuser,room_id,startdate,detail,treatment
) values(
NEWID(),@unit_id,@PoliceArea_id,@apply_id,@adduser_id,@addDate,@watchuser,@room_id,@startdate,@detail,@treatment
)");
                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwBreakRoles.apply_id), 
                            new SqlParameter("@adduser_id",jwBreakRoles.adduser_id), 
                            new SqlParameter("@addDate",jwBreakRoles.addDate==null?(object)DBNull.Value:jwBreakRoles.addDate), 
                            new SqlParameter("@watchuser",jwBreakRoles.watchuser), 
                            new SqlParameter("@room_id",jwBreakRoles.room_id), 
                            new SqlParameter("@startdate",jwBreakRoles.startdate==null?(object)DBNull.Value:jwBreakRoles.startdate), 
                            new SqlParameter("@detail",jwBreakRoles.detail),
                            new SqlParameter("@treatment",jwBreakRoles.treatment)
                        };
                        int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                        return r;
                    }
                    else
                    {
                        string sqlCheckEdit = string.Format(@" select * from JW_BreakRoles where breakroles_id='{0}' and adduser_id='{1}' "
                            , jwBreakRoles.breakroles_id
                            , jwBreakRoles.adduser_id
                            );
                        int count = SqlHelper.DataTable(sqlCheckEdit, CommandType.Text).Rows.Count;
                        if (count == 0)
                        {
                            return -1;
                        }
                        //编辑
                        string sqlUpdate = string.Format(@"update JW_BreakRoles set 
                                    unit_id=@unit_id
                                    ,PoliceArea_id=@PoliceArea_id
                                    ,apply_id=@apply_id
                                    ,adduser_id=@adduser_id
                                    ,addDate=@addDate
                                    ,watchuser=@watchuser
                                    ,room_id=@room_id
                                    ,startdate=@startdate
                                    ,detail=@detail
                                    ,treatment=@treatment 
                                where 
                                    breakroles_id=@breakroles_id
                        ");

                        SqlParameter[] pars = new SqlParameter[]
                        {
                            new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                            new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                            new SqlParameter("@apply_id",jwBreakRoles.apply_id), 
                            new SqlParameter("@adduser_id",jwBreakRoles.adduser_id), 
                            new SqlParameter("@addDate",jwBreakRoles.addDate==null?(object)DBNull.Value:jwBreakRoles.addDate), 
                            new SqlParameter("@watchuser",jwBreakRoles.watchuser), 
                            new SqlParameter("@room_id",jwBreakRoles.room_id), 
                            new SqlParameter("@startdate",jwBreakRoles.startdate==null?(object)DBNull.Value:jwBreakRoles.startdate), 
                            new SqlParameter("@detail",jwBreakRoles.detail),
                            new SqlParameter("@treatment",jwBreakRoles.treatment),
                            new SqlParameter("@breakroles_id",jwBreakRoles.breakroles_id)
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
        public int DeleteBreakRoles(string breakroles_id,string user_id)
        {
            //string sqlCheckDel = string.Format(@" select * from JW_BreakRoles where breakroles_id='{0}' and adduser_id='{1}' "
            //               , breakroles_id
            //               , user_id
            //               );
            //int count = SqlHelper.DataTable(sqlCheckDel, CommandType.Text).Rows.Count;
            //if (count == 0)
            //{
            //    return -1;
            //}

            string sql = string.Format(@" 
                        delete JW_BreakRoles where breakroles_id='{0}'
                        "
                , breakroles_id
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
        /// 绑定发生房间
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public DataTable RoomIdListJson(string apply_id)
        {
            //            string sql = string.Format(@" select r.* from Base_Room r 
            //join JW_Apply a on r.Unit_id=a.unit_id 
            //where r.state=1 and  a.apply_id='{0}' ", apply_id);
            string sql = string.Format(@"select br.* from JW_Usedetail ju
join Base_Room br on ju.room_id=br.Room_id where ju.apply_id='{0}' and isend=0", apply_id);
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

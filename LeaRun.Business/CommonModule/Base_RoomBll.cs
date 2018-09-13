//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using System;
using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace LeaRun.Business
{
    public class Base_RoomBll : RepositoryFactory<Base_Room>
    {
        public DataTable GetRoomTypeList()
        {
            string sql = string.Format(" select * from Base_RoomType where type=1 ");
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

        public DataTable PoliceAreaListJson(string unit_id)
        {
            string sql = string.Format(@" select * from Base_PoliceArea where unit_id='{0}' ", unit_id);
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

        public int AddRoom(Base_Room room, string type)
        {
            if (type == "add")
            {
                string sql = string.Format(@"insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state) values(NEWID(),@RoomName,@RoomCode,@RoomType_id,@PoliceArea_id,@Unit_id,@state)");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@RoomName",room.RoomName), 
                    new SqlParameter("@RoomCode",room.RoomCode), 
                    new SqlParameter("@RoomType_id",room.RoomType_id), 
                    new SqlParameter("@PoliceArea_id",room.PoliceArea_id==null?(object)DBNull.Value:room.PoliceArea_id), 
                    new SqlParameter("@Unit_id",room.Unit_id), 
                    new SqlParameter("@state",room.state)
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
                string sql = string.Format(@"update Base_Room set RoomName=@RoomName,RoomCode=@RoomCode,RoomType_id=@RoomType_id,PoliceArea_id=@PoliceArea_id,Unit_id=@Unit_id,state=@state where Room_id=@Room_id");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@RoomName",room.RoomName), 
                    new SqlParameter("@RoomCode",room.RoomCode), 
                    new SqlParameter("@RoomType_id",room.RoomType_id), 
                    new SqlParameter("@PoliceArea_id",room.PoliceArea_id==null?(object)DBNull.Value:room.PoliceArea_id), 
                    new SqlParameter("@Unit_id",room.Unit_id), 
                    new SqlParameter("@state",room.state), 
                    new SqlParameter("@Room_id",room.Room_id)
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

        public string GridPageRoomJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(unit_id))
                {
                    sqlWhere = string.Format(" where r.unit_id='{0}'", unit_id);
                }

                string sqlLoadAll = string.Format(" select * from Base_Room r {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by Room_id) rowNumber 
,r.Room_id 
,r.RoomName 
,r.RoomCode 
,rt.Name roomtype_id  
,u.unit unit_id 
,r.state  
from Base_Room r 
join Base_RoomType rt on r.RoomType_id=rt.RoomType_id  
join Base_Unit u on r.Unit_id=u.Base_Unit_id  {4}
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

        public int DeleteRoom(string keyValue)
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Format(@"
delete  
Base_Room 
where Room_id='{0}' 
"
                , keyValue
                );
            sb.Append(sql);

            try
            {
                int r = Repository().ExecuteBySql(sb);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetUnitByRoomId(string roomId)
        {
            string sql = string.Format(@"
                                    select u.unit from Base_Room r
                                    join Base_Unit u on r.Unit_id=u.Base_Unit_id 
                                    where Room_id='{0}'
                                        "
                                    , roomId
                                    );
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0]["unit"].ToString();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public int IsExsitMonitorApplicationByRoomId(string roomId)
        {
            string sql = string.Format(@"
                                            select Room_id from Base_MonitorApplication 
                                            where Room_id='{0}'
                                                "
                                    , roomId
                                    );
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
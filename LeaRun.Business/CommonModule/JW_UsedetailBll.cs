using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using LeaRun.Utilities;
using System.Diagnostics;
using System.Data;

namespace LeaRun.Business
{
    public class JW_UsedetailBll : RepositoryFactory<JW_Usedetail>
    {
        public string GridPageUsedetailJson(string ParameterJson, string apply_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                if (!string.IsNullOrEmpty(apply_id))
                {
                    sqlWhere = string.Format(" where jud.apply_id='{0}'", apply_id);
                }

                string sqlLoadAll = string.Format(" select * from JW_Usedetail jud {0}", sqlWhere);
                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over (order by jud.addDate) rowNumber, jud.*,bu.unit unitname,bpa.AreaName policeareaname,br.RoomName roomname,
case when  DATEDIFF(s,jud.startdate,isnull(jud.enddate,getdate()))>43200 then 1 else 0 end as islater
from JW_Usedetail jud 
join Base_Unit bu on jud.unit_id=bu.Base_Unit_id 
join Base_PoliceArea bpa on jud.PoliceArea_id=bpa.PoliceArea_id 
join Base_Room br on jud.room_id=br.Room_id
where jud.apply_id='{5}'
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

        public DataTable SetUsedetailForm(string jwUsedetail_id)
        {
            string sql = string.Format(@"
                    select jud.Usedetail_id usedetail_id,jud.PoliceArea_id policearea_id,jud.addDate adddate,jud.*,bu.unit unitname,
                    bpa.AreaName policeareaname,buser.RealName addusername,br.RoomName roomname from JW_Usedetail jud
                    join Base_Unit bu on jud.unit_id=bu.Base_Unit_id
                    join Base_PoliceArea bpa on jud.PoliceArea_id=bpa.PoliceArea_id
                    join Base_User buser on jud.adduser_id=buser.UserId
                    join Base_Room br on jud.room_id=br.Room_id
                    where jud.Usedetail_id='{0}'
            ", jwUsedetail_id);
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

        public int SubmitUsedetailForm(string usedetail_id, string apply_id, string enddate, string quxiang)
        {
            string sql = string.Format(@"
                    update JW_Usedetail set enddate='{0}',isend='1',quxiang='{3}' where Usedetail_id='{1}';
                    update JW_Apply set state=3 where apply_id='{2}';
                ", enddate, usedetail_id, apply_id, quxiang);

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using System.Data;
using System.Data.SqlClient;

namespace LeaRun.Business
{
    public partial class JW_Apply_room_ZDJSBll : RepositoryFactory<JW_Apply_room>
    {
        /// <summary>
        /// 人员第一次进入的时候，向数据库插入数据
        /// </summary>
        /// <param name="jwUsedetail"></param>
        /// <returns></returns>
        public int FirstCheckIn(JW_Usedetail jwUsedetail)
        {
            //1.获取当前监居区所在单位的ID
            string sqlPoliceUnitId = string.Format(@"
                        select pa.unit_id from JW_Apply ja
                        join Base_PoliceArea pa on ja.PoliceArea_id=pa.PoliceArea_id
                        where ja.apply_id='{0}'", jwUsedetail.apply_id);
            DataTable dtPoliceUnitId = SqlHelper.DataTable(sqlPoliceUnitId, CommandType.Text);
            string unit_id = dtPoliceUnitId.Rows[0]["unit_id"].ToString();

            JW_Apply_room jwApplyRoom = new JW_Apply_room()
            {
                apply_room_id = Guid.NewGuid().ToString(),
                unit_id = unit_id,
                apply_id = jwUsedetail.apply_id,
                adduser_id = jwUsedetail.adduser_id,
                adddate = jwUsedetail.addDate,
                Room_id = jwUsedetail.room_id,
                startdate = jwUsedetail.startdate,
                state = 1
            };

            string sqlInsert = string.Format(@"
insert into JW_Apply_room(apply_room_id,unit_id,apply_id,adduser_id,adddate,Room_id,startdate,state)
values(@apply_room_id,@unit_id,@apply_id,@adduser_id,@adddate,@Room_id,@startdate,@state)");

            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@apply_room_id",jwApplyRoom.apply_room_id),
                new SqlParameter("@unit_id",jwApplyRoom.unit_id),
                new SqlParameter("@apply_id",jwApplyRoom.apply_id),
                new SqlParameter("@adduser_id",jwApplyRoom.adduser_id),
                new SqlParameter("@adddate",jwApplyRoom.adddate==null?(object)DBNull.Value:jwApplyRoom.adddate),
                new SqlParameter("@Room_id",jwApplyRoom.Room_id),
                new SqlParameter("@startdate",jwApplyRoom.startdate==null?(object)DBNull.Value:jwApplyRoom.startdate),
                new SqlParameter("@state",jwApplyRoom.state),
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

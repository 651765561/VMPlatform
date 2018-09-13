using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Entity;
using LeaRun.Entity.CommonModule;
using LeaRun.Repository;

namespace LeaRun.Business.CommonModule
{
    public partial class JW_ChangeRoomZDJSBll
    {
        /// <summary>
        /// 更换房间的时候，进行房间绑定
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable RoomListJson(string apply_id, string user_id, string unit_id)
        {
            string sql = string.Format(@" select br.* from JW_Apply ja
join Base_Room br on ja.PoliceArea_id=br.PoliceArea_id
where ja.apply_id='{0}' and Room_id not in (select room_id from JW_Apply_room where enddate is null) and br.state=1", apply_id);
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
        /// 人员进入 保存
        /// </summary>
        /// <param name="apply_id"></param>
        /// <param name="user_id"></param>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public int SubmitCheckInForm(JW_Apply_room jwApplyRoom, string unit_id)
        {
            //先判断JW_Apply_room表中是否有该apply_id的记录
            DataTable dt = new DataTable();
            string checksql = string.Format(@" select  * from JW_Apply_room where apply_id='{0}' and enddate is null ", jwApplyRoom.apply_id);
            try
            {
                dt = SqlHelper.DataTable(checksql, CommandType.Text);
                if (dt.Rows.Count == 1)
                {
                    //有数据
                    if (dt.Rows[0]["state"].ToString() != "1")
                    {
                        return Convert.ToInt32(dt.Rows[0]["state"].ToString());
                    }
                }
                else
                {
                    //数据异常
                    return -2;
                }
            }
            catch (Exception)
            {
                //数据异常
                return -2;
            }

            //获取监居区所在单位的主键
            string sqlGetAreaUnit = string.Format(@"select bu.*,ja.PoliceArea_id from Base_Unit bu
                                    join JW_Apply ja on bu.Base_Unit_id=ja.unit_id 
                                    where ja.apply_id='{0}'", jwApplyRoom.apply_id);
            string policeAreaId = SqlHelper.DataTable(sqlGetAreaUnit, CommandType.Text).Rows[0]["PoliceArea_id"].ToString();
            string unitId = SqlHelper.DataTable(sqlGetAreaUnit, CommandType.Text).Rows[0]["Base_Unit_id"].ToString();


            //1.更新JW_Apply_room; 2.插入JW_Apply_room； 场所安全检查

            string sql = string.Format(@"

update JW_Apply_room set enddate=@startdate,state=6 where apply_room_id=@apply_room_id;

insert into JW_Apply_room(apply_room_id,unit_id,apply_id,adduser_id,adddate,Room_id,startdate,state) 
values(NEWID(),@unit_id,@apply_id,@adduser_id,@adddate,@Room_id ,@startdate,@state);

--update JW_SendPolice set state=2 where type=3 and Object_id=@apply_id and user_id=@adduser_id and state=1;

insert into JW_SafetyCheck(SafetyCheck_id,unit_id,PoliceArea_id,apply_id,adduser_id,addDate,room_id,checkdate,detail) 
values(NEWID(),@unit_id,@PoliceArea_id,@apply_id,@adduser_id,GETDATE(),@Room_id,GETDATE(),'安全')
");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@apply_room_id",dt.Rows[0]["apply_room_id"].ToString()), 
                new SqlParameter("@unit_id",unitId), 
                new SqlParameter("@apply_id",jwApplyRoom.apply_id),
                new SqlParameter("@adduser_id",jwApplyRoom.adduser_id),
                new SqlParameter("@adddate",jwApplyRoom.adddate),
                new SqlParameter("@Room_id",jwApplyRoom.Room_id),
                new SqlParameter("@startdate",jwApplyRoom.startdate), 
                new SqlParameter("@state",jwApplyRoom.state),
                new SqlParameter("@PoliceArea_id",policeAreaId) 
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                if (r > 0)
                {
                    return 1;
                }
                else
                {
                    return -2;
                }
            }
            catch (Exception)
            {
                return -2;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using LeaRun.Utilities;
namespace LeaRun.Business
{
    public class JW_ScheduleBll : RepositoryFactory<JW_Schedule>
    {
        /// <summary>
        /// 根据unit_id来获取这个单位下的所有的办案区
        /// </summary>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable LoadPoliceAreaByUnitId(string unit_id)
        {

            //=====================================处理调警人员开始============================================================================================================


            // 处理调警令或调警申请中的法警---->处理方式：临时改变unit_id   （调警令:sp.type=7 ,  法警本人已确认:sp.state=1 , 调警令待执行状态:：op.state=4）
            string sqlOrderPolice = string.Format(@"
                     select to_unit_id from JW_SendPolice sp 
                     join JW_OrderPolice op on sp.Object_id=op.orderpolice_id 
                     where sp.type=7 and sp.state=1 and op.from_unit_id=(select companyid from base_user where userid='{0}') and op.state=4
                     and sp.user_id='{0}'
           ",ManageProvider.Provider.Current().UserId );
            DataTable dtOrderPolice = SqlHelper.DataTable(sqlOrderPolice, CommandType.Text);
            if (dtOrderPolice != null && dtOrderPolice.Rows.Count > 0)
            {
                unit_id = dtOrderPolice.Rows[0][0].ToString();
            }

            // 处理调警申请中的法警---->处理方式：临时改变unit_id   （调警令:sp.type=6 ,  法警本人已确认:sp.state=1 , 调警申请待执行状态:：ap.state=4）
            string sqlAssignPolice = string.Format(@"
                     select to_unit_id from JW_SendPolice sp 
                      join JW_AssignPolice ap on sp.Object_id=ap.callpolice_id 
                      join Base_User bu on sp.user_id=bu.UserId
                      where sp.type=6 and sp.state=1 and ap.from_unit_id=(select companyid from base_user where userid='{0}') and ap.state=4
                      and sp.user_id='{0}'
           ", ManageProvider.Provider.Current().UserId);
            DataTable dtAssignPolice = SqlHelper.DataTable(sqlAssignPolice, CommandType.Text);
            if (dtAssignPolice != null && dtAssignPolice.Rows.Count > 0)
            {
                unit_id = dtAssignPolice.Rows[0][0].ToString();
            }

            //=====================================处理调警人员开始============================================================================================================


            string sql = string.Format(@" select * from Base_PoliceArea where state=1 and unit_id='{0}' order by AreaType ", unit_id);
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
        /// 提交数据
        /// </summary>
        /// <param name="jwSchedule"></param>
        /// <returns></returns>
        public int SubmitFormData(JW_Schedule jwSchedule)
        {
            string sqlInsert = string.Format(@"insert into JW_Schedule(
                    Schedule_id,unit_id,PoliceArea_id,adduser_id,adddate,DutyUser_id,user_id,user_name,startdate,enddate,type,detail
                    ) values(
                    @Schedule_id,@unit_id,@PoliceArea_id,@adduser_id,GETDATE(),@DutyUser_id,@user_id,@user_name,@startdate,@enddate,@type,@detail
                    )");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@Schedule_id",jwSchedule.Schedule_id), 
                new SqlParameter("@unit_id",jwSchedule.unit_id), 
                new SqlParameter("@PoliceArea_id",jwSchedule.PoliceArea_id), 
                new SqlParameter("@adduser_id",jwSchedule.adduser_id), 
                new SqlParameter("@DutyUser_id",jwSchedule.DutyUser_id), 
                new SqlParameter("@user_id",jwSchedule.user_id), 
                new SqlParameter("@user_name",jwSchedule.user_name), 
                new SqlParameter("@startdate",jwSchedule.startdate), 
                new SqlParameter("@enddate",jwSchedule.enddate), 
                new SqlParameter("@type",jwSchedule.type), 
                new SqlParameter("@detail",jwSchedule.detail)
            };

            try
            {
                int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                return r;
            }
            catch (Exception exception)
            {
                return 0;
            }
        }

        public DataTable LoadFormData(string start, string end, string policeAreaType, string policeAreaID)
        {
            string sqlLoad =
                string.Format(@"select js.*,bu.RealName dutyUser_name from JW_Schedule js 
                                join Base_User bu on js.DutyUser_id=bu.UserId 
                                where js.PoliceArea_id='{0}' and startdate>'{1}' and startdate<'{2}'",
                    policeAreaID, start, end);
            try
            {
                DataTable dt = SqlHelper.DataTable(sqlLoad, CommandType.Text);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int DelFormData(string id)
        {
            string sqlDel = string.Format(@" delete JW_Schedule where Schedule_id='{0}' ", id);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int UpdateFormData(JW_Schedule jwSchedule)
        {
            string sqlUpdate = string.Format(@"update JW_Schedule set 
                                                                    unit_id=@unit_id
                                                                    ,PoliceArea_id=@PoliceArea_id
                                                                    ,adduser_id=@adduser_id
                                                                    ,adddate=GETDATE()
                                                                    ,DutyUser_id=@DutyUser_id
                                                                    ,user_id=@user_id
                                                                    ,user_name=@user_name
                                                                    ,startdate=@startdate
                                                                    ,enddate=@enddate
                                                                    ,type=@type
                                                                    ,detail=@detail  
                                                                    where 
                                                                    Schedule_id=@Schedule_id"
                                                                                    );

            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@Schedule_id",jwSchedule.Schedule_id), 
                new SqlParameter("@unit_id",jwSchedule.unit_id), 
                new SqlParameter("@PoliceArea_id",jwSchedule.PoliceArea_id), 
                new SqlParameter("@adduser_id",jwSchedule.adduser_id), 
                new SqlParameter("@DutyUser_id",jwSchedule.DutyUser_id), 
                new SqlParameter("@user_id",jwSchedule.user_id), 
                new SqlParameter("@user_name",jwSchedule.user_name), 
                new SqlParameter("@startdate",jwSchedule.startdate), 
                new SqlParameter("@enddate",jwSchedule.enddate), 
                new SqlParameter("@type",jwSchedule.type), 
                new SqlParameter("@detail",jwSchedule.detail)
            };

            try
            {
                int r = SqlHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, pars);
                return r;
            }
            catch (Exception exception)
            {
                return 0;
            }
        }




    }
}

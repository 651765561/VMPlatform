//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Entity.CommonModule;
using LeaRun.Utilities;
using System.Diagnostics;
using LeaRun.Entity;


namespace LeaRun.Business
{
    /// <summary>
    /// JW_PoliceApply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.18 10:00</date>
    /// </author>
    /// </summary>
    public class JW_PoliceApplyActionBll : RepositoryFactory<JW_PoliceApply>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();

               string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by apply_id) rowNumber
                                                ,pa.apply_id
                                               , pa.unit_id
                                               , pa.unitname
                                               , pa.dep_id
                                               , pa.depname
                                               , pa.adduser_id
                                               , pa.adddate
                                               , pa.tasktype_id
                                               , pa.usedate
                                               , pa.actuser_id
                                               , pa.state
                                               , sp.type
                                               , sp.sendPolice_id
                                        from  JW_SendPolice sp join JW_PoliceApply pa  on sp.object_id=pa.apply_id where  sp.state  in (1) and  sp.user_id ='{0}' 
                                      
                                        union select 
                                          distinct  ROW_NUMBER() over(order by sp.senddate desc) rowNumber
                                               , sp.sendPolice_id as apply_id
                                               , sp.unit_id
                                               , unit.unit as unitname
                                               , ondu.PoliceArea_id as dep_id
                                               , area.AreaName as depname
                                               , ondu.adduser_id as adduser_id
                                               , ondu.adddate as adddate
                                               , ondu.tasktype_id as tasktype_id
                                               , '' as usedate
                                               , '' as actuser_id
                                               , sp.state as state
                                               , sp.type
                                               , sp.sendPolice_id
                                              from JW_SendPolice sp  
                                        left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                                        left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                                        left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                                        where   sp.type in (1,2,3,8,9) and (sp.state  in (3) and ondu.state=99 ) and  sp.user_id ='{0}' and  Charindex('!@#$%^&*()',isnull(actdetail,''))<=0
                                     ) as a   "

                         , ManageProvider.Provider.Current().UserId
                        );

                string sql =
                string.Format(
                    @" select * from ( 
                                        {4}
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                    , (pageIndex - 1) * pageSize + 1
                    , pageIndex * pageSize
                    , jqgridparam.sidx
                    , jqgridparam.sord
                    , sqlTotal
                    );

                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                    page = jqgridparam.page, //当前页码
                    records = dt.Rows.Count, //总记录数
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
        /// 更新派警表内OBJECT_ID为 用警表对应ID的 STATE 为5实施完成
        /// </summary>
        /// <param name="keyValue">用警表对应ID</param>
        /// <returns></returns>
        public int UpdateState(string keyValue, string actdate, string actdetail, string enddate, string type, string user_id)
        {
            if (type == "submit")
            {
                //1.检测当前的这个派警是否是调警令来的
                string sqlCheckPJ =
                    string.Format(@"select * from JW_SendPolice where user_id='{0}' and type=7 and state=1", user_id);
                DataTable dtCheckPJ = SqlHelper.DataTable(sqlCheckPJ, CommandType.Text);
                if (dtCheckPJ.Rows.Count > 0)
                {
                    //是调警令来的
                    //1.结束这条调警记录，2改变调警表中的状态    update jw_orderpolice set state=5 where orderpolice_id='{1}';
                    string sqlProPJ = string.Format(@"update jw_sendpolice set state=3 where sendpolice_id='{0}'; 
                                                    
                                                    
                                                    ", dtCheckPJ.Rows[0]["sendpolice_id"], dtCheckPJ.Rows[0]["object_id"]);
                    SqlHelper.ExecuteNonQuery(sqlProPJ, CommandType.Text);
                }

                //1.检测当前的这个派警是否是调警申请来的
                string sqlCheckDJSQ =
                    string.Format(@"select * from JW_SendPolice where user_id='{0}' and type=6 and state=1", user_id);
                DataTable dtCheckDJSQ = SqlHelper.DataTable(sqlCheckDJSQ, CommandType.Text);
                if (dtCheckDJSQ.Rows.Count > 0)
                {
                    //是调警来的
                    //1.结束这条调警记录，2改变调警申请表中的状态  update JW_AssignPolice set state=5 where callpolice_id='{1}';
                    string sqlProDJSQ = string.Format(@"update jw_sendpolice set state=3 where sendpolice_id='{0}'; 
                                                     
                                                    
                                                    ", dtCheckDJSQ.Rows[0]["sendpolice_id"], dtCheckDJSQ.Rows[0]["object_id"]);
                    SqlHelper.ExecuteNonQuery(sqlProDJSQ, CommandType.Text);
                }


                string sqlSubmit = string.Format(@"update JW_SendPolice set state=3,actdate=@actdate,enddate=@enddate,actdetail=@actdetail  where object_id=@keyValue and user_id=@user_id;" );
                //查询是否所有人都执勤登记了
                string sqlCheckAllAction ="select count(1) from JW_SendPolice where object_id='"+keyValue+"' and state<>3";
                int iNoAction = Convert.ToInt32(SqlHelper.DataTable(sqlCheckAllAction, CommandType.Text).Rows [0][0]) ;
                if (iNoAction == 1)
                { 
                     sqlSubmit=sqlSubmit+" update JW_PoliceApply set state=5,actdate=@actdate,enddate=@enddate,actdetail=@actdetail where apply_id=@keyValue";
                }


                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@keyValue",keyValue), 
                    new SqlParameter("@actdate",actdate),
                    new SqlParameter("@enddate",enddate),
                    new SqlParameter("@actdetail",actdetail),
                    new SqlParameter("@user_id",user_id)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlSubmit, CommandType.Text, pars);
                    return 0;
                }
                catch (Exception)
                {
                    return -2;
                }
            }
            else if (type == "save")
            {
                string sqlSave = string.Format(@" update JW_PoliceApply set state=4 where apply_id=@keyValue;update JW_SendPolice set actdate=@actdate,enddate=@enddate,actdetail=@actdetail  where object_id=@keyValue and user_id=@user_id");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@keyValue",keyValue), 
                    new SqlParameter("@actdate",actdate),
                    new SqlParameter("@enddate",(enddate=="&nbsp;"||enddate=="")?(object)DBNull.Value:enddate),
                    new SqlParameter("@actdetail",actdetail),
                    new SqlParameter("@user_id",user_id)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlSave, CommandType.Text, pars);
                    return -1;
                }
                catch (Exception)
                {
                    return -2;
                }
            }
            else
            {
                return -2;
            }








            //string sql = " update JW_SendPolice set state=3 where object_id ='" + keyValue + "' ;update JW_PoliceApply set state=5,";
            //sql = sql + " actdate='" + actdate + "',actdetail='" + actdetail + "' where apply_id ='" + keyValue + "'";
            //try
            //{
            //    int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
            //    return r;
            //}
            //catch (Exception)
            //{
            //    return -1;
            //}
        }




        /// <summary>
        /// 更新派警表内OBJECT_ID为 用警表对应ID的 STATE 为5实施完成
        /// </summary>
        /// <param name="keyValue">用警表对应ID</param>
        /// <returns></returns>
        public int UpdateState_GDCS(string keyValue, string actdate, string actdetail, string enddate, string type, string user_id)
        {
            if (type == "submit")
            {
                string sqlSubmit = string.Format(@"update JW_SendPolice set state=3,actdate=@actdate,enddate=@enddate,actdetail=@actdetail  where SendPolice_id=@keyValue;");

                //查询是否所有人都执勤登记了
                string sqlCheckAllAction = "select count(1) from JW_SendPolice where object_id=(select object_id from  JW_SendPolice where SendPolice_id='" + keyValue + "') and state=3 and Charindex('!@#$%^&*()',actdetail)<=0 ";
                int iNoAction = Convert.ToInt32(SqlHelper.DataTable(sqlCheckAllAction, CommandType.Text).Rows[0][0]);
                if (iNoAction == 1)
                {
                    sqlSubmit = sqlSubmit + " update JW_OnDuty set state=1 where OnDuty_id=(select object_id from  JW_SendPolice where SendPolice_id='" + keyValue + "');update JW_SendPolice set actdetail=replace(actdetail,'!@#$%^&*()','')  where object_id=(select object_id from  JW_SendPolice where SendPolice_id='" + keyValue + "');";
                }

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@keyValue",keyValue), 
                    new SqlParameter("@actdate",actdate),
                    new SqlParameter("@enddate",enddate),
                    new SqlParameter("@actdetail",actdetail+"!@#$%^&*()"),//代表执勤已结束但没有登记
                    new SqlParameter("@user_id",user_id)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlSubmit, CommandType.Text, pars);
                    return 0;
                }
                catch (Exception)
                {
                    return -2;
                }
            }
            else if (type == "save")
            {
                string sqlSave = string.Format(@"update JW_SendPolice set actdate=@actdate,enddate=@enddate,actdetail=@actdetail  where SendPolice_id=@keyValue;");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@keyValue",keyValue), 
                    new SqlParameter("@actdate",actdate),
                    new SqlParameter("@enddate",(enddate=="&nbsp;"||enddate=="")?(object)DBNull.Value:enddate),
                    new SqlParameter("@actdetail",actdetail),
                    new SqlParameter("@user_id",user_id)
                };
                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sqlSave, CommandType.Text, pars);
                    return -1;
                }
                catch (Exception)
                {
                    return -2;
                }
            }
            else
            {
                return -2;
            }

         
           
        }




        /// <summary>
        /// 获得登陆者信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetInfo(string keyValue, string UserId)
        {
            string sql = string.Format(@" 
                select 
                actdate,enddate,actdetail from jw_sendpolice
                WHERE object_id='{0}' and User_Id='{1}' 
                "
                , keyValue
                , UserId
                );
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

      
    }
}
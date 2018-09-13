//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using LeaRun.DataAccess;
using System;

namespace LeaRun.Business
{
    /// <summary>
    /// People_Clear
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.02.04 18:23</date>
    /// </author>
    /// </summary>
    public class People_ClearBll : RepositoryFactory<People_Clear>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageJsonMyQuery(string ParameterJson, JqGridParam jqgridparam, string designation, string startdate, string enddate, string state)
        {
             string user_id = ManageProvider.Provider.Current().UserId;
            //string unit_id = ManageProvider.Provider.Current().CompanyId;

            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select p.people_id,r.name as room_id,p.designation,p.name,p.sex,p.account,p.adddate as startdate,p.enddate,p.state from people p";                              
                sql = sql + " left join Base_Room r on r.room_id=p.room_id";
              


                //sql = sql + " where p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                //  sql = sql + " order by p.People_id";


                if (designation != "" && designation != null)//番号
                {
                    sql = sql + " where p.designation like '" + designation.Trim() + "'";
                    if (startdate != "")//入所开始日期
                    {
                        sql = sql + " and  p.adddate> '" + startdate + "'";
                    }
                    if (enddate != "")//入所结束日期
                    {
                        sql = sql + " and  p.adddate< '" + enddate + "'";

                    }

                    if (state != "")
                    {
                        sql = sql + " and p.state='" + state + "'";
                    }
                }
               
                //if (room_id != "")//监室
                //{
                //    sql = sql + " and p.room_id='" + room_id + "'";
                //}
                //if (name != "")//姓名
                //{
                //    sql = sql + " and p.name like '%" + name.Trim() + "%'";
                //}
                //if (sex != "")//性别
                //{
                //    sql = sql + " and p.sex = '" + sex + "'";
                //}
                //if (startdate != "")//入所开始日期
                //{
                //    sql = sql + " and  p.startdate> '" + startdate + "'";
                //}
                //if (enddate != "")//入所结束日期
                //{
                //    sql = sql + " and  p.startdate< '" + enddate + "'";
                //}
                //sql = sql + " order by p.People_id " + jqgridparam.sord;


                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dt.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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

        //获取指定行数据并获取从表相应数据
        public DataTable getRowData(string KeyValue)
        {
            // Stopwatch watch = CommonHelper.TimerStart();
            try
            {
                string sql = "select p.people_id,r.name as room_id,p.designation,p.name,p.sex,p.account,p.adddate as startdate,p.enddate,p.state from people p";
                sql = sql + " left join Base_Room r on r.room_id=p.room_id";
                sql = sql + " where p.people_id='" + KeyValue +"'";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }

            catch (Exception)
            {
                return null;
            }


        }

        //获取当前用户
        public string getUser(string UserId)
        {


            string sql = "select RealName from Base_User where UserId='" + UserId + "'";
            DataTable dt = Repository().FindTableBySql(sql);
            if (dt.Rows.Count > 0)
            {
                string s = dt.Rows[0][0].ToString();
                return s;
            }
            else
            {
                return "";
            }



        }

        //执行出所清户
        public string  clear(string people_id)
        {
            // Stopwatch watch = CommonHelper.TimerStart();
            try
            {
                string sql = "update people  set  account=0,state=2 ";

                sql = sql + " where people_id='" + people_id + "'";
                DataTable dt = Repository().FindTableBySql(sql);
                return "success";
            }

            catch (Exception)
            {
                return null;
            }


        }
        //清户时发送一条资金往来，转移剩余的钱
        public string  insertPeopleMoney(string people_id,string designation, decimal account,string enddate ) {
            string user_id = ManageProvider.Provider.Current().UserId;
            
            string time = DateTime.Now.ToString();
            int state = 0;
            //if (Convert.ToDateTime(enddate) >= Convert.ToDateTime(time))
            //{
            //    state = 0;
            //}
            //else
            //{
            //    state = 3;
            //}
            try { 
                string id = Guid.NewGuid().ToString();
                string sql = "insert into People_Money(People_Money.peoplemoney_id,People_Money.people_id,People_Money.designation,People_Money.addUser,People_Money.adddate,People_Money.MoneyType_id,People_Money.account,People_Money.design,People_Money.checker,People_Money.state)" +
                    "values ('" + id + "', '" + people_id + "','" + designation + "','" + user_id + "','" + time + "',(select Base_MoneyType.MoneyType_id from Base_MoneyType where name='清户退款'),"
                    + "'"+ account +"','清户退款',' ' ,'"+ state +"')";
                DataTable dt = Repository().FindTableBySql(sql);
                return "success";
                    
            }catch(Exception){
                return null;
            }
           
        }
    }
}
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
using System;
using LeaRun.DataAccess;

namespace LeaRun.Business
{
    /// <summary>
    /// People_Money_Check
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.02.01 14:37</date>
    /// </author>
    /// </summary>
    public class People_Money_CheckBll : RepositoryFactory<People_Money_Check>
    {
        string user_id = ManageProvider.Provider.Current().UserId;
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageJsonMyQuery(string ParameterJson, JqGridParam jqgridparam, string designation, string moneytype, string area, string startdate, string enddate ,string state)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select m.peoplemoney_id,m.designation,b.name as room_id,p.name,t.name as MoneyType_id,m.account,m.adddate,m.design,Base_User.RealName as checker,m.state,";
                sql = sql + " CONVERT(varchar(20),m.checkdate, 120) checkdate  from people_money m";
                sql = sql + " left join people p on p.people_id=m.people_id";
                sql = sql + " left join Base_Room b on b.room_id=p.room_id";
                sql = sql + " left join Base_MoneyType t on t.moneytype_id=m.moneytype_id";
                sql = sql + " left join Base_User  on m.checker=Base_User.UserId";


                //sql = sql + " where p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                //  sql = sql + " order by p.People_id";


                if (designation != ""&& designation!=null)//番号
                {
                    sql = sql + " where m.designation like '" + designation.Trim() + "'";
                    if (startdate != "")//入所开始日期
                    {
                        sql = sql + " and  p.adddate> '" + startdate + "'";
                    }
                    if (enddate != "")//入所结束日期
                    {
                        sql = sql + " and  p.adddate< '" + enddate + "'";

                    }
                    if (moneytype != "")
                    {
                        sql = sql + " and t.MoneyType_id='" + moneytype + "'";
                    }
                    if(state!="")
                    {
                        sql = sql + " and m.state='" + state + "'";
                    }
                }
                else {
                    sql = sql + " where m.state = 0";
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
                string sql = "select m.peoplemoney_id,m.designation,b.name as room_id,p.name as name,p.selfmoney,t.name as MoneyType_id,m.account,m.adddate,m.design,m.checker,m.state,";
                sql = sql + " CONVERT(varchar(20),m.checkdate, 120) checkdate  from people_money m";
                sql = sql + " left join people p on p.people_id=m.people_id";
                sql = sql + " left join Base_Room b on b.room_id=p.room_id";
                sql = sql + " left join Base_MoneyType t on t.moneytype_id=m.moneytype_id";
                sql = sql + " where m.peoplemoney_id ='" + KeyValue + "'";
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

        public DataTable getMoneyType()
        {
            try
            {
                string sql = "select name from Base_MoneyType ";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }


        //获取当前资金类型的id
        public string getMoneyTypeId(string MoneyType)
        {


            string sql = "select MoneyType_id from Base_MoneyType where name='" + MoneyType + "'";
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


        //获取当前监区的id
        public string getAreaId(string Area)
        {


            string sql = "select Area_id from Base_Area where name='" + Area + "'";
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

        public DataTable getArea()
        {
            try
            {
                string sql = "select name from Base_Area ";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string getState(string KeyValue) {
            string sql = "select state from people_money where peoplemoney_id='" + KeyValue + "'";
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

        //审核
        public DataTable check(string KeyValue)
        {

            string time = DateTime.Now.ToString();
            //string sql =
            //     string.Format(@"insert into Base_Room(Name,Code,Area_id,User_id ) values (@name,@code,@areaid,@userid)");
            string id = Guid.NewGuid().ToString();
            string sql = "update people_money set state=1,checker='"+ user_id 
                        +"',checkdate='"+ time +"' where peoplemoney_id='" + KeyValue + "'";
            try
            {
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        //弃审
        public DataTable checkCancel(string KeyValue)
        {

            string time = DateTime.Now.ToString();
            //string sql =
            //     string.Format(@"insert into Base_Room(Name,Code,Area_id,User_id ) values (@name,@code,@areaid,@userid)");
            string id = Guid.NewGuid().ToString();
            string sql = "update people_money set state=0,checker='" + user_id
                        + "',checkdate='" + time + "' where peoplemoney_id='" + KeyValue + "'";
            try
            {

                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }
}
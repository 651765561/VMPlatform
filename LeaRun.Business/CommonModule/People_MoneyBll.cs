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
    /// People_Money
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 11:53</date>
    /// </author>
    /// </summary>
    public class People_MoneyBll : RepositoryFactory<People_Money>
    {
        
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageJsonMyQuery(string ParameterJson, JqGridParam jqgridparam, string designation, string moneytype,string area, string startdate, string enddate)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
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


                if (designation != "" && designation != null)//番号
                {
                    sql = sql + " where m.designation = '" + designation.Trim() + "'";
                    if (startdate != "")//开始日期
                    {
                        sql = sql + " and  m.adddate> '" + startdate + "'";
                    }
                    if (enddate != "")//结束日期
                    {
                        sql = sql + " and  m.adddate< '" + enddate + "'";

                    }
                    if (moneytype != "")
                    {
                        sql = sql + " and t.MoneyType_id='" + moneytype + "'";
                    }
                    if (area != "")
                    {
                        sql = sql + " and b.Area_id='" + area + "'";
                    }
                }
                else {
                    sql = sql + " order by m.adddate desc";
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

        /// <summary>
        /// 获取打印的数据
        /// </summary>
        public DataTable GetPrintInfo(string KeyValue)
        {
            string sql =" select m.peoplemoney_id,m.designation,b.name as room_id,p.name,t.name as MoneyType_id,m.account,m.adddate,m.design,Base_User.RealName as checker,m.state,";
            sql = sql + " CONVERT(varchar(20),m.checkdate, 120) checkdate  from people_money m";
            sql = sql + " left join people p on p.people_id=m.people_id";
            sql = sql + " left join Base_Room b on b.room_id=p.room_id";
            sql = sql + " left join Base_MoneyType t on t.moneytype_id=m.moneytype_id";
            sql = sql + " left join Base_User  on m.checker=Base_User.UserId";
            sql = sql + " where 1=1 ";
            if (KeyValue.Length>0)
            {
                sql += " and m.peoplemoney_id ='" + KeyValue+"'";
             }
          return  DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];
        }


        public DataTable GetAllPrintInfo(string KeyValue)
        {
            string sql = " select m.peoplemoney_id,m.designation,b.name as room_id,p.name,t.name as MoneyType_id,m.account, CONVERT(varchar(20),m.adddate, 23) adddate,m.design,Base_User.RealName as checker,m.state,";
            sql = sql + " CONVERT(varchar(20),m.checkdate, 120) checkdate,ar.Code   from people_money m";
            sql = sql + " left join people p on p.people_id=m.people_id";
            sql = sql + " left join Base_Room b on b.room_id=p.room_id";
            sql = sql + " left join Base_MoneyType t on t.moneytype_id=m.moneytype_id";
            sql = sql + " left join Base_User  on m.checker=Base_User.UserId";
            sql = sql + "  left join Base_Area ar on b.Area_id=ar.Area_id";
            sql = sql + " where 1=1 ";
            if (KeyValue.Length > 0)
            {
                sql += " and m.peoplemoney_id  in (" + KeyValue + ")  order by  name ,MoneyType_id";
            }
            return DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];
        }



        //添加资金往来记录
        public int AddPeopleMoney(People_Money peoplemoney)
        {

               
                //string sql =
                //     string.Format(@"insert into Base_Room(Name,Code,Area_id,User_id ) values (@name,@code,@areaid,@userid)");
                string id = Guid.NewGuid().ToString();
                string sql = "insert into People_Money(peoplemoney_id,people_id,designation,addUser,adddate,MoneyType_id,account,design,checker,state)" +
                    "values ('" + id + "', " +
                    "(select People.People_id from People where People.name='"+ peoplemoney.people_id +"'),'" +                    
                    peoplemoney.designation + "'," +
                    "(select Base_User.UserId from Base_User where Base_User.RealName='" + peoplemoney.addUser + "'),'" + 
                    peoplemoney.adddate + "'," +
                     "(select Base_MoneyType.MoneyType_id from Base_MoneyType where Base_MoneyType.name='" + peoplemoney.MoneyType_id + "'),'" + 
                    
                     peoplemoney.account + "','" +
                     peoplemoney.design + "','" +
                    peoplemoney.checker + "','" +
                   // peoplemoney.checkdate + "','" +
                    peoplemoney.state + "')";



                try
                {

                    int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            

        }

        //修改资金往来记录
        public int updatePeopleMoney(People_Money peoplemoney,string KeyValue)
        {


            //string sql =
            //     string.Format(@"insert into Base_Room(Name,Code,Area_id,User_id ) values (@name,@code,@areaid,@userid)");
            string id = Guid.NewGuid().ToString();
            string sql = "update People_Money set peoplemoney_id ='" + id + "',";
            sql=sql+"people_id=(select People.People_id from People where People.name='" + peoplemoney.people_id + "'),";
            sql=sql+"designation='" + peoplemoney.designation + "',";
            sql=sql+"addUser=(select Base_User.UserId from Base_User where Base_User.RealName='" + peoplemoney.addUser + "'),";
            sql=sql+"adddate='" +peoplemoney.adddate + "',";
            sql=sql+"MoneyType_id=(select Base_MoneyType.MoneyType_id from Base_MoneyType where Base_MoneyType.name='" + peoplemoney.MoneyType_id + "'),";
            sql = sql + "account='" + peoplemoney.account + "',design='" + peoplemoney.design + "',checker='" + peoplemoney.checker + "',state='" + peoplemoney.state + "' where peoplemoney_id='" + KeyValue + "'";
              
  



            try
            {

                
                int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                return r;
            }
            catch (Exception ex)
            {
                return 0;
            }


        }

        //获取指定行数据并获取从表相应数据
        public DataTable getRowData(string KeyValue)
        {
            // Stopwatch watch = CommonHelper.TimerStart();
            try
            {
                string sql = "select pm.designation,p.name as people_id,p.account as selfmoney,mt.name as MoneyType_id,pm.account,pm.design from people_money pm ";
                 sql = sql + " left join people p on p.people_id=pm.people_id";
                 sql = sql + " left join base_moneytype mt on mt.moneytype_id=pm.moneytype_id";
                 sql = sql + " where pm.peoplemoney_id='"+KeyValue+"'";
                //" LEFT JOIN Base_GoodsType" +
                //" ON Base_Goods.goodstype_id=Base_GoodsType.goodstype_id" +
               
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }

            catch (Exception)
            {
                return null;
            }


        }

        //通过番号获取指定行人员数据
        public DataTable getPeople(string KeyValue)
        {
            // Stopwatch watch = CommonHelper.TimerStart();
            try
            {
                string sql = "select name as people_id,room_id,selfmoney  " +
                " FROM people" +
                    //" LEFT JOIN Base_GoodsType" +
                    //" ON Base_Goods.goodstype_id=Base_GoodsType.goodstype_id" +
                " where designation='" + KeyValue + "'";
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
            else {
                return "";
            }
            
            
            
        }

        public DataTable getMoneyType(string code)
        {
            try
            {
                string sql = "select name from Base_MoneyType where code='"+code+"'";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DataTable getMoneyTypeName()
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
        public DataTable getAreaId(string Area)
        {

            try
            {
                string sql = "select Area_id from Base_Area where name='" + Area + "'";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
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
        //根据id删除数据
        public int del(string strKeyValue)
        {
            string sql = string.Format(@" delete from people_money where peoplemoney_id ='" + strKeyValue + "'");
            try
            {
                int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

      
    }
}
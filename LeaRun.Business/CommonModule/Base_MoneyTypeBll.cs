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
using System.Data.SqlClient;
using System.Data;
using System;
using LeaRun.DataAccess;
using System.Diagnostics;



namespace LeaRun.Business
{
    /// <summary>
    /// Base_MoneyType
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.10 09:35</date>
    /// </author>
    /// </summary>
    public class Base_MoneyTypeBll : RepositoryFactory<Base_MoneyType>
    {

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetMoneyType(string ParameterJson, JqGridParam jqgridparam,string queryMessage)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select MoneyType_id,name,type,state,code,orderby from base_MoneyType";
                if (queryMessage != "" && queryMessage != null)
                {
                    sql = sql + " where name like  '%" + queryMessage +
                        "%' or type like '%" + queryMessage +
                        "%' or state like '%" + queryMessage +
                        "%' or code like  '%" + queryMessage +
                        "%' or orderby like '%" + queryMessage +"%'";
                       

                }
                sql = sql + " order by  CAST(code as int)  asc";

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
        //检查是否重复
        public DataTable check(string code)
        {
            string sql = "select * from base_moneytype where code='" + code + "'";
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



        public int AddMoneyType(Base_MoneyType moneyType, string strKeyValue)
        {
            if (strKeyValue == "")//新增
            {
                string id = Guid.NewGuid().ToString();
                //string sql =
                //     string.Format(@"insert into Base_MoneyType(name,type,state,code,orderby ) values (@name,@type,@state,@code,@orderby)");
                string sql = "insert into Base_MoneyType(MoneyType_id,name,type,state,code,orderby) values ( '"+ id +"','" + moneyType.name + "','" + moneyType.type + "','" + moneyType.state + "','" + moneyType.code + "','" + moneyType.orderby + "')";
                //SqlParameter[] pars = new SqlParameter[]
                //{
                //    new SqlParameter("@name",moneyType.name),
                //    new SqlParameter("@type",moneyType.type),
                //    new SqlParameter("@state",moneyType.state),
                //    new SqlParameter("@code",moneyType.code),
                //    new SqlParameter("@orderby",moneyType.orderby)
                  
                //};

                try
                {

                    int r = DbHelper.ExecuteNonQuery(CommandType.Text,sql );
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else
            {
                string sql =
                    string.Format(@"update Base_MoneyType set name=@name,type=@type,state=@state,code=@code,orderby=@orderby where MoneyType_id=@KeyValue");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@name",moneyType.name),
                    new SqlParameter("@type",moneyType.type),
                    new SqlParameter("@state",moneyType.state),
                    new SqlParameter("@code",moneyType.code),
                    new SqlParameter("@orderby",moneyType.orderby),
                    new SqlParameter("@KeyValue",strKeyValue.ToString())
                  
                };

                try
                {
                    int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql, pars);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 根据对象Id获取数据
        /// </summary>
        /// <param name="KeyValue">对象ID</param>
        /// <returns></returns>
        public DataTable GetForm(string strKeyValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  name ,
                                    type ,
                                    state ,
                                    code,
                                    orderby,
                            FROM    Base_MoneyType where MoneyType_id='" + strKeyValue + "';");

            
            return Repository().FindTableBySql(strSql.ToString());
        }

       //根据id删除数据
        public int del(string strKeyValue)
        {
            string sql = string.Format(@" delete from Base_MoneyType where MoneyType_id ='" + strKeyValue + "'");
            try
            {
                int r = DbHelper.ExecuteNonQuery(CommandType.Text,sql );
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
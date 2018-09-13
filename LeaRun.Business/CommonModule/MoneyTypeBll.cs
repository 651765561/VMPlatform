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

namespace LeaRun.Business
{
    /// <summary>
    /// MoneyType
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.06 16:13</date>
    /// </author>
    /// </summary>
    public class MoneyTypeBll : RepositoryFactory<MoneyType>
    {
        public int AddMoneyType(MoneyType moneyType, string strKeyValue)
        {
            if (strKeyValue == "")//新增
            {
                string sql =
                     string.Format(@"insert into MoneyType(name,type,state,code,orderby ) values (@name,@type,@state,@code,@orderby)");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@name",moneyType.name),
                    new SqlParameter("@Code",moneyType.type),
                    new SqlParameter("@Code",moneyType.state),
                    new SqlParameter("@Code",moneyType.code),
                    new SqlParameter("@Code",moneyType.orderby)
                  
                };

                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
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
                    string.Format(@"update MoneyType set name=@name,type=@type,state=@state,code=@code,orderby=@orderby where id=@KeyValue");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@name",moneyType.name),
                    new SqlParameter("@type",moneyType.type),
                    new SqlParameter("@state",moneyType.state),
                    new SqlParameter("@code",moneyType.code),
                    new SqlParameter("@orderby",moneyType.orderby),
                    new SqlParameter("@KeyValue",Convert.ToInt32(strKeyValue))
                  
                };

                try
                {
                    int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        ///// <summary>
        ///// 判断当前监区是否有监室
        ///// </summary>
        ///// <param name="dep_id">部门ID</param>
        ///// <returns></returns>
        //public string HasRoom(string areaid)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select  count(1) from Room where Areaid='" + areaid + "'");
        //    return DbHelper.ExecuteScalar(CommandType.Text, strSql.ToString()).ToString();
        //}
        public int del(string strKeyValue)
        {
            string sql = string.Format(@" delete from MoneyType where id ='" + strKeyValue + "'");
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
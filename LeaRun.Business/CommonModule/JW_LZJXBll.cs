//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System;
using System.Diagnostics;


namespace LeaRun.Business
{
    /// <summary>
    /// JW_LZJX
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.20 11:31</date>
    /// </author>
    /// </summary>
    public class JW_LZJXBll : RepositoryFactory<JW_LZJX>
    {

        public DataTable GetData(string LZJX_id, string type)
        {
            StringBuilder strSql = new StringBuilder();

            if (type == "add")
            {
                strSql.Append("SELECT * from JW_LZJXBase where state=1");
            }
            else
            {
                strSql.Append("SELECT  * from JW_LZJX  where LZJX_id='" + LZJX_id + "'");
            }
          
            return Repository().FindTableBySql(strSql.ToString());
        }


        public DataTable GetDataCount(string LZJX_id, string type)
        {
            StringBuilder strSql = new StringBuilder();

            if (type == "add")
            {
                strSql.Append("SELECT  count(*)  FROM JW_LZJXBase  where state=1");
            }
            else
            {
                strSql.Append("SELECT  itemCount from JW_LZJX  where LZJX_id='" + LZJX_id + "'");
            }

            return Repository().FindTableBySql(strSql.ToString());
        }


        public int DeleteJW_LZJX(string LZJX_id)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("DELETE  FROM  JW_LZJX  where LZJX_id='" + LZJX_id + "'");
            return Repository().ExecuteBySql(strSql);
        }




        public string GridPageJsonMy(JqGridParam jqgridparam)
        {
            try
            {
                string unit_id = ManageProvider.Provider.Current().CompanyId;
                string user_id = ManageProvider.Provider.Current().UserId;
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" 
                                            select 
                                                ROW_NUMBER() over(order by LZJX_id) rowNumber,
                                                 LZJX_id,unit_id,adduser_id,convert(varchar(20),adddate,120) adddate
                                                ,itemCount,item1,value1,item2,value2,item3,value3,item4,value4,item5,value5,item6,value6
                                                ,item7,value7,item8,value8,item9,value9
                                                ,item10,value10,item11,value11,item12,value12,item13,value13,item14,value14,item15,value15
                                                ,unit.unit,use1.RealName
                                                 FROM JW_LZJX LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                                 LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id  
                            "
                            );
                if (unit_id != Share.UNIT_ID_JS)
                {
                    sqlTotal = sqlTotal + " WHERE JW_LZJX.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + unit_id + "' or parent_unit_id ='" + unit_id + "') ";
                }
                else
                {
                    //sqlTotal=sqlTotal+"  where 1=1  ";

                }

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

                 string sql2 =
              string.Format(
                @" select * from ( 
                                        {4}
                                        ) as a  
                                      "
                , (pageIndex - 1) * pageSize + 1
                , pageIndex * pageSize
                , jqgridparam.sidx
                , jqgridparam.sord
                , sqlTotal
                );
                 DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

                   var JsonData = new
                    {
                        total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                        page = jqgridparam.page, //当前页码
                        records = dt2.Rows.Count, //总记录数
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
     

    }
}
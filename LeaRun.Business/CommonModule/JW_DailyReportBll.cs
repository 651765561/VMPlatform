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
    /// JW_DailyReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.21 14:06</date>
    /// </author>
    /// </summary>
    public class JW_DailyReportBll : RepositoryFactory<JW_DailyReport>
    {
        public int DeleteJW_DailyReport(string DailyReport_id)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("DELETE  FROM  JW_DailyReport  where DailyReport_id='" + DailyReport_id + "'");
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
                        @" select * from ( 
                                            select 
                                                ROW_NUMBER() over(order by adddate desc) rowNumber,
                                                 DailyReport_id,unit_id,adduser_id,convert(varchar(20),adddate,120) adddate
                                                ,reportYear,reportNum,reportAllNum,basicinfo,importantinfo 
                                                ,dailyinfo,submit,deliver,editing,review,issue
                                                ,case JW_DailyReport.needReport when 1 then '上报' else '不上报' end needReport
                                                ,case JW_DailyReport.needPublish when 1 then '发布' else '不发布' end needPublish
                                                ,unit.unit,use1.RealName
                                                 FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                                 LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id   
                                                 where adduser_id='{0}'
                                               ) as a  where 1=1
                                           "
                            , user_id
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

        public DataSet Getbasicinfo()
        {
            DataSet ds = new DataSet();
            StringBuilder strSql_basicinfo1 = new StringBuilder();
            StringBuilder strSql_basicinfo2 = new StringBuilder();
            StringBuilder strSql_basicinfo3 = new StringBuilder();

            strSql_basicinfo1.Append(" select  count(1)  from  JW_SendPolice where state>=0 and unit_id='"+ManageProvider.Provider.Current().CompanyId+"'");
            strSql_basicinfo1.Append(" and convert(varchar(10),SendDate,120)=convert(varchar(10),getdate(),120) group by Object_id");
            strSql_basicinfo2.Append(" select  count(1)  from  JW_SendPolice where state>=0 and unit_id='"+ManageProvider.Provider.Current().CompanyId+"'");
            strSql_basicinfo2.Append(" and convert(varchar(10),SendDate,120)=convert(varchar(10),getdate(),120) group by Object_id,user_id");
            strSql_basicinfo3.Append(" select  item1,value1 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value1,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item2,value2 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value2,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item3,value3 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value3,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item4,value4 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value4,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item5,value5 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value5,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item6,value6 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value6,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item7,value7 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value7,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item8,value8 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value8,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item9,value9 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value9,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item10,value10 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value10,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item11,value11 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value11,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item12,value12 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value12,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item13,value13 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value13,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item14,value14 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value14,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
            strSql_basicinfo3.Append(" union ");
            strSql_basicinfo3.Append(" select  item15,value15 from JW_LZJX where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and isnull(value15,0)<>0 ");
            strSql_basicinfo3.Append(" and convert(varchar(10),adddate,120)=convert(varchar(10),getdate(),120)");
           
            try
            {
                DataTable dt_basicinfo1 = SqlHelper.DataTable(strSql_basicinfo1.ToString(), CommandType.Text);
                DataTable dt_basicinfo2 = SqlHelper.DataTable(strSql_basicinfo2.ToString(), CommandType.Text);
                DataTable dt_basicinfo3 = SqlHelper.DataTable(strSql_basicinfo3.ToString(), CommandType.Text);
                ds.Tables.Add(dt_basicinfo1);
                ds.Tables.Add(dt_basicinfo2);
                ds.Tables.Add(dt_basicinfo3);
                return ds;
                
            }
            catch (Exception)
            {
                return null;
            }

        }


        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel()
        {

           
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                       
                                            select 
                                                 unit.unit as '报告单位'
                                                ,use1.RealName as '报告人员'
                                                ,convert(varchar(20),adddate,120) as '日报日期'
                                                ,reportYear as '报告年份'
                                                ,reportNum as '报告期数'
                                                ,reportAllNum as '报告总期数'
                                                ,replace(basicinfo,'&nbsp;','') as '警务基本信息'
                                                ,replace(importantinfo,'&nbsp;','') as '重要警务信息'
                                                ,replace(dailyinfo,'&nbsp;','') as '日常工作信息'
                                                ,submit as '报'
                                                ,deliver as '送'
                                                ,editing as '编校'
                                                ,review as '审核'
                                                ,issue as '签发'
                                                ,case JW_DailyReport.needReport when 1 then '上报' else '不上报' end as '是否上报'
                                                ,case JW_DailyReport.needPublish when 1 then '发布' else '不发布' end as '是否发布'
                                               
                                                 FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                                 LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id   
                                                 where adduser_id='{0}'
                                              
                                           "
                            , user_id
                            );


                DataTable dt = SqlHelper.DataTable(sqlTotal, CommandType.Text);//Repository().FindTableBySql(sql);


                return dt;
            }
            catch (Exception)
            {
                return null;
            }



        }
      
    }
}
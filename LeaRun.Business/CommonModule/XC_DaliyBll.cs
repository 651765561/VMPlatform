//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2018
// Software Developers @ Learun 2018
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

namespace LeaRun.Business
{
    /// <summary>
    /// XC_Daliy
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.04.19 11:19</date>
    /// </author>
    /// </summary>
    public class XC_DaliyBll : RepositoryFactory<XC_Daliy>
    {
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
                                                 xc_daliy_id,unit_id,adduser_id,convert(varchar(10),adddate,120) adddate
                                                ,reportYear,reportNum,reportAllNum,basicinfo,xcinfo 
                                                ,operationinfo,videoinfo,submit,deliver,editing,review
                                                ,unit.unit,use1.RealName
                                                 FROM XC_Daliy LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
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
    }
}
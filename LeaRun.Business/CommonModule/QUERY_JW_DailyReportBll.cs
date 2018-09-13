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
using System.Data.Common;

namespace LeaRun.Business
{
    /// <summary>
    /// QUERY_JW_DailyReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.09 10:07</date>
    /// </author>
    /// </summary>
    public class QUERY_JW_DailyReportBll : RepositoryFactory<QUERY_JW_DailyReport>
    {
        /// <summary>
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                 string sqlTotal =
                    string.Format(
                        @" 
                                        select 
                                            ROW_NUMBER() over(order by adddate desc) rowNumber,
                                            DailyReport_id,unit_id,adduser_id,convert(varchar(10),adddate,120) adddate
                                           ,reportYear,reportNum,reportAllNum,basicinfo,importantinfo
                                           ,dailyinfo,submit,deliver,editing,review,issue
                                           ,case JW_DailyReport.needReport when '1' then '�ϱ�' else '���ϱ�' end needReport
                                           ,case JW_DailyReport.needPublish when '1' then '����' else '������' end needPublish
                                           ,unit.unit,use1.RealName
                                           FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                           LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id
                                         "
                        );

                                           
                //if (unit_id != Share.UNIT_ID_JS)
                //{
                //    sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or ( JW_DailyReport.needPublish='1'  AND JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + unit_id + "' or parent_unit_id ='" + unit_id + "') ) )";
                //}
                //else
                //{
                //    sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or (  JW_DailyReport.needPublish='1' ) )";
                
                //}

                 //��ԭ�е�Ȩ�޻���������������Ժ��Ժ�����Կ���ʡԺ�������ձ�
                 if (unit_id != Share.UNIT_ID_JS)
                 {
                     sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or ( JW_DailyReport.needPublish='1'  AND ( JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + unit_id + "' or parent_unit_id ='" + unit_id + "') or JW_DailyReport.unit_id='"+Share.UNIT_ID_JS+"')) )";
                 }
                 else
                 {
                     sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or (  JW_DailyReport.needPublish='1' ) )";

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
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��               
                    page = jqgridparam.page, //��ǰҳ��
                    records = dt2.Rows.Count, //�ܼ�¼��
                    costtime = CommonHelper.TimerEnd(watch), //��ѯ���ĵĺ�����
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
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string applydatestart, string applydateend, string basicinfo, string importantinfo, string dailyinfo, string contianssubordinateunit)
        {
         
            try
            {
                string user_id = ManageProvider.Provider.Current().UserId;
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                  string sqlTotal =
                    string.Format(
                        @" select 
                                            ROW_NUMBER() over(order by adddate desc) rowNumber,
                                            DailyReport_id,unit_id,adduser_id,convert(varchar(10),adddate,120) adddate
                                            ,reportYear,reportNum,reportAllNum,basicinfo,importantinfo
                                            ,dailyinfo,submit,deliver,editing,review,issue
                                            ,case JW_DailyReport.needReport when '1' then '�ϱ�' else '���ϱ�' end needReport
                                            ,case JW_DailyReport.needPublish when '1' then '����' else '������' end needPublish
                                            ,unit.unit,use1.RealName
                                            FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                            LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id
                                            WHERE 1=1
                                          "
                        );

                 
                //if (unit_id != "")//��λID
                //{
                //    sqlTotal = sqlTotal + " and JW_DailyReport.unit_id ='" + unit_id + "'";
                //}
                  if (unit_id != "")//��λID
                  {
                      if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                      {
                          if (contianssubordinateunit == "1")
                          {
                              sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id ='" + unit_id + "' or (JW_DailyReport.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ) or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) ";
                          }
                          else
                          {
                              sqlTotal = sqlTotal + " and JW_DailyReport.needPublish='1' and ( JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                          }
                      }
                      else
                      {
                          if (contianssubordinateunit == "1")
                          {
                              sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' ";
                          }
                          else
                          {
                              sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' and (JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                          }
                      }
                  }
                  else
                  {
                      if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                      {
                          sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + ManageProvider.Provider.Current().CompanyId + "' or parent_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "') or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) )";
                                                            
                      }
                      else
                      {
                          sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "' ";
                      } 
                  }

                if (applydatestart != "")//������ʼ����
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate> '" + applydatestart + "'";
                }
                if (applydateend != "")//������������
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate< '" + applydateend + "'";
                }
                if (basicinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.basicinfo like '%" + basicinfo.Trim() + "%'";
                }
                if (importantinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.importantinfo like '%" + importantinfo + "%'";
                }
                if (dailyinfo != "")//���֤��
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.dailyinfo like '%" + dailyinfo + "%'";
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
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��               
                    page = jqgridparam.page, //��ǰҳ��
                    records = dt2.Rows.Count, //�ܼ�¼��
                    costtime = CommonHelper.TimerEnd(watch), //��ѯ���ĵĺ�����
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
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel(string unit_id, string applydatestart, string applydateend, string basicinfo, string importantinfo, string dailyinfo, string contianssubordinateunit)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                       select 
                                         
                                             unit.unit as '������λ'
                                            ,use1.RealName as '������Ա'
                                            ,convert(varchar(10),adddate,120) as '��������'
                                            ,reportYear as '�������'
                                            ,reportNum as '��������'
                                            ,reportAllNum as '����������'
                                            ,replace(basicinfo,'&nbsp;','') as '���������Ϣ'
                                            ,replace(importantinfo,'&nbsp;','') as '��Ҫ������Ϣ'
                                            ,replace(dailyinfo,'&nbsp;','') as '�ճ�������Ϣ'
                                            ,submit as '��'
                                            ,deliver as '��'
                                            ,editing as '��У'
                                            ,review as '���'
                                            ,issue as 'ǩ��'
                                            ,case JW_DailyReport.needReport when '1' then '�ϱ�' else '���ϱ�' end as '�Ƿ��ϱ�'
                                            ,case JW_DailyReport.needPublish when '1' then '����' else '������' end as '�Ƿ񷢲�' 
                                            FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                            LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id
                                            WHERE 1=1
                                          "
                        );


                //if (unit_id != "")//��λID
                //{
                //    sqlTotal = sqlTotal + " and JW_DailyReport.unit_id ='" + unit_id + "'";
                //}
                if (unit_id != "")//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id ='" + unit_id + "' or (JW_DailyReport.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ) or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and JW_DailyReport.needPublish='1' and ( JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' and (JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + ManageProvider.Provider.Current().CompanyId + "' or parent_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "') or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) )";

                    }
                    else
                    {
                        sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "' ";
                    }
                }

                if (applydatestart != "")//������ʼ����
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate> '" + applydatestart + "'";
                }
                if (applydateend != "")//������������
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate< '" + applydateend + "'";
                }
                if (basicinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.basicinfo like '%" + basicinfo.Trim() + "%'";
                }
                if (importantinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.importantinfo like '%" + importantinfo + "%'";
                }
                if (dailyinfo != "")//���֤��
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.dailyinfo like '%" + dailyinfo + "%'";
                }
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
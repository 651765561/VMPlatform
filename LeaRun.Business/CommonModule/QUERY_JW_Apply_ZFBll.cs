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
using LeaRun.Entity.BaseUtility;

namespace LeaRun.Business
{
    /// <summary>
    /// JW_Apply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.17 11:59</date>
    /// </author>
    /// </summary>
    public class QUERY_JW_Apply_ZFBll : RepositoryFactory<QUERY_JW_Apply_ZF>
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
                          @"     select ROW_NUMBER() over(order by senddate desc) rowNumber,*  from 
  ( 
        select 
                                          distinct  sp.Object_id
                                               , sp.type
                                               , pa.apply_id
                                               , pa.unit_id
                                               , pa.unitname
                                               , pa.dep_id
                                               , pa.depname
                                               , pa.adduser_id
                                               , pa.adddate
                                               , pa.tasktype_id
                                               , pa.usedate
                                               , pa.actuser_id
                                               , sp.state
                                               , u.RealName
                                               ,sp.user_id
                                               ,CONVERT(varchar(20), sp.senddate, 120)  senddate
                                               ,case when Datediff(day,sp.actdate,sp.enddate)=0 
                                                                        then 1 
                                                                        else Datediff(day,sp.actdate,sp.enddate) end
                                                                        as counts 
                                              ,CONVERT(varchar(20), sp.actdate, 120)  actdate
                                              ,CONVERT(varchar(20), sp.enddate, 120)  enddate
                                              ,sp.actdetail
                                              ,sp.sendpolice_id
                                        from JW_SendPolice sp  
                                        left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                                        left join base_user u on u.UserId=sp.user_id
                                       
                                        where   sp.type in (4,5)  and sp.unit_id='{0}' 
                                     
                                        union select distinct
                                                 sp.Object_id
                                               , sp.type
                                               , sp.sendpolice_id as apply_id
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
                                               ,u.RealName
                                               ,sp.user_id
                                               ,CONVERT(varchar(20), sp.senddate, 120)  senddate
                                               ,ISNULL(case when Datediff(day,sp.actdate,sp.enddate)=0 
                                                                        then 1 
                                                                        else Datediff(day,sp.actdate,sp.enddate) end,1)
                                                                        as counts 
                                              ,CONVERT(varchar(20), sp.actdate, 120)  actdate
                                              ,CONVERT(varchar(20), sp.enddate, 120)  enddate
                                              ,replace(sp.actdetail,'!@#$%^&*()','') actdetail
                                              ,sp.sendpolice_id
                                              from JW_SendPolice sp  
                                        left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                                        left join base_user u on sp.user_id=u.userid
                                        left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                                        left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                                       
                                        where   sp.type in (1,2,3,8,9)    and sp.unit_id='{0}' 
                                        and sp.actdate is not null
)a
                                      "
                         , unit_id
                         );//and year(sp.SendDate)=year(getdate())

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
        /*lwl*/
        public string GridPageApplyTongJiJson(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            string contianssubordinateunit = "";
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                //                string sqlTotal = string.Format(@"   select   ROW_NUMBER() over(order by num desc) rowNumber,* from   
                //                        ( select max(pa.unit_id) as unit_id ,  max(pa.unitname ) as unitname, 
                //                          u.RealName, pa.tasktype_id,pa.tasktype_id as tasktype_val
                //                          ,count(1) as num
                //                           
                //                    from JW_SendPolice sp  
                //                    left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                //                    left join base_user u on u.UserId=sp.user_id
                //                    left join Base_Department dep on pa.dep_id=dep.Dep_id
                //                   
                //                    where   sp.type in (4,5) 
                //                  
                //                    ");
                string sqlTotal = string.Format(@"select   ROW_NUMBER() over(order by num desc) rowNumber,* from 
 (select unit_id , max(unitname) unitname,RealName ,tasktype_id ,max(tasktype_val) tasktype_val,count(1) num from (select pa.unit_id as unit_id , pa.unitname  as unitname, 
                          u.RealName, pa.tasktype_id,pa.tasktype_id as tasktype_val
                          
                           
                    from JW_SendPolice sp  
                    left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                    left join base_user u on u.UserId=sp.user_id
                    left join Base_Department dep on pa.dep_id=dep.Dep_id
                   
                    where   sp.type in (4,5) ");

                if (unit_id != "" && unit_id != null)//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (pa.unit_id ='" + unit_id + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }


                /*��ѯ���1����*/
                //  sqlTotal += " group by  u.RealName ,pa.tasktype_id ";

                /*��ѯ���2sql���*/
                //                string sqlTotal2 =
                //                string.Format(@"union 
                //        select  max( unit.base_unit_id ) unit_id,  max( unit.unit ) as unitname ,
                //                           u.RealName
                //                           , ondu.tasktype_id as tasktype_id,ondu.tasktype_id as tasktype_val
                //							,count(1) as num
                //                    from JW_SendPolice sp  
                //                    left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                //                    left join base_user u on sp.user_id=u.userid
                //                    left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                //                    left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                //                    where   sp.type in (1,2,3,8,9)   
                //                   
                //          
                //         ");

                string sqlTotal2 = string.Format(@"union all select  unit.base_unit_id  unit_id,  unit.unit as unitname ,
                           u.RealName
                           , ondu.tasktype_id as tasktype_id,ondu.tasktype_id as tasktype_val
							
                    from JW_SendPolice sp  
                    left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                    left join base_user u on sp.user_id=u.userid
                    left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                    left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                    where   sp.type in (1,2,3,8,9)  ");
                if (unit_id != "" && unit_id != null)//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + unit_id + "' or (sp.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }

                sqlTotal2 = sqlTotal2 + "  and sp.actdate is not null ";

                //sqlTotal2 += " group by u.RealName ,ondu.tasktype_id";

                //sqlTotal2 = sqlTotal2 + " ) a ";

                sqlTotal2 += " ) t1 group by  unit_id,RealName,tasktype_id) b";

                string sql =
                string.Format(
                    @" select * from ( 
                                        {4} 
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                    , (pageIndex - 1) * pageSize + 1
                    , pageIndex * pageSize
                    , "num"
                    , jqgridparam.sord
                    , sqlTotal + sqlTotal2
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
                , sqlTotal + sqlTotal2
                );
                DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal + sqlTotal2, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��               
                    page = jqgridparam.page, //��ǰҳ��
                    records = dt2.Rows.Count, //�ܼ�¼��
                    costtime = CommonHelper.TimerEnd(watch), //��ѯ���ĵĺ�����
                    rows = dt
                };
                return JsonData.ToJson();

            }
            catch (Exception ex)
            {
                return "";

            }
        }

        /*lwlͼ��ͳ��*/

        public string GetPersonForTuBiao(string unit_id, string policename, string startDate, string endDate, string contianssubordinateunit)
        {
            List<Person> list = new List<Person>();
            Stopwatch watch = CommonHelper.TimerStart();

            string sqlTotal = string.Format(@"select   ROW_NUMBER() over(order by num desc) rowNumber,* from 
 (select unit_id ,max(unitname) unitname, RealName ,tasktype_id ,max(tasktype_val) tasktype_val,count(1) num from (select pa.unit_id as unit_id , pa.unitname  as unitname, 
                          u.RealName, pa.tasktype_id,pa.tasktype_id as tasktype_val
                          
                           
                    from JW_SendPolice sp  
                    left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                    left join base_user u on u.UserId=sp.user_id
                    left join Base_Department dep on pa.dep_id=dep.Dep_id
                   
                    where   sp.type in (4,5) 

");
            if (startDate != "" && endDate != "" && endDate != null)
            {
                sqlTotal += " and  CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "'";


            }
            else
            {
                sqlTotal = sqlTotal + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";
            }
            if (unit_id != "" && unit_id != null)//��λID
            {
                if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                {
                    if (contianssubordinateunit == "1")
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + unit_id + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                    }
                    else
                    {
                        sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                    }
                }
                else
                {
                    if (contianssubordinateunit == "1")
                    {

                    }
                    else
                    {
                        sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                    }
                }
            }
            else
            {
                if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                {
                    sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                }
                else
                {

                }
            }

            if (policename != "" && policename != null)//�ɾ�����
            {
                sqlTotal = sqlTotal + " and u.RealName like '%" + policename.Trim() + "%'";
            }


            string sqlTotal2 =
            string.Format(@"union all select  unit.base_unit_id  unit_id,  unit.unit as unitname ,
                           u.RealName
                           , ondu.tasktype_id as tasktype_id,ondu.tasktype_id as tasktype_val
							
                    from JW_SendPolice sp  
                    left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                    left join base_user u on sp.user_id=u.userid
                    left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                    left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                    where   sp.type in (1,2,3,8,9) ");

            if (startDate != "" && endDate != "" && endDate != null)
            {
                sqlTotal2 += " and CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "'";


            }
            else
            {
                sqlTotal2 = sqlTotal2 + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";
            }
            if (unit_id != "" && unit_id != null)//��λID
            {
                if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                {
                    if (contianssubordinateunit == "1")
                    {
                        sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + unit_id + "' or (sp.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                    }
                    else
                    {
                        sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                    }
                }
                else
                {
                    if (contianssubordinateunit == "1")
                    {

                    }
                    else
                    {
                        sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                    }
                }
            }
            else
            {
                if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                {
                    sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                }
                else
                {

                }
            }
            if (policename != "" && policename != null)//�ɾ�����
            {
                sqlTotal2 = sqlTotal2 + " and u.RealName like '%" + policename.Trim() + "%'";
            }
            sqlTotal2 = sqlTotal2 + "  and sp.actdate is not null ";



            sqlTotal2 += ") t1 group by  unit_id,RealName,tasktype_id) b";

            string sql =
               string.Format(
                   @" select * from ( 
                                        {0} 
                                        ) as a  where RealName is not null  and unit_id is not null  order by   unit_id "

                   , sqlTotal + sqlTotal2
                   );
            DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
            Base_UnitBll bllunit = new Base_UnitBll();
            foreach (DataRow r in dt.Rows)
            {
                Person p;
                string RealName = r["RealName"].ToString();
                string unitId = r["unit_id"].ToString();
                if (list.Exists(m => m.UnitId == unitId && m.PName == RealName))
                {
                    p = list.Where(m => m.UnitId == unitId && m.PName == RealName).FirstOrDefault();
                }
                else
                {
                    p = new Person();
                }
                p.PId = int.Parse(r["rowNumber"].ToString());
                p.PName = r["RealName"].ToString();
                int tasktype_id = int.Parse(r["tasktype_id"].ToString());
                int num = int.Parse(r["num"].ToString());
                p.TaskTypeList.Where(n => n.TaskTypeId == tasktype_id).FirstOrDefault().Num = num;
                p.UnitId = unitId;

                p.UnitSortCode = bllunit.GetSortCode(unitId);
                var h = bllunit.GetSortCode("00727146-bec9-4991-b730-62f24b52f802");
                var hh = bllunit.GetSortCode("02cdc733-3ed0-4aeb-855a-a6776810471c");
                p.UnitName = r["unitname"].ToString();
                if (!list.Exists(m => m.UnitId == unitId && m.PName == RealName))
                {
                    list.Add(p);
                }

            }
            list = list.OrderBy(p => p.UnitSortCode).ToList();
            return list.ToJson();
        }



        public string GridPageApplyTongJiJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string policename, string startDate, string endDate, string contianssubordinateunit)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                //                string sqlTotal = string.Format(@"   select   ROW_NUMBER() over(order by num desc) rowNumber,* from   
                //                        ( select max(pa.unit_id) as unit_id , max(pa.unitname ) as unitname, 
                //                          u.RealName, pa.tasktype_id,pa.tasktype_id as tasktype_val
                //                          ,count(1) as num
                //                           
                //                    from JW_SendPolice sp  
                //                    left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                //                    left join base_user u on u.UserId=sp.user_id
                //                    left join Base_Department dep on pa.dep_id=dep.Dep_id
                //                   
                //                    where   sp.type in (4,5) 
                //                  
                //                    ");
                string sqlTotal = string.Format(@"select   ROW_NUMBER() over(order by num desc) rowNumber,* from 
 (select unit_id ,max(unitname) unitname, RealName ,tasktype_id ,max(tasktype_val) tasktype_val,count(1) num from (select pa.unit_id as unit_id , pa.unitname  as unitname, 
                          u.RealName, pa.tasktype_id,pa.tasktype_id as tasktype_val
                          
                           
                    from JW_SendPolice sp  
                    left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                    left join base_user u on u.UserId=sp.user_id
                    left join Base_Department dep on pa.dep_id=dep.Dep_id
                   
                    where   sp.type in (4,5) 

");
                if (startDate != "" && endDate != "" && endDate != null)
                {
                    sqlTotal += " and  CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "'";
                    // sqlTotal = sqlTotal + " and CONVERT(varchar(100), sp.actdate, 23)='" + startDate + "'";
                    // sqlTotal = sqlTotal + " and CONVERT(varchar(100), sp.endDate, 23)='" + endDate + "'";

                }
                if (unit_id != "" && unit_id != null)//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (pa.unit_id ='" + unit_id + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }

                if (policename != "" && policename != null)//�ɾ�����
                {
                    sqlTotal = sqlTotal + " and u.RealName like '%" + policename.Trim() + "%'";
                }
                /*��ѯ���1����*/
                //   sqlTotal += " group by  u.RealName ,pa.tasktype_id ";

                /*��ѯ���2sql���*/
                //                string sqlTotal2 =
                //                string.Format(@"union 
                //        select  max( unit.base_unit_id ) unit_id,  max( unit.unit ) as unitname ,
                //                           u.RealName
                //                           , ondu.tasktype_id as tasktype_id,ondu.tasktype_id as tasktype_val
                //							,count(1) as num
                //                    from JW_SendPolice sp  
                //                    left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                //                    left join base_user u on sp.user_id=u.userid
                //                    left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                //                    left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                //                    where   sp.type in (1,2,3,8,9)   
                //                   
                //          
                //         ");

                string sqlTotal2 =
                string.Format(@"union all select  unit.base_unit_id  unit_id,  unit.unit as unitname ,
                           u.RealName
                           , ondu.tasktype_id as tasktype_id,ondu.tasktype_id as tasktype_val
							
                    from JW_SendPolice sp  
                    left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                    left join base_user u on sp.user_id=u.userid
                    left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                    left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                    where   sp.type in (1,2,3,8,9) ");

                if (startDate != "" && endDate != "" && endDate != null)
                {
                    sqlTotal2 += " and CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "'";
                    // sqlTotal2 = sqlTotal2 + " and CONVERT(varchar(100), sp.actdate, 23)='" + startDate + "'";
                    //  sqlTotal2 = sqlTotal2 + " and CONVERT(varchar(100), sp.endDate, 23)='" + endDate + "'";

                }
                if (unit_id != "" && unit_id != null)//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + unit_id + "' or (sp.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }
                if (policename != "" && policename != null)//�ɾ�����
                {
                    sqlTotal2 = sqlTotal2 + " and u.RealName like '%" + policename.Trim() + "%'";
                }
                sqlTotal2 = sqlTotal2 + "  and sp.actdate is not null ";

                //  sqlTotal2 += " group by u.RealName ,ondu.tasktype_id";

                // sqlTotal2 = sqlTotal2 + " ) a ";

                sqlTotal2 += ") t1 group by  unit_id,RealName,tasktype_id) b";

                string sql =
                string.Format(
                    @" select * from ( 
                                        {4} 
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                    , (pageIndex - 1) * pageSize + 1
                    , pageIndex * pageSize
                    , "num"
                    , jqgridparam.sord
                    , sqlTotal + sqlTotal2
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
                , sqlTotal + sqlTotal2
                );
                DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal + sqlTotal2, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��               
                    page = jqgridparam.page, //��ǰҳ��
                    records = dt2.Rows.Count, //�ܼ�¼��
                    costtime = CommonHelper.TimerEnd(watch), //��ѯ���ĵĺ�����
                    rows = dt
                };
                return JsonData.ToJson();

            }
            catch (Exception ex)
            {


            }
            return "";
        }
        /*lwl*/


        /// <summary>
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel(string unit_id, string tasktype_id, string policename, string depname, string type, string state,
            string contianssubordinateunit, string startDate, string endDate)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                     select 
                                                pa.unitname as '�þ���λ'
                                               , '����-' + pa.depname  as '�þ�����' 
                                               , u.RealName as '�ɾ�����'
                                               ,ISNULL( case when Datediff(day,sp.actdate,sp.enddate)=0 
                                                        then 1 
                                                        else Datediff(day,sp.actdate,sp.enddate) end,1)
                                                        as '�˴�' 

                                               , case sp.type   when 1 then '�참��'
                                                                when 2 then '�̼�참��'
                                                                when 3 then 'ָ������'
                                                                when 4 then '�þ�����'
                                                                when 5 then 'ֱ���ɾ�'
                                                                when 6 then '��������'
                                                                when 7 then '������'
                                                                when 8 then '����Ӵ�'
                                                                when 9 then '���ذ���'
                                                 end as '�ɾ���ʽ'

                                         , case pa.tasktype_id  when 1 then '���������ֳ�'
                                                                when 2 then 'ִ�д���'
                                                                when 3 then 'ִ�од�'
                                                                when 4 then 'Э��ִ��ָ���������Ӿ�ס'
                                                                when 5 then 'Э��ִ�о���������'
                                                                when 6 then '����׷�����ӻ������ӵķ���������'
                                                                when 7 then '�����Ѳ�����'
                                                                when 8 then '��Ѻ���������˱����˻��ﷸ'
                                                                when 9 then '���ܷ��������˱����˻��ﷸ'
                                                                when 10 then '�ʹ﷨������'
                                                                when 11 then '���������Ա��ȫ'
                                                                when 12 then '�칫���참������Ӵ�����ִ��'
                                                                when 13 then '���봦��ͻ���¼�����'
                                                                when 14 then '�����������'
                                                 end as '��������'

                                              ,CONVERT(varchar(20), sp.actdate, 120)  as 'ִ�ڿ�ʼʱ��'
                                              ,CONVERT(varchar(20), sp.enddate, 120)  as 'ִ�ڽ���ʱ��'
                                              , case sp.state   when 0 then '��ȷ��'
                                                                when 1 then '��ȷ�ϴ�ʵʩ'
                                                                when 2 then '��ʵʩ'
                                                                when 3 then 'ʵʩ���'
                                                                when -1 then 'ȷ���˻�'
                                                                when -2 then 'ȡ������'
                                                end as '״̬'
                                              ,sp.actdetail as 'ʵʩ���'
                                            ,1 as qqq
                                        from JW_SendPolice sp  
                                        left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                                        left join base_user u on u.UserId=sp.user_id
                                        left join Base_Department dep on pa.dep_id=dep.Dep_id
                                       
                                        where   sp.type in (4,5) 
                                      


                             "
                        );

                if (unit_id != "")//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (pa.unit_id ='" + unit_id + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }

                if (startDate != null && startDate != "" && endDate != "" && endDate != null)
                {
                    sqlTotal += " and CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "' ";
                    //  sqlTotal = sqlTotal + " and CONVERT(varchar(100), sp.actdate, 23)='" + startDate + "'";
                    // sqlTotal = sqlTotal + " and CONVERT(varchar(100), sp.endDate, 23)='" + endDate + "'";

                }
                else
                {
                    sqlTotal = sqlTotal + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";
                }

                if (tasktype_id != "0")//����������
                {
                    sqlTotal = sqlTotal + " and pa.tasktype_id = '" + tasktype_id + "'";
                }
                if (policename != "")//�ɾ�����
                {
                    sqlTotal = sqlTotal + " and u.RealName like '%" + policename.Trim() + "%'";
                }

                if (depname != "")//�þ�����
                {
                    sqlTotal = sqlTotal + " and dep.[type] = '" + depname + "'";
                }
                if (type != "")//�ɾ���ʽ
                {
                    sqlTotal = sqlTotal + " and   sp.type = '" + type + "'";
                }

                string sqlTotal2 =
                 string.Format(
                     @"  union select 
                                               unit.unit as '�þ���λ'
                                                , '������-' + area.AreaName  as  '�þ�����' 
                                               , u.RealName as '�ɾ�����'
                                               ,ISNULL(case when Datediff(day,sp.actdate,sp.enddate)=0 
                                                                then 1 
                                                                else Datediff(day,sp.actdate,sp.enddate) end,1)
                                                                as '�˴�' 
                       
                                               , case sp.type   when 1 then '�참��'
                                                                when 2 then '�̼�참��'
                                                                when 3 then 'ָ������'
                                                                when 4 then '�þ�����'
                                                                when 5 then 'ֱ���ɾ�'
                                                                when 6 then '��������'
                                                                when 7 then '������'
                                                                when 8 then '����Ӵ�'
                                                                when 9 then '���ذ���'
                                                 end as '�ɾ���ʽ'
                                         , case ondu.tasktype_id  when 1 then '���������ֳ�'
                                                                when 2 then 'ִ�д���'
                                                                when 3 then 'ִ�од�'
                                                                when 4 then 'Э��ִ��ָ���������Ӿ�ס'
                                                                when 5 then 'Э��ִ�о���������'
                                                                when 6 then '����׷�����ӻ������ӵķ���������'
                                                                when 7 then '�����Ѳ�����'
                                                                when 8 then '��Ѻ���������˱����˻��ﷸ'
                                                                when 9 then '���ܷ��������˱����˻��ﷸ'
                                                                when 10 then '�ʹ﷨������'
                                                                when 11 then '���������Ա��ȫ'
                                                                when 12 then '�칫���참������Ӵ�����ִ��'
                                                                when 13 then '���봦��ͻ���¼�����'
                                                                when 14 then '�����������'
                                                 end as '��������'
                                              ,CONVERT(varchar(20), sp.actdate, 120)  as 'ִ�ڿ�ʼʱ��'
                                              ,CONVERT(varchar(20), sp.enddate, 120)  as 'ִ�ڽ���ʱ��'
                                              , case sp.state   when 0 then '��ȷ��'
                                                                when 1 then '��ȷ�ϴ�ʵʩ'
                                                                when 2 then '��ʵʩ'
                                                                when 3 then 'ʵʩ���'
                                                                when -1 then 'ȷ���˻�'
                                                                when -2 then 'ȡ������'
                                                 end as '״̬'
                                              ,replace(sp.actdetail,'!@#$%^&*()','') as 'ʵʩ���'
                                            ,2 as qqq
                                        from JW_SendPolice sp  
                                        left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                                        left join base_user u on sp.user_id=u.userid
                                        left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                                        left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                                        where   sp.type in (1,2,3,8,9)   
                                       
                             "
                       );

                if (startDate != null && startDate != "" && endDate != null && endDate != "")
                {
                    sqlTotal2 += " and CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "'";
                    // sqlTotal2 = sqlTotal2 + " and CONVERT(varchar(100), sp.actdate, 23)='" + startDate + "'";
                    //  sqlTotal2 = sqlTotal2 + " and CONVERT(varchar(100), sp.endDate, 23)='" + endDate + "'";

                }
                else
                {
                    sqlTotal2 = sqlTotal2 + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";

                }

                if (unit_id != "")//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + unit_id + "' or (sp.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }
                if (tasktype_id != "0")//����������
                {
                    sqlTotal2 = sqlTotal2 + " and ondu.tasktype_id = '" + tasktype_id + "'";
                }
                if (policename != "")//�ɾ�����
                {
                    sqlTotal2 = sqlTotal2 + " and u.RealName like '%" + policename.Trim() + "%'";
                }

                if (type != "")//�ɾ���ʽ
                {
                    sqlTotal2 = sqlTotal2 + " and    sp.type = '" + type + "'";
                }
                sqlTotal2 = sqlTotal2 + "  and sp.actdate is not null ";
                sqlTotal2 = sqlTotal2 + "  ) aaa   order by qqq asc , ִ�ڿ�ʼʱ�� ";



                DataTable dt = SqlHelper.DataTable(sqlTotal + sqlTotal2, CommandType.Text);//Repository().FindTableBySql(sql);


                return dt;
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
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string tasktype_id,
            string policename, string depname, string type, string state, string contianssubordinateunit, string startDate, string endDate)
        {

            try
            {

                //
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                  string.Format(
                      @"  select 
                                            ROW_NUMBER() over(order by qqq asc) rowNumber,*
                                            from ( select 
                                               distinct sp.Object_id
                                               , sp.type
                                               , pa.apply_id
                                               , pa.unit_id
                                               , pa.unitname
                                               , pa.dep_id
                                               , '����-' + pa.depname  as depname
                                               , pa.adduser_id
                                               , pa.adddate
                                               , pa.tasktype_id
                                               , pa.usedate
                                               , pa.actuser_id
                                               , sp.state
                                               , u.RealName
                                               ,sp.user_id
                                               ,CONVERT(varchar(20), sp.senddate, 120)  senddate
                                               ,ISNULL( case when Datediff(day,sp.actdate,sp.enddate)=0 
                                                                        then 1 
                                                                        else Datediff(day,sp.actdate,sp.enddate) end,1)
                                                                        as counts 
                                              ,CONVERT(varchar(20), sp.actdate, 120)  actdate
                                              ,CONVERT(varchar(20), sp.enddate, 120)  enddate
                                              ,sp.actdetail
                                              ,sp.sendpolice_id
,1 as qqq
                                        from JW_SendPolice sp  
                                        left join JW_PoliceApply  pa on  sp.object_id=pa.apply_id
                                        left join base_user u on u.UserId=sp.user_id
                                        left join Base_Department dep on pa.dep_id=dep.Dep_id
                                       
                                        where   sp.type in (4,5) 
                                      


                             "
                        );

                if (startDate != null && startDate != "" && endDate != "" && endDate != null)
                {
                    sqlTotal += " and CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "' ";
                    //  sqlTotal = sqlTotal + " and CONVERT(varchar(100), sp.actdate, 23)='" + startDate + "'";
                    // sqlTotal = sqlTotal + " and CONVERT(varchar(100), sp.endDate, 23)='" + endDate + "'";

                }
                else
                {
                    sqlTotal = sqlTotal + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";
                }
                if (unit_id != "")//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (pa.unit_id ='" + unit_id + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and pa.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }
                if (tasktype_id != "0")//����������
                {
                    sqlTotal = sqlTotal + " and pa.tasktype_id = '" + tasktype_id + "'";
                }
                if (policename != "")//�ɾ�����
                {
                    sqlTotal = sqlTotal + " and u.RealName like '%" + policename.Trim() + "%'";
                }
                //if (applydatestart != "")//���ÿ�ʼ����
                //{
                //    sqlTotal = sqlTotal + " and  sp.senddate> '" + applydatestart + "'";
                //}
                //else
                //    sqlTotal = sqlTotal + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";
                if (depname != "" && depname != null)//�þ�����
                {
                    sqlTotal = sqlTotal + " and dep.[type] = '" + depname + "'";
                }
                if (type != "" && type != null)//�ɾ���ʽ
                {
                    sqlTotal = sqlTotal + " and   sp.type = '" + type + "'";
                }

                //sqlTotal = sqlTotal + "  and sp.actdate is not null ";
                //if (state != "")//����������
                //{
                // sqlTotal = sqlTotal + " and sp.state in " + state;
                //  if (state == "(3)")
                //{
                //      sqlTotal = sqlTotal + "  and sp.actdate is not null ";
                //}
                //}

                string sqlTotal2 =
                 string.Format(
                     @"  union select 
                                             distinct sp.Object_id
                                               , sp.type
                                               , sp.sendpolice_id as apply_id
                                               , sp.unit_id
                                               , unit.unit as unitname
                                               , ondu.PoliceArea_id as dep_id
                                               , '������-' + area.AreaName  as depname
                                               , ondu.adduser_id as adduser_id
                                               , ondu.adddate as adddate
                                               , ondu.tasktype_id as tasktype_id
                                               , '' as usedate
                                               , '' as actuser_id
                                               , sp.state as state
                                               ,u.RealName
                                               ,sp.user_id
                                               ,CONVERT(varchar(20), sp.senddate, 120)  senddate
                                               ,ISNULL(case when Datediff(day,sp.actdate,sp.enddate)=0 
                                                                        then 1 
                                                                        else Datediff(day,sp.actdate,sp.enddate) end,1)
                                                                        as counts 
                                              ,CONVERT(varchar(20), sp.actdate, 120)  actdate
                                              ,CONVERT(varchar(20), sp.enddate, 120)  enddate
                                              ,replace(sp.actdetail,'!@#$%^&*()','') actdetail
                                              ,sp.sendpolice_id
                                            ,2 as qqq

                                        from JW_SendPolice sp  
                                        left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id
                                        left join base_user u on sp.user_id=u.userid
                                        left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                                        left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                                        where   sp.type in (1,2,3,8,9)   
                                       
                             "
                       );

                if (startDate != null && startDate != "" && endDate != null && endDate != "")
                {
                    sqlTotal2 += " and CONVERT(varchar(100), sp.actdate, 23) between   '" + startDate + "' and '" + endDate + "'";
                    // sqlTotal2 = sqlTotal2 + " and CONVERT(varchar(100), sp.actdate, 23)='" + startDate + "'";
                    //  sqlTotal2 = sqlTotal2 + " and CONVERT(varchar(100), sp.endDate, 23)='" + endDate + "'";

                }
                else
                {
                    sqlTotal2 = sqlTotal2 + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";

                }
                if (unit_id != null && unit_id != "")//��λID
                {
                    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + unit_id + "' or (sp.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal2 = sqlTotal2 + " and sp.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        sqlTotal2 = sqlTotal2 + " and (sp.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }
                if (tasktype_id != "0")//����������
                {
                    sqlTotal2 = sqlTotal2 + " and ondu.tasktype_id = '" + tasktype_id + "'";
                }
                if (policename != "")//�ɾ�����
                {
                    sqlTotal2 = sqlTotal2 + " and u.RealName like '%" + policename.Trim() + "%'";
                }
                //if (applydatestart != "")//���ÿ�ʼ����
                //{
                //    sqlTotal2 = sqlTotal2 + " and  sp.senddate> '" + applydatestart + "'";
                //}
                //else
                //    sqlTotal2 = sqlTotal2 + " and  sp.senddate> (SELECT DATEADD(yy, DATEDIFF(yy,0,getdate()), 0))";
                if (type != "" && type != null)//�ɾ���ʽ
                {
                    sqlTotal2 = sqlTotal2 + " and    sp.type = '" + type + "'";
                }
                sqlTotal2 = sqlTotal2 + "  and sp.actdate is not null ";
                //if (state != "")//����������
                //{
                //    sqlTotal2 = sqlTotal2 + " and sp.state in " + state;
                //    if (state == "(3)")
                //    {
                //        sqlTotal2 = sqlTotal2 + "  and sp.actdate is not null ";
                //    }
                //}

                sqlTotal2 = sqlTotal2 + " ) a ";



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
                    , sqlTotal + sqlTotal2
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
                , sqlTotal + sqlTotal2
                );
                DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal + sqlTotal2, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��               
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
        /// ��λ��
        /// </summary>
        /// <param name="unit_id">��λID</param>
        /// <returns></returns>
        public DataTable GetList(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --����
                                                    u.unit as name ,	          --��λ����
                                                    ISNULL(uf.Base_Unit_id,0) AS parent_id ,--�ϼ���λID
                                                    uf.unit as parent_name,		  --�ϼ�����
                                                    u.sortcode ,				  --�����ֶ�
                                                    u.code,   					  --Code�ֶ�
                                                    'Unit' AS Sort                --�����ֶ�
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id
                                        ) T WHERE 1=1 ");
            List<DbParameter> parameter = new List<DbParameter>();
            //if (!string.IsNullOrEmpty(unit_id))
            //{
            //    strSql.Append(" AND u.Base_Unit_id = @unit_id");
            //    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
            //}
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //strSql.Append(" AND ( Dep_id IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //strSql.Append(" ) )");
            }
            strSql.Append(" ORDER BY convert(int,SortCode) ASC");
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="unit_id">��λID</param>
        /// <returns></returns>
        public DataTable GetListArea(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   pa.PoliceArea_id as dep_id ,  --����
                                                    pa.AreaName as name ,	      --����������
                                                    '0' AS  parent_id ,             --�ϼ���λID
                                                    '' as parent_name,		      --�ϼ�����
                                                    pa.sortcode ,				  --�����ֶ�
                                                    pa.code,   					  --Code�ֶ�
                                                    'PoliceArea' AS Sort,         --�����ֶ�
                                                    pa.AreaType,
                                                    pa.unit_id
                                          FROM      Base_PoliceArea pa
                                        ) T WHERE 1=1 and T.AreaType in (3,4)");
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(unit_id))
            {
                strSql.Append(" AND t.unit_id = @unit_id");
                parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //strSql.Append(" AND ( Dep_id IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //strSql.Append(" ) )");
            }
            strSql.Append(" ORDER BY convert(int,SortCode) ASC");
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }



        /// <summary>
        /// �˻ط����ľ���Ǽ�
        /// </summary>
        /// <param name="keyValue">�ɾ����Ӧsendpolice_id</param>
        /// <returns></returns>
        public int ReturnAction(string keyValue, string Object_id, string type)
        {
            int r = 0;
            if (type == "�þ�����" || type == "ֱ���ɾ�")
            {
                string sqlSubmit = "update JW_SendPolice set state=1,actdate = null ,enddate = null ,actdetail=''  where SendPolice_id='" + keyValue + "';";
                sqlSubmit = sqlSubmit + " update JW_PoliceApply set state=3,actdate='',enddate='',actdetail='' where apply_id='" + Object_id + "'";
                r = SqlHelper.ExecuteNonQuery(sqlSubmit, CommandType.Text);

            }
            else
            {
                string sqlSubmit = "update JW_SendPolice set state=3,actdate = null,enddate = null,actdetail=''  where SendPolice_id='" + keyValue + "';";
                sqlSubmit = sqlSubmit + " update JW_OnDuty set state=99 where OnDuty_id='" + Object_id + "'";
                r = SqlHelper.ExecuteNonQuery(sqlSubmit, CommandType.Text);
            }
            return r;




        }




    }
}
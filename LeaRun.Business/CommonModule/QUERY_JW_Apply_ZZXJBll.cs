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
    /// JW_Apply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.17 11:59</date>
    /// </author>
    /// </summary>
    public class QUERY_JW_Apply_ZZXJBll : RepositoryFactory<QUERY_JW_Apply>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
          
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by adddate desc) rowNumber
                                            , * 
                                        from JW_Apply 
                                            where unit_id='{0}' and type in (1,2) and state in (3,4,5) 
                                         and  PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where AreaType in (1,5,2))
                                        ) as a   "
                        , unit_id
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

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string dep_id, string ajname, string docCode, string applydatestart, string applydateend, string sarname, string sex, string contianssubordinateunit, string ishis)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" 
                                        select 
                                            ROW_NUMBER() over(order by ja.adddate desc) rowNumber, ja.* ,bpa.AreaName from JW_Apply ja
                                            left join base_policearea bpa on ja.PoliceArea_id=bpa.PoliceArea_id
                                            left join Base_Department dep on ja.dep_id=dep.Dep_id
                                            where ja.type in (1,2) 
                                            and  ja.PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where AreaType in (1,5,2))
                                         
                                       "
                        );//and ja.state in  (3,4,5)

                //if (unit_id != "")//单位ID
                //{
                //    sqlTotal = sqlTotal + " and JW_Apply.unit_id='" + unit_id + "'";
                //}
                //else
                //{
                //    if (unit_id != Share.UNIT_ID_JS)//非省院选择本单位及下级单位
                //    {
                //        sqlTotal = sqlTotal + " and (JW_Apply.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (JW_Apply.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                //    }
                //}
                if (unit_id != "")//单位ID
                {
                    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (ja.unit_id ='" + unit_id + "' or (ja.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and ja.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and ja.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        sqlTotal = sqlTotal + " and (ja.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (ja.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {
                        
                    } 
                }
                if (dep_id != "" )//部门ID
                {
                   sqlTotal = sqlTotal + " and dep.[type] = '" + dep_id + "'";
                }
                if (ajname != "")//案件名称
                {
                    sqlTotal = sqlTotal + " and (select  top 1 name from Case_caseinfo where case_id=case_id) like '%" + ajname.Trim() + "%'";
                }
                if (docCode != "")//法律文书号
                {
                    sqlTotal = sqlTotal + " and ja.docCode like '%" + docCode.Trim() + "%'";
                }
                if (applydatestart != "")//实际进入时间
                {
                    sqlTotal = sqlTotal + " and  ja.fact_indate>= '" + applydatestart + "'";
                }
                if (applydateend != "")//实际进入时间
                {
                    sqlTotal = sqlTotal + " and  ja.fact_indate<= '" + applydateend + "'";
                }
                if (sarname != "")//涉案人名称
                {
                    sqlTotal = sqlTotal + " and ja.userName like '%" + sarname.Trim() + "%'";
                }
                if (sex != "")//涉案人性别
                {
                    sqlTotal = sqlTotal + " and ja.userSex = '" + sex + "'";
                }
                if (ishis != "" && ishis != null)//状态
                {
                    if (ishis == "true")//历史0.5  ,实时非0,5
                    {
                        sqlTotal = sqlTotal + " and ja.state in (0,5) ";
                    }
                    else
                    {
                        sqlTotal = sqlTotal + " and ja.state not in (0,5) ";
                    }
                    
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

        /// <summary>
        /// 单位数
        /// </summary>
        /// <param name="unit_id">单位ID</param>
        /// <returns></returns>
        public DataTable GetList(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    ISNULL(uf.Base_Unit_id,0) AS parent_id ,--上级单位ID
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
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
        /// 警务区数
        /// </summary>
        /// <param name="unit_id">单位ID</param>
        /// <returns></returns>
        public DataTable GetListArea(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   pa.PoliceArea_id as dep_id ,  --主键
                                                    pa.AreaName as name ,	      --警务区名称
                                                    '0' AS  parent_id ,             --上级单位ID
                                                    '' as parent_name,		      --上级部门
                                                    pa.sortcode ,				  --排序字段
                                                    pa.code,   					  --Code字段
                                                    'PoliceArea' AS Sort,         --分类字段
                                                    pa.AreaType,
                                                    pa.unit_id
                                          FROM      Base_PoliceArea pa
                                        ) T WHERE 1=1 and T.AreaType in (1,5,2)");
            List<DbParameter> parameter = new List<DbParameter>();
            //if (!string.IsNullOrEmpty(unit_id))
            //{
            strSql.Append(" AND t.unit_id = @unit_id");
            parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
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
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel(string unit_id, string dep_id, string ajname, string docCode, string applydatestart, string applydateend, string sarname, string sex, string contianssubordinateunit, string ishis)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
             
                string sqlTotal =
                    string.Format(
                        @" 
                                        select ja.unitname as  '办案单位',ja.depname as '办案部门',bpa.AreaName as '办案区',ja.userName as '涉案人姓名',
                                               ja.userSex as '性别',ja.userType as '类型',replace(ja.docCode,'&nbsp;','') as '法律文书号',
                                               CONVERT(varchar(20), ja.fact_indate, 120) as '实际进入时间',CONVERT(varchar(20), ja.fact_outdate, 120) as '实际离开时间',
                                               case ja.state when 1 then '待法警部门审核' 
                                               when 1 then '待法警部门审核'
                                               when 2 then '待分管领导审批'
                                               when 3 then '待实施'
                                               when 4 then '实施中'
                                               when 5 then '结束'
                                               when 6 then '取消'
                                               when -1 then '法警部门审核退回'
                                               when -2 then '分管领导审批退回'
                                               end  as '状态',replace(ja.roomdetail,'&nbsp;','') as '功能房要求'
                                              from JW_Apply ja
                                            left join base_policearea bpa on ja.PoliceArea_id=bpa.PoliceArea_id
                                            left join Base_Department dep on ja.dep_id=dep.Dep_id                                            
                                            where ja.type in (1,2) 
                                            and  ja.PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where AreaType in (1,5,2))
                                         
                                       "
                        );//and ja.state in  (3,4,5)

             
                if (unit_id != "")//单位ID
                {
                    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (ja.unit_id ='" + unit_id + "' or (ja.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and ja.unit_id ='" + unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and ja.unit_id ='" + unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        sqlTotal = sqlTotal + " and (ja.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (ja.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }
                if (dep_id != "")//部门ID
                {
                    sqlTotal = sqlTotal + " and dep.[type] = '" + dep_id + "'";
                }
                if (ajname != "")//案件名称
                {
                    sqlTotal = sqlTotal + " and (select  top 1 name from Case_caseinfo where case_id=case_id) like '%" + ajname.Trim() + "%'";
                }
                if (docCode != "")//法律文书号
                {
                    sqlTotal = sqlTotal + " and ja.docCode like '%" + docCode.Trim() + "%'";
                }
                if (applydatestart != "")//实际进入时间
                {
                    sqlTotal = sqlTotal + " and  ja.fact_indate>= '" + applydatestart + "'";
                }
                if (applydateend != "")//实际进入时间
                {
                    sqlTotal = sqlTotal + " and  ja.fact_indate<= '" + applydateend + "'";
                }
                if (sarname != "")//涉案人名称
                {
                    sqlTotal = sqlTotal + " and ja.userName like '%" + sarname.Trim() + "%'";
                }
              
                if (sex != "")//涉案人性别
                {
                    sqlTotal = sqlTotal + " and ja.userSex = '" + sex + "'";
                }
                if (ishis != "" && ishis != null)//状态
                {
                    if (ishis == "true")//历史0.5  ,实时非0,5
                    {
                        sqlTotal = sqlTotal + " and ja.state in (0,5) ";
                    }
                    else
                    {
                        sqlTotal = sqlTotal + " and ja.state not in (0,5) ";
                    }

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
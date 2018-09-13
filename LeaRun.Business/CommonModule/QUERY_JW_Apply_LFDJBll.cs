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
    public class QUERY_JW_Apply_LFDJBll : RepositoryFactory<QUERY_JW_Apply_LFDJ>
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
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by check_time desc) rowNumber,
                                            checkIn_LF_Id,name,case sex when '1' then '男' else '女' end as sex ,sfz_id,checkInfo,goods,
                                            CONVERT(varchar(20), lf.check_time, 120) check_time ,remarks,lf.address,tel,lf.unit,photo,lf.PoliceArea_id,
                                            pa.AreaName as  PoliceArea_name,u.unit as Unit_name,us.RealName,lf.alertLever,
                                            lf.department,lf.info, lf.userJD from CheckIn_LF lf
                                            left join  Base_PoliceArea pa on lf.PoliceArea_id=pa.PoliceArea_id 
                                            left join  Base_Unit u on pa.unit_id=u.Base_Unit_id 
                                            left join  Base_user us on us.userid=lf.adduser_id 
                                            where  pa.AreaType in (3,4)  and pa.unit_id='{0}'
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
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string PoliceArea_id, string ajstate, string applydatestart, string applydateend, string lfrname, string lfrsex, string lfrcode, string contianssubordinateunit, string alertLever)
        {

            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                string.Format(
                        @"
                                        select 
                                            ROW_NUMBER() over(order by  check_time desc) rowNumber,pa.unit_id ,
                                            checkIn_LF_Id,name,case sex when '1' then '男' else '女' end as sex ,sfz_id,checkInfo,goods,
                                            CONVERT(varchar(20), lf.check_time, 120) check_time ,remarks,lf.address,tel,lf.unit,photo,lf.PoliceArea_id,
                                            pa.AreaName as  PoliceArea_name,u.unit as Unit_name,us.RealName,lf.alertLever,
                                            lf.department,lf.info, lf.userJD from CheckIn_LF lf
                                            left join  Base_PoliceArea pa on lf.PoliceArea_id=pa.PoliceArea_id 
                                            left join  Base_Unit u on pa.unit_id=u.Base_Unit_id 
                                            left join  Base_user us on us.userid=lf.adduser_id 
                                            where  pa.AreaType in (3,4)  
                                        "
                        );
              
                if (unit_id != "")//单位ID
                {
                    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
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
                    if (ManageProvider.Provider.Current().CompanyId  != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {
                    }
                }
                if (PoliceArea_id != "")//警务区ID
                {
                    sqlTotal = sqlTotal + " and pa.PoliceArea_id = '" + PoliceArea_id + "'";
                }
                if (ajstate != "")//安检情况
                {
                    sqlTotal = sqlTotal + " and lf.checkInfo like '%" + ajstate.Trim() + "%'";
                }
               
                if (applydatestart != "")//来访开始日期
                {
                    sqlTotal = sqlTotal + " and  lf.check_time> '" + applydatestart.Replace("00:00:00","") + " 00:00:00'";
                }
                if (applydateend != "")//来访结束日期
                {
                    sqlTotal = sqlTotal + " and  lf.check_time< '" + applydateend.Replace("00:00:00", "") + " 23:59:59'";
                }
                if (lfrname != "")//来访人名称
                {
                    sqlTotal = sqlTotal + " and lf.name like '%" + lfrname.Trim() + "%'";
                }
              
                if (lfrsex != "")//来访人性别
                {
                    sqlTotal = sqlTotal + " and lf.sex = '" + lfrsex + "'";
                }
                if (lfrcode != "")//身份证号
                {
                    sqlTotal = sqlTotal + " and lf.sfz_id like '%" + lfrcode + "%'";
                }
               
                if (alertLever != "")//关注级别
                {
                    sqlTotal = sqlTotal + " and lf.alertLever =  '" + alertLever + "'";
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
        /// 所有单位
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
        /// 本单位及下级单位
        /// </summary>
        /// <param name="unit_id">单位ID</param>
        /// <returns></returns>
        public DataTable GetList_quanxian(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (unit_id != Share.UNIT_ID_JS)//非省院选择本单位及下级单位
            {
                strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    '0' AS parent_id ,--上级单位ID
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id
                                        ) T WHERE 1=1 ");

                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND t.dep_id in (select base_unit_id from base_unit where base_unit_ID=@unit_id or parent_unit_id =@unit_id) ");
                    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
                }
            }
            else//省院选择全部单位
            {
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
                if (!string.IsNullOrEmpty(unit_id))
                {

                }
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
        /// 下级单位
        /// </summary>
        /// <param name="unit_id">单位ID</param>
        /// <returns></returns>
        public DataTable GetList_quanxianOnlyLower(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (unit_id != Share.UNIT_ID_JS)//非省院选择本单位及下级单位
            {
                strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    '0' AS parent_id ,--上级单位ID
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id
                                        ) T WHERE 1=1 ");

                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND t.dep_id in (select base_unit_id from base_unit where parent_unit_id =@unit_id) ");
                    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
                }
            }
            else//省院选择全部单位
            {
                strSql.Append(@"SELECT  *
                                FROM    (
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    --ISNULL(uf.Base_Unit_id,0) AS parent_id ,--上级单位ID
 case when uf.Base_Unit_id=@unit_id then '0' else uf.Base_Unit_id end as parent_id,
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id 
                                        ) T WHERE 1=1 ");
                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND t.dep_id <> '" + Share.UNIT_ID_JS + "'");
                    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
                }
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
        /// 获取下一级直属单位
        /// </summary>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable GetList_quanxianOnlyLowerNext(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (unit_id != Share.UNIT_ID_JS)//非省院选择本单位及下级单位
            {
                strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    '0' AS parent_id ,--上级单位ID
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id
                                        ) T WHERE 1=1 ");

                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND t.dep_id in (select base_unit_id from base_unit where parent_unit_id =@unit_id) ");
                    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
                }
            }
            else//省院选择全部单位
            {
                strSql.Append(@"SELECT  *
                                FROM    (
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    --ISNULL(uf.Base_Unit_id,0) AS parent_id ,--上级单位ID
 case when uf.Base_Unit_id=@unit_id then '0' else uf.Base_Unit_id end as parent_id,
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id 
                                        ) T WHERE 1=1 ");
                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND t.dep_id <> '" + Share.UNIT_ID_JS + "' and parent_id='0' ");
                    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
                }
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
                                        ) T WHERE 1=1 and T.AreaType in (3,4)");
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
        /// 警务区数所有
        /// </summary>
        /// <param name="unit_id">单位ID</param>
        /// <returns></returns>
        public DataTable GetListAreaAll(string unit_id)
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
                                        ) T WHERE 1=1 ");
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
        public DataTable GridPageApplyJsonExcel( string unit_id, string PoliceArea_id, string ajstate, string applydatestart, string applydateend, string lfrname, string lfrsex, string lfrcode, string contianssubordinateunit, string alertLever)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                        select 
                                              u.unit as '接待单位'
                                             ,pa.AreaName as  '接待场所'
                                             ,us.RealName as '执勤法警'
                                             ,name as '来访人姓名'
                                             ,case sex when '1' then '男' else '女' end as '来访人性别'
                                             ,sfz_id as '身份证号'
                                             ,lf.address as '地址'
                                             ,CONVERT(varchar(20), lf.check_time, 120) '来访日期'
                                             ,replace(checkInfo,'&nbsp;','') as '安检情况'
                                             ,replace(remarks,'&nbsp;','') as '信访情况'
                                             ,lf.alertLever as '关注级别'
                                             from CheckIn_LF lf
                                             left join  Base_PoliceArea pa on lf.PoliceArea_id=pa.PoliceArea_id 
                                             left join  Base_Unit u on pa.unit_id=u.Base_Unit_id 
                                             left join  Base_user us on us.userid=lf.adduser_id 
                                             where  pa.AreaType in (3,4)  
                                        "
                        );

                if (unit_id != "")//单位ID
                {
                    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
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
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        sqlTotal = sqlTotal + " and (pa.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (pa.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {
                    }
                }
                if (PoliceArea_id != "")//警务区ID
                {
                    sqlTotal = sqlTotal + " and pa.PoliceArea_id = '" + PoliceArea_id + "'";
                }
                if (ajstate != "")//安检情况
                {
                    sqlTotal = sqlTotal + " and lf.checkInfo like '%" + ajstate.Trim() + "%'";
                }

                if (applydatestart != "")//来访开始日期
                {
                    sqlTotal = sqlTotal + " and  lf.check_time> '" + applydatestart.Replace("00:00:00", "") + " 00:00:00'";
                }
                if (applydateend != "")//来访结束日期
                {
                    sqlTotal = sqlTotal + " and  lf.check_time< '" + applydateend.Replace("00:00:00", "") + " 23:59:59'";
                }
                if (lfrname != "")//来访人名称
                {
                    sqlTotal = sqlTotal + " and lf.name like '%" + lfrname.Trim() + "%'";
                }

                if (lfrsex != "")//来访人性别
                {
                    sqlTotal = sqlTotal + " and lf.sex = '" + lfrsex + "'";
                }
                if (lfrcode != "")//身份证号
                {
                    sqlTotal = sqlTotal + " and lf.sfz_id like '%" + lfrcode + "%'";
                }

                if (alertLever != "")//关注级别
                {
                    sqlTotal = sqlTotal + " and lf.alertLever =  '" + alertLever + "'";
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
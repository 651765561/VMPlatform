//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
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
using LeaRun.DataAccess;
using System.Data.Common;

namespace LeaRun.Business
{
    /// <summary>
    /// base_area
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.03 14:55</date>
    /// </author>
    /// </summary>
    public class AreaBll : RepositoryFactory<Base_Area>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetArea(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select area_id,name,Code from base_area";
                sql = sql + " order by area_id";

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
        /// 添加，编辑
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public int AddArea(Base_Area area, string strKeyValue)
        {
            if (strKeyValue == "")//新增
            {
                string area_id = Guid.NewGuid().ToString();
                string sql =
                     string.Format(@"insert into base_area(area_id,name,Code ) values (@area_id,@name,@Code)");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@area_id",Guid.NewGuid().ToString()),
                    new SqlParameter("@name",area.name),
                    new SqlParameter("@Code",area.Code)
                  
                };

                try
                {
                    int r = DbHelper.ExecuteNonQuery(CommandType.Text,sql, pars);
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
                    string.Format(@"update base_area set name=@name,Code=@Code where area_id=@KeyValue");

                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@name",area.name),
                    new SqlParameter("@Code",area.Code),
                    new SqlParameter("@KeyValue",strKeyValue)
                  
                };

                try
                {
                    int r = DbHelper.ExecuteNonQuery(CommandType.Text,sql, pars);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 判断当前监区是否有监室
        /// </summary>
        /// <param name="dep_id">部门ID</param>
        /// <returns></returns>
        public string HasRoom(string area_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  count(1) from base_Room where Area_id='" + area_id + "'");
            return DbHelper.ExecuteScalar(CommandType.Text, strSql.ToString()).ToString();
        }



        /// <summary>
        /// 登录时选择看守所用
        /// </summary>
        /// <param name="unit_id">单位ID</param>
        /// <returns></returns>
        public DataTable GetList()
        {    
             new DbHelper("LeaRunFramework_SqlServer");//讲连接数据库强制改为WEB_Center
//            StringBuilder strSql = new StringBuilder();
//            strSql.Append(@"SELECT top 1 '' as  dbName,'市属单位' as name,0 as parent_id,'father' as type,0 as index1 from Base_unit
//                            union
//                            SELECT dbName, name,'' as parent_id,'' as type,ksscode as index1  from Base_unit  where type='市属单位'
//                            union
//                            SELECT top 1 '' as  dbName,'看守所' as name,0 as parent_id,'father' as type,'310100000' as index1  from Base_unit
//                            union
//                            SELECT dbName, name,'' as parent_id,'' as type,ksscode as index1  from Base_unit  where type='看守所'
//                            union
//                            SELECT top 1 '' as  dbName,'拘留所' as name,0 as parent_id,'father' as type,'310900000' as index1 from Base_unit
//                            union
//                            SELECT dbName, name,'' as parent_id,'' as type,ksscode as index1  from Base_unit  where type='拘留所'
//                                       ");
//            List<DbParameter> parameter = new List<DbParameter>();

//            strSql.Append(" ORDER BY index1");


            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  d.id  ,                 
                                    d.name ,	                
                                    d.dbName,
                                    d.parent_id ,				
                                    d1.name as parent_name
                                    FROM      
                                    Base_unit d
                                    LEFT JOIN Base_unit d1 ON d.Parent_id = d1.id order by d.type
                                       ");
            List<DbParameter> parameter = new List<DbParameter>();

            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }



        /// <summary>
        /// 绑定看守所信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetData()
        {
            new DbHelper("LeaRunFramework_SqlServer");//讲连接数据库强制改为WEB_Center
            string sql = "";
            sql = " select case Name when '市属单位' then '======市属单位=======' when '看守所' then '=======看守所========'  when '拘留所' then '=======拘留所========' else Name end as name, ";
            sql = sql + "  dbname as code ";
            sql = sql + " from Base_unit order by parent_id";
           
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
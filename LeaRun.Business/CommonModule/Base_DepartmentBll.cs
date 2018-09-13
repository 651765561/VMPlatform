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
using System.Data.Common;
using LeaRun.DataAccess;

namespace LeaRun.Business
{
    /// <summary>
    /// Base_Department
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.08 10:05</date>
    /// </author>
    /// </summary>
    public class Base_DepartmentBll : RepositoryFactory<Base_Department>
    {
        #region "Index页面用"

            /// <summary>
            /// 部门管理INDEX点击树节点时展开右侧列表   （警务区管理也用到）
            /// </summary>
            /// <param name="unit_id">单位ID</param>
            /// <returns></returns>
            public DataTable GetList(string unit_id)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   d.dep_id  ,                 --主键
                                                    d.name ,	                --部门名称
                                                    d.parent_id ,				--上级部门ID
                                                    d1.name as parent_name,		--上级部门
                                                    d.state ,				    --启用状态
                                                    d.unit_id ,				    --所属单位Id
                                                    u.unit as unit_name ,	    --所属单位名称
                                                    d.sortcode ,				--排序字段
                                                    d.code,   					--Code字段
                                                    'Department' AS Sort        --分类字段
                                          FROM      Base_Department d
                                                    LEFT JOIN Base_Department d1 ON d.Parent_id = d1.Dep_id
                                                    LEFT JOIN Base_Unit u ON u.Base_Unit_id = d.unit_id
                                        ) T WHERE 1=1 ");
                List<DbParameter> parameter = new List<DbParameter>();
                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND unit_id = @unit_id");
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
            /// 判断当前部门是否有下级部门
            /// </summary>
            /// <param name="dep_id">部门ID</param>
            /// <returns></returns>
            public string HasChildDep(string dep_id)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select  count(1) from base_department where Parent_id='" + dep_id + "'");

                return DbHelper.ExecuteScalar(CommandType.Text, strSql.ToString()).ToString();
            }

            /// <summary>
            /// 判断当前部门是否有下级部门是否有用户
            /// </summary>
            /// <param name="dep_id">部门ID</param>
            /// <returns></returns>
            public string HasUser(string dep_id)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select  count(1) from base_user where dep_id='" + dep_id + "'");
                return DbHelper.ExecuteScalar(CommandType.Text, strSql.ToString()).ToString();
            }

        #endregion

        #region "Form页面用"
            
            #region "公用"

                /// <summary>
                /// 删除Base_DepartmentLeader 表数据
                /// </summary>
                /// <param name="dep_id">部门ID</param>
                /// <returns></returns>
                public int delete_Base_DepartmentLeader(string dep_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("delete from Base_DepartmentLeader where Dep_id='" + dep_id + "'");
                    return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());

                }

                /// <summary>
                /// 插入Base_DepartmentLeader 表数据
                /// </summary>
                /// <param name="dep_id">部门ID</param>
                /// <param name="user_id">用户ID</param>
                /// <returns></returns>
                public int insert_Base_DepartmentLeader(string dep_id, string user_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert Base_DepartmentLeader(Dep_id,User_id) values ('" + dep_id + "','" + user_id.Replace("'", "") + "')");
                    return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
                }

            #endregion

            #region "基本信息TAB"
                /// <summary>
                /// 查询Base_DepartmentLeader
                /// </summary>
                /// <param name="CompanyId">部门ID</param>
                /// <returns></returns>
                public DataTable GetBase_DepartmentLeader(string dep_id, string flag)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("SELECT  d.User_id,u.RealName from Base_DepartmentLeader d left join Base_User u on d.User_id=u.UserId where d.Dep_id = '" + dep_id + "'");
                    return Repository().FindTableBySql(strSql.ToString());
                }

            #endregion

            #region "监控人员TAB"
                /// <summary>
                ///根据部门Id加载监控用户列表
                /// </summary>
                /// <param name="DepartmentId">部门ID</param>
                /// <returns></returns>
                public DataTable GerDepUser_JK(string DepartmentId, string Unit_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    List<DbParameter> parameter = new List<DbParameter>();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId ,				--用户ID
                                                u.Account ,				--账户
                                                u.RealName ,			--姓名
                                                u.Code ,				--工号
                                                u.Gender ,				--性别
                                                u.CompanyId ,			--公司ID
                                                u.dep_id ,	        	--部门ID
                                                u.SortCode,   			--排序码
                                                AA.HAS                  --是否有权限
                                      FROM      Base_User u
                                                LEFT JOIN 
                                                (select  User_id AS HAS from Base_Departmentleader  
                                                 WHERE dep_id = @DepartmentId ) AA
                                                 ON AA.HAS=UserId
                                    ) T WHERE 1=1 and CompanyId=@Unit_id");


                    parameter.Add(DbFactory.CreateDbParameter("@DepartmentId", DepartmentId));
                    parameter.Add(DbFactory.CreateDbParameter("@Unit_id", Unit_id));


                    if (!ManageProvider.Provider.Current().IsSystem)
                    {
                        //strSql.Append(" AND ( UserId IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                        //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                        //strSql.Append(" ) )");
                    }
                    strSql.Append(" ORDER BY SortCode ASC");
                    return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
                }
            #endregion

        #endregion

        #region"其他坏人调用"
                /// <summary>
                /// 获取 公司、部门 列表
                /// </summary>
                /// <returns></returns>
                public DataTable GetTree()
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    Base_Unit_id as CompanyId,				--公司ID
												Base_Unit_id AS DepartmentId ,--部门ID
                                                Code ,					--编码
                                                unit ,				--名称
                                                parent_unit_id as ParentId ,				--节点ID
                                                '0' as deppid,
                                                SortCode,				--排序编码
                                                'Company' AS Sort,		--分类
                                                '0' as isdepnode
                                      FROM      Base_Unit		--公司表
                                      UNION
                                      SELECT    unit_id as CompanyId,				--公司ID
												Dep_id,			--部门ID
                                                Code ,					--编码
                                                Name ,				--名称
                                                unit_id AS ParentId ,	--节点ID
                                                Parent_id as deppid,
                                                SortCode,				--排序编码
                                                'Department' AS Sort,	--分类
                                                '1' as isdepnode
                                      FROM      Base_Department			--部门表ParentId=0
                                    ) T WHERE 1=1 ");
                    if (!ManageProvider.Provider.Current().IsSystem)
                    {
                        //strSql.Append(" AND ( DepartmentId IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                        //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                        //strSql.Append(" ) )");
                    }
                    strSql.Append(" ORDER BY SortCode ASC");
                    return Repository().FindTableBySql(strSql.ToString());
                }



                public DataTable GetTreess()
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    Base_Unit_id as CompanyId,				--公司ID
												Base_Unit_id AS DepartmentId ,--部门ID
                                                Code ,					--编码
                                                unit ,				--名称
                                                parent_unit_id as ParentId ,				--节点ID
                                                '0' as deppid,
                                                SortCode,				--排序编码
                                                'Company' AS Sort,		--分类
                                                '0' as isdepnode
                                      FROM      Base_Unit		--公司表
                                      UNION
                                      SELECT    unit_id as CompanyId,				--公司ID
												Dep_id,			--部门ID
                                                Code ,					--编码
                                                Name ,				--名称
                                                unit_id AS ParentId ,	--节点ID
                                                Parent_id as deppid,
                                                SortCode,				--排序编码
                                                'Department' AS Sort,	--分类
                                                '1' as isdepnode
                                      FROM      Base_Department			--部门表ParentId=0
                                    ) T WHERE 1=1 ");
                    List<DbParameter> parameter = new List<DbParameter>();
                    

                    strSql.Append(" AND CompanyId = @unit_id ");
                    parameter.Add(DbFactory.CreateDbParameter("@unit_id", ManageProvider.Provider.Current().CompanyId));
                    strSql.Append(" or CompanyId in (select a from GetChildUnit(@unit_ids))  ");
                    parameter.Add(DbFactory.CreateDbParameter("@unit_ids", ManageProvider.Provider.Current().CompanyId));
                    if (!ManageProvider.Provider.Current().IsSystem)
                    {
                       
                    }
                    strSql.Append(" ORDER BY  SortCode ASC");
                    return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
                  
                }


                /// <summary>
                /// 根据公司id获取可监控人员列表
                /// </summary>
                /// <param name="CompanyId">公司ID</param>
                /// <returns></returns>
                public DataTable GetUserList(string unit_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId as user_id  ,     	--主键
                                                u.CompanyId ,	            --公司ID
                                                dep_id AS parent_id,        --所属部门 ID 
                                                u.RealName as user_name,    --用户姓名
                                                u.sortcode ,				--排序字段
                                                'User' AS sort              --分类字段
                                      FROM      Base_User u
                                      UNION
                                      SELECT    d.dep_id as user_id  ,     	--主键
                                                d.unit_Id AS CompanyId,	    --公司ID
                                                d.parent_id ,     		    --上级部门ID
                                                d.Name as user_name,        --部门名称
                                                d.sortcode ,                --排序字段  
                                                'Department' AS sort        --分类字段
                                      FROM      Base_Department d
                                    ) T WHERE 1=1 ");
                    List<DbParameter> parameter = new List<DbParameter>();
                    if (!string.IsNullOrEmpty(unit_id))
                    {
                        strSql.Append(" AND CompanyId = @unit_id");
                        parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
                    }
                    //if (!ManageProvider.Provider.Current().IsSystem)
                    //{
                    //    strSql.Append(" AND ( Dep_id IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                    //    strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                    //    strSql.Append(" ) )");
                    //}
                    strSql.Append(" ORDER BY CompanyId ASC,sortcode ASC");
                    return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
                }


                /// <summary>
                /// 获取公司列表
                /// </summary>
                /// <returns></returns>
                public List<Base_Department> GetTreeList()
                {
                    StringBuilder WhereSql = new StringBuilder();
                    if (!ManageProvider.Provider.Current().IsSystem)
                    {
                        //WhereSql.Append(" AND ( [Base_Unit_id] IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                        //WhereSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                        //WhereSql.Append(" ) )");
                    }
                    WhereSql.Append(" ORDER BY SortCode ASC");
                    return Repository().FindList(WhereSql.ToString());
                }
        #endregion
    }
}
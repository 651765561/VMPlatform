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
        #region "Indexҳ����"

            /// <summary>
            /// ���Ź���INDEX������ڵ�ʱչ���Ҳ��б�   ������������Ҳ�õ���
            /// </summary>
            /// <param name="unit_id">��λID</param>
            /// <returns></returns>
            public DataTable GetList(string unit_id)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   d.dep_id  ,                 --����
                                                    d.name ,	                --��������
                                                    d.parent_id ,				--�ϼ�����ID
                                                    d1.name as parent_name,		--�ϼ�����
                                                    d.state ,				    --����״̬
                                                    d.unit_id ,				    --������λId
                                                    u.unit as unit_name ,	    --������λ����
                                                    d.sortcode ,				--�����ֶ�
                                                    d.code,   					--Code�ֶ�
                                                    'Department' AS Sort        --�����ֶ�
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
            /// �жϵ�ǰ�����Ƿ����¼�����
            /// </summary>
            /// <param name="dep_id">����ID</param>
            /// <returns></returns>
            public string HasChildDep(string dep_id)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select  count(1) from base_department where Parent_id='" + dep_id + "'");

                return DbHelper.ExecuteScalar(CommandType.Text, strSql.ToString()).ToString();
            }

            /// <summary>
            /// �жϵ�ǰ�����Ƿ����¼������Ƿ����û�
            /// </summary>
            /// <param name="dep_id">����ID</param>
            /// <returns></returns>
            public string HasUser(string dep_id)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select  count(1) from base_user where dep_id='" + dep_id + "'");
                return DbHelper.ExecuteScalar(CommandType.Text, strSql.ToString()).ToString();
            }

        #endregion

        #region "Formҳ����"
            
            #region "����"

                /// <summary>
                /// ɾ��Base_DepartmentLeader ������
                /// </summary>
                /// <param name="dep_id">����ID</param>
                /// <returns></returns>
                public int delete_Base_DepartmentLeader(string dep_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("delete from Base_DepartmentLeader where Dep_id='" + dep_id + "'");
                    return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());

                }

                /// <summary>
                /// ����Base_DepartmentLeader ������
                /// </summary>
                /// <param name="dep_id">����ID</param>
                /// <param name="user_id">�û�ID</param>
                /// <returns></returns>
                public int insert_Base_DepartmentLeader(string dep_id, string user_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert Base_DepartmentLeader(Dep_id,User_id) values ('" + dep_id + "','" + user_id.Replace("'", "") + "')");
                    return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
                }

            #endregion

            #region "������ϢTAB"
                /// <summary>
                /// ��ѯBase_DepartmentLeader
                /// </summary>
                /// <param name="CompanyId">����ID</param>
                /// <returns></returns>
                public DataTable GetBase_DepartmentLeader(string dep_id, string flag)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("SELECT  d.User_id,u.RealName from Base_DepartmentLeader d left join Base_User u on d.User_id=u.UserId where d.Dep_id = '" + dep_id + "'");
                    return Repository().FindTableBySql(strSql.ToString());
                }

            #endregion

            #region "�����ԱTAB"
                /// <summary>
                ///���ݲ���Id���ؼ���û��б�
                /// </summary>
                /// <param name="DepartmentId">����ID</param>
                /// <returns></returns>
                public DataTable GerDepUser_JK(string DepartmentId, string Unit_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    List<DbParameter> parameter = new List<DbParameter>();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId ,				--�û�ID
                                                u.Account ,				--�˻�
                                                u.RealName ,			--����
                                                u.Code ,				--����
                                                u.Gender ,				--�Ա�
                                                u.CompanyId ,			--��˾ID
                                                u.dep_id ,	        	--����ID
                                                u.SortCode,   			--������
                                                AA.HAS                  --�Ƿ���Ȩ��
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

        #region"�������˵���"
                /// <summary>
                /// ��ȡ ��˾������ �б�
                /// </summary>
                /// <returns></returns>
                public DataTable GetTree()
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    Base_Unit_id as CompanyId,				--��˾ID
												Base_Unit_id AS DepartmentId ,--����ID
                                                Code ,					--����
                                                unit ,				--����
                                                parent_unit_id as ParentId ,				--�ڵ�ID
                                                '0' as deppid,
                                                SortCode,				--�������
                                                'Company' AS Sort,		--����
                                                '0' as isdepnode
                                      FROM      Base_Unit		--��˾��
                                      UNION
                                      SELECT    unit_id as CompanyId,				--��˾ID
												Dep_id,			--����ID
                                                Code ,					--����
                                                Name ,				--����
                                                unit_id AS ParentId ,	--�ڵ�ID
                                                Parent_id as deppid,
                                                SortCode,				--�������
                                                'Department' AS Sort,	--����
                                                '1' as isdepnode
                                      FROM      Base_Department			--���ű�ParentId=0
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
                            FROM    ( SELECT    Base_Unit_id as CompanyId,				--��˾ID
												Base_Unit_id AS DepartmentId ,--����ID
                                                Code ,					--����
                                                unit ,				--����
                                                parent_unit_id as ParentId ,				--�ڵ�ID
                                                '0' as deppid,
                                                SortCode,				--�������
                                                'Company' AS Sort,		--����
                                                '0' as isdepnode
                                      FROM      Base_Unit		--��˾��
                                      UNION
                                      SELECT    unit_id as CompanyId,				--��˾ID
												Dep_id,			--����ID
                                                Code ,					--����
                                                Name ,				--����
                                                unit_id AS ParentId ,	--�ڵ�ID
                                                Parent_id as deppid,
                                                SortCode,				--�������
                                                'Department' AS Sort,	--����
                                                '1' as isdepnode
                                      FROM      Base_Department			--���ű�ParentId=0
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
                /// ���ݹ�˾id��ȡ�ɼ����Ա�б�
                /// </summary>
                /// <param name="CompanyId">��˾ID</param>
                /// <returns></returns>
                public DataTable GetUserList(string unit_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId as user_id  ,     	--����
                                                u.CompanyId ,	            --��˾ID
                                                dep_id AS parent_id,        --�������� ID 
                                                u.RealName as user_name,    --�û�����
                                                u.sortcode ,				--�����ֶ�
                                                'User' AS sort              --�����ֶ�
                                      FROM      Base_User u
                                      UNION
                                      SELECT    d.dep_id as user_id  ,     	--����
                                                d.unit_Id AS CompanyId,	    --��˾ID
                                                d.parent_id ,     		    --�ϼ�����ID
                                                d.Name as user_name,        --��������
                                                d.sortcode ,                --�����ֶ�  
                                                'Department' AS sort        --�����ֶ�
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
                /// ��ȡ��˾�б�
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
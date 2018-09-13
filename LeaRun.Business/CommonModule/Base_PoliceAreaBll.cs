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
using System.Diagnostics;
using System;

namespace LeaRun.Business
{
    /// <summary>
    /// Base_PoliceArea
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.18 13:09</date>
    /// </author>
    /// </summary>
    public class Base_PoliceAreaBll : RepositoryFactory<Base_PoliceArea>
    {
        #region "Indexҳ����"

        /// <summary>
        /// ���ݹ�˾id��ȡ�����б�
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <returns></returns>
        public string GetList(JqGridParam jqgridparam, string unit_id)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                        string.Format(
                            @" select * from ( 
                                            select 
                                                ROW_NUMBER() over(order by policearea_id) rowNumber,
                                                    p.PoliceArea_id as policearea_id  ,     --����
                                                    p.AreaName as areaname,	                --����������
                                                    p.AreaType as areatype,				    --����������
                                                    p.Address as address,		            --��������ַ
                                                    p.BuildDate,             	            --��������������
                                                   -- CONVERT(varchar(10), p.BuildDate, 120)  as builddate,	            --��������������
                                                    p.YearExamine as yearexamine,    	    --��������������
                                                    p.RoomInfo as roominfo,                 --������Ϣ
                                                    p.DepLeader as depleader,               --�����쵼
                                                    p.UnitLeader as unitleader,             --��λ�쵼
                                                    p.state,                                --״̬
                                                    p.SortCode  as sortcode,	            --�����ֶ�
                                                    p.code,   					            --Code�ֶ�
                                                    'PoliceArea' AS Sort,                   --�����ֶ�
                                                    p.unit_id,           	                --������λID
                                                    u.unit as unit_name 	                --������λ����
                                          FROM      Base_PoliceArea p
                                                    LEFT JOIN Base_Unit u ON u.Base_Unit_id = p.Unit_id
                                        ) as a where 1=1  "
                                    );
         
                if (!string.IsNullOrEmpty(unit_id))
                {
                    sqlTotal = sqlTotal+ " AND a.unit_id  ='"+unit_id+"'";
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
                 var JsonData = new
                    {
                        total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��
                        page = jqgridparam.page, //��ǰҳ��
                        records = dt.Rows.Count, //�ܼ�¼��
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


        public DataTable GetPageList(string unit_id, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" select * from ( 
                                            select 
                                                ROW_NUMBER() over(order by policearea_id) rowNumber,
                                                    p.PoliceArea_id as policearea_id  ,     --����
                                                    p.AreaName as areaname,	                --����������
                                                    p.AreaType as areatype,				    --����������
                                                    p.Address as address,		            --��������ַ
                                                    p.BuildDate,             	            --��������������
                                                   -- CONVERT(varchar(10), p.BuildDate, 120)  as builddate,	            --��������������
                                                    p.YearExamine as yearexamine,    	    --��������������
                                                    p.RoomInfo as roominfo,                 --������Ϣ
                                                    p.DepLeader as depleader,               --�����쵼
                                                    p.UnitLeader as unitleader,             --��λ�쵼
                                                    p.state,                                --״̬
                                                    p.SortCode  as sortcode,	            --�����ֶ�
                                                    p.code,   					            --Code�ֶ�
                                                    'PoliceArea' AS Sort,                   --�����ֶ�
                                                    p.unit_id,           	                --������λID
                                                    u.unit as unit_name 	                --������λ����
                                          FROM      Base_PoliceArea p
                                                    LEFT JOIN Base_Unit u ON u.Base_Unit_id = p.Unit_id
                                        ) a  where 1=1   ");
            if (!string.IsNullOrEmpty(unit_id))
            {
                strSql.Append(" AND unit_id = @unit_id");
                parameter.Add(DbFactory.CreateDbParameter("@unit_id", unit_id));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //strSql.Append(" AND ( RoleId IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //strSql.Append(" ) )");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }


        public int Delete_Base_PoliceArea(string keyValue)
        {
            //ɾ����������������ݣ����ж�JW_Apply ����JW_OnDuty���Ƿ�������ʹ�õľ�����
            string sqlCheckPoliceArea = string.Format(@"select count(*) from JW_Apply where PoliceArea_id='{0}' and state not in (0,5)", keyValue);
            string sqlCheckPoliceAreas = string.Format(@" select count(*) from JW_OnDuty where PoliceArea_id='{0}' and state = 0", keyValue);

            int Count = Repository().FindCountBySql(sqlCheckPoliceArea) + Repository().FindCountBySql(sqlCheckPoliceAreas);
            if (Count > 0)
            {
                return -100;//��ʾ��ǰ����������ʹ�ã�������ɾ�� 
            }
            StringBuilder sb = new StringBuilder();
            string sql = string.Format(@"delete Base_PoliceArea where PoliceArea_id='{0}' ", keyValue);
            sb.Append(sql);

            try
            {
                int r = Repository().ExecuteBySql(sb);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region "Formҳ����"

            #region "����"

                /// <summary>
                /// �����û�ID����û�����
                /// </summary>
                /// <param name="CompanyId">��˾ID</param>
                /// <returns></returns>
                public DataTable GetUserName(string user_id)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append(@"SELECT  *
                                    FROM    ( 
                                               SELECT   RealName as realname ,UserId               --�û�����
                                               FROM      Base_User 
                                            ) T WHERE 1=1 ");
                    List<DbParameter> parameter = new List<DbParameter>();
                    if (!string.IsNullOrEmpty(user_id))
                    {
                        strSql.Append(" AND UserId = @user_id");
                        parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@user_id", user_id));
                    }
                    if (!ManageProvider.Provider.Current().IsSystem)
                    {
                        //strSql.Append(" AND ( Dep_id IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                        //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                        //strSql.Append(" ) )");
                    }
                    //strSql.Append(" ORDER BY convert(int,SortCode) ASC");
                    return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
                }

            #endregion

            #region "������ϢTAB"

                /// <summary>
                /// ���ݾ�����id��ȡ�����쵼����λ�쵼�б�
                /// </summary>
                /// <param name="policeArea_id">������id</param>
                /// <returns></returns>
                public DataTable GetBase_LeaderName(string policeArea_id,string flag)
                {
                    StringBuilder strSql = new StringBuilder();
                    if (flag=="dep")
                    {
                      strSql.Append(@"SELECT  *
                                    FROM    ( 
                                               SELECT   isnull(DepLeader,'') as dep ,PoliceArea_id                --�����쵼
                                               FROM      Base_PoliceArea 
                                            ) T WHERE 1=1 ");
                    }
                    else
                    {
                        strSql.Append(@"SELECT  *
                                    FROM    ( 
                                               SELECT    isnull(UnitLeader,'') as unit,PoliceArea_id                --�����쵼
                                               FROM      Base_PoliceArea 
                                            ) T WHERE 1=1 ");
                    }
                    List<DbParameter> parameter = new List<DbParameter>();
                    if (!string.IsNullOrEmpty(policeArea_id))
                    {
                        strSql.Append(" AND PoliceArea_id = @policeArea_id");
                        parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@policeArea_id", policeArea_id));
                    }
                    if (!ManageProvider.Provider.Current().IsSystem)
                    {
                        //strSql.Append(" AND ( Dep_id IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                        //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                        //strSql.Append(" ) )");
                    }
                    //strSql.Append(" ORDER BY convert(int,SortCode) ASC");
                    return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
                }
                

            #endregion

            #region "�����쵼TAB"

                /// <summary>
                ///���ݲ���Id���ز����쵼�б�
                /// </summary>
                /// <param name="DepartmentId">����ID</param>
                ///<param name="Unit_id">��λID</param>
                /// <returns></returns>
                public DataTable GetPoliceAreaLeader(string policeAreaId, string Unit_id, string flag)
                {
                    StringBuilder strSql = new StringBuilder();
                    List<DbParameter> parameter = new List<DbParameter>();
                    if (flag == "dep")
                    {
                        strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId ,				--�û�ID
                                                u.Account ,				--�˻�
                                                u.RealName ,			--����
                                                u.Code ,				--����
                                                u.Gender ,				--�Ա�
                                                u.CompanyId ,			--��˾ID
                                                u.dep_id ,	        	--����ID
                                                u.SortCode,   			--������
                                                Charindex(u.UserId,p.DepLeader) AS HAS
                                      FROM      Base_User u
                                                LEFT JOIN 
                                                Base_PoliceArea p on p.PoliceArea_id=@policeAreaId
                                    ) T WHERE 1=1 and CompanyId=@Unit_id  ");

                    }
                    else
                    {
                        strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId ,				--�û�ID
                                                u.Account ,				--�˻�
                                                u.RealName ,			--����
                                                u.Code ,				--����
                                                u.Gender ,				--�Ա�
                                                u.CompanyId ,			--��˾ID
                                                u.dep_id ,	        	--����ID
                                                u.SortCode,   			--������
                                                Charindex(u.UserId,p.UnitLeader) AS HAS
                                      FROM      Base_User u
                                                LEFT JOIN 
                                                Base_PoliceArea p on p.PoliceArea_id=@policeAreaId
                                    ) T WHERE 1=1 and CompanyId=@Unit_id  ");
                    }
                    parameter.Add(DbFactory.CreateDbParameter("@policeAreaId", policeAreaId));
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

            #region "��λ�쵼TAB"

//                /// <summary>
//                ///���ݲ���Id���ز����쵼�б�
//                /// </summary>
//                /// <param name="DepartmentId">����ID</param>
//                ///<param name="Unit_id">��λID</param>
//                /// <returns></returns>
//                public DataTable GetPliceAreaUnit(string policeAreaId, string Unit_id)
//                {
//                    StringBuilder strSql = new StringBuilder();
//                    List<DbParameter> parameter = new List<DbParameter>();
//                    strSql.Append(@"SELECT  *
//                            FROM    ( SELECT    u.UserId ,				--�û�ID
//                                                u.Account ,				--�˻�
//                                                u.RealName ,			--����
//                                                u.Code ,				--����
//                                                u.Gender ,				--�Ա�
//                                                u.CompanyId ,			--��˾ID
//                                                u.dep_id ,	        	--����ID
//                                                u.SortCode,   			--������
//                                                Charindex(u.UserId,p.UnitLeader) AS HAS
//                                      FROM      Base_User u
//                                                LEFT JOIN 
//                                                Base_PoliceArea p on p.PoliceArea_id=@policeAreaId
//                                    ) T WHERE 1=1 and CompanyId=@Unit_id  ");


//                    parameter.Add(DbFactory.CreateDbParameter("@policeAreaId", policeAreaId));
//                    parameter.Add(DbFactory.CreateDbParameter("@Unit_id", Unit_id));


//                    if (!ManageProvider.Provider.Current().IsSystem)
//                    {
//                        //strSql.Append(" AND ( UserId IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
//                        //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
//                        //strSql.Append(" ) )");
//                    }
//                    strSql.Append(" ORDER BY SortCode ASC");
//                    return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
//                }

            #endregion


                #region "һ���Ե��뾯��������"
             
                public int inputData2()
                {
                    string strInsert = "";
                    ////string strDel = "delete from Base_PoliceArea ";
                    ////DbHelper.ExecuteNonQuery(CommandType.Text, strDel);

                    string strQueryUnit = "select * from Base_unit ";
                    DataTable dt=  DbHelper.GetDataSet(CommandType.Text, strQueryUnit).Tables[0];
                    for (int i=0;i<dt.Rows.Count;i++)
                    {
                        
                           ////ʡԺ,���У����� ����
                           // strInsert = "";
                           // strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           // strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','����참��','1',getdate(),getdate(),";
                           // strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'1','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-1')" ;
                           // DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                           // strInsert = "";
                           // strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           // strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','�̼�참��','2',getdate(),getdate(),";
                           // strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'2','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-2')";
                           // DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                           // strInsert = "";
                           // strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           // strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','Ϊ���������','3',getdate(),getdate(),";
                           // strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'3','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-3')";
                           // DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                           // strInsert = "";
                           // strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           // strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','����Ӵ���','4',getdate(),getdate(),";
                           // strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'4','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-4')";
                           // DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                           //strInsert = "";
                           //strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           //strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','���ذ���','7',getdate(),getdate(),";
                           //strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'7','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-7')";
                           //DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);



                           // //ʡԺ,���м������
                           // if (dt.Rows[i]["code"].ToString().Length == 2 || dt.Rows[i]["code"].ToString().Length == 5)
                           // {

                           //     strInsert = "";
                           //     strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           //     strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','���а참��','5',getdate(),getdate(),";
                           //     strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'5','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-5')";
                           //     DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                           //     strInsert = "";
                           //     strInsert = " insert  Base_PoliceArea (PoliceArea_id,AreaName,AreaType,BuildDate,YearExamine,unit_id,state,SortCode,Code)";
                           //     strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','ָ������','6',getdate(),getdate(),";
                           //     strInsert = strInsert + " '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1,'6','" + dt.Rows[i]["Base_Unit_id"].ToString() + "-6')";
                           //     DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                           // }
                    } 
                     
                 

                   
                    return 1;
                }
                #endregion

                #region "һ���Ե��뷨��ֵ���ҷ���"
             
                public int inputData3()
                {
                    ////string strInsert = "";

                    ////string strQueryUnit = "select * from Base_unit ";// insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state)  values ('9327648a-afd9-4d06-8c7a-ad1ea303b233','����참������ֵ����','500','e8bfbbe6-60de-4162-9c00-37e0d2a694dA',(select max(policearea_id)    from base_policearea where unit_id= 'ca2ba28c-c98b-4935-9183-b451e873'  and areatype=1), 'ca2ba28c-c98b-4935-9183-b451e873',1)
                    ////DataTable dt = DbHelper.GetDataSet(CommandType.Text, strQueryUnit).Tables[0];
                    ////for (int i = 0; i < dt.Rows.Count; i++)
                    ////{
                       
                    ////    strInsert = "";
                    ////    strInsert = " insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state) ";
                    ////    strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','����참������ֵ����','500','e8bfbbe6-60de-4162-9c00-37e0d2a694dA',";
                    ////    strInsert = strInsert + "(select max(policearea_id)    from base_policearea where unit_id= '" + dt.Rows[i]["Base_Unit_id"].ToString() + "'  and areatype=1), '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1)";
                    ////    DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                    ////    strInsert = "";
                    ////    strInsert = " insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state) ";
                    ////    strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','�̼�참������ֵ����','501','e8bfbbe6-60de-4162-9c00-37e0d2a694dB',";
                    ////    strInsert = strInsert + "(select max(policearea_id)    from base_policearea where unit_id= '" + dt.Rows[i]["Base_Unit_id"].ToString() + "'  and areatype=2), '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1)";
                    ////    DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                    ////    strInsert = "";
                    ////    strInsert = " insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state) ";
                    ////    strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','ָ����������ֵ����','502','e8bfbbe6-60de-4162-9c00-37e0d2a694dC',";
                    ////    strInsert = strInsert + "(select max(policearea_id)    from base_policearea where unit_id= '" + dt.Rows[i]["Base_Unit_id"].ToString() + "'  and areatype=6), '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1)";
                    ////    DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                    ////    strInsert = "";
                    ////    strInsert = " insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state) ";
                    ////    strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','����Ӵ�����ֵ����','503','e8bfbbe6-60de-4162-9c00-37e0d2a694dD',";
                    ////    strInsert = strInsert + "(select max(policearea_id)    from base_policearea where unit_id= '" + dt.Rows[i]["Base_Unit_id"].ToString() + "'  and areatype=4), '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1)";
                    ////    DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                    ////    strInsert = "";
                    ////    strInsert = " insert into Base_Room(Room_id,RoomName,RoomCode,RoomType_id,PoliceArea_id,Unit_id,state) ";
                    ////    strInsert = strInsert + " values ('" + System.Guid.NewGuid().ToString() + "','���ذ�������ֵ����','504','e8bfbbe6-60de-4162-9c00-37e0d2a694dE',";
                    ////    strInsert = strInsert + "(select max(policearea_id)    from base_policearea where unit_id= '" + dt.Rows[i]["Base_Unit_id"].ToString() + "'  and areatype=7), '" + dt.Rows[i]["Base_Unit_id"].ToString() + "',1)";
                    ////    DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                    ////}




                    return 1;
                }
                #endregion


                #region "һ���Ը��¾����������쵼�͵�λ�쵼" 

                /// <summary>
                /// ���ذ���7=����참��1
                /// </summary>
                /// <param name="policeArea_id">������id</param>
                /// <returns></returns>
                public int inputData()
                {
                    //string strInsert = "";
                    //////string strDel = "delete from Base_PoliceArea ";
                    //////DbHelper.ExecuteNonQuery(CommandType.Text, strDel);

                    //string strQueryUnit = "select * from Base_policearea where areatype=1 ";
                    //DataTable dt = DbHelper.GetDataSet(CommandType.Text, strQueryUnit).Tables[0];
                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{

                    //    strInsert = "";
                    //    strInsert = " update  Base_PoliceArea set depleader='" + dt.Rows[i]["depleader"].ToString() + "' , unitleader='" + dt.Rows[i]["unitleader"].ToString()+"'";
                    //    strInsert = strInsert + " where unit_id='" + dt.Rows[i]["unit_id"].ToString() + "' and code='" + dt.Rows[i]["unit_id"].ToString()+ "-7'";
                      
                    //    DbHelper.ExecuteNonQuery(CommandType.Text, strInsert);
                      
                    //}




                    return 1;
                }
                #endregion
        #endregion
    }
}
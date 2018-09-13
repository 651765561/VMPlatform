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
    public class Query_VideoCenterBll : RepositoryFactory<Query_VideoCenter>
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
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by apply_id) rowNumber
                                            , * 
                                        from JW_Apply 
                                            where unit_id='{0}' and type=1 and state in (3,4,5)
                                         and  PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where AreaType in (1,5))

                                        ) as a  
                                      "
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

        /// <summary>
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string dep_id, string ajname, string docCode, string applydatestart, string applydateend, string sarname, string userType, string sex, string state, string PoliceArea_id)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal = " select * from (  select   ROW_NUMBER() over(order by apply_id) rowNumber, *  from JW_Apply ";
                sqlTotal = sqlTotal + " where type=1 and state in  (3,4,5) ";
                sqlTotal = sqlTotal + "    and  PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where AreaType in (1,5))";

                if (unit_id != "")//��λID
                {
                    sqlTotal = sqlTotal + " and unit_id='" + unit_id + "'";
                }
                else
                {
                    if (unit_id != Share.UNIT_ID_JS)//��ʡԺѡ�񱾵�λ���¼���λ
                    {
                        sqlTotal = sqlTotal + " and (unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                }


                //if (unit_id != "")//��λID
                //{
                //    if (unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                //    {
                //        if (contianssubordinateunit == "1")
                //        {
                //            sqlTotal = sqlTotal + " and (JW_DailyReport.unit_id ='" + unit_id + "' or (JW_DailyReport.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                //        }
                //        else
                //        {
                //            sqlTotal = sqlTotal + " and JW_DailyReport.unit_id ='" + unit_id + "'";
                //        }
                //    }
                //    else
                //    {
                //        if (contianssubordinateunit == "1")
                //        {

                //        }
                //        else
                //        {
                //            sqlTotal = sqlTotal + " and JW_DailyReport.unit_id ='" + unit_id + "'";
                //        }
                //    }
                //}
                //else
                //{
                //    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                //    {
                //        sqlTotal = sqlTotal + " and (JW_DailyReport.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (JW_DailyReport.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                //    }
                //    else
                //    {

                //    }
                //}







                if (dep_id != "")//����ID
                {
                    sqlTotal = sqlTotal + " and dep_id='" + dep_id + "'";
                }
                if (ajname != "")//��������
                {
                    sqlTotal = sqlTotal + " and (select  top 1 name from Case_caseinfo where case_id=case_id) like '%" + ajname.Trim() + "%'";
                }
                if (docCode != "")//���������
                {
                    sqlTotal = sqlTotal + " and docCode like '%" + docCode.Trim() + "%'";
                }
                if (applydatestart != "")//���뿪ʼ����
                {
                    sqlTotal = sqlTotal + " and  adddate> '" + applydatestart + "'";
                }
                if (applydateend != "")//�����������
                {
                    sqlTotal = sqlTotal + " and  adddate< '" + applydateend + "'";
                }
                if (sarname != "")//�永������
                {
                    sqlTotal = sqlTotal + " and userName like '%" + sarname.Trim() + "%'";
                }
                if (userType != "")//�永������
                {
                    sqlTotal = sqlTotal + " and userType = '" + userType + "'";
                }
                if (sex != "")//�永���Ա�
                {
                    sqlTotal = sqlTotal + " and userSex = '" + sex + "'";
                }
                if (state != "")//���뵥״̬
                {
                    sqlTotal = sqlTotal + " and state = " + state;
                }
                if (PoliceArea_id != "")//������
                {
                    sqlTotal = sqlTotal + " and PoliceArea_id = '" + PoliceArea_id + "'";
                }

                sqlTotal = sqlTotal + "  ) as a  ";

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
                                        ) T WHERE 1=1 and T.AreaType in (1,5)");
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
    }
}
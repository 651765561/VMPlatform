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
    /// CaseBreakRoles
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.07 10:51</date>
    /// </author>
    /// </summary>
    public class CaseBreakRolesBll : RepositoryFactory<CaseBreakRoles>
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
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by breakroles_id) rowNumber
                                            ,br.breakroles_id
                                            ,br.unit_id
                                            ,br.PoliceArea_id
                                            ,br.apply_id
                                            ,br.adduser_id
                                            ,br.addDate
                                            ,br.watchuser
                                            ,br.room_id
                                            ,CONVERT(varchar(20), br.startdate, 120) startdate
                                            ,br.detail
                                            ,br.treatment ,u.unit,pa.AreaName,ja.userName
                                        from JW_BreakRoles br 
                                        left join base_unit u on u.base_unit_id=br.unit_id 
                                        left join base_policearea pa on pa.PoliceArea_id=br.PoliceArea_id  
                                        left join jw_apply ja on ja.apply_id=br.apply_id  
                                            where br.unit_id='{0}' 
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
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string PoliceArea_id, string applydatestart, string applydateend, string wjContent, string czContent, string policeName, string userName)
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
                                         select ROW_NUMBER() over(order by breakroles_id) rowNumber ,br.breakroles_id,br.unit_id ,br.PoliceArea_id ,br.apply_id
                                         ,br.adduser_id,br.addDate ,br.watchuser,br.room_id ,br.startdate ,br.detail,br.treatment ,u.unit,pa.AreaName,ja.userName
                                         from JW_BreakRoles br left join base_unit u on u.base_unit_id=br.unit_id left join base_policearea pa on pa.PoliceArea_id=br.PoliceArea_id  
                                         left join jw_apply ja on ja.apply_id=br.apply_id 
                               where 1=1
                                       "
                        );

                if (unit_id != "")//��λID
                {
                    sqlTotal = sqlTotal + " and br.unit_id='" + unit_id + "'";
                }
                if (PoliceArea_id != "")//������
                {
                    sqlTotal = sqlTotal + " and br.PoliceArea_id = '" + PoliceArea_id + "'";
                }
                if (applydatestart != "")//����ʱ�俪ʼ
                {
                    sqlTotal = sqlTotal + " and  br.startdate> '" + applydatestart + "'";
                }
                if (applydateend != "")//����ʱ�����
                {
                    sqlTotal = sqlTotal + " and  br.startdate< '" + applydateend + "'";
                }
                if (wjContent != "")//Υ��Υ�����
                {
                    sqlTotal = sqlTotal + " and br.detail like '%" + wjContent.Trim() + "%'";
                }
                if (czContent != "")//�������
                {
                    sqlTotal = sqlTotal + " and br.treatment like '%" + czContent.Trim() + "%'";
                }
                if (policeName != "")//ִ�ڷ���
                {
                    sqlTotal = sqlTotal + " and br.watchuser like '%" + policeName.Trim() + "%'";
                }
                if (userName != "")//�永��
                {
                    sqlTotal = sqlTotal + " and ja.userName like '%" + userName.Trim() + "%'";
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
    }
}
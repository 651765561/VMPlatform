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
using System.Collections;

namespace LeaRun.Business
{
    /// <summary>
    /// JW_Apply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.17 11:59</date>
    /// </author>
    /// </summary>
    public class XC_RecordBll : RepositoryFactory<QUERY_JW_Apply>
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
                        @"  select * from ( 
                                           select 
                                            ROW_NUMBER() over(order by xc.xc_datetime desc) rowNumber, xc.*
                                            ,unit.unit as unitName ,bxc_unit.unit as bxc_unitName,user1.realname from XC_Main xc
                                            left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                                            left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                                            left join base_user user1 on xc.xc_user_id=user1.UserId
                                            where xc.xc_res in (1) and  xc.xc_state in (1,2)
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
        /// <param name="jqgridparam"></param>
        /// <param name="unit_id"></param>
        /// <param name="bxc_unit_id"></param>
        /// <param name="xc_datetimestart"></param>
        /// <param name="xc_datetimeend"></param>
        /// <param name="xc_place"></param>
        /// <param name="xc_detail"></param>
        /// <param name="xc_state"></param>
        /// <param name="xc_type"></param>
        /// <param name="xc_Man">Ѳ����</param>
        /// <param name="contianssubordinateunit"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string bxc_unit_id, string xc_datetimestart,
            string xc_datetimeend, string xc_place, string xc_detail, string xc_state, string xc_type,  string xc_man ,string xc_res, string contianssubordinateunit)
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
                                            ROW_NUMBER() over(order by xc.xc_datetime desc) rowNumber, xc.*
                                            ,unit.unit as unitName ,bxc_unit.unit as bxc_unitName,user1.realname from XC_Main xc
                                            left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                                            left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                                            left join base_user user1 on xc.xc_user_id=user1.UserId
                                            where  1=1
                                       "
                        );

                if (!string.IsNullOrEmpty( xc_res)) {
                    sqlTotal = sqlTotal + " and xc.xc_res in (" + xc_res + ")";
                }

                if (bxc_unit_id != "")//��Ѳ�鵥λID
                {
                    if (bxc_unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (xc.bxc_unit_id ='" + bxc_unit_id + "' or (xc.bxc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + bxc_unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and xc.bxc_unit_id ='" + bxc_unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and xc.bxc_unit_id ='" + bxc_unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        // sqlTotal = sqlTotal + " and (xc.xc_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (xc.xc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }

                //Ѳ����
                if (xc_man != "")
                {
                    sqlTotal = sqlTotal + " and user1.realname like '%" + xc_man + "%' ";
                }
                if (unit_id != "")//��Ѳ�鵥λID
                {
                    sqlTotal = sqlTotal + " and xc.xc_unit_id ='" + unit_id + "'";
                }
                if (xc_datetimestart != "")//Ѳ��ʱ�俪ʼ
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime>= '" + xc_datetimestart + "'";
                }
                if (xc_datetimeend != "")//Ѳ��ʱ�����
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime<= '" + xc_datetimeend + "'";
                }
                if (xc_place != "")//Ѳ�鳡��
                {
                    sqlTotal = sqlTotal + " and xc.xc_place = '" + xc_place + "'";
                }
                if (xc_detail != "")//Ѳ����ϸ
                {
                    sqlTotal = sqlTotal + " and xc.xc_detail like '%" + xc_detail + "%'";
                }
                if (xc_state != "")//״̬
                {
                    sqlTotal = sqlTotal + " and xc.xc_state = '" + xc_state + "'";
                }
                if (xc_type != "")//Ѳ������
                {
                    sqlTotal = sqlTotal + " and xc.xc_type =" + xc_type;
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
                dt.Columns.Add("imageUrl", Type.GetType("System.String"));
                foreach (DataRow item in dt.Rows)
                {
                    string xc_id = item["xc_id"].ToString();
                    ArrayList arr = GetXunChaPic(xc_id);
                    if (arr != null && arr.Count > 0)
                    {
                        item["imageUrl"] = arr[0].ToString();
                    }
                }
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

        /*lwl���˹�Ѳ���¼ͼƬ*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xc_id">Ѳ�鵥λid</param>
        /// <returns></returns>
        public ArrayList GetXunChaPic(string xc_id)
        {
            ArrayList arr = new ArrayList();
            string sqlStr =string.Format( @" 
                        select  up.*  from XC_MAIN x,JW_Upload up 
                         where x.xc_id=up.[Object_id]and x.xc_id='" + xc_id + "'"
                            );
            DataTable dt= SqlHelper.DataTable(sqlStr, CommandType.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                     string picName=item["load"].ToString();
                     arr.Add(picName);
                }
            }
            return arr;
        }
        /// <summary>
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel(string unit_id, string bxc_unit_id, string xc_datetimestart, string xc_datetimeend, string xc_place,
            string xc_detail, string xc_state, string xc_type, string xc_man, string xc_res, string contianssubordinateunit)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                        select 
                                             unit.unit as 'Ѳ�鵥λ' 
                                            ,bxc_unit.unit as '��Ѳ�鵥λ'
                                            ,user1.realname as 'Ѳ����'
                                            ,CONVERT(varchar(20), xc.xc_datetime, 120) as 'Ѳ��ʱ��'
                                            ,xc.xc_place as 'Ѳ�鳡��'
                                            ,xc.xc_detail as 'Ѳ��������'
                                            ,case xc.xc_state when 0 then '���˶�' 
                                               when 1 then '������'
                                               when 2 then '������'
                                               when 3 then '�Ѵ���'
                                            end  as '״̬'
                                            from XC_Main xc
                                            left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                                            left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                                            left join base_user user1 on xc.xc_user_id=user1.UserId
                                            where  1=1
                                       " 
                        );
                if (!string.IsNullOrEmpty(xc_res))
                {
                    sqlTotal = sqlTotal + " and xc.xc_res in (" + xc_res + ")";
                }
                if (bxc_unit_id != "")//��Ѳ�鵥λID
                {
                    if (bxc_unit_id != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and (xc.bxc_unit_id ='" + bxc_unit_id + "' or (xc.bxc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + bxc_unit_id + "' ))) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and xc.bxc_unit_id ='" + bxc_unit_id + "'";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and xc.bxc_unit_id ='" + bxc_unit_id + "'";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//���ǽ���ʡԺ
                    {
                        //sqlTotal = sqlTotal + " and (xc.xc_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (xc.xc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                    }
                    else
                    {

                    }
                }
                //Ѳ����
                if (xc_man != "")
                {
                    sqlTotal = sqlTotal + " and user1.realname like '%" + xc_man + "%' ";
                }
                if (unit_id != "")//��Ѳ�鵥λID
                {
                    sqlTotal = sqlTotal + " and xc.xc_unit_id ='" + unit_id + "'";
                }
                if (xc_datetimestart != "")//Ѳ��ʱ�俪ʼ
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime>= '" + xc_datetimestart + "'";
                }
                if (xc_datetimeend != "")//Ѳ��ʱ�����
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime<= '" + xc_datetimeend + "'";
                }
                if (xc_place != "")//Ѳ�鳡��
                {
                    sqlTotal = sqlTotal + " and xc.xc_place = '" + xc_place + "'";
                }
                if (xc_detail != "")//Ѳ����ϸ
                {
                    sqlTotal = sqlTotal + " and xc.xc_detail like '%" + xc_detail + "%'";
                }
                if (xc_state != "")//״̬
                {
                    sqlTotal = sqlTotal + " and xc.xc_state = '" + xc_state + "'";
                }
                if (xc_type != "")//Ѳ������
                {
                    sqlTotal = sqlTotal + " and xc.xc_type =" + xc_type;
                }
                DataTable dt = SqlHelper.DataTable(sqlTotal, CommandType.Text);//Repository().FindTableBySql(sql);


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
        public string GridPageApplyJsonDetail(string ParameterJson, JqGridParam jqgridparam, string xc_id)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;

            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @"  select * from ( 
                                           select 
                                            ROW_NUMBER() over(order by xc.xc_datetime desc) rowNumber, con.name 
                                            ,con.detail, con.orders, con.place
                                            ,case con.type when 1 then '�淶���' when 2 then '��Ƶ���' end as  type1
                                            from XC_Main xc
                                            left join XC_Detail xcd on xc.xc_id=xcd.xc_id
                                            left join XC_Content con on xcd.xc_content_id=con.xc_content_id
                                             where  xc.xc_id='{0}'
                                        ) as a   "
                        , xc_id
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
        /// ��ȡѲ��ͷ��Ϣ
        /// </summary>
        /// <param name="xc_id"></param>
        /// <returns></returns>
        public DataTable GetHeadInfo(string xc_id)
        {
            string sql = string.Format(@" 
                select 
                 unit.unit 
                ,bxc_unit.unit 
                ,user1.realname
                ,CONVERT(varchar(20), xc.xc_datetime, 120) xc_datetime
                ,xc.xc_detail
                ,user2.realname
                ,CONVERT(varchar(20), xc.handle_date, 120) handle_date
                ,xc.handle_detail
                ,xc.xc_state
                from XC_Main xc
                left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                left join base_user user1 on xc.xc_user_id=user1.UserId
                left join base_user user2 on xc.handle_user_id=user2.UserId
                where xc.xc_id = '{0}'
                "
                , xc_id
                );
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// �ļ��ϴ��������ݿ�Ĵ���
        /// </summary>
        /// <returns></returns>
        public int Uploader(string unit_id, string user_id, string time, string type, string keyValue, string loaction, string name, string fileName)
        {
            string sql =
                string.Format(
                    @"insert JW_Upload(upload_id,unit_id,uploaduser_id,uploadDate,type,Object_id,load,realName) values(@upload_id,@unit_id,@uploaduser_id,@uploadDate,@type,@Object_id,@load,@realName)");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@upload_id",fileName), 
                new SqlParameter("@unit_id",unit_id), 
                new SqlParameter("@uploaduser_id",user_id), 
                new SqlParameter("@uploadDate",time), 
                new SqlParameter("@type",type), 
                new SqlParameter("@Object_id",keyValue), 
                new SqlParameter("@load",loaction), 
                new SqlParameter("@realName",name)
            };

            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// ���Ѿ����صķ�������
        /// </summary>
        /// <param name="object_id"></param>
        /// <returns></returns>
        public string BindLawFiles(string object_id)
        {
            string sql =
               string.Format(@"select * from JW_Upload where type='{0}' and Object_id='{1}' order by uploadDate"
                   , "20"
                   , object_id
                   );
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt == null || dt.Rows.Count <= 0 || dt.Rows[0]["upload_id"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    //������
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.AppendFormat("<li id='{0}'><a href='{1}' target='_blank'>{2}</a>&nbsp;&nbsp;" + "<a href='javascript:void(0);' title='ɾ��' onclick='deleteOwner(\"{0}\")' name='rmlink'>ɾ��</a></li>"
                            , row["upload_id"]
                            , row["load"]
                            , row["realName"]
                            );
                    }
                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ɾ����������
        /// </summary>
        /// <param name="upload_id"></param>
        /// <returns></returns>
        public string DelLawFiles(string upload_id)
        {
            string sql = string.Format(@" delete JW_Upload where upload_id='{0}' "
                , upload_id
                );
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r <= 0)
                {
                    return string.Empty;
                }
                else
                {
                    return "delSuccess";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #region lwl
        /// <summary>
        /// ��ȡ�Զ�Ѳ��
        /// </summary>
        /// <param name="xc_user_id"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public int GetAutoXunChaNum(string xc_user_id, string startdate, string enddate)
        {
            int num = 0;
            string sql = @"SELECT  
              count(1) as num
              FROM [jmsxpt2016].[dbo].[XC_Main]
              where  xc_user_id='"+xc_user_id+@"'
                and xc_type=2 and xc_datetime between '" + startdate + "'  and '" + enddate + "'";
              DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
              if (dt.Rows.Count > 0)
              {
                  num = int.Parse(dt.Rows[0][0].ToString());
              }
              return num;
        }
        #endregion
    }
}
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
using System;
using System.Diagnostics;
using System.Data.SqlClient;

namespace LeaRun.Business
{
    /// <summary>
    /// JW_AssignPolice
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.28 10:49</date>
    /// </author>
    /// </summary>
    public class JW_AssignPoliceBll : RepositoryFactory<JW_AssignPolice>
    {
        /// <summary>
        /// �б����
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql =
                    string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by ja.callpolice_id) rowNumber
                                            , ja.* ,CONVERT(varchar(20), ja.userdate, 120) AS userdate1,u.unit as unitfrom,u2.unit as unitto,ja.state as state1
                                           from JW_AssignPolice ja  left join base_unit u on ja.from_unit_id=u.base_unit_id
                                           left join base_unit u2 on ja.to_unit_id=u2.base_unit_id
                                       
                                            where ja.to_unit_id='{4}' and ja.adduser='{5}' 
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by a.addDate  "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , unit_id
                        , ManageProvider.Provider.Current().UserId
                        );
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dt.Rows.Count * 1.0 / jqgridparam.rows)), //��ҳ��
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
        /// ɾ�����������
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteJWPoliceApply(string apply_id)
        {
            string sql = string.Format(@"update JW_SendPolice set state=-2 where object_id='" + apply_id + "'; delete JW_AssignPolice where callpolice_id ='" + apply_id + "'");
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// ��λ�б���
        /// </summary>
        /// <param name="unit_id">��λID,��ʱ����</param>
        /// <returns></returns>
        public DataTable GetList(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id  ,                   --����
                                                    u.unit AS Name ,	                --��λ����
                                                    isnull(u2.Base_Unit_id,'0') as parent_unit_id,  --�ϼ�����ID
                                                    u2.unit as parent_name ,	        --�ϼ���λ����
                                                    u.sortcode ,				        --�����ֶ�
                                                    u.code,   					        --Code�ֶ�
                                                    'Unit' AS Sort                      --�����ֶ�
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit u2 ON u.parent_unit_id = u2.Base_Unit_id
                                        ) T WHERE 1=1 ");
            List<DbParameter> parameter = new List<DbParameter>();
            //if (!string.IsNullOrEmpty(unit_id))
            //{
            //    strSql.Append(" AND unit_id = @unit_id");
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
        /// �����ɾ���
        /// </summary>
        /// <param name="keyValue">�ɾ�����Object_id����� �þ����ӦID��</param>
        /// <param name="sendPolice_id">��ԱID���ö��ŷָ�</param>
        /// <returns></returns>
        public string SubmitJW_YJ(string keyValue, string sendPolice_id)
        {
            if (sendPolice_id != "" && sendPolice_id != "&nbsp;")
            {
                //���ɾ��Ĵ���

                #region ɾ��
                //1.��ɾ��ԭ��������
                string sqlDel = string.Format(@"
                    delete JW_SendPolice where  Object_id='{0}';
                    "
                    , keyValue
                    );
                sqlDel = string.Format(@"
                                    declare @errorNumber int=0 
                                    begin tran 
                                    {0} 
                                    set @errorNumber+=@@ERROR 
                                    if(@errorNumber>0) 
	                                    begin 
		                                    rollback tran 
	                                    end 
                                    else 
	                                    begin 
		                                    commit tran 
	                                    end 
                                    "
                    , sqlDel.ToString()
                    );

                try
                {
                    int r2 = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                }
                catch (Exception)
                {
                    return "�����쳣���ɾ�ʧ��";
                }

                //2.���õ���ǰ��λ�ļ��
                string sqlGetUnit = string.Format(@" select p.from_unit_id,u.longname from JW_AssignPolice p join base_unit u on p.from_unit_id=u.Base_Unit_id where p.callpolice_id='{0}' ", keyValue);
                string unitID = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["from_unit_id"].ToString();
                string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

                //3.�õ����ݿ��е�ǰ��λ��������ˮ��
                string code = string.Empty;
                string sqlGetCode =
                    string.Format(
                        @" select MAX(CONVERT(int,SUBSTRING(SendCode,6,7))) code from JW_SendPolice where Unit_id='{0}' ",
                        unitID);
                DataTable dtCode = SqlHelper.DataTable(sqlGetCode, CommandType.Text);
                if (dtCode == null || dtCode.Rows.Count <= 0 || dtCode.Rows[0]["code"].ToString() == string.Empty)
                {
                    code = DateTime.Now.Year.ToString() + "001";
                }
                else
                {
                    string year = dtCode.Rows[0]["code"].ToString().Substring(0, 4);
                    string co = dtCode.Rows[0]["code"].ToString().Substring(4, 3);
                    if (year == DateTime.Now.Year.ToString())
                    {
                        code = (Convert.ToInt32(year + co) + 1).ToString();
                    }
                    else
                    {
                        code = DateTime.Now.Year.ToString() + "001";
                    }
                }

                #endregion

                #region ���

                StringBuilder sb = new StringBuilder();
                //������µ�����
                if (!sendPolice_id.Contains(","))
                {
                    //ֻѡ����һ����
                    sb.AppendFormat(
                        @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                        , unitID
                        , unitShotName + "�쾯�" + code + "����"
                        , ManageProvider.Provider.Current().UserId
                        , DateTime.Now
                        , "6"
                        , keyValue
                        , sendPolice_id
                        , "0"
                        );

                }
                else
                {
                    //ѡ���˶����
                    string[] ids = sendPolice_id.Split(',');

                    for (int i = 0; i < ids.Length; i++)
                    {
                        sb.AppendFormat(
                          @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                          , unitID
                          , unitShotName + "�쾯�" + (Convert.ToInt32(code) + i) + "����"
                          , ManageProvider.Provider.Current().UserId
                          , DateTime.Now
                          , "6"
                          , keyValue
                          , ids[i]
                          , "0"
                          );
                    }


                }

                string sqlInsert = string.Format(@"
                                    declare @errorNumber int=0 
                                    begin tran 
                                    {0} 
                                    set @errorNumber+=@@ERROR 
                                    if(@errorNumber>0) 
	                                    begin 
		                                    rollback tran 
	                                    end 
                                    else 
	                                    begin 
		                                    commit tran 
	                                    end 
                                    "
                    , sb.ToString()
                    );


                try
                {
                    int r3 = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
                }
                catch (Exception)
                {
                    return "�����쳣���ɾ�ʧ��";
                }
                #endregion

                //#region"����״̬"
                //#endregion
            }
            return "�����ɹ�";
        }

        /// <summary>
        /// �����ɾ�����OBJECT_IDΪ �þ����ӦID�� STATE Ϊ5ʵʩ���
        /// </summary>
        /// <param name="keyValue">�þ����ӦID</param>
        /// <returns></returns>
        public int UpdatePJState(string keyValue, int PYstate)
        {
            string sql = string.Format(@" update JW_SendPolice set state=1 where object_id ='{0}' ;update JW_AssignPolice set state="+PYstate+" where callpolice_id ='{0}' ; ", keyValue);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// ����������������Ÿ�������������
        /// </summary>
        /// <param name="jwAssignPolice"></param>
        /// <returns></returns>
        public int SubmitFormMyPostFJLDBack(JW_AssignPolice jwAssignPolice)
        {
            string sql = string.Format(@" update JW_AssignPolice set fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,state=@state where callpolice_id=@callpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@fjuser_id",jwAssignPolice.fjuser_id), 
                new SqlParameter("@fjdate",jwAssignPolice.fjdate), 
                new SqlParameter("@fjdetail",jwAssignPolice.fjdetail), 
                new SqlParameter("@state",jwAssignPolice.state), 
                new SqlParameter("@callpolice_id",jwAssignPolice.callpolice_id)
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
        /// ����������������Ÿ������ύ���ֹ��쵼����
        /// </summary>
        /// <param name="jwOrderPolice"></param>
        /// <returns></returns>
        public int SubmitFormMyPostFGLD(JW_AssignPolice jwAssignPolice)
        {
            string sql = string.Format(@" update JW_AssignPolice set fjuser_id=@fjuser_id,fjdate=@fjdate,fjdetail=@fjdetail,state=@state where callpolice_id=@callpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@fjuser_id",jwAssignPolice.fjuser_id), 
                new SqlParameter("@fjdate",jwAssignPolice.fjdate), 
                new SqlParameter("@fjdetail",jwAssignPolice.fjdetail), 
                new SqlParameter("@state",jwAssignPolice.state), 
                new SqlParameter("@callpolice_id",jwAssignPolice.callpolice_id)
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
        /// ����������ֹ��쵼�˻�
        /// </summary>
        /// <returns></returns>
        public int SubmitFormMySendPoliceBackJW(JW_AssignPolice jwAssignPolice)
        {
            string sql = string.Format(@" update JW_AssignPolice set leaderuser_id=@leaderuser_id,leaderdate=@leaderdate,leaderdetail=@leaderdetail,state=@state where callpolice_id=@callpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@leaderuser_id",jwAssignPolice.leaderuser_id), 
                new SqlParameter("@leaderdate",jwAssignPolice.leaderdate), 
                new SqlParameter("@leaderdetail",jwAssignPolice.leaderdetail), 
                new SqlParameter("@state",jwAssignPolice.state), 
                new SqlParameter("@callpolice_id",jwAssignPolice.callpolice_id)
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
        /// ����������������Ÿ����ˣ��ɾ�
        /// </summary>
        /// <param name="jwAssignPolice"></param>
        /// <param name="sendPolice_id"></param>
        /// <returns></returns>
        public int SubmitFormMySendPolice(JW_AssignPolice jwAssignPolice, string sendPolice_id)
        {
            string sqlCheckState = string.Format(@"select * from JW_AssignPolice where callpolice_id='{0}'", jwAssignPolice.callpolice_id);
            DataTable dt = SqlHelper.DataTable(sqlCheckState, CommandType.Text);
            if (dt.Rows[0]["state"].ToString() == "2")
            {
                //���ֹ��쵼����
                return 10002;
            }
            if (dt.Rows[0]["state"].ToString() == "-1")
            {
                //�������Ÿ���������˻�
                return -10001;
            }
            if (dt.Rows[0]["state"].ToString() == "-2")
            {
                //�ֹ��쵼����˻�
                return -10002;
            }


            SubmitJW_YJ(jwAssignPolice.callpolice_id, sendPolice_id);
            string sqlUpdate = string.Format(@" update JW_AssignPolice set fjuser_id=@fjuser_id,fjdetail=@fjdetail,fjdate=@fjdate,state=@state where callpolice_id=@callpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@fjuser_id",jwAssignPolice.fjuser_id), 
                new SqlParameter("@fjdetail",jwAssignPolice.fjdetail), 
                new SqlParameter("@fjdate",jwAssignPolice.fjdate),
                new SqlParameter("@state",jwAssignPolice.state), 
                new SqlParameter("@callpolice_id",jwAssignPolice.callpolice_id)
            };

            int r = SqlHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, pars);
            return r;
        }

        public int SubmitFormSendPoliceConfirm(string callpolice_id, string sendPoliceId)
        {
            string sql = string.Format(@" 
                                        update JW_AssignPolice set state=4 where callpolice_id='{0}';
                                        update JW_SendPolice set state=1 where SendPolice_id='{1}';

                                        ", callpolice_id, sendPoliceId);

            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

         /// <summary>
        /// ������������
        /// </summary>
        /// <param name="keyValue">����������ӦAssignPolice_id</param>
        /// <returns></returns>
        public int EndAssignPolice(string keyValue)
        {
            int r = 0;
            string sqlSubmit = "update JW_AssignPolice set state=5 where callPolice_id='" + keyValue + "';";
            r = SqlHelper.ExecuteNonQuery(sqlSubmit, CommandType.Text);

            return r;
        }
    }
}
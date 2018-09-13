//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using System;
using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace LeaRun.Business
{
    public class Base_MapBll : RepositoryFactory<Base_MapBll>
    {
        string unit_allchildren = "";
        string unit_allparent = "";
        public string GetUnitList(string name)
        {
            string sql = string.Format(" select * from Base_Unit where unit like '" + name + "%' or parent_unit_id in(select Base_Unit_id from Base_Unit where unit like '" + name + "%')");
            try
            {
                //ManageProvider.Provider.Current().ObjectId
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string getuserrole()
        {
            string user_id = ManageProvider.Provider.Current().UserId;
            string sql = string.Format(" select RealName from Base_User where UserId='" + user_id + "'");
            try
            {
                //ManageProvider.Provider.Current().ObjectId
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetRoomTypeList(string name)
        {
            string sql = string.Format(" select * from Base_Room where Unit_id in (select Base_Unit_id from Base_Unit where unit like '" + name + "%' or parent_unit_id in(select Base_Unit_id from Base_Unit where unit like '" + name + "%'))");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetChannelList(string name)
        {
            string sql = string.Format(" select * from Base_MonitorChannels where MonitorServer_id in(select MonitorServer_id from Base_MonitorServer where Unit_id in (select Base_Unit_id from Base_Unit where unit like '" + name + "%' or parent_unit_id in(select Base_Unit_id from Base_Unit where unit like '" + name + "%')))");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// ȫʡ����
        /// </summary>
        /// <returns></returns>
        public string GetAllUnitList()
        {
            string sql = string.Format("select * from Base_Unit");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetAllAreaList(string type)
        {
            string sql;
            if (type == "1")
                sql = string.Format("select * from Base_PoliceArea where AreaType in('1','2','5')");
            else if (type == "2")
                sql = string.Format("select * from Base_PoliceArea where AreaType in('6')");
            else if (type == "3")
                sql = string.Format("select * from Base_PoliceArea where AreaType in('3','4')");
            else if (type == "4")
                sql = string.Format("select * from Base_MonitorChannels");
            else if (type == "5")
                sql = string.Format("select * from Base_Room");
            else if (type == "6")
                sql = string.Format("select * from Case_caseinfo where state='1'");
            else if (type == "7")
                sql = string.Format("select * from JW_Usedetail where isend='0'");
            else if (type == "8")
                sql = string.Format("select * from JW_SendPolice where state in(0,1,2)");
            else if (type == "9")
                sql = string.Format("select * from JW_Apply where type='3' and state in(4)");
            else if (type == "10")
                sql = string.Format("select * from CheckIn_LF");
            else if (type == "11")
                sql = string.Format("select * from JW_Usedetail where (isend='0' and datediff(hour ,startdate,getdate())>12) or (isend='1' and datediff(hour ,startdate,enddate)>12 and datediff(day,startdate,getdate())=0)");
            else if (type == "12")
                sql = string.Format("select * from JW_BreakRoles where datediff(day,startdate,getdate())=0 ");
            else
                sql = "";
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getcityinfo()
        {
            string sql = string.Format("select * from Base_Unit where parent_unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c' order by sortcode");
            try
            {
                string cityinfo = "";
                string sql2 = string.Format("select count(*) from Base_MonitorChannels where MonitorServer_id in(select MonitorServer_id from Base_MonitorServer where Unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c')");
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo = "ʡԺ," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    sql2 = string.Format("select count(*) from Base_MonitorChannels where MonitorServer_id in(select MonitorServer_id from Base_MonitorServer where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    if (dt.Rows[i]["unit"].ToString() == "���Ƹ���Ժ")
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 2) + "," + dt3.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getjwtypeinfo()
        {
            string sql = string.Format("select count(*),type from JW_Apply group by type ");
            try
            {
                string cityinfo = "";
                string sql2 = string.Format("select count(*) from CheckIn_LF");
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo = "����Ӵ�," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (dt.Rows[i]["type"].ToString() == "1")
                    {
                        cityinfo += "|����참��," + dt.Rows[i][0].ToString();
                    }
                    else if (dt.Rows[i]["type"].ToString() == "2")
                    {
                        cityinfo += "|�̼�참��," + dt.Rows[i][0].ToString();
                    }
                    else if (dt.Rows[i]["type"].ToString() == "3")
                    {
                        cityinfo += "|ָ������," + dt.Rows[i][0].ToString();
                    }
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getjwmonthinfo()
        {
            string sql = string.Format("select monthname from Base_Month order by monthname");
            try
            {
                unit_allchildren = ManageProvider.Provider.Current().CompanyId;
                getchild(unit_allchildren);
                string cityinfo = "";
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    string sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and month(SendDate)='" + dt.Rows[i]["monthname"].ToString() + "'  and unit_id in('" + unit_allchildren + "')");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["monthname"].ToString() + "��," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["monthname"].ToString() + "��," + dt2.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getcityjwinfo()
        {
            string sql = string.Format("select * from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
            try
            {
                string cityinfo = "";
                string sql2 = string.Format("select count(*) from JW_SendPolice where unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo = "ʡԺ," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    sql2 = string.Format("select count(*) from JW_SendPolice where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    if (dt.Rows[i]["unit"].ToString() == "���Ƹ���Ժ")
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 2) + "," + dt3.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //��������
        public string GetfordoList(string type)
        {
            string user_id = ManageProvider.Provider.Current().UserId;
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string sql;
            if (type == "1")//�������Ÿ��������
                sql = string.Format("select a.state from JW_Apply a join Base_PoliceArea p on a.PoliceArea_id=p.PoliceArea_id where a.state in('1') and (Charindex('" + user_id + "',DepLeader)>0) union all select state from JW_PoliceApply where state in('1') and unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',DepLeader)>0) union all select state from JW_OrderPolice where state in('1') and from_unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',DepLeader)>0)");
            else if (type == "2")//�����쵼����
                sql = string.Format("select a.state from JW_Apply a join Base_PoliceArea p on a.PoliceArea_id=p.PoliceArea_id where a.state in('2') and (Charindex('" + user_id + "',UnitLeader)>0) union all select state from JW_PoliceApply where state in('2') and unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',UnitLeader)>0) union all select state from JW_OrderPolice where state in('1') and from_unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',UnitLeader)>0)");
            else if (type == "3")//����ִ��
                sql = string.Format("select state from JW_SendPolice where user_id='" + user_id + "' and state in('0')");
            else if (type == "4")//�����˻�
                sql = string.Format("select state from JW_Apply where type='1' and adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "5")//�̼��˻�
                sql = string.Format("select state from JW_Apply where type='2' and adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "6")//ָ�������˻�
                sql = string.Format("select state from JW_Apply where type='3' and adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "7")//�ɾ�ȷ���˻�
                sql = string.Format("select state from JW_SendPolice where SendUser_id='" + user_id + "' and state in('-1')");
            else if (type == "8")//�˻�
                sql = string.Format("select state from JW_PoliceApply where adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "9")//�Ű�ִ��
                sql = string.Format("select type from JW_Schedule where (DutyUser_id='" + user_id + "' or Charindex('" + user_id + "',user_id)>0) and datediff(day,startdate,getdate())=-1");
            else if (type == "10")//ִ�о���Ǽ�
                sql = string.Format(@"select sp.sendpolice_id from JW_SendPolice sp join JW_PoliceApply pa on sp.object_id=pa.apply_id where sp.state  in ('1') and  sp.user_id ='{0}' 
                                      union 
                                      select sp.sendpolice_id from JW_SendPolice sp  
                                      left join JW_onduty  ondu on  sp.object_id=ondu.onduty_id 
                                      left join base_unit unit on unit.Base_Unit_id=sp.unit_id
                                      left join Base_PoliceArea area on area.PoliceArea_id=ondu.PoliceArea_id
                                      where sp.type in (1,2,3,8,9) and (sp.state  in (3) and ondu.state=99 ) 
                                      and sp.user_id ='{0}' and  Charindex('!@#$%^&*()',isnull(actdetail,''))<=0 "
                                      , user_id);
            else if (type == "11")//��������
                //sql = string.Format("select state from JW_AssignPolice where from_unit_id='" + unit_id + "' and state='1' ");
                sql = string.Format("select state from JW_AssignPolice where from_unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',DepLeader)>0) and state='1' ");
            else if (type == "12")//Ѳ�鷴��
                //sql = string.Format("select state from JW_AssignPolice where from_unit_id='" + unit_id + "' and state='1' ");
                sql = string.Format("select xc_state from XC_Main where bxc_unit_id='" + unit_id + "' and xc_state='1' ");
            else
                sql = "";
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetXCrb(string myid)
        {
            unit_allparent = ManageProvider.Provider.Current().CompanyId;
            getparent(unit_allparent);
            string cityinfo = "";
            try
            {
                string sql = string.Format("select top 5 xc_daliy_id,b.unit,r.adddate from XC_Daliy r join Base_Unit b on b.Base_Unit_id=r.unit_id where r.unit_id in('" + unit_allparent + "') or r.unit_id ='e2c79c56-5b58-4c62-b2a9-3bb7492c' order by adddate desc");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["unit"].ToString() + "," + dt.Rows[i]["adddate"].ToString().Split(' ')[0] + "," + dt.Rows[i]["xc_daliy_id"].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString() + "," + dt.Rows[i]["adddate"].ToString().Split(' ')[0] + "," + dt.Rows[i]["xc_daliy_id"].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //////public string Getjwtypeinfo2()//����״̬
        //////{
        //////    string sql = string.Format("select code,FullName from Base_DataDictionaryDetail where datadictionaryid='ae8ada69-5adf-490d-93c1-4201f6f2b51f'");
        //////    try
        //////    {
        //////        unit_allchildren = ManageProvider.Provider.Current().CompanyId;
        //////        getchild(unit_allchildren);
        //////        string cityinfo = "";
        //////        DataTable dt = Repository().FindTableBySql(sql.ToString());
        //////        for (int i = 0; i <= dt.Rows.Count - 1; i++)
        //////        {

        //////            if (dt.Rows[i]["FullName"].ToString() == "���������ֳ�")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo = "�����ֳ�," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "ִ�д���")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|ִ�д���," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "ִ�од�")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|ִ�од�," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "Э��ִ��ָ���������Ӿ�ס")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|Э�����," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "Э��ִ�о���������")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|Э���в�," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "����׷�����ӻ������ӵķ���������")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|����׷��," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "�����Ѳ�����")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|�����Ѳ�," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "��Ѻ���������˱����˻��ﷸ")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|��Ѻ��Ա," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "���ܷ��������˱����˻��ﷸ")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|������Ա," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "�ʹ﷨������")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|�ʹ�����," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "���������Ա��ȫ")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|������Ա," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "�칫���참������Ӵ�����ִ��")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|����ִ��," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "���봦��ͻ���¼�����")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|���봦ͻ," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "�����������")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|��������," + dt2.Rows[0][0].ToString();
        //////            }
        //////        }
        //////        return cityinfo;
        //////    }
        //////    catch (Exception)
        //////    {
        //////        return null;
        //////    }
        //////}

        /// <summary>
        /// ��ȡ�����״̬
        /// </summary>
        /// <param name="type">����������</param>
        /// <param name="unitId">��ǰ��¼�û��ĵ�λ����</param>
        /// <param name="roomType">���س���ָ��������ȫ���ķ�������</param>
        /// <returns></returns>
        public string GetRoomState(string type, string unit_id, string user_id, int width)
        {
            try
            {
                if (unit_id == null || unit_id.ToString() == "")
                {
                    unit_id = ManageProvider.Provider.Current().CompanyId;
                }
            }
            catch(Exception ex)
            {}



            //=====================================���������Ա��ʼ============================================================================================================

            // ����������еķ���---->����ʽ����ʱ�ı�unit_id   ��������:sp.type=7 ,  ����������ȷ��:sp.state=1 , �������ִ��״̬:��op.state=4��
            string sqlOrderPolice = string.Format(@"
                     select to_unit_id from JW_SendPolice sp 
                     join JW_OrderPolice op on sp.Object_id=op.orderpolice_id 
                     where sp.type=7 and sp.state=1 and op.from_unit_id=(select companyid from base_user where userid='{0}') and op.state=4
                     and sp.user_id='{0}'
           ", user_id);
            DataTable dtOrderPolice = SqlHelper.DataTable(sqlOrderPolice, CommandType.Text);
            if (dtOrderPolice != null && dtOrderPolice.Rows.Count > 0)
            {
                unit_id = dtOrderPolice.Rows[0][0].ToString();
            }

             // ������������еķ���---->����ʽ����ʱ�ı�unit_id   ��������:sp.type=6 ,  ����������ȷ��:sp.state=1 , ���������ִ��״̬:��ap.state=4��
             string sqlAssignPolice = string.Format(@"
                     select to_unit_id from JW_SendPolice sp 
                      join JW_AssignPolice ap on sp.Object_id=ap.callpolice_id 
                      join Base_User bu on sp.user_id=bu.UserId
                      where sp.type=6 and sp.state=1 and ap.from_unit_id=(select companyid from base_user where userid='{0}') and ap.state=4
                      and sp.user_id='{0}'
           ", user_id);
             DataTable dtAssignPolice = SqlHelper.DataTable(sqlAssignPolice, CommandType.Text);
             if (dtAssignPolice != null && dtAssignPolice.Rows.Count > 0)
             {
                 unit_id = dtAssignPolice.Rows[0][0].ToString();
             }

             //========================================���������Ա����=========================================================================================================

            



            string sqlArea = "select * from base_policearea where unit_id='"+unit_id+"' and areatype='" + type + "'";
            DataTable dtArea = SqlHelper.DataTable(sqlArea, CommandType.Text);      //������        
            string sql = "";
            string sqlOther = "";//type == "-1"ʱ����������ִ�ڷ���14�������޾�����������Ե�������
            string returnHtml = "";

            if (type == "-1")//�����참
             {
                        sqlOther = string.Format(@"
                        select bu.RealName,case jpa.tasktype_id 
                        when 1 then '���������ֳ�'
                        when 2 then 'ִ�д���'
                        when 3 then 'ִ�од�'
                        when 5 then 'Э��ִ�о���������'
                        when 6 then '����׷�����ӻ������ѵķ���������'
                        when 7 then '�����Ѳ�����'
                        when 8 then '��Ѻ���������˱����˻��ﷸ'
                        when 10 then '�ʹ﷨������'
                        when 11 then '���������Ա'
                        when 13 then '���봦��ͻ���¼�����'
                        when 14 then '�����������'
                        end  tasktype_id
                        from JW_SendPolice jsp
                        left join Base_user bu on bu.UserId =jsp.user_id 
                        left join JW_policeapply jpa on jpa.apply_id= jsp.object_id  
                        where jsp.unit_id='{0}' and jsp.state=1  and jsp.type in (4,5)
                        and jpa.tasktype_id not in (4,9,12)
                        "
                              , unit_id
                               );

                        DataTable dtOther = SqlHelper.DataTable(sqlOther, CommandType.Text);      //���з���
                        
                        returnHtml +=
                                   "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>����ִ�ڷ��� (�����ͣ����������ʾִ������)</h7></div>";
                        returnHtml +=
                            "<div style=\"text-align:left;margin-top:5px;margin-left:5px;font-weight:bold;\"><table ><tr>";

                        for (int i = 0; i <= dtOther.Rows.Count - 1; i++)
                        {
                            //  returnHtml += "<div  align='left' ><img src=\"/Content/Images/Icon16/user_female.png\"><input type='txt' readonly='readonly' style='width:45px;' value='" + dt.Rows[i]["RealName"].ToString() + "'  title='" + dt.Rows[i]["tasktype_id"].ToString() + "'/>";
                            returnHtml += "<td style=\"width:50px\" title='" + dtOther.Rows[i]["tasktype_id"].ToString() +
                                          "'>&nbsp;&nbsp;&nbsp;<img src='../../Content/Images/Icon16/user_policeman(2).png'/><br/>" +
                                          dtOther.Rows[i]["RealName"].ToString() + "</td>";
                        }
                        returnHtml += "</tr></table></div>";
             }
           
            
            try
            {

                for (int iArea = 0; iArea < dtArea.Rows.Count; iArea++)
                {
                    string policeAreaId = dtArea.Rows[iArea]["PoliceArea_id"].ToString();
                    if (type == "1")//����참��
                    {
                        sql = string.Format(@"
                                            select room.room_id,room.RoomName,isnull(usenow.isend,1) isend,usenow.username,usenow.apply_id,room.PoliceArea_id from
                                                (select br.room_id ,bpa.AreaType, br.RoomName,br.unit_id,RoomCode,bpa.PoliceArea_id from Base_Room br
                                                left join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id and  bpa.unit_id=br.unit_id 
                                                where br.unit_id='{1}' and br.state=1 
                                                and bpa.AreaType='{0}' and Charindex('����ֵ����',br.RoomName)<=0
                                                and br.PoliceArea_id='{2}') room
                                                left  join 
                                                (select ud.room_id,min(ud.isend) isend,max(ja.userName) userName,min(ja.apply_id) apply_id  from  JW_Usedetail ud 
                                                join jw_apply ja on ja.apply_id=ud.apply_id where ud.isend=0 group by ud.room_id ) usenow
                                                on room.room_id=usenow.Room_id order by room.RoomCode
                                              "
                               , type
                               , unit_id
                               , policeAreaId
                                );
                    }
                    else if (type == "2")//�̼�참��
                    {
                        sql = string.Format(@"
                        select room.room_id,room.RoomName,isnull(usenow.isend,1) isend,usenow.username,usenow.apply_id,room.PoliceArea_id from
                        (select br.room_id ,bpa.AreaType, br.RoomName,br.unit_id,RoomCode,bpa.PoliceArea_id from Base_Room br
                        left join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id and  bpa.unit_id=br.unit_id 
                        where br.unit_id='{1}' and br.state=1 
                        and bpa.AreaType='{0}' and Charindex('����ֵ����',br.RoomName)<=0
                        and br.PoliceArea_id='{2}') room
                        left  join 
                        (select ud.room_id,min(ud.isend) isend,max(ja.userName) userName,min(ja.apply_id) apply_id  from  JW_Usedetail ud 
                        join jw_apply ja on ja.apply_id=ud.apply_id where ud.isend=0 group by ud.room_id ) usenow
                        on room.room_id=usenow.Room_id order by room.RoomCode
                      "
                               , type
                               , unit_id
                               , policeAreaId
                                );
                    }
                    else if (type == "4")//����Ӵ�
                    {
                        sql = string.Format(@"
                        select br.* from Base_Room br 
						join Base_PoliceArea bpa on br.PoliceArea_id=bpa.PoliceArea_id 
						join Base_Unit bu on bu.Base_Unit_id=bpa.unit_id 
						where bu.Base_Unit_id='{0}' and bpa.AreaType='{1} '
                        and br.PoliceArea_id='{2}'
                        ", unit_id
                         , type
                         , policeAreaId);
                    }
                    else if (type == "6")//ָ������
                    {
                        sql = string.Format(@"
                                            select room.room_id,room.RoomName,usenow.enddate ,isnull(usenow.userName,'') userName ,isnull(usenow.state,99) state ,usenow.apply_id,room.PoliceArea_id from
                                                (select br.room_id ,bpa.AreaType, br.RoomName,br.unit_id,RoomCode,bpa.PoliceArea_id from Base_Room br
                                                left join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id and  bpa.unit_id=br.unit_id 
                                                where br.unit_id='{1}' and br.state=1 
                                                and bpa.AreaType='{0}' and Charindex('����ֵ����',br.RoomName)<=0
                                                and br.PoliceArea_id='{2}') room
                                                left  join 
                                                (select jar.room_id,min(jar.enddate) enddate,max(userName) userName,min(jar.state) state,min(ja.apply_id) apply_id  from  JW_Apply_room jar 
                                                join jw_apply ja on ja.apply_id=jar.apply_id 
                                                where jar.enddate is null and jar.startdate is not null group by jar.room_id 
                                                union 
                                                select jad.room_id,min(jad.intime) enddate,max(userName) userName,-999 as state,min(ja.apply_id) apply_id  from  JW_Apply_detail jad 
                                                left join JW_Apply_room jar  on jad.Apply_room_id=jar.Apply_room_id
                                                left join jw_apply ja on ja.apply_id=jar.apply_id 
                                                where jad.intime is null and jad.outtime is not null and jad.Room_id is not null group by jad.room_id 
                                                ) usenow
                                                on room.room_id=usenow.Room_id order by room.RoomCode
                                            "
                              , type
                              , unit_id
                              , policeAreaId
                               );

                    }
                    else if (type == "7")//���ذ���
                    {
                        sql = string.Format(@"
                        select br.* from Base_Room br 
						join Base_PoliceArea bpa on br.PoliceArea_id=bpa.PoliceArea_id 
						join Base_Unit bu on bu.Base_Unit_id=bpa.unit_id 
						where bu.Base_Unit_id='{0}' and bpa.AreaType='{1}' 
                        and br.PoliceArea_id='{2}'
                        ", unit_id
                         , type
                         , policeAreaId);
                    }
                  

                    DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);      //���з���

                    int heighta = 0;
                    int ai = 0;
                    if (dt.Rows.Count == 0)
                    {
                        heighta = 0;
                    }
                    else
                    {
                        if (width <= 1366)
                        {
                            ai = (int)Math.Ceiling((decimal)(dt.Rows.Count + 1) / 5);
                            heighta = ai * 82 + 40;
                        }
                        else if (width > 1366 && width <= 1600)
                        {
                            ai = (int)Math.Ceiling((decimal)(dt.Rows.Count + 1) / 6);
                            heighta = ai * 82 + 40;
                        }
                        else if (width > 1600 && width <= 1920)
                        {
                            ai = (int)Math.Ceiling((decimal)(dt.Rows.Count + 1) / 7);
                            heighta = ai * 82 + 40;
                        }

                    }

                    returnHtml += "<div style=\" width:100%; height:" + heighta + "px;\">";


                    if (type == "1") //����참��
                    {

                        string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "1");

                        string disabled1 = CheckOnDuty(unit_id, user_id, policeAreaId, "1", "1").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled2 = CheckOnDuty(unit_id, user_id, policeAreaId, "1", "2").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled3 = CheckOnDuty(unit_id, user_id, policeAreaId, "1", "3").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";


                        //returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>����참��</h7></div>";
                        returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 15%; text-align:left;\">" + dtArea.Rows[iArea]["areaName"].ToString() + " </th><th style=\"width: 10%; text-align:right;\">ֵ���쵼:</th><td style=\"width: 10%; text-align:left;\">" +
                                      "<span id='leaderUser1'>" + usersMsg.Split('|')[0] + "</span>" +
                                      "</td><th style=\"width: 10%; text-align:right;\">ֵ����Ա:</th><td style=\"width: 28%; text-align:left;\">" +
                                      "<span id='fjUser1'>" + usersMsg.Split('|')[1] + "</span>" +
                                      "</td><td style=\"width: 20%; text-align:center;\"><button  style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','1'); " + disabled1 + " " +
                                      ">��ʼִ��</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','2'); " + disabled2 + " " +
                                      ">���Ӱ�</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\"" +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','3'); " + disabled3 + " " +
                                      ">����ִ��</button></td></tr></table></div>";

                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            string sqlVideoId = "";
                            string sqlCanVideo = "";
                            string strVideoId = "";
                            //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ bu.parent_unit_id=(select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                            sqlCanVideo = string.Format(@"select 1 from  base_policearea bpa
                                                    join 
                                                    (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                    (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                    bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                    bu.parent_unit_id='0') a
                                                    on bpa.unit_id=a.base_unit_id 
                                                    where bpa.areatype={1}  
                                                    and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                    union 
                                                    select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                    , policeAreaId
                                                    , type
                                                    , ManageProvider.Provider.Current().UserId

                                                     );
                            DataTable dtCanVideo = SqlHelper.DataTable(sqlCanVideo, CommandType.Text);

                            if (dtCanVideo != null && dtCanVideo.Rows.Count > 0)
                            {
                                //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                                sqlVideoId = string.Format(@"
                                            select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid from Base_MonitorApplication a
                                            join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                            join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                            where Room_id='{0}' and a.type='��ط���'
                                        "
                                     , dt.Rows[i]["room_id"].ToString()
                                 );
                                DataTable dtVideoId = SqlHelper.DataTable(sqlVideoId, CommandType.Text);

                                if (dtVideoId != null && dtVideoId.Rows.Count > 0)
                                {
                                    for (int j = 0; j <= dtVideoId.Rows.Count - 1; j++)
                                    {
                                        strVideoId += dtVideoId.Rows[j]["VideoId"].ToString() + ",";
                                    }
                                }
                                //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                            }
                            //�жϵ�½���Ƿ���Ȩ�޿���ؽ���


                            if (dt.Rows[i]["isend"].ToString() == "0")//������ʹ����
                            {
                                if (strVideoId != "")//��½����Ȩ�޿����
                                {
                                    returnHtml += "<div  align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:10px;height:30px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>" + dt.Rows[i]["userName"].ToString() + "</div><div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId + "','" + dt.Rows[i]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div>";
                                }
                                else//��½��û��Ȩ�޿����
                                {
                                    returnHtml += "<div  align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:10px;height:30px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>" + dt.Rows[i]["userName"].ToString() + "</div>";
                                }
                            }
                            else//����û��ʹ��
                            {
                                returnHtml += "<div  align='left' style=\"cursor:hand;background-color:Green;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:15px;font-weight:bold\">�����С���</div>";
                            }
                            returnHtml += "</div>";
                        }
                        //���ӷ���ֵ����
                        //��ȡ����
                        string policeName = "";
                        if (dt.Rows.Count > 0)//�з���ģ�������ֵ����
                        {


                            string sqlVideoId_fjzbs = "";
                            string sqlCanVideo_fjzbs = "";
                            string strVideoId_fjzbs = "";
                            //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                            sqlCanVideo_fjzbs = string.Format(@"select 1 from  base_policearea bpa
                                                                            join 
                                                                            (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                            (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                            bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                            bu.parent_unit_id='0') a
                                                                            on bpa.unit_id=a.base_unit_id 
                                                                            where bpa.areatype={1}  
                                                                            and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                            union 
                                                                            select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                    , policeAreaId
                                                    , type
                                                    , ManageProvider.Provider.Current().UserId

                                                     );
                            DataTable dtCanVideo_fjzbs = SqlHelper.DataTable(sqlCanVideo_fjzbs, CommandType.Text);
                            DataTable dtVideoId_fjzbs = null;
                            if (dtCanVideo_fjzbs != null && dtCanVideo_fjzbs.Rows.Count > 0)
                            {
                                //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                                sqlVideoId_fjzbs = string.Format(@"
                                                                    select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid,a.Room_id,r.RoomName from Base_MonitorApplication a
                                                                    join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                                    join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                                    join Base_Room r on a.Room_id=r.Room_id
                                                                    where a.Room_id=(select max(room_id) from Base_Room br join Base_RoomType brt on  br.RoomType_id=brt.RoomType_id and brt.Name='����참������ֵ����' where br.PoliceArea_id='{0}') and a.type='��ط���'
                                                                "
                                     , policeAreaId
                                 );
                                dtVideoId_fjzbs = SqlHelper.DataTable(sqlVideoId_fjzbs, CommandType.Text);

                                if (dtVideoId_fjzbs != null && dtVideoId_fjzbs.Rows.Count > 0)
                                {
                                    for (int j = 0; j <= dtVideoId_fjzbs.Rows.Count - 1; j++)
                                    {
                                        strVideoId_fjzbs += dtVideoId_fjzbs.Rows[j]["VideoId"].ToString() + ",";
                                    }
                                }
                                //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                            }
                            //�жϵ�½���Ƿ���Ȩ�޿���ؽ���



                            string sql1 = string.Format(@"
                                                        select  distinct bu.RealName from JW_SendPolice js 
                                                        join Base_User bu on bu.UserId=js.user_id 
                                                        join JW_OnDuty jo on jo.OnDuty_id=js.Object_id 
                                                        join Base_PoliceArea bpa on bpa.PoliceArea_id=jo.PoliceArea_id 
                                                        where js.state in ('1','2') and js.type='1' and bpa.unit_id='{0}'
                                                        and bpa.PoliceArea_id='{1}'
                                                        ", unit_id,policeAreaId);


                            DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                            policeName = "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr><td>";
                            for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                            {
                                policeName += dt1.Rows[j]["RealName"].ToString() + ",";
                            }
                            policeName += "</td></tr></table></div>";
                            //returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����ֵ����</div>" + policeName + "</div>";
                            if (strVideoId_fjzbs != "")//��½����Ȩ�޿����
                            {
                                returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����참������ֵ����</div>" + policeName + "<div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId_fjzbs + "','" + dtVideoId_fjzbs.Rows[0]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div></div></div>";
                            }
                            else
                            {
                                returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����참������ֵ����</div>" + policeName + "</div></div>";
                            }
                        }
                    }

                    else if (type == "4")//����Ӵ�
                    {

                        string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "8");

                        string disabled1 = CheckOnDuty(unit_id, user_id, policeAreaId, "8", "1").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled2 = CheckOnDuty(unit_id, user_id, policeAreaId, "8", "2").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled3 = CheckOnDuty(unit_id, user_id, policeAreaId, "8", "3").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";


                        //returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>����참��</h7></div>";
                        returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 15%; text-align:left;\">" + dtArea.Rows[iArea]["areaName"].ToString() + "</th><th style=\"width: 10%; text-align:right;\">ֵ���쵼:</th><td style=\"width: 10%; text-align:left;\">" +
                                      "<span id='leaderUser1'>" + usersMsg.Split('|')[0] + "</span>" +
                                      "</td><th style=\"width: 10%; text-align:right;\">ֵ����Ա:</th><td style=\"width: 28%; text-align:left;\">" +
                                      "<span id='fjUser1'>" + usersMsg.Split('|')[1] + "</span>" +
                                      "</td><td style=\"width: 20%; text-align:center;\"><button  style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','8','1'); " + disabled1 + " " +
                                      ">��ʼִ��</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','8','2'); " + disabled2 + " " +
                                      ">���Ӱ�</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\"" +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','8','3'); " + disabled3 + " " +
                                      ">����ִ��</button></td></tr></table></div>";
                        //���ӷ���ֵ����
                        //��ȡ����
                        string policeName = "";


                        string sqlVideoId_fjzbs = "";
                        string sqlCanVideo_fjzbs = "";
                        string strVideoId_fjzbs = "";
                        //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                        sqlCanVideo_fjzbs = string.Format(@"select 1 from  base_policearea bpa
                                                                                        join 
                                                                                        (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                                        (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                                        bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                                        bu.parent_unit_id='0') a
                                                                                        on bpa.unit_id=a.base_unit_id 
                                                                                        where bpa.areatype={1}  
                                                                                        and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                                        union 
                                                                                        select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                , policeAreaId
                                                , type
                                                , ManageProvider.Provider.Current().UserId

                                                 );
                        DataTable dtCanVideo_fjzbs = SqlHelper.DataTable(sqlCanVideo_fjzbs, CommandType.Text);
                        DataTable dtVideoId_fjzbs = null;
                        if (dtCanVideo_fjzbs != null && dtCanVideo_fjzbs.Rows.Count > 0)
                        {
                            //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                            sqlVideoId_fjzbs = string.Format(@"
                                                                                select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid,a.Room_id,r.RoomName from Base_MonitorApplication a
                                                                                join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                                                join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                                                join Base_Room r on a.Room_id=r.Room_id
                                                                                where a.Room_id=(select max(room_id) from Base_Room br join Base_RoomType brt on  br.RoomType_id=brt.RoomType_id and brt.Name='����Ӵ�����ֵ����' where br.PoliceArea_id='{0}') and a.type='��ط���'
                                                                            "
                                 , policeAreaId
                             );
                            dtVideoId_fjzbs = SqlHelper.DataTable(sqlVideoId_fjzbs, CommandType.Text);

                            if (dtVideoId_fjzbs != null && dtVideoId_fjzbs.Rows.Count > 0)
                            {
                                for (int j = 0; j <= dtVideoId_fjzbs.Rows.Count - 1; j++)
                                {
                                    strVideoId_fjzbs += dtVideoId_fjzbs.Rows[j]["VideoId"].ToString() + ",";
                                }
                            }
                            //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                        }
                        //�жϵ�½���Ƿ���Ȩ�޿���ؽ���

                        string sql1 = string.Format(@"
                                                                    select  distinct bu.RealName from JW_SendPolice js 
                                                                    join Base_User bu on bu.UserId=js.user_id 
                                                                    join JW_OnDuty jo on jo.OnDuty_id=js.Object_id 
                                                                    join Base_PoliceArea bpa on bpa.PoliceArea_id=jo.PoliceArea_id 
                                                                    where js.state in ('1','2') and js.type='8' and bpa.unit_id='{0}'
                                                                    and bpa.PoliceArea_id='{1}'
                                                                    ", unit_id,policeAreaId);


                        DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                        policeName = "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr>";
                        for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                        {
                            //policeName += "<td><img src='../../Content/Images/Icon16/user_policeman(2).png'/><br/>" + dt1.Rows[j]["RealName"].ToString() + "</td>";
                            policeName += dt1.Rows[j]["RealName"].ToString() + ",";
                        }
                        policeName += "</tr></table></div>";
                        //returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����ֵ����</div>" + policeName + "</div>";
                        if (strVideoId_fjzbs != "")//��½����Ȩ�޿����
                        {
                            returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����Ӵ�����ֵ����</div>" + policeName + "<div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId_fjzbs + "','" + dtVideoId_fjzbs.Rows[0]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div></div></div>";
                        }
                        else
                        {
                            returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����Ӵ�����ֵ����</div>" + policeName + "</div></div>";
                        }
                    }
                    else if (type == "7")//���ذ���
                    {

                        string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "9");

                        string disabled1 = CheckOnDuty(unit_id, user_id, policeAreaId, "9", "1").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled2 = CheckOnDuty(unit_id, user_id, policeAreaId, "9", "2").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled3 = CheckOnDuty(unit_id, user_id, policeAreaId, "9", "3").Split('|')[0].ToString() == "error" ? " " +
                                                                                                                                       "disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 15%; text-align:left;\">" + dtArea.Rows[iArea]["areaName"].ToString() + "</th><th style=\"width: 10%; text-align:right;\">ֵ���쵼:</th><td style=\"width: 10%; text-align:left;\">" +
                                      "<span id='leaderUser1'>" + usersMsg.Split('|')[0] + "</span>" +
                                      "</td><th style=\"width: 10%; text-align:right;\">ֵ����Ա:</th><td style=\"width: 28%; text-align:left;\">" +
                                      "<span id='fjUser1'>" + usersMsg.Split('|')[1] + "</span>" +
                                      "</td><td style=\"width: 20%; text-align:center;\"><button  style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','9','1'); " + disabled1 + " " +
                                      ">��ʼִ��</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','9','2'); " + disabled2 + " " +
                                      ">���Ӱ�</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\"" +
                                      "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','9','3'); " + disabled3 + " " +
                                      ">����ִ��</button></td></tr></table></div>";

                      

                        //���ӷ���ֵ����
                        //��ȡ����
                        string policeName = "";


                        string sqlVideoId_fjzbs = "";
                        string sqlCanVideo_fjzbs = "";
                        string strVideoId_fjzbs = "";
                        //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                        sqlCanVideo_fjzbs = string.Format(@"select 1 from  base_policearea bpa
                                                                                        join 
                                                                                        (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                                        (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                                        bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                                        bu.parent_unit_id='0') a
                                                                                        on bpa.unit_id=a.base_unit_id 
                                                                                        where bpa.areatype={1}  
                                                                                        and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                                        union 
                                                                                        select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                , policeAreaId
                                                , type
                                                , ManageProvider.Provider.Current().UserId

                                                 );
                        DataTable dtCanVideo_fjzbs = SqlHelper.DataTable(sqlCanVideo_fjzbs, CommandType.Text);
                        DataTable dtVideoId_fjzbs = null;
                        if (dtCanVideo_fjzbs != null && dtCanVideo_fjzbs.Rows.Count > 0)
                        {
                            //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                            sqlVideoId_fjzbs = string.Format(@"
                                                                                select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid,a.Room_id,r.RoomName from Base_MonitorApplication a
                                                                                join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                                                join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                                                join Base_Room r on a.Room_id=r.Room_id
                                                                                where a.Room_id=(select max(room_id) from Base_Room br join Base_RoomType brt on  br.RoomType_id=brt.RoomType_id and brt.Name='���ذ�������ֵ����' where br.PoliceArea_id='{0}') and a.type='��ط���'
                                                                            "
                                 , policeAreaId
                             );
                            dtVideoId_fjzbs = SqlHelper.DataTable(sqlVideoId_fjzbs, CommandType.Text);

                            if (dtVideoId_fjzbs != null && dtVideoId_fjzbs.Rows.Count > 0)
                            {
                                for (int j = 0; j <= dtVideoId_fjzbs.Rows.Count - 1; j++)
                                {
                                    strVideoId_fjzbs += dtVideoId_fjzbs.Rows[j]["VideoId"].ToString() + ",";
                                }
                            }
                            //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                        }
                        //�жϵ�½���Ƿ���Ȩ�޿���ؽ���

                        string sql1 = string.Format(@"
                                                                    select  distinct bu.RealName from JW_SendPolice js 
                                                                    join Base_User bu on bu.UserId=js.user_id 
                                                                    join JW_OnDuty jo on jo.OnDuty_id=js.Object_id 
                                                                    join Base_PoliceArea bpa on bpa.PoliceArea_id=jo.PoliceArea_id 
                                                                    where js.state in ('1','2') and js.type='9' and bpa.unit_id='{0}'
                                                                    and bpa.PoliceArea_id='{1}'
                                                                    ", unit_id,policeAreaId);


                        DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                        policeName = "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr>";
                        for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                        {
                            policeName += dt1.Rows[j]["RealName"].ToString() + ",";
                        }
                        policeName += "</tr></table></div>";
                        //returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����ֵ����</div>" + policeName + "</div>";
                        if (strVideoId_fjzbs != "")//��½����Ȩ�޿����
                        {
                            returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;���ذ�������ֵ����</div>" + policeName + "<div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId_fjzbs + "','" + dtVideoId_fjzbs.Rows[0]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div></div></div>";
                        }
                        else
                        {
                            returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;���ذ�������ֵ����</div>" + policeName + "</div></div>";
                        }

                    }

                    else if (type == "2") //�̼�참��
                    {
                      
                        string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "2");

                        string disabled1 = CheckOnDuty(unit_id, user_id, policeAreaId, "2", "1").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\" " : "class=\"btn btn-xs btn-pink\" ";
                        string disabled2 = CheckOnDuty(unit_id, user_id, policeAreaId, "2", "2").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled3 = CheckOnDuty(unit_id, user_id, policeAreaId, "2", "3").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        //returnHtml +="<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>�̼�참��</h7></div>";
                        returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 15%; text-align:left;\">" + dtArea.Rows[iArea]["areaName"].ToString() + "</th><th style=\"width: 10%; text-align:right;\">ֵ���쵼:</th><td style=\"width: 10%; text-align:left;\">" +
                                     "<span id='leaderUser2'>" + usersMsg.Split('|')[0] + "</span>" +
                                     "</td><th style=\"width: 10%; text-align:right;\">ֵ����Ա:</th><td style=\"width: 28%; text-align:left;\">" +
                                     "<span id='fjUser2'>" + usersMsg.Split('|')[1] + "</span>" +
                                     "</td><td style=\"width: 20%; text-align:center;\"><button style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                     "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','2','1'); " + disabled1 + " " +
                                     ">��ʼִ��</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                     "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','2','2'); " + disabled2 + " " +
                                     ">���Ӱ�</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                     "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','2','3'); " + disabled3 + " " +
                                     ">����ִ��</button></td></tr></table></div>";


                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            string sqlVideoId = "";
                            string sqlCanVideo = "";
                            string strVideoId = "";
                            //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                            sqlCanVideo = string.Format(@"select 1 from  base_policearea bpa
                                                                join 
                                                                (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                bu.parent_unit_id='0') a
                                                                on bpa.unit_id=a.base_unit_id 
                                                                where bpa.areatype={1}  
                                                                and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                union 
                                                                select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                    , policeAreaId
                                                    , type
                                                    , ManageProvider.Provider.Current().UserId

                                                     );
                            DataTable dtCanVideo = SqlHelper.DataTable(sqlCanVideo, CommandType.Text);

                            if (dtCanVideo != null && dtCanVideo.Rows.Count > 0)
                            {
                                //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                                sqlVideoId = string.Format(@"
                                                        select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid from Base_MonitorApplication a
                                                        join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                        join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                        where Room_id='{0}' and a.type='��ط���'
                                                    "
                                     , dt.Rows[i]["room_id"].ToString()
                                 );
                                DataTable dtVideoId = SqlHelper.DataTable(sqlVideoId, CommandType.Text);

                                if (dtVideoId != null && dtVideoId.Rows.Count > 0)
                                {
                                    for (int j = 0; j <= dtVideoId.Rows.Count - 1; j++)
                                    {
                                        strVideoId += dtVideoId.Rows[j]["VideoId"].ToString() + ",";
                                    }
                                }
                                //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                            }
                            //�жϵ�½���Ƿ���Ȩ�޿���ؽ���


                            if (dt.Rows[i]["isend"].ToString() == "0")//������ʹ����
                            {
                                if (strVideoId != "")//��½����Ȩ�޿����
                                {
                                    returnHtml += "<div  align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:10px;height:30px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>" + dt.Rows[i]["userName"].ToString() + "</div><div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId + "','" + dt.Rows[i]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div>";
                                }
                                else//��½��û��Ȩ�޿����
                                {
                                    returnHtml += "<div  align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:10px;height:30px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>" + dt.Rows[i]["userName"].ToString() + "</div>";
                                }
                            }
                            else //����û��ʹ��
                            {
                                returnHtml +=
                                    "<div  align='left' style=\"cursor:hand;background-color:Green;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" +
                                    dt.Rows[i]["RoomName"].ToString() +
                                    "</div><div style=\"text-align:center;margin-top:15px;font-weight:bold\">�����С���</div>";
                            }
                            returnHtml += "</div>";
                        }
                        //���ӷ���ֵ����
                        //��ȡ����
                        string policeName = "";

                        if (dt.Rows.Count > 0) //�з���ģ�������ֵ����
                        {

                            string sqlVideoId_fjzbs = "";
                            string sqlCanVideo_fjzbs = "";
                            string strVideoId_fjzbs = "";
                            //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                            sqlCanVideo_fjzbs = string.Format(@"select 1 from  base_policearea bpa
                                                                                        join 
                                                                                        (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                                        (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                                        bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                                        bu.parent_unit_id='0') a
                                                                                        on bpa.unit_id=a.base_unit_id 
                                                                                        where bpa.areatype={1}  
                                                                                        and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                                        union 
                                                                                        select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                    , policeAreaId
                                                    , type
                                                    , ManageProvider.Provider.Current().UserId

                                                     );
                            DataTable dtCanVideo_fjzbs = SqlHelper.DataTable(sqlCanVideo_fjzbs, CommandType.Text);
                            DataTable dtVideoId_fjzbs = null;
                            if (dtCanVideo_fjzbs != null && dtCanVideo_fjzbs.Rows.Count > 0)
                            {
                                //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                                sqlVideoId_fjzbs = string.Format(@"
                                                                                select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid,a.Room_id,r.RoomName from Base_MonitorApplication a
                                                                                join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                                                join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                                                join Base_Room r on a.Room_id=r.Room_id
                                                                                where a.Room_id=(select max(room_id) from Base_Room br join Base_RoomType brt on  br.RoomType_id=brt.RoomType_id and brt.Name='�̼�참������ֵ����' where br.PoliceArea_id='{0}') and a.type='��ط���'
                                                                            "
                                     , policeAreaId
                                 );
                                dtVideoId_fjzbs = SqlHelper.DataTable(sqlVideoId_fjzbs, CommandType.Text);

                                if (dtVideoId_fjzbs != null && dtVideoId_fjzbs.Rows.Count > 0)
                                {
                                    for (int j = 0; j <= dtVideoId_fjzbs.Rows.Count - 1; j++)
                                    {
                                        strVideoId_fjzbs += dtVideoId_fjzbs.Rows[j]["VideoId"].ToString() + ",";
                                    }
                                }
                                //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                            }
                            //�жϵ�½���Ƿ���Ȩ�޿���ؽ���



                            string sql1 = string.Format(@"
                                                                    select  distinct bu.RealName from JW_SendPolice js 
                                                                    join Base_User bu on bu.UserId=js.user_id 
                                                                    join JW_OnDuty jo on jo.OnDuty_id=js.Object_id 
                                                                    join Base_PoliceArea bpa on bpa.PoliceArea_id=jo.PoliceArea_id 
                                                                    where js.state in ('1','2') and js.type='2' and bpa.unit_id='{0}'
                                                                    and bpa.PoliceArea_id='{1}'
                                                                    ", unit_id,policeAreaId);


                            DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                            policeName =
                                "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr>";
                            for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                            {
                                policeName +=  dt1.Rows[j]["RealName"].ToString() + ",";
                            }
                            policeName += "</tr></table></div>";
                            // returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����ֵ����</div>" +policeName + "</div>";
                            if (strVideoId_fjzbs != "")//��½����Ȩ�޿����
                            {
                                returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;�̼�참������ֵ����</div>" + policeName + "<div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId_fjzbs + "','" + dtVideoId_fjzbs.Rows[0]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div></div></div>";
                            }
                            else
                            {
                                returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;�̼�참������ֵ����</div>" + policeName + "</div></div>";
                            }
                        }
                    }

                    else if (type == "6") //ָ������
                    {

                        //returnHtml +="<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>ָ������</h7></div>";

                        //returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 25%; text-align:left;\">ָ������</th><th style=\"width: 10%; text-align:right;\">ֵ���쵼:</th><td style=\"width: 15%; text-align:left;\">" +
                        //             "<span id='leaderUser1'></span>" +
                        //             "</td><th style=\"width: 10%; text-align:right;\">ֵ����Ա:</th><td style=\"width: 15%; text-align:left;\">" +
                        //             "<span id='fjUser1'></span>" +
                        //             "</td><td style=\"width: 25%; text-align:center;\"><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" >��ʼִ��</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" >���Ӱ�</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" >����ִ��</button></td></tr></table></div>";

                       

                        string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "3");

                        string disabled1 = CheckOnDuty(unit_id, user_id, policeAreaId, "3", "1").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled2 = CheckOnDuty(unit_id, user_id, policeAreaId, "3", "2").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";
                        string disabled3 = CheckOnDuty(unit_id, user_id, policeAreaId, "3", "3").Split('|')[0].ToString() == "error" ? " disabled='disabled'; class=\"btn btn-xs\"" : "class=\"btn btn-xs btn-pink\"";



                        returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 15%; text-align:left;\">" + dtArea.Rows[iArea]["areaName"].ToString() + "</th><th style=\"width: 10%; text-align:right;\">ֵ���쵼:</th><td style=\"width: 10%; text-align:left;\">" +
                                     "<span id='leaderUser3'>" + usersMsg.Split('|')[0] + "</span>" +
                                     "</td><th style=\"width: 10%; text-align:right;\">ֵ����Ա:</th><td style=\"width: 28%; text-align:left;\">" +
                                     "<span id='fjUser3'>" + usersMsg.Split('|')[1] + "</span>" +
                                     "</td><td style=\"width: 20%; text-align:center;\"><button style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                     "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','3','1'); " + disabled1 + " " +
                                     ">��ʼִ��</button><button style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                     "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','3','2'); " + disabled2 + " " +
                                     ">���Ӱ�</button><button  style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                     "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','3','3'); " + disabled3 + " " +
                                     ">����ִ��</button></td></tr></table></div>";

                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            string sqlVideoId = "";
                            string sqlCanVideo = "";
                            string strVideoId = "";
                            //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                            sqlCanVideo = string.Format(@"select 1 from  base_policearea bpa
                                                                join 
                                                                (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                bu.parent_unit_id='0') a
                                                                on bpa.unit_id=a.base_unit_id 
                                                                where bpa.areatype={1}  
                                                                and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                union 
                                                                select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                    , policeAreaId
                                                    , type
                                                    , ManageProvider.Provider.Current().UserId

                                                     );
                            DataTable dtCanVideo = SqlHelper.DataTable(sqlCanVideo, CommandType.Text);

                            if (dtCanVideo != null && dtCanVideo.Rows.Count > 0)
                            {
                                //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                                sqlVideoId = string.Format(@"
                                                        select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid from Base_MonitorApplication a
                                                        join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                        join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                        where Room_id='{0}' and a.type='��ط���'
                                                    "
                                     , dt.Rows[i]["room_id"].ToString()
                                 );
                                DataTable dtVideoId = SqlHelper.DataTable(sqlVideoId, CommandType.Text);

                                if (dtVideoId != null && dtVideoId.Rows.Count > 0)
                                {
                                    for (int j = 0; j <= dtVideoId.Rows.Count - 1; j++)
                                    {
                                        strVideoId += dtVideoId.Rows[j]["VideoId"].ToString() + ",";
                                    }
                                }
                                //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                            }
                            //�жϵ�½���Ƿ���Ȩ�޿���ؽ���



                            string stateName = "";

                            if (dt.Rows[i]["userName"].ToString() != "") //������ʹ����
                            {
                                switch (dt.Rows[i]["state"].ToString())
                                {
                                    case "1":
                                        stateName = "������ס��";
                                        break;
                                    case "2":
                                        stateName = "���������";
                                        break;
                                    case "3":
                                        stateName = "��ҽ��";
                                        break;
                                    case "4":
                                        stateName = "�����ҽ��";
                                        break;
                                    case "5":
                                        stateName = "�����";
                                        break;
                                    case "7":
                                        stateName = "���������";
                                        break;
                                    case "-999":
                                        stateName = "ʹ����";
                                        break;
                                    default:
                                        break;
                                }



                                if (strVideoId != "")//��½����Ȩ�޿����
                                {
                                    //returnHtml += "<div align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:120px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div><img src='../../Content/Images/Icon32/user_firefighter.png' width='18px' height='7px'  />&nbsp;&nbsp;" + dt.Rows[i]["userName"].ToString() + "��" + stateName + "</div><div>" + policeName + "</div>";
                                    returnHtml += "<div align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:5px;height:20px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>&nbsp;&nbsp;" + dt.Rows[i]["userName"].ToString() + "</div><div style=\"text-align:center;font-weight:bold\">" + stateName + "</div><div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId + "','" + dt.Rows[i]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div>";
                                }
                                else//��½��û��Ȩ�޿����
                                {
                                    returnHtml += "<div align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:5px;height:20px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>&nbsp;&nbsp;" + dt.Rows[i]["userName"].ToString() + "</div><div style=\"text-align:center;font-weight:bold\">" + stateName + "</div>";

                                }


                            }
                            else //����û��ʹ��
                            {
                                returnHtml +=
                                    "<div align='left' style=\"cursor:hand;background-color:Green;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" +
                                    dt.Rows[i]["RoomName"].ToString() +
                                    "</div><div style=\"text-align:center;margin-top:15px;font-weight:bold\">�����С���</div>";
                            }
                            returnHtml += "</div>";
                        }
                        //���ӷ���ֵ����
                        //��ȡ����
                        string policeName = "";

                        if (dt.Rows.Count > 0) //�з���ģ�������ֵ����
                        {


                            string sqlVideoId_fjzbs = "";
                            string sqlCanVideo_fjzbs = "";
                            string strVideoId_fjzbs = "";
                            //�жϵ�½���Ƿ���Ȩ�޿���ؿ�ʼ
                            sqlCanVideo_fjzbs = string.Format(@"select 1 from  base_policearea bpa
                                                                                        join 
                                                                                        (select base_unit_id from base_unit bu where bu.base_unit_id=
                                                                                        (select bpa.unit_id from base_policearea bpa where bpa.policearea_id='{0}') or 
                                                                                        bu.base_unit_id=(select bu1.parent_unit_id from base_policearea bpa join base_unit bu1 on bpa.unit_id=bu1.base_unit_id and  bpa.policearea_id='{0}') or 
                                                                                        bu.parent_unit_id='0') a
                                                                                        on bpa.unit_id=a.base_unit_id 
                                                                                        where bpa.areatype={1}  
                                                                                        and ((Charindex('{2}',DepLeader)>0) or (Charindex('{2}',UnitLeader)>0)) 
                                                                                        union 
                                                                                        select 1 from jw_onduty where state=0 and policearea_id='{0}' and dutyuser_id='{2}'"
                                                    , policeAreaId
                                                    , type
                                                    , ManageProvider.Provider.Current().UserId

                                                     );
                            DataTable dtCanVideo_fjzbs = SqlHelper.DataTable(sqlCanVideo_fjzbs, CommandType.Text);
                            DataTable dtVideoId_fjzbs = null;
                            if (dtCanVideo_fjzbs != null && dtCanVideo_fjzbs.Rows.Count > 0)
                            {
                                //�����½����Ȩ�޿����   ��÷����Ӧ�ļ���豸��ʼ
                                sqlVideoId_fjzbs = string.Format(@"
                                                                                select PlatKind+MonitorApplication_id+'!'+c.Channelname as Videoid,a.Room_id,r.RoomName from Base_MonitorApplication a
                                                                                join Base_MonitorChannels c on a.MonitorChannels_id=c.MonitorChannels_id
                                                                                join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id
                                                                                join Base_Room r on a.Room_id=r.Room_id
                                                                                where a.Room_id=(select max(room_id) from Base_Room br join Base_RoomType brt on  br.RoomType_id=brt.RoomType_id and brt.Name='ָ����������ֵ����' where br.PoliceArea_id='{0}') and a.type='��ط���'
                                                                            "
                                     , policeAreaId
                                 );
                                dtVideoId_fjzbs = SqlHelper.DataTable(sqlVideoId_fjzbs, CommandType.Text);

                                if (dtVideoId_fjzbs != null && dtVideoId_fjzbs.Rows.Count > 0)
                                {
                                    for (int j = 0; j <= dtVideoId_fjzbs.Rows.Count - 1; j++)
                                    {
                                        strVideoId_fjzbs += dtVideoId_fjzbs.Rows[j]["VideoId"].ToString() + ",";
                                    }
                                }
                                //�����½����Ȩ�޿���� ��÷����Ӧ�ļ���豸����
                            }
                            //�жϵ�½���Ƿ���Ȩ�޿���ؽ���

                            string sql1 = string.Format(@"
                                                                    select  distinct bu.RealName from JW_SendPolice js 
                                                                    join Base_User bu on bu.UserId=js.user_id 
                                                                    join JW_OnDuty jo on jo.OnDuty_id=js.Object_id 
                                                                    join Base_PoliceArea bpa on bpa.PoliceArea_id=jo.PoliceArea_id 
                                                                    where js.state in ('1','2') and js.type='3' and bpa.unit_id='{0}'
                                                                    and bpa.PoliceArea_id='{1}'
                                                                    ", unit_id,policeAreaId);



                            DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                            policeName =
                                "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr><td>";
                            for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                            {
                                policeName += dt1.Rows[j]["RealName"].ToString() + ",";
                            }
                            policeName += "</td></tr></table></div>";
                            // returnHtml +="<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;����ֵ����</div>" + policeName + "</div>";
                            if (strVideoId_fjzbs != "")//��½����Ȩ�޿����
                            {
                                returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;ָ����������ֵ����</div>" + policeName + "<div><input type='button' value='�鿴���' onclick=\"openVideo('" + strVideoId_fjzbs + "','" + dtVideoId_fjzbs.Rows[0]["room_id"].ToString() + "','" + type + "')\" style=\"width:124px;\" /></div></div></div>";
                            }
                            else
                            {
                                returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;ָ����������ֵ����</div>" + policeName + "</div></div>";
                            }
                        }
                    }
                  
                } 
                return returnHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public string GetRoomInfo(string roomId, string type)
        {
            string sqlRoomInfo = "";
            string returnHtml = "";
            try 
            {
                if (type == "1" || type == "2")//����참��,�̼�참��
                {
                    sqlRoomInfo = string.Format(@"
                    select ci.name,ja.unitname,ja.depname,ja.userName,u.RealName from  JW_Usedetail ud join jw_apply ja on ud.apply_id=ja.apply_id
                    left join Case_caseinfo  ci on ci.case_id= ja.case_id left join base_user u  on u.userid= ja.asker_id
                    where ud.room_id='{0}' and ud.isend=0
                      "
                         , roomId
                         );
                }
                else
                {
                    sqlRoomInfo = string.Format(@"
                    select ci.name,ja.unitname,ja.depname,ja.userName,u.RealName from  JW_Apply_room jam  join jw_apply ja on jam.apply_id=ja.apply_id 
                   left join Case_caseinfo  ci on ci.case_id= ja.case_id left join base_user u  on u.userid= ja.asker_id
                   where jam.room_id='{0}' and jam.state in (1,2,3,4,5)
                      "
                         , roomId
                         );
                }
                DataTable dtRoomInfo = SqlHelper.DataTable(sqlRoomInfo, CommandType.Text);
                if (dtRoomInfo != null && dtRoomInfo.Rows.Count > 0)
                {
                    returnHtml += "�������ƣ�" + dtRoomInfo.Rows[0]["name"].ToString() + "<br>";
                    returnHtml += "�참��λ��" + dtRoomInfo.Rows[0]["unitname"].ToString() + "<br>";
                    returnHtml += "�참���ţ�" + dtRoomInfo.Rows[0]["depname"].ToString() + "<br>"; ;
                    returnHtml += "�а���Ա��" + dtRoomInfo.Rows[0]["RealName"].ToString() + "<br>"; ;
                    returnHtml += "�� �� �ˣ�" + dtRoomInfo.Rows[0]["userName"].ToString();
                   
                }
                return returnHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void getchild(string myid)
        {
            string sql = string.Format("select Base_unit_id,unit,code from Base_Unit where parent_unit_id='" + myid + "' order by code");
            DataTable dt = Repository().FindTableBySql(sql.ToString());
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                unit_allchildren += "','" + dt.Rows[i]["Base_unit_id"].ToString();
                getchild(dt.Rows[i]["Base_unit_id"].ToString());
            }
        }

        private void getparent(string myid)
        {
            string sql = string.Format("select parent_unit_id,code from Base_Unit where Base_unit_id='" + myid + "' order by code");
            DataTable dt = Repository().FindTableBySql(sql.ToString());
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                unit_allchildren += "','" + dt.Rows[i]["parent_unit_id"].ToString();
                getparent(dt.Rows[i]["parent_unit_id"].ToString());
            }
        }

        public string Getjcrb(string myid)
        {
            unit_allparent = ManageProvider.Provider.Current().CompanyId;
            getparent(unit_allparent);
            string cityinfo = "";
            try
            {
                string sql = string.Format("select top 5 DailyReport_id,b.unit,r.adddate from JW_DailyReport r join Base_Unit b on b.Base_Unit_id=r.unit_id where r.unit_id in('" + unit_allparent + "') or r.unit_id ='e2c79c56-5b58-4c62-b2a9-3bb7492c' order by adddate desc");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["unit"].ToString() + "," + dt.Rows[i]["adddate"].ToString().Split(' ')[0] + "," + dt.Rows[i]["DailyReport_id"].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString() + "," + dt.Rows[i]["adddate"].ToString().Split(' ')[0] + "," + dt.Rows[i]["DailyReport_id"].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string getisleader()
        {
            string user_id = ManageProvider.Provider.Current().UserId;
            string sql;
            sql = string.Format("select AreaName from Base_PoliceArea where (Charindex('" + user_id + "',DepLeader)>0) or (Charindex('" + user_id + "',UnitLeader)>0)");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        /// <summary>
        /// ȫʡ��Ժ���ݰ���ʡԺ
        /// </summary>
        /// <returns></returns>
        public string GetAllCityUnitList()
        {
            StringBuilder builder = new StringBuilder();
            string sql = string.Format("select unit from base_unit where len(sortcode)<3");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    builder.Append(dt.Rows[i][0].ToString() + ",");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //���� �󶨵�λ
        public string GetParentCityUnitList(ref string companyid, string toLast)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0)
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    string sql = string.Format(@"select unit,parent_unit_id,case when Base_Unit_id='" + parent_company_id + @"' then 1 else 0 end as toLast from base_unit 
                                                    where Base_Unit_id in 
                                                        (
                                                                        select Base_Unit_id from Base_Unit where 
                                                                        Base_Unit_id = '" + parent_company_id + @"' or parent_unit_id in
                                                                                (select Base_Unit_id from Base_Unit where Base_Unit_id = '" + parent_company_id + @"')
                                                        ) 
                                                    order by sortcode");

                    DataTable dt = Repository().FindTableBySql(sql.ToString());
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (companyid == "e2c79c56-5b58-4c62-b2a9-3bb7492c" && dt.Rows[i]["parent_unit_id"].ToString() == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                        {
                            if (dt.Rows[i][0].ToString() == "���Ƹ���Ժ")
                                builder.Append(dt.Rows[i][0].ToString().Substring(0, 3) + ",");
                            else
                                builder.Append(dt.Rows[i][0].ToString().Substring(0, 2) + ",");
                        }
                        else
                            builder.Append(dt.Rows[i][0].ToString() + ",");
                       // builder.Append(dt.Rows[i][0].ToString() + ",");
                    }
                    builder.Remove(builder.Length - 1, 1);
                    return builder.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetParentCityData(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0)
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    #region
                    string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + parent_company_id + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid
                                        ,sum(case when Datediff(day,isnull(actdate,getdate()),isnull(enddate,getdate()))=0 
                                            then 1 
                                            else abs(Datediff(day,isnull(actdate,getdate()),isnull(enddate,getdate()))) end
                                            ) as y
                                        ,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join jw_sendpolice b on a. Base_Unit_id=b.Unit_id and type in (1,2,3,4,5,8,9) and b.state =3
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + parent_company_id + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + parent_company_id + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname
                        ");

                    DataTable dt = new DataTable();

                    dt = Repository().FindTableBySql(sql.ToString());
                    if (dt.Rows.Count > 0)
                        return dt;
                    else
                        return null;
                    #endregion
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DataTable GetParentCityDataHandleCenter(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0)
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    #region
                    string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + parent_company_id + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid
                                        ,count(apply_id) as y
                                        ,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join JW_Apply b on a. Base_Unit_id=b.Unit_id and b.state in (0,5) and year(b.fact_indate)=year(getdate())
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + parent_company_id + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + parent_company_id + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname
                        ");

                    DataTable dt = new DataTable();

                    dt = Repository().FindTableBySql(sql.ToString());
                    if (dt.Rows.Count > 0)
                        return dt;
                    else
                        return null;
                    #endregion
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetParentCityDataAct(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0)
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    #region
                    string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + parent_company_id + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid,count(SendPolice_id) as y,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join jw_sendpolice b on a. Base_Unit_id=b.Unit_id and type in (1,2,3,4,5,8,9) and and (b.state=1 or b.state =2)
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + parent_company_id + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + parent_company_id + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname
                        ");

                    DataTable dt = new DataTable();

                    dt = Repository().FindTableBySql(sql.ToString());
                    if (dt.Rows.Count > 0)
                        return dt;
                    else
                        return null;
                    #endregion
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetParentCityDataActHandleCenter(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0)
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    #region
                    string sql = string.Format(@"
                                    select a.companyid,a.y,case when a.companyid='" + parent_company_id + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid
                                        ,count(apply_id) as y
                                        ,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join JW_Apply b on a. Base_Unit_id=b.Unit_id and b.state not in (0,5) and (b.fact_indate is null  or year(b.fact_indate)=year(getdate()))
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + parent_company_id + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + parent_company_id + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname
                        ");

                    DataTable dt = new DataTable();

                    dt = Repository().FindTableBySql(sql.ToString());
                    if (dt.Rows.Count > 0)
                        return dt;
                    else
                        return null;
                    #endregion
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //���� ��ʷ����
        public string GetALLParentCityData(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0 && dt_p.Rows[0][0].ToString() != "0")
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
                    string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
                    try
                    {
                        StringBuilder cityinfo = new StringBuilder();
                        string sql2 = string.Format(@"select isnull(sum(case when Datediff(day,actdate,enddate)=0 
                                            then 1 
                                            else Datediff(day,actdate,enddate) end
                                            ),0) as y from JW_SendPolice where unit_id='" + companyid + "' and  type in (1,2,3,4,5,8,9) and state=3 and year(SendDate)=year(getdate())");
                        // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"color\":\"" + colors[0] + "\",\"tolast\":1},");

                        DataTable dt = Repository().FindTableBySql(sql.ToString());
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sql2 = string.Format(@"select  isnull(sum(case when Datediff(day,actdate,enddate)=0 
                                            then 1 
                                            else Datediff(day,actdate,enddate) end
                                            ),0) as y from JW_SendPolice where type in (1,2,3,4,5,8,9) and year(SendDate)=year(getdate()) and state=3 and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                            //sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��

                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                        }
                        cityinfo.Remove(cityinfo.Length - 1, 1);
                        cityinfo.Append("]");
                        return cityinfo.ToString();
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //��ʷ��¼�����ϼ�
        public string GetALLParentCityDataHandleCenter(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0 && dt_p.Rows[0][0].ToString() != "0")
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
                    string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
                    try
                    {
                        StringBuilder cityinfo = new StringBuilder();
                        string sql2 = string.Format(@"select count(apply_id) as y from JW_Apply where unit_id='" + companyid + "' and state in (0,5) and year(fact_indate)=year(getdate())");
                        // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"color\":\"" + colors[0] + "\",\"tolast\":1},");

                        DataTable dt = Repository().FindTableBySql(sql.ToString());
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sql2 = string.Format(@"select  count(apply_id) as y from JW_Apply where year(fact_indate)=year(getdate()) and state in (0,5) and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                            //sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��

                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                        }
                        cityinfo.Remove(cityinfo.Length - 1, 1);
                        cityinfo.Append("]");
                        return cityinfo.ToString();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
      
         //���� ʵʱ����
        public string GetALLParentCityDataAct(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0 && dt_p.Rows[0][0].ToString() != "0")
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
                    string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
                    try
                    {
                        StringBuilder cityinfo = new StringBuilder();
                        string sql2 = string.Format(@"select count(SendPolice_id) as y from JW_SendPolice where unit_id='" + companyid + "' and  type in (1,2,3,4,5,8,9) and (state=1 or state =2)");
                        // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"tolast\":1},");

                        DataTable dt = Repository().FindTableBySql(sql.ToString());
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sql2 = string.Format(@"select  count(SendPolice_id) as y from JW_SendPolice where type in (1,2,3,4,5,8,9)  and (state=1 or state =2) and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                            //sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��

                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                        }
                        cityinfo.Remove(cityinfo.Length - 1, 1);
                        cityinfo.Append("]");
                        return cityinfo.ToString();
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //ʵʱ��¼�����ϼ�
        public string GetALLParentCityDataActHandleCenter(ref string companyid, string toLast)
        {
            try
            {
                string sql_p = "";
                if (toLast == "0")
                    sql_p = "select parent_unit_id from base_unit where Base_Unit_id='" + companyid + "'";
                else
                    sql_p = "select Base_Unit_id from base_unit where Base_Unit_id='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count > 0 && dt_p.Rows[0][0].ToString() != "0")
                {
                    string parent_company_id = dt_p.Rows[0][0].ToString();
                    companyid = parent_company_id;
                    string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
                    string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
                    try
                    {
                        StringBuilder cityinfo = new StringBuilder();
                        string sql2 = string.Format(@"select count(apply_id) as y from JW_Apply where unit_id='" + companyid + "'and (fact_indate is null  or year(fact_indate)=year(getdate())) and state not in (0,5)");
                        // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"tolast\":1},");

                        DataTable dt = Repository().FindTableBySql(sql.ToString());
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sql2 = string.Format(@"select  count(apply_id) as y from JW_Apply where  state not in (0,5) and (fact_indate is null  or year(fact_indate)=year(getdate())) and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                            //sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��

                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                        }
                        cityinfo.Remove(cityinfo.Length - 1, 1);
                        cityinfo.Append("]");
                        return cityinfo.ToString();
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //��һ����ʷ����
        public string GetALLCityData(string companyid)
        {
            string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
            string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
            try
            {
                StringBuilder cityinfo = new StringBuilder();
                string sql2 = string.Format(@"select isnull(sum(case when Datediff(day,actdate,enddate)=0 
                                            then 1 else Datediff(day,actdate,enddate) end),0) as y 
                                            from JW_SendPolice where unit_id='" + companyid + "' and type in (1,2,3,4,5,8,9) and state=3 and year(SendDate)=year(getdate())");
                // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"tolast\":1},");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql2 = string.Format(@"select isnull(sum(case when Datediff(day,actdate,enddate)=0 
                                            then 1 
                                            else Datediff(day,actdate,enddate) end
                                            ),0) as y from JW_SendPolice where type in (1,2,3,4,5,8,9) and year(SendDate)=year(getdate()) and state=3 and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    //  sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                }
                cityinfo.Remove(cityinfo.Length - 1, 1);
                cityinfo.Append("]");
                return cityinfo.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        ////���е���ʷ��¼�ؼ�����Ϣ
        public string GetALLCityDataHandleCenter(string companyid)
        {
            string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
            string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
            try
            {
                StringBuilder cityinfo = new StringBuilder();
                string sql2 = string.Format(@" select count(apply_id)as y from JW_Apply where unit_id='" + companyid + "' and state in (0,5) and Year(fact_indate)=Year(getdate()) group by unit_id");
                // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"tolast\":1},");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql2 = string.Format(@"select count(apply_id)as y from JW_Apply  where year(fact_indate)=year(getdate()) and state in (0,5) and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    //  sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                }
                cityinfo.Remove(cityinfo.Length - 1, 1);
                cityinfo.Append("]");
                return cityinfo.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //��һ��ʵʱ����
        public string GetALLCityDataAct(string companyid)
        {
            string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
            string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
            try
            {
                StringBuilder cityinfo = new StringBuilder();
                string sql2 = string.Format(@"select count(SendPolice_id) as y from JW_SendPolice where unit_id='" + companyid + "' and type in (1,2,3,4,5,8,9) and (state=1 or state =2)");
                // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"tolast\":1},");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql2 = string.Format(@"select count(SendPolice_id) as y from JW_SendPolice where type in (1,2,3,4,5,8,9) and (state=1 or state =2) and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    //  sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                }
                cityinfo.Remove(cityinfo.Length - 1, 1);
                cityinfo.Append("]");
                return cityinfo.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //���е�ʵʱ��¼�ؼ�����Ϣ
        public string GetALLCityDataActHandleCenter(string companyid)
        {
            string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
            string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
            try
            {
                StringBuilder cityinfo = new StringBuilder();
                string sql2 = string.Format(@"select count(apply_id)as y from JW_Apply where unit_id='" + companyid + "' and state not in (0,5) and (fact_indate is null  or year(fact_indate)=year(getdate())) group by unit_id");
                // string sql2 = string.Format("select count(*) from JW_policeapply where unit_id='" + companyid + "' "); //������ƥ��
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"tolast\":1},");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql2 = string.Format(@"select count(apply_id)as y from JW_Apply  where (fact_indate is null  or year(fact_indate)=year(getdate())) and state not in (0,5) and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    //  sql2 = string.Format("select count(*) from JW_policeapply where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')"); //������ƥ��
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"tolast\":0},");
                }
                cityinfo.Remove(cityinfo.Length - 1, 1);
                cityinfo.Append("]");
                return cityinfo.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        //
        public DataTable GetCityData(string companyid, string toLast)
        {
            //'colors['++']' 
            string sql_last = @"
    if object_id(N'tempdb..#b',N'U') is not null
	    drop table #b
 
select bdl.code,bdl.FullName,bdl.Remark into #b from dbo.Base_DataDictionary bd left join 
	dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='tasktype'

select  '" + companyid + @"'as companyid,aaa.code,aaa.remark,aaa.counts as y from 
                                        (
                                         select aa.code,aa.remark,isnull(bb.counts,0)as counts,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY aa.code)% 9)as sort from 
                                            #b aa left join 
	                                        (
                                            select code,max(remark)as remark,sum(counts)as counts from 
	                                           (
		                                                select code,max(remark)as remark,sum(counts)as counts from 
                                                            (
						                                                select a.code,max(a.remark)as remark,isnull(sum(case when Datediff(day,c.actdate,c.enddate)=0 
                                                                        then 1 
                                                                        else Datediff(day,c.actdate,c.enddate) end
                                                                        ),0) as counts  from #b a 
						                                                right join JW_PoliceApply b on b.tasktype_id = a.code
						                                                right join JW_SendPolice c on c.Object_id=b.apply_id  
							                                                where b.unit_id ='" + companyid + @"' and c.type in (4,5) and c.state =3 and year(c.SendDate)=year(getdate())
						                                                group by a.code
						                                       
			                                                 ) tmp1 group by code 
                                                        union all
                                                            select code,max(remark)as remark,sum(counts)as counts from 
                                                            (
						                                           select a.code,max(a.remark)as remark,isnull(sum(case when Datediff(day,c.actdate,c.enddate)=0 
                                                                    then 1 
                                                                    else Datediff(day,c.actdate,c.enddate) end
                                                                    ),0)as counts  from #b a 
						                                                right join jw_onduty b on b.tasktype_id = a.code
						                                                right join JW_SendPolice c on c.Object_id=b.OnDuty_id  
							                                                where b.unit_id ='" + companyid + @"' and c.type in (1,2,3,8,9) and c.state =3 and year(c.SendDate)=year(getdate())
						                                                group by a.code
						                                       
			                                                ) tmp2 group by code 
                                                ) tt group by code

	                                        )bb on aa.code=bb.code where aa.code>0
                                        )aaa 
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )bbb
                                   on aaa.sort = bbb.code   order by convert(int, aaa.code)";

            string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + companyid + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid
                                            ,isnull(sum(case when Datediff(day,actdate,enddate)=0 
                                            then 1 
                                            else Datediff(day,actdate,enddate) end
                                            ),0) as y
                                        ,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join jw_sendpolice b on a. Base_Unit_id=b.Unit_id and b.type IN (1,2,3,4,5,8,9) and b.state = 3
 and year(b.SendDate)=year(getdate())	                                    
where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + companyid + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname order by convert(int, b.code)
                        ");
            try
            {
                DataTable dt = new DataTable();
                if (toLast == "1")
                    dt = Repository().FindTableBySql(sql_last.ToString());
                else
                {
                    string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";
                    DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                    if (dt_p.Rows.Count > 0)
                        dt = Repository().FindTableBySql(sql.ToString());
                    else
                        dt = Repository().FindTableBySql(sql_last.ToString());
                }

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //ʵʱ��¼����Ժ��Ϣ
        public DataTable GetCityDataHandleCenter(string companyid, string toLast)
        {
            string sql_last = @"
    if object_id(N'tempdb..#b',N'U') is not null
	    drop table #b
    if object_id(N'tempdb..#a',N'U') is not null
	    drop table #a
select Dep_id as code,name as remark,SortCode,type into #b from Base_Department where unit_id= '" + companyid + @"'
select type into #a from Base_Department where unit_id= '" + companyid + @"' group by type

select   '" + companyid + @"' as companyid,max(aaa.code)as code,ccc.type as remark,sum(aaa.counts) as y from 
#a ccc left join 
                                        (
                                         select aa.type,aa.SortCode,aa.code,aa.remark,isnull(bb.counts,0)as counts,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY aa.code)% 9)as sort from 
                                            #b aa left join 
	                                        (
                                               select code,max(remark)as remark,sum(counts)as counts from 
                                                                (
						                                                    select a.code,max(a.remark)as remark,COUNT(apply_id) as counts  from #b a 
						                                                    right join JW_Apply b on b.dep_id = a.code
							                                                 where b.unit_id = '" + companyid + @"' and b.state  in (0,5) and year(b.fact_indate)=year(getdate())
						                                                    group by a.code
						                                       
			                                                     ) tmp1 group by code 
	                                        )bb on aa.code=bb.code 
                                        )aaa on aaa.type =ccc.type
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    ) bbb
                                   on aaa.sort = bbb.code group by ccc.type 
                                   order by max(aaa.SortCode)";

            string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + companyid + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid,count(apply_id) as y,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join JW_Apply b on a. Base_Unit_id=b.Unit_id and b.state  in (0,5) and year(b.fact_indate)=year(getdate())
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + companyid + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname order by convert(int, b.code)
                        ");
            //    union all
            //select a.code,max(a.remark)as remark,count(SendPolice_id)as counts from #b a 
            //right join JW_OnDuty b on b.tasktype_id = a.code
            //right join JW_SendPolice d on d.Object_id=b.OnDuty_id 
            //where d.unit_id ='" + companyid + @"' and (d.type=0 or d.type=1 or d.type=2 or d.type=3)
            //group by a.code

            //or c.type=5
            try
            {
                DataTable dt = new DataTable();
                if (toLast == "1")
                    dt = Repository().FindTableBySql(sql_last.ToString());
                else
                {
                    string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";
                    DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                    if (dt_p.Rows.Count > 0)
                        dt = Repository().FindTableBySql(sql.ToString());
                    else
                        dt = Repository().FindTableBySql(sql_last.ToString());
                }

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataTable GetCityDataAct(string companyid, string toLast)
        {
            //'colors['++']' 
            string sql_last = @"
    if object_id(N'tempdb..#b',N'U') is not null
	    drop table #b
    
select bdl.code,bdl.FullName,bdl.Remark into #b from dbo.Base_DataDictionary bd left join 
	dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='tasktype'

select  '" + companyid + @"'as companyid,aaa.code,aaa.remark,aaa.counts as y from 
                                        (
                                         select aa.code,aa.remark,isnull(bb.counts,0)as counts,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY aa.code)% 9)as sort from 
                                            #b aa left join 
	                                        (
                                               select code,max(remark)as remark,sum(counts)as counts from 
	                                           (
		                                            select code,max(remark)as remark,sum(counts)as counts from 
                                                        (
                                                                        select a.code,max(a.remark)as remark,count(SendPolice_id) as counts  from #b a 
						                                                            right join JW_PoliceApply b on b.tasktype_id = a.code
						                                                            right join JW_SendPolice c on c.Object_id=b.apply_id  
							                                                            where b.unit_id ='" + companyid + @"' and (c.type=4 or c.type=5 ) and (c.state=1 or c.state =2)
						                                                            group by a.code
						                                       
			                                            ) tmp1 group by code 
                                                    union all
                                                    select code,max(remark)as remark,sum(counts)as counts from
                                                        (
						                                            select a.code,max(a.remark)as remark,count(SendPolice_id) as counts  from #b a 
						                                            right join jw_onduty b on b.tasktype_id = a.code
						                                            right join JW_SendPolice c on c.Object_id=b.OnDuty_id  
							                                            where b.unit_id ='" + companyid + @"' and c.type in (1,2,3,8,9) and (c.state=1 or c.state =2)
						                                            group by a.code
						                                       
			                                            ) tmp2 group by code 
                                                ) tt group by code
	                                        )bb on aa.code=bb.code where aa.code>0
                                        )aaa 
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )bbb
                                   on aaa.sort = bbb.fullname   order by convert(int, aaa.code)";

            string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + companyid + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid,count(SendPolice_id) as y,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join jw_sendpolice b on a. Base_Unit_id=b.Unit_id and type IN (1,2,3,4,5,8,9) and (b.state=1 or b.state =2)
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + companyid + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname order by convert(int, b.code)
                        ");
            //    union all
            //select a.code,max(a.remark)as remark,count(SendPolice_id)as counts from #b a 
            //right join JW_OnDuty b on b.tasktype_id = a.code
            //right join JW_SendPolice d on d.Object_id=b.OnDuty_id 
            //where d.unit_id ='" + companyid + @"' and (d.type=0 or d.type=1 or d.type=2 or d.type=3)
            //group by a.code

            //or c.type=5
            try
            {
                DataTable dt = new DataTable();
                if (toLast == "1")
                    dt = Repository().FindTableBySql(sql_last.ToString());
                else
                {
                    string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";
                    DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                    if (dt_p.Rows.Count > 0)
                        dt = Repository().FindTableBySql(sql.ToString());
                    else
                        dt = Repository().FindTableBySql(sql_last.ToString());
                }

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //��ʷ��¼����Ժ��Ϣ
        public DataTable GetCityDataActHandleCenter(string companyid, string toLast)
        {
            string sql_last = @"
    if object_id(N'tempdb..#b',N'U') is not null
	    drop table #b
    if object_id(N'tempdb..#a',N'U') is not null
	    drop table #a
select Dep_id as code,name as remark,SortCode,type into #b from Base_Department where unit_id= '" + companyid + @"'
select type into #a from Base_Department where unit_id= '" + companyid + @"' group by type

select   '" + companyid + @"' as companyid,max(aaa.code)as code,ccc.type as remark,sum(aaa.counts) as y from 
#a ccc left join 
                                        (
                                         select aa.type,aa.SortCode,aa.code,aa.remark,isnull(bb.counts,0)as counts,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY aa.code)% 9)as sort from 
                                            #b aa left join 
	                                        (
                                               select code,max(remark)as remark,sum(counts)as counts from 
                                                                (
						                                                    select a.code,max(a.remark)as remark,COUNT(apply_id) as counts  from #b a 
						                                                    right join JW_Apply b on b.dep_id = a.code
							                                                 where b.unit_id = '" + companyid + @"' and b.state not in (0,5) and (b.fact_indate is null  or year(b.fact_indate)=year(getdate()))
						                                                    group by a.code
						                                       
			                                                     ) tmp1 group by code 
	                                        )bb on aa.code=bb.code 
                                        )aaa on aaa.type =ccc.type
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    ) bbb
                                   on aaa.sort = bbb.code group by ccc.type 
                                   order by max(aaa.SortCode)";

            string sql = string.Format(@"
                                   select a.companyid,a.y,case when a.companyid='" + companyid + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid,count(apply_id) as y,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join JW_Apply b on a. Base_Unit_id=b.Unit_id and b.state not in (0,5) and (b.fact_indate is null  or year(b.fact_indate)=year(getdate()))
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + companyid + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='��ɫ'
                                    )b 
                                    on a.sort = b.fullname order by convert(int, b.code)
                        ");
            //    union all
            //select a.code,max(a.remark)as remark,count(SendPolice_id)as counts from #b a 
            //right join JW_OnDuty b on b.tasktype_id = a.code
            //right join JW_SendPolice d on d.Object_id=b.OnDuty_id 
            //where d.unit_id ='" + companyid + @"' and (d.type=0 or d.type=1 or d.type=2 or d.type=3)
            //group by a.code

            //or c.type=5
            try
            {
                DataTable dt = new DataTable();
                if (toLast == "1")
                    dt = Repository().FindTableBySql(sql_last.ToString());
                else
                {
                    string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";
                    DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                    if (dt_p.Rows.Count > 0)
                        dt = Repository().FindTableBySql(sql.ToString());
                    else
                        dt = Repository().FindTableBySql(sql_last.ToString());
                }

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetAllTasktype()
        {
            StringBuilder builder = new StringBuilder();
            string sql = string.Format(@"select bdl.Remark from dbo.Base_DataDictionary bd left join 
	                dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='tasktype' and bdl.code>'0'   order by CONVERT(int, bdl.code)");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    builder.Append(dt.Rows[i][0].ToString() + ",");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetAllDepKind(string companyid)
        {
            StringBuilder builder = new StringBuilder();
            string sql = string.Format(@"select type as Remark from Base_Department where unit_id= '" + companyid + @"' group by type order by max(SortCode)");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    builder.Append(dt.Rows[i][0].ToString() + ",");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        

        //��ҳ�������״ͼ�����ݵ�¼�ߵ�λID����
        public string GetCityUnitList(string companyid)
        {

            //companyid = "e2c79c56-5b58-4c62-b2a9-3bb7492c";
            try
            {
                string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count == 0)
                {
                    return GetAllTasktype();
                }
                StringBuilder builder = new StringBuilder();
                string sql = string.Format(@"select unit,parent_unit_id,case when Base_Unit_id='" + companyid + @"' then 1 else 0 end as toLast from base_unit 
                                                where Base_Unit_id in 
                                                    (
                                                                    select Base_Unit_id from Base_Unit where 
                                                                    Base_Unit_id = '" + companyid + @"' or parent_unit_id in
                                                                            (select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')
                                                    ) 
                                                order by sortcode");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (companyid == "e2c79c56-5b58-4c62-b2a9-3bb7492c" && dt.Rows[i]["parent_unit_id"].ToString() == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                    {
                        if (dt.Rows[i][0].ToString() == "���Ƹ���Ժ")
                            builder.Append(dt.Rows[i][0].ToString().Substring(0, 3) + ",");
                        else
                            builder.Append(dt.Rows[i][0].ToString().Substring(0, 2) + ",");
                    }
                    else
                        builder.Append(dt.Rows[i][0].ToString()+ ",");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetCityUnitListHandleCenter(string companyid)
        {

            //companyid = "e2c79c56-5b58-4c62-b2a9-3bb7492c";
            try
            {
                string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count == 0)
                {
                    return this.GetAllDepKind(companyid);
                }
                StringBuilder builder = new StringBuilder();
                string sql = string.Format(@"select unit,parent_unit_id,case when Base_Unit_id='" + companyid + @"' then 1 else 0 end as toLast from base_unit 
                                                where Base_Unit_id in 
                                                    (
                                                                    select Base_Unit_id from Base_Unit where 
                                                                    Base_Unit_id = '" + companyid + @"' or parent_unit_id in
                                                                            (select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')
                                                    ) 
                                                order by sortcode");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (companyid == "e2c79c56-5b58-4c62-b2a9-3bb7492c" && dt.Rows[i]["parent_unit_id"].ToString() == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                    {
                        if (dt.Rows[i][0].ToString() == "���Ƹ���Ժ")
                            builder.Append(dt.Rows[i][0].ToString().Substring(0, 3) + ",");
                        else
                            builder.Append(dt.Rows[i][0].ToString().Substring(0, 2) + ",");
                    }
                    else
                        builder.Append(dt.Rows[i][0].ToString() + ",");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string SubmitFormOnDuty(string dutyType, string fjUsers_ids, JW_OnDuty jwOnDuty, string CurrentOnDuty_id)
        {
            if (dutyType == "3")
            {
                //������ǽ���ִ��//  update JW_SendPolice set state='3' where Object_id='{0}'; 
                string sqlEnd = string.Format(@"
                                    update JW_OnDuty set state='99' where OnDuty_id='{0}';
                                    update JW_SendPolice set state='3' where Object_id='{0}'; 
                                    ", CurrentOnDuty_id);
                try
                {
                    SqlHelper.ExecuteNonQuery(sqlEnd, CommandType.Text);
                    return "1|1|����ִ�ڳɹ�";
                }
                catch (Exception exception)
                {
                    return "0|0|�����쳣��";
                }
            }




            //1.�ȴ���jw_onduty 
            string onDuty_id = Guid.NewGuid().ToString();
            string sqlJWOnDutyInsert = string.Empty;
            //string sqlJWOnDutyUpdateState = string.Empty;
            string sqlUpdateDutySendPoliceState = string.Empty;
            if (dutyType == "1" || dutyType == "2")
            {
                //sqlJWOnDutyUpdateState = string.Format(@" update JW_OnDuty set state='1' where PoliceArea_id='{0}'  ", jwOnDuty.PoliceArea_id);
                //try
                //{
                //    SqlHelper.ExecuteNonQuery(sqlJWOnDutyUpdateState, CommandType.Text);
                //}
                //catch (Exception exception)
                //{
                //    return "0|0|�����쳣��";
                //}
                if (dutyType == "2")
                {
                    sqlUpdateDutySendPoliceState = string.Format(@"
                                    update JW_OnDuty set state='99' where OnDuty_id='{0}';
                                    update JW_SendPolice set state='3' where Object_id='{0}';
                                    ", CurrentOnDuty_id);
                }
                else
                {
                    sqlUpdateDutySendPoliceState = string.Format(@"
                                    update JW_OnDuty set state='1' where OnDuty_id='{0}';
                                    update JW_SendPolice set state='3' where Object_id='{0}';
                                    ", CurrentOnDuty_id);
                }
                try
                {
                    SqlHelper.ExecuteNonQuery(sqlUpdateDutySendPoliceState, CommandType.Text);
                }
                catch (Exception exception)
                {
                    return "0|0|�����쳣��";
                }

                //��ʼִ��
                sqlJWOnDutyInsert = string.Format(@"insert into JW_OnDuty(
                                                                OnDuty_id,unit_id,PoliceArea_id,adduser_id,adddate,tasktype_id,DutyUser_id,SendDate,OldDutyUser_id,type,detail,state,Object_id
                                                  ) values(
                                        @OnDuty_id,@unit_id,@PoliceArea_id,@adduser_id,getdate(),@tasktype_id,@DutyUser_id,@SendDate,@OldDutyUser_id,@type,@detail,@state,@Object_id
                                                                )");
                SqlParameter[] parsJWOnDutyInsert = new SqlParameter[]
                {
                    new SqlParameter("@OnDuty_id",onDuty_id), 
                    new SqlParameter("@unit_id",jwOnDuty.unit_id), 
                    new SqlParameter("@PoliceArea_id",jwOnDuty.PoliceArea_id), 
                    new SqlParameter("@adduser_id",jwOnDuty.adduser_id), 
                    new SqlParameter("@tasktype_id",jwOnDuty.tasktype_id), 
                    new SqlParameter("@DutyUser_id",jwOnDuty.DutyUser_id), 
                    new SqlParameter("@SendDate",jwOnDuty.SendDate), 
                    new SqlParameter("@OldDutyUser_id",jwOnDuty.OldDutyUser_id), 
                    new SqlParameter("@type",jwOnDuty.type), 
                    new SqlParameter("@detail",jwOnDuty.detail),
                    new SqlParameter("@state",jwOnDuty.state),
                    new SqlParameter("@Object_id",jwOnDuty.Object_id)
                };
                try
                {
                    SqlHelper.ExecuteNonQuery(sqlJWOnDutyInsert, CommandType.Text, parsJWOnDutyInsert);
                }
                catch (Exception exception)
                {
                    return "1|0|ֵ��Ǽǳɹ����ɾ�ʧ�ܣ�";
                }

                if (dutyType == "2")
                {
                    //���������ǽ��Ӱ࣬��Ҫ��֮ǰ���ɾ�״̬��Ϊ3��ʵʩ���


                }


            }
            //else if (dutyType == "2")
            //{
            //    //���Ӱ�
            //}
            //else if (dutyType == "3")
            //{
            //    //����ִ��
            //}


            //jw_onduty������ݲ�������֮�󣬿�ʼ����jw_sendpolice
            string sqlGetUnit = string.Format(@" select * from Base_Unit where Base_Unit_id='{0}' ", jwOnDuty.unit_id);
            string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

            //�õ����ݿ��е�ǰ��λ��������ˮ��
            string code = string.Empty;
            string sqlGetCode =
                string.Format(
                    @" select MAX(CONVERT(int,SUBSTRING(SendCode,6,7))) code from JW_SendPolice where Unit_id='{0}' ",
                    jwOnDuty.unit_id);
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

            StringBuilder sb = new StringBuilder();

            if (fjUsers_ids == "&nbsp;" || string.IsNullOrEmpty(fjUsers_ids))
            {
                //û��ѡ����������
                fjUsers_ids = jwOnDuty.DutyUser_id;
            }
            else
            {
                //ѡ������������
                fjUsers_ids = jwOnDuty.DutyUser_id + "," + fjUsers_ids;
            }


            //�õ���֮��������µ�����
            if (!fjUsers_ids.Contains(","))
            {
                //ֻѡ����һ����
                sb.AppendFormat(
                    @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}',getdate(),'{3}','{4}','{5}','{6}'); "
                    , jwOnDuty.unit_id
                    , unitShotName + "�쾯�ɡ�" + code + "����"
                    , jwOnDuty.DutyUser_id
                    , jwOnDuty.type
                    , onDuty_id
                    , fjUsers_ids
                    , "1"
                    );
            }
            else
            {
                //ѡ���˶����
                string[] ids = fjUsers_ids.Split(',');

                for (int i = 0; i < ids.Length; i++)
                {
                    sb.AppendFormat(
                      @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}',getdate(),'{3}','{4}','{5}','{6}'); "
                      , jwOnDuty.unit_id
                      , unitShotName + "�쾯�ɡ�" + (Convert.ToInt32(code) + i) + "����"
                      , jwOnDuty.DutyUser_id
                      , jwOnDuty.type
                      , onDuty_id
                      , ids[i]
                      , "1"
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
                SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
                return "1|1|ֵ��Ǽǳɹ����ɾ��ɹ�";
            }
            catch (Exception)
            {
                return "1|0|ֵ��Ǽǳɹ����ɾ�ʧ��";
            }


            return "";
        }

        public string GetOnDutyUsersMsg(string unit_id, string policeArea_id, string policeAreaType)
        {
            string sqlLeaderUser =
                string.Format(
                    @" select top 1 *,bu.RealName from JW_OnDuty jod  join Base_User bu on jod.DutyUser_id=bu.UserId  where jod.unit_id='{0}' and jod.PoliceArea_id='{1}' and jod.type ='{2}'  and jod.state = 0 order by jod.SendDate desc ",
                    unit_id, policeArea_id, policeAreaType);

            DataTable dtLeaderUser = SqlHelper.DataTable(sqlLeaderUser, CommandType.Text);
            if (dtLeaderUser.Rows.Count <= 0)
            {
                return "|";
            }
            else
            {
                string sqlFjUser = string.Format(@"  select *,bu.RealName from JW_SendPolice jsp join Base_User bu on jsp.user_id=bu.UserId  where jsp.Object_id='{0}' and jsp.user_id<>'{1}' and jsp.state in ('1','2')", dtLeaderUser.Rows[0]["OnDuty_id"], dtLeaderUser.Rows[0]["DutyUser_id"]);

                DataTable dtFjUser = SqlHelper.DataTable(sqlFjUser, CommandType.Text);
                if (dtFjUser.Rows.Count <= 0)
                {
                    return dtLeaderUser.Rows[0]["RealName"] + "|";
                }
                else if (dtFjUser.Rows.Count == 1)
                {
                    return dtLeaderUser.Rows[0]["RealName"] + "|" + dtFjUser.Rows[0]["RealName"];
                }
                else
                {
                    string fjUser = dtFjUser.Rows[0]["RealName"].ToString();
                    for (int i = 1; i < dtFjUser.Rows.Count; i++)
                    {
                        fjUser += "," + dtFjUser.Rows[i]["RealName"];
                    }

                    return dtLeaderUser.Rows[0]["RealName"] + "|" + fjUser;
                }
            }
        }

        //״̬:success,error|��ʾ���|��һ����Ҫֵ����˵����Ű���е�����|��ǰ����ֵ��������ֵ����е�����
        public string CheckOnDuty(string unit_id, string user_id, string policeArea_id, string policeAreaType,
            string dutyType)
        {
            //�жϵ�ǰ������참����û������ֵ��
            string sqlCheckPoliceDuty = string.Format(@"select * from JW_OnDuty where PoliceArea_id='{0}' and state=0", policeArea_id);
            DataTable dtCheckPoliceDuty;
            try
            {
                dtCheckPoliceDuty = SqlHelper.DataTable(sqlCheckPoliceDuty, CommandType.Text);
            }
            catch (Exception)
            {
                return "error|�����쳣||";
            }

            if (dtCheckPoliceDuty.Rows.Count > 0)
            {
                //����ֵ��
                if (dtCheckPoliceDuty.Rows[0]["DutyUser_id"].ToString() == user_id)
                {
                    //��ǰ���ֵ��������Լ�
                    if (dutyType == "2" || dutyType == "3")
                    {
                        //�жϽ��Ӱ�ͽ���ִ�ڰ�ť��Ȩ��
                        //�ж��Լ�֮��û���˱��Ű���
                        string sqlGetScheduleId = string.Format(@"select top 1 * from JW_Schedule 
                                where startdate>(select startdate from JW_Schedule where Schedule_id='{0}')
                                and PoliceArea_id='{1}' 
                                order by startdate ", dtCheckPoliceDuty.Rows[0]["Object_id"].ToString(), policeArea_id);

                        DataTable dtScheduleId;
                        try
                        {
                            dtScheduleId = SqlHelper.DataTable(sqlGetScheduleId, CommandType.Text);
                        }
                        catch (Exception)
                        {
                            return "error|�����쳣||";
                        }

                        if (dtScheduleId.Rows.Count > 0)
                        {
                            //���������Ǻ�������ť�Ļ������Բ���
                            //�Լ�֮�󣬻�����������ֵ�࣬��ô�Լ����Խ��Ӱ�������°�
                            return "success||" + dtScheduleId.Rows[0]["Schedule_id"].ToString() + "|" + dtCheckPoliceDuty.Rows[0]["OnDuty_id"];
                        }
                        else
                        {
                            //���������Ǻ�������ť�Ļ������Բ���
                            //�Լ�֮��û������ֵ���ˣ���ô�Լ���ֻ�ܽ����°���
                            if (dutyType == "2")
                            {
                                //���ܵ�����Ӱ�
                                return "error|�������һ���Ű���||";
                            }
                            else
                            {
                                return "success|||" + dtCheckPoliceDuty.Rows[0]["OnDuty_id"];
                            }
                        }
                    }
                    else
                    {
                        //�жϿ�ʼִ�ڰ�ť��Ȩ��
                        //���������ǵ�һ����ť�Ļ������ܽ��в���
                        return "error|���Ѿ���ֵ����||";
                    }
                }
                else
                {
                    //��ǰ���ֵ����˲����Լ�
                    string dutyMsg = string.Empty;
                    if (dutyType == "1")
                    {
                        dutyMsg = "��ʼִ��";
                    }
                    else if (dutyType == "2")
                    {
                        dutyMsg = "���Ӱ�";
                    }
                    else
                    {
                        dutyMsg = "����ִ��";
                    }
                    //��ǰ���ֵ����˲����Լ�
                    return "error|�ð참���Ѿ�������ֵ���У������ܽ���" + dutyMsg + "||";
                }
            }
            else
            {
                //û����ֵ��
                //�жϵ�ǰ������참���ڴ�ʱ��û�б������Ű�
                string sqlCheckDutyThisTime = String.Format(@" select top 1 * from JW_Schedule where startdate<= GETDATE() and enddate>= GETDATE() and PoliceArea_id='{0}' order by startdate ", policeArea_id);
                DataTable dtCheckDutyThisTime;
                try
                {
                    dtCheckDutyThisTime = SqlHelper.DataTable(sqlCheckDutyThisTime, CommandType.Text);
                }
                catch (Exception)
                {
                    return "error|�����쳣||";
                }

                if (dtCheckDutyThisTime.Rows.Count > 0)
                {
                    //���Ű�
                    //���������Ű�����ǲ�����
                    if (dtCheckDutyThisTime.Rows[0]["DutyUser_id"].ToString() == user_id)
                    {
                        //��ǰ��������Ű�������ң���ô��ֻ�ܵ����һ������ʼִ��
                        if (dutyType == "1")
                        {
                            return "success||" + dtCheckDutyThisTime.Rows[0]["Schedule_id"].ToString() + "|";
                        }
                        else
                        {
                            return "error|����û�п�ʼִ��||";
                        }
                    }
                    else
                    {
                        //��ǰ��������Ű���˲�����
                        return "error|����û�п�ʼִ��||";
                    }
                }
                else
                {
                    //û���Ű�
                    //���ҵ��뵱ǰʱ��������Ǹ���ʼ�ϰ��ʱ����ˣ�
                    string sqlCheckNearestTime = string.Format(@" select top 1 * from JW_Schedule 
                                                                     where 
                                                                     PoliceArea_id='{0}' 
                                                                     and startdate>=GETDATE() 
                                                                     order by startdate ", policeArea_id);
                    DataTable dtCheckNearestTime;
                    try
                    {
                        dtCheckNearestTime = SqlHelper.DataTable(sqlCheckNearestTime, CommandType.Text);
                    }
                    catch (Exception)
                    {
                        return "error|�����쳣||";
                    }

                    if (dtCheckNearestTime.Rows.Count > 0)
                    {
                        //������뵱ǰʱ��������ϰ����
                        if (dtCheckNearestTime.Rows[0]["DutyUser_id"].ToString() == user_id)
                        {
                            //���������
                            if (dutyType == "1")
                            {
                                return "success||" + dtCheckNearestTime.Rows[0]["Schedule_id"].ToString() + "|";
                            }
                            else
                            {
                                return "error|����û�п�ʼִ��||";
                            }
                        }
                        else
                        {
                            return "error|����û�п�ʼִ��||";
                        }
                    }
                    else
                    {
                        //û���뵱ǰʱ��������ϰ����
                        return "error|��û��Ȩ�޲���||";
                    }
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="unit_id">��ǰ�û����ڵ�λ������</param>
        /// <param name="user_id">��ǰ�û���������ֵ�ฺ����</param>
        /// <param name="policeArea_id">��ǰ������İ참��������</param>
        /// <param name="policeAreaType">��ǰ������İ참��������</param>
        /// <param name="dutyType">��ǰ������İ�ť������</param>
        /// <param name="schedule_id">��һ����Ҫֵ��������Ű���е�����</param>
        /// <param name="onDuty_id">��ǰ����ֵ�������ֵ����е�����</param>
        /// <returns></returns>
        public DataTable LoadOnDutyData(string unit_id, string user_id, string policeArea_id, string policeAreaType,
            string dutyType, string schedule_id, string onDuty_id)
        {
            if (dutyType == "1")
            {
                //���������ǿ�ʼִ��
                string sql1 = string.Format(@"select jws.*,bu.RealName from JW_Schedule  jws
                                            join Base_User bu on jws.DutyUser_id=bu.UserId
                                            where Schedule_id='{0}'", schedule_id);
                try
                {
                    DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                    return dt1;
                }
                catch (Exception)
                {
                    return new DataTable();
                }

            }
            else if (dutyType == "2")
            {
                //���������ǽ��Ӱ�
                string sql2 = string.Format(@"
        select jwo.DutyUser_id jiaobuser_id,bu.RealName jiaobuser,'' jiebuser_id,'' jiebuser,'' qtuser_id,'' qtuser,jwo.detail from JW_OnDuty jwo 
        join Base_User bu on jwo.DutyUser_id=bu.UserId 
        where PoliceArea_id='{0}' 
        and state='0'
        union all 
        select * from (
        select top 1 '' jiaobuser_id,'' jiaobuser,jws.DutyUser_id jiebuser_id,bu.RealName jiebuser,jws.user_id qtuser_id,jws.user_name qtuser,'' detail from JW_Schedule jws 
        join Base_User bu on jws.DutyUser_id=bu.UserId
        where 
        startdate >(select top 1 startdate from JW_Schedule jws join JW_OnDuty jwo on jws.Schedule_id=jwo.Object_id where jws.PoliceArea_id='{0}' and state='0') 
        and jws.PoliceArea_id='{0}'
        order by jws.startdate 
        ) as T
                                           ", policeArea_id);
                try
                {
                    DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);
                    return dt2;
                }
                catch (Exception)
                {
                    return new DataTable();
                }
            }


            else if (dutyType == "3")
            {
                string sql3 = string.Format(@"
        select jwo.DutyUser_id,bu.RealName,jwo.detail  from JW_OnDuty jwo 
        join JW_Schedule jws on jwo.Object_id=jws.Schedule_id 
        join Base_User bu on jwo.DutyUser_id=bu.UserId 
        where 
        jwo.PoliceArea_id='{0}' 
        and state=0
                                            ", policeArea_id);
                try
                {
                    DataTable dt3 = SqlHelper.DataTable(sql3, CommandType.Text);
                    return dt3;
                }
                catch (Exception)
                {
                    return new DataTable();
                }
            }

            return null;
        }

        #region lwl Ѳ������ͳ��
        /// <summary>
        /// ��ȡ�ϼ����
        /// </summary>
        /// <param name="unitid"></param>
        /// <returns></returns>
        public string GetParentUnitId(string unitid)
        {
            string parent_unit_id = "";
            DataTable dt = new DataTable();
            StringBuilder strsql = new StringBuilder();
            strsql.Append(@"select * from base_unit where base_unit_id='" + unitid + "'");
            dt = SqlHelper.DataTable(strsql.ToString(), CommandType.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                parent_unit_id = dt.Rows[0]["parent_unit_id"].ToString();
            }
            return parent_unit_id;
        }
        /// <summary>
        /// ����λͳ��Ѳ������
        /// </summary>
        /// <param name="parent_unit_id"></param>
        /// <returns></returns>
        public chart GetXunChaWenTiByUnit(string parent_unit_id)
        {
            chart ch = new chart();
            //��ȡ��λ��Ϣ
            DataTable dt_units = GetUnits(parent_unit_id);
            ArrayList categories = new ArrayList();
            List<serie> data = new List<serie>();
            //tolast:0���ã�tolast��1�����ã�ʡԺ��
            foreach (DataRow r in dt_units.Rows)
            {
                string unitName = r["unit"].ToString();
                string unitId = r["base_unit_id"].ToString();
                serie obj = new serie();
                obj.companyid = unitId;
                obj.ParentCompanyId = parent_unit_id;
                if (unitId == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                {
                    obj.tolast = 1;

                }
                else
                {
                    if (HaveSubUnits(unitId))
                    {
                        obj.tolast = 0;
                    }
                    else
                    {
                        obj.tolast = 1;
                    }

                }
                /*��������ͳ������*/
                if (parent_unit_id == "e2c79c56-5b58-4c62-b2a9-3bb7492c" && unitId != "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                {
                    obj.y = GetXunChaWenTiNum(unitId, 1);
                }
                else
                /*��������ͳ������*/
                {
                    obj.y = GetXunChaWenTiNum(unitId, 0);
                }
                categories.Add(unitName);
                data.Add(obj);
            }
            ch.categories = categories;
            ch.data = data;
            return ch;
        }
        public chart GetXunChaWenTiByType(string unit_id)
        {
            chart ch = new chart();
            //��ȡ��λ��Ϣ
            DataTable dt_xunchaType = GetXunChaType();
            ArrayList categories = new ArrayList();
            List<serie> data = new List<serie>();
            foreach (DataRow r in dt_xunchaType.Rows)
            {
                string name = r["name"].ToString();
                string place = r["place"].ToString();
                string xc_content_id = r["xc_content_id"].ToString();
                serie obj = new serie();
                //����Ƿ��Ǻ����¼���λ
                if (HaveSubUnits(unit_id))
                {
                    obj.ParentCompanyId = unit_id;
                }
                else
                {
                    obj.ParentCompanyId = GetParentUnitId(unit_id);
                }

                obj.xc_content_id = xc_content_id;
                obj.xc_TypeName = name + "[" + place + "]";
                int num = GetXunChaWenTiNumByWenTiType(unit_id, xc_content_id);
                obj.y = num;
                data.Add(obj);
                categories.Add(name + "[" + place + "]");
            }
            ch.categories = categories;
            ch.data = data;
            return ch;
        }
        private bool HaveSubUnits(string unitId)
        {
            DataTable dt = new DataTable();
            StringBuilder strsql = new StringBuilder();
            strsql.Append(@"select * from base_unit
            where parent_unit_id='" + unitId + "'");
            dt = SqlHelper.DataTable(strsql.ToString(), CommandType.Text);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            /*select * from base_unit
    where parent_unit_id='648f2750-505e-4a34-9b71-f4ea5861'*/
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent_unit_id"></param>
        /// <returns></returns>
        public chart GetXunChaWenForChart(string parent_unit_id, int tolast)
        {
            chart ch = new chart();
            if (tolast == 0)
            {
                ch = GetXunChaWenTiByUnit(parent_unit_id);
            }
            else
            {
                ch = GetXunChaWenTiByType(parent_unit_id);
            }

            return ch;
        }
        /// <summary>
        /// ��ȡ��λ��Ϣ
        /// </summary>
        /// <param name="parent_unit_id"></param>
        /// <returns></returns>
        public DataTable GetUnits(string parent_unit_id)
        {
            DataTable dt = new DataTable();
            StringBuilder strsql = new StringBuilder();
            //            if (parent_unit_id == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
            //            {
            //                strsql.Append(@"select base_unit_id, unit from base_unit where parent_unit_id 
            //                in('0','e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
            //            }
            //            else
            //            {
            //                strsql.Append(@"select base_unit_id, unit from base_unit 
            //                where parent_unit_id in('" + parent_unit_id + "') order by sortcode");
            //            }
            strsql.Append(@"select base_unit_id, unit from base_unit 
                where parent_unit_id in('" + parent_unit_id + "') or base_unit_id='" + parent_unit_id + "' order by sortcode");
            dt = SqlHelper.DataTable(strsql.ToString(), CommandType.Text);
            return dt;
        }

        public DataTable GetXunChaType()
        {
            DataTable dt = new DataTable();
            StringBuilder strsql = new StringBuilder();

            strsql.Append(@"select distinct  xc_content_id, name,[place] from XC_Content
            order by  name,[place]");
            dt = SqlHelper.DataTable(strsql.ToString(), CommandType.Text);
            return dt;
        }
        /// <summary>
        /// ��ȡĳ����λ��Ѳ����������
        /// </summary>
        /// <param name="unit_id">��λ���</param>
        /// <param name="sub">�Ƿ����¼�����</param>
        /// <returns></returns>
        public int GetXunChaWenTiNum(string unit_id, int sub)
        {
            DataTable dt = new DataTable();
            StringBuilder strsql = new StringBuilder();
            if (sub == 1)
            {
                strsql.Append(@"select  count(*)  as num , max(u.unit) as unit from XC_Main m , base_unit u
            where xc_res=1  and m.bxc_unit_id=u.base_unit_id
            and ( u.parent_unit_id in('" + unit_id + @"') or base_unit_id in('" + unit_id + @"')) 
            and Year(m.xc_datetime)=Year(getdate())");
            }
            else
            {
                strsql.Append(@"select  count(*) as num, max(u.unit) as unit  from XC_Main m , base_unit u
                where xc_res=1  and m.bxc_unit_id=u.base_unit_id
                and  base_unit_id in('" + unit_id + @"') and Year(m.xc_datetime)=Year(getdate())");
            }
            dt = SqlHelper.DataTable(strsql.ToString(), CommandType.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                int num = int.Parse(dt.Rows[0]["num"].ToString());
                return num;
            }
            return 0;
        }

        /// <summary>
        /// ����λ��Ѳ���������� ��ȡ��������
        /// </summary>
        /// <param name="bxc_unit_id">��λ���</param>
        /// <param name="xc_content_id">Ѳ���������ͱ��</param>
        /// <returns></returns>
        public int GetXunChaWenTiNumByWenTiType(string bxc_unit_id, string xc_content_id)
        {
            DataTable dt = new DataTable();
            StringBuilder strsql = new StringBuilder();
            strsql.Append(@"select  count(1) as num from XC_Main  m ,xc_content con ,XC_Detail de
            ,base_unit u
            where  de.xc_content_id=con.xc_content_id
            and m.xc_id=de.xc_id
            and m.bxc_unit_id=u.base_unit_id
            and con.xc_content_id='" + xc_content_id + @"'
            and m.bxc_unit_id='" + bxc_unit_id + @"'
            and Year(m.xc_datetime)=Year(getdate()) ");
            dt = SqlHelper.DataTable(strsql.ToString(), CommandType.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                int num = int.Parse(dt.Rows[0]["num"].ToString());
                return num;
            }
            return 0;
        }

        #endregion

        public string GetXCZS()
        {
            string sql = string.Format("select count(*) from XC_Main where year(xc_datetime)=year(getdate()) and xc_type='1'");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetXCWT(string type)
        {

            string sql="" ;
            if (type=="1")
                sql = string.Format("select count(*) as num from XC_Main where year(xc_datetime)=year(getdate()) and month(xc_datetime)=month(getdate()) and day(xc_datetime)=day(getdate()) and xc_res=1 and xc_place='�참����' and xc_type='1'");
            else if (type == "2")
                sql = string.Format("select count(*) as num from XC_Main where year(xc_datetime)=year(getdate()) and month(xc_datetime)=month(getdate()) and day(xc_datetime)=day(getdate()) and xc_res=1 and xc_place='����Ӵ�' and xc_type='1'");
            else if (type == "3")
                sql = string.Format("select count(*) as num from XC_Main where year(xc_datetime)=year(getdate()) and month(xc_datetime)=month(getdate()) and day(xc_datetime)=day(getdate()) and xc_res=1 and xc_place='�참����Ѳ��' and xc_type='2'");
            else if (type == "4")
                sql = string.Format("select count(*) as num from XC_Main where year(xc_datetime)=year(getdate()) and month(xc_datetime)=month(getdate()) and day(xc_datetime)=day(getdate()) and xc_res=1 and xc_place='����Ӵ�Ѳ��' and xc_type='2'");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    #region lwl

    public class chart
    {
        public ArrayList categories { get; set; }

        public List<serie> data { get; set; }
    }

    public class serie
    {
        public string ParentCompanyId { get; set; }

        public string companyid { get; set; }
        public int tolast { get; set; }
        public int y { get; set; }
        public string xc_content_id { get; set; }

        public string xc_TypeName { get; set; }
    }
    #endregion

      
}
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
    public class Base_ShowRightBll : RepositoryFactory<Base_ShowRightBll>
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
        /// 全省数据
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
                cityinfo = "省院," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    sql2 = string.Format("select count(*) from Base_MonitorChannels where MonitorServer_id in(select MonitorServer_id from Base_MonitorServer where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    if (dt.Rows[i]["unit"].ToString()=="连云港市院")
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
                cityinfo = "控申接待," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (dt.Rows[i]["type"].ToString() == "1")
                    {
                        cityinfo += "|自侦办案区," + dt.Rows[i][0].ToString();
                    }
                    else if (dt.Rows[i]["type"].ToString() == "2")
                    {
                        cityinfo += "|刑检办案区," + dt.Rows[i][0].ToString();
                    }
                    else if (dt.Rows[i]["type"].ToString() == "3")
                    {
                        cityinfo += "|指定居所," + dt.Rows[i][0].ToString();
                    }
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getmonitortypeinfo()
        {
            //string sql = string.Format("select roomtype_id,name,bigtype from Base_RoomType where type='2' order by orders");
            string sql = string.Format("select distinct bigtype from Base_RoomType where type='2' and bigtype<>'其他2'");
            try
            {
                string cityinfo = "";
                string sql2 = string.Format("select count(*) from Base_MonitorApplication where room_id<>''");
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo = "功能房," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    string sql3 = string.Format("select count(*) from Base_MonitorApplication where Object_id in(select roomtype_id from Base_RoomType where bigtype='" + dt.Rows[i]["bigtype"].ToString() + "')");
                    DataTable dt3 = Repository().FindTableBySql(sql3.ToString());

                    if (dt.Rows[i]["bigtype"].ToString() == "监所")
                    {
                        cityinfo += "|监所监控," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "控申为民服务中心")
                    {
                        cityinfo += "|控申为民中心," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "刑检办案区")
                    {
                        cityinfo += "|刑检办案区," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "指定居所")
                    {
                        cityinfo += "|指定居所," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "法庭")
                    {
                        cityinfo += "|庭审直播," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "自侦办案区")
                    {
                        cityinfo += "|自侦办案区," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "机关重要场所")
                    {
                        cityinfo += "|机关重要场所," + dt3.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["bigtype"].ToString() == "其他")
                    {
                        cityinfo += "|安防监控," + dt3.Rows[0][0].ToString();
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
                for (int i =0; i <= dt.Rows.Count - 1; i++)
                {
                    string sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and month(SendDate)='" + dt.Rows[i]["monthname"].ToString() + "'  and unit_id in('" + unit_allchildren + "')");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getcityjwinfo ()
        {
            string sql = string.Format("select * from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
            try
            {
                string cityinfo = "";
                string sql2 = string.Format("select count(*) from JW_SendPolice where unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo = "省院," + dt2.Rows[0][0].ToString();

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    sql2 = string.Format("select count(*) from JW_SendPolice where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    if (dt.Rows[i]["unit"].ToString() == "连云港市院")
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

        public string GetfordoList(string type)
        {
            string user_id=ManageProvider.Provider.Current().UserId;
            string sql;
            if (type == "1")//法警部门负责人审核
                sql = string.Format("select a.state from JW_Apply a join Base_PoliceArea p on a.PoliceArea_id=p.PoliceArea_id where a.state in('1') and (Charindex('" + user_id + "',DepLeader)>0) union select state from JW_PoliceApply where state in('1') and unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',DepLeader)>0)");
            else if (type == "2")//警务领导审批
                sql = string.Format("select a.state from JW_Apply a join Base_PoliceArea p on a.PoliceArea_id=p.PoliceArea_id where a.state in('2') and (Charindex('" + user_id + "',DepLeader)>0) union select state from JW_PoliceApply where state in('2') and unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',DepLeader)>0)");
            else if (type == "3")//警务执勤
                sql = string.Format("select state from JW_SendPolice where user_id='" + user_id + "' and state in('0')");
            else if (type == "4")//退回
                sql = string.Format("select state from JW_Apply where adduser_id='" + user_id + "' and state in('-1','-2') union select state from JW_PoliceApply where adduser_id='" + user_id + "' and state in('-1','-2')");
        
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

        public string Getjwtypeinfo2()//本月执行警务分类汇总
        {
            string sql = string.Format("select code,FullName from Base_DataDictionaryDetail where datadictionaryid='ae8ada69-5adf-490d-93c1-4201f6f2b51f'");
            try
            {
                unit_allchildren = ManageProvider.Provider.Current().CompanyId;
                getchild(unit_allchildren);
                string cityinfo = "";
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    
                    if (dt.Rows[i]["FullName"].ToString() == "保护犯罪现场")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo = "保护现场," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "执行传唤")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|执行传唤," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "执行拘传")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|执行拘传," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "协助执行指定居所监视居住")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|协助监居," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "协助执行拘留、逮捕")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|协助拘捕," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "参与追捕在逃或者脱逃的犯罪嫌疑人")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|参与追捕," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "参与搜查任务")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|参与搜查," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "提押犯罪嫌疑人被告人或罪犯")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|提押人员," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "看管犯罪嫌疑人被告人或罪犯")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|看管人员," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "送达法律文书")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|送达文书," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "保护检察人员安全")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|保护人员," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "办公、办案、控申接待场所执勤")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|场所执勤," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "参与处置突发事件任务")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|参与处突," + dt2.Rows[0][0].ToString();
                    }
                    else if (dt.Rows[i]["FullName"].ToString() == "完成其他任务")
                    {
                        string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo += "|其他任务," + dt2.Rows[0][0].ToString();
                    }
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
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
                string sql = string.Format("select top 10 DailyReport_id,b.unit,r.adddate from JW_DailyReport r join Base_Unit b on b.Base_Unit_id=r.unit_id where r.unit_id in('" + unit_allparent + "') order by adddate desc");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if(cityinfo=="")
                        cityinfo = dt.Rows[i]["unit"].ToString() + "," + dt.Rows[i]["adddate"].ToString() + "," + dt.Rows[i]["DailyReport_id"].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString() + "," + dt.Rows[i]["adddate"].ToString() + "," + dt.Rows[i]["DailyReport_id"].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getbamonthinfo()
        {
            string sql = string.Format("select monthname from Base_Month order by monthname");
            try
            {
                string cityinfo = "";
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    string sql2 = string.Format("select count(*) from JW_Apply where year(fact_indate)=year(getdate()) and month(fact_indate)='" + dt.Rows[i]["monthname"].ToString() + "' and type in('1','2')");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getjjmonthinfo()
        {
            string sql = string.Format("select monthname from Base_Month order by monthname");
            try
            {
                string cityinfo = "";
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    string sql2 = string.Format("select count(*) from JW_Apply where year(fact_indate)=year(getdate()) and month(fact_indate)='" + dt.Rows[i]["monthname"].ToString() + "' and type in('3')");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Getjdmonthinfo()
        {
            string sql = string.Format("select monthname from Base_Month order by monthname");
            try
            {
                string cityinfo = "";
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    string sql2 = string.Format("select count(*) from CheckIn_LF where year(check_time)=year(getdate()) and month(check_time)='" + dt.Rows[i]["monthname"].ToString() + "'");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["monthname"].ToString() + "月," + dt2.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
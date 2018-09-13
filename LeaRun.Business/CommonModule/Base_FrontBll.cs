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
    public class Base_FrontBll : RepositoryFactory<Base_FrontBll>
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

        public DataTable getallregcode()
        {
            DataTable dt = new DataTable();
            string sql = "select regcode from base_unit order by sortcode";
            dt = Repository().FindTableBySql(sql.ToString());
            return dt;
        }

        public string getallpolicenum()//获取当年履职趋势图的总量
        {
            unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            getchild(unit_allchildren);
            string sql = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and unit_id in('" + unit_allchildren + "')");
            try
            {
                //ManageProvider.Provider.Current().ObjectId
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0][0].ToString() + "|" + unit_allchildren;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string getallHandleCenternum()//按月办案中心使用趋势总数
        {
             unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            getchild(unit_allchildren);
            string sql = string.Format("select count(apply_id) from JW_Apply where year(adddate)=year(getdate()) and unit_id in('" + unit_allchildren + "')");
            try
            {
                //ManageProvider.Provider.Current().ObjectId
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0][0].ToString() + "|" + unit_allchildren;
            }
            catch (Exception)
            {
                return null;
            }
        }
            
        //饼状图总计
        public string gettypepolicenum(string type,string strunintid)//获取各类警务区使用情况
        {
            //unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            //getchild(unit_allchildren);

            string sql="";
            if (type == "1")//自侦
                //sql = string.Format("select count(*) from JW_Apply where type='1' and state in('4') and unit_id in('" + strunintid + "')");
                sql = string.Format("select count(*) from JW_Usedetail where isend=0 and unit_id in('" + strunintid + "')  and policearea_id in (select  policearea_id from Base_PoliceArea where areatype=1)");
            else if (type == "2")//刑检
                sql = string.Format("select count(*) from JW_Apply where type='2' and state in('4') and unit_id in('" + strunintid + "')");
            else if (type == "3")//监居
                sql = string.Format("select count(*) from JW_Apply where type='3' and state in('4') and unit_id in('" + strunintid + "')");
            else if (type == "4")//控申接待
                sql = string.Format("select count(*) from CheckIn_LF where datediff(day,check_time,getdate())=0 and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where unit_id in('" + strunintid + "'))");
            else if (type == "5")//其他场所
                sql = "select count(*) from Base_unit where Base_unit_id in('" + strunintid + "')";
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
        
        public string getuserrole()
        {
            string user_id = ManageProvider.Provider.Current().UserId;
            string sql = string.Format("select * from dbo.Base_PoliceArea where (Charindex('" + user_id + "',DepLeader)>0) or  (Charindex('" + user_id + "',UnitLeader)>0)");
            try
            {
                //ManageProvider.Provider.Current().ObjectId
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows.Count.ToString();
            }
            catch (Exception)
            {
                return "0";
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
            string strunitid = ManageProvider.Provider.Current().CompanyId;
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
            else if (type == "6")//在办案
                sql = string.Format("select * from Case_caseinfo where state='1' and unit_id='" + strunitid + "'");
            else if (type == "7")//在用房间
                sql = string.Format("select * from JW_Usedetail where isend='0' and unit_id='" + strunitid + "'");
            else if (type == "8")//在执勤
                sql = string.Format("select * from JW_SendPolice where state in(1) and unit_id='" + strunitid + "'");
            else if (type == "9")//指定监居
                sql = string.Format("select * from JW_Apply_room where state in(1,2,3,4,5) and unit_id='" + strunitid + "'");
                //sql = string.Format("select * from JW_Apply where type='3' and state in(4)");
            else if (type == "10")
                sql = string.Format("select * from CheckIn_LF");
            else if (type == "11")//办案超时
                sql = string.Format("select * from JW_Usedetail where ((isend='0' and datediff(hour ,startdate,getdate())>12) or (isend='1' and datediff(hour ,startdate,enddate)>12 and datediff(day,startdate,getdate())=0)) and unit_id='" + strunitid + "'");
            else if (type == "12")//违纪违规
                sql = string.Format("select * from JW_BreakRoles where datediff(day,startdate,getdate())=0 and unit_id='" + strunitid + "'");
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

        public string Getjwtypeinfo()
        {
            unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            getchild(unit_allchildren);
            //string sql = string.Format("select count(*),type from JW_Apply where datediff(day,startdate,getdate())=0 and unit_id in('" + unit_allchildren + "') group by type ");
            string sql = string.Format("select count(*),type from JW_Apply where state in('4') and unit_id in('" + unit_allchildren + "') group by type ");
            try
            {
                string cityinfo = "";
                string sql2 = string.Format("select count(*) from CheckIn_LF where datediff(day,check_time,getdate())=0 and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where unit_id in('" + unit_allchildren + "')) ");
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

        //饼状图数量
        public string GetjwtypeinfoBAQ(string type)
        {
            unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            //getchild(unit_allchildren);
            if (unit_allchildren == "e2c79c56-5b58-4c62-b2a9-3bb7492c")//sheng
            { 
                string sql = string.Format("select * from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
                try
                {
                    string cityinfo = "";
                    string sql2 = "";
                    if (type == "1")//自侦办案区,改为根据usedetail办案表读取//20180726自侦和刑检合并
                        sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and state in('4') and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                        //sql2 = string.Format("select count(*) from JW_Usedetail where isend=0 and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c' and policearea_id in (select  policearea_id from Base_PoliceArea where areatype=1 and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c')");
                    else if (type == "2")//刑检办案区
                        sql2 = string.Format("select count(*) from JW_Apply where type='2' and state in('4') and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    else if (type == "3")//指定居所
                        sql2 = string.Format("select count(*) from JW_Apply where type='3' and state in('4') and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    else if (type == "4")//控申接待
                        sql2 = string.Format("select count(*) from CheckIn_LF where datediff(day,check_time,getdate())=0 and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c'))");
                    else if (type == "42")//控申接待
                        sql2 = "";
                    else if (type == "5")//其他场所
                        sql2 = "";
                    else if (type == "6")//履职数量
                        sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2') and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");

                    if (sql2 != "")
                    {
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        if (dt2.Rows[0][0].ToString() != "0")
                            cityinfo = "省院," + dt2.Rows[0][0].ToString();
                    }
                    else
                        cityinfo = "省院,1";
                   
                    DataTable dt = Repository().FindTableBySql(sql.ToString());
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (type == "1")//自侦办案区
                            sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and state in('4') and (Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                            //sql2 = string.Format("select count(*) from JW_Usedetail where isend=0 and (Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')) and policearea_id in (select  policearea_id from Base_PoliceArea where areatype=1)");
                        else if (type == "2")//刑检办案区
                            sql2 = string.Format("select count(*) from JW_Apply where type='2' and state in('4') and (Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "3")//指定居所
                            sql2 = string.Format("select count(*) from JW_Apply where type='3' and state in('4') and (Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "4")//控申接待
                            sql2 = string.Format("select count(*) from CheckIn_LF where datediff(day,check_time,getdate())=0 and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "42")//控申接待
                            sql2 = "";
                        else if (type == "5")//其他场所
                            sql2 = "";
                        else if (type == "6")//履职数量
                            sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2') and unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");

                        if (sql2 != "")
                        {
                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                            {
                                if (dt3.Rows[0][0].ToString() != "0")
                                {
                                    if (cityinfo == "")
                                        cityinfo = dt.Rows[i]["unit"].ToString().Substring(0, 4) + "," + dt3.Rows[0][0].ToString();
                                    else
                                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 4) + "," + dt3.Rows[0][0].ToString();
                                }
                            }
                            else
                            {
                                if (dt3.Rows[0][0].ToString() != "0")
                                {
                                    if (cityinfo == "")
                                        cityinfo = dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString();
                                    else
                                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString();
                                }

                            }
                        }
                        else
                        {
                            if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                                cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 4) + ",1";
                            else
                                cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + ",1";
                        }
                      
                    }
                    if (cityinfo!="")
                        return cityinfo;
                    else
                        return ",0";
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                string sqlcity = string.Format("select Base_Unit_id,unit from Base_unit where Base_Unit_id='" + unit_allchildren + "' or parent_unit_id ='" + unit_allchildren + "' order by code");
                DataTable dtcity = Repository().FindTableBySql(sqlcity.ToString());
                string cityinfo = "";
                for( int j=0;j<=dtcity.Rows.Count-1;j++)//shi
                {
                    string sql2 = "";
                    if (type == "1")//自侦办案区
                        //sql2 = string.Format("select count(*) from JW_Apply where type='1' and state in('4') and PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                        sql2 = string.Format("select count(*) from JW_Usedetail where isend=0 and PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "' and areatype=1)");
                    else if (type == "2")//刑检办案区
                        sql2 = string.Format("select count(*) from JW_Apply where type='2' and state in('4') and  PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                    else if (type == "3")//指定居所
                        sql2 = string.Format("select count(*) from JW_Apply where type='3' and state in('4') and  PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                    else if (type == "4")//控申接待
                        sql2 = string.Format("select count(*) from CheckIn_LF where datediff(day,check_time,getdate())=0 and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                    else if (type == "42")//控申接待
                        sql2 = "";
                    else if (type == "5")//其他场所
                        sql2 = "";
                    else if (type == "6")//履职数量
                         sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2') and unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "'");
                        
                    if (sql2 != "")
                    {
                        DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                        if (dt3.Rows[0][0].ToString() != "0")
                        {
                            if (cityinfo == "")
                                cityinfo = dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                            else
                                cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                        }
                    }
                    else
                    {
                        if (cityinfo == "")
                            cityinfo = dtcity.Rows[j]["unit"].ToString() + ",1";
                        else
                            cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + ",1";
                    }
                        
                }
           
               
                if (cityinfo != "")
                    return cityinfo;
                else
                    return ",0";
            }

        }

        public string GetjwtypeinfoBAQCity(string strunitid, string type)
        {
            string sqlcity = string.Format("select Base_Unit_id,unit from Base_unit where Base_Unit_id='" + strunitid + "' or parent_unit_id ='" + strunitid + "' order by code");
            DataTable dtcity = Repository().FindTableBySql(sqlcity.ToString());
            string cityinfo = "";
            for (int j = 0; j <= dtcity.Rows.Count - 1; j++)//shi
            {
                string sql2 = "";
                if (type == "1")//自侦办案区
                    //sql2 = string.Format("select count(*) from JW_Apply where type='1' and state in('4') and PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                    sql2 = string.Format("select count(*) from JW_Usedetail where isend=0 and PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "' and areatype=1)");
                else if (type == "2")//刑检办案区
                    sql2 = string.Format("select count(*) from JW_Apply where type='2' and state in('4') and  PoliceArea_id in(select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");

                DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                if (dt3.Rows[0][0].ToString() != "0")
                {
                    if (cityinfo == "")
                        cityinfo = dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                }
            }
            return cityinfo;
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

        public string getHCmonthinfo()
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
                    string sql2 = string.Format("select count(*) from JW_Apply where year(adddate)=year(getdate()) and month(adddate)='" + dt.Rows[i]["monthname"].ToString() + "'  and unit_id in('" + unit_allchildren + "')");
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
        public string getMonthDayCount(string mon)
        {
            string cityinfo = "";
            int month = Convert.ToInt16(mon.Substring(0, mon.Length - 1));
            int days = System.DateTime.DaysInMonth(System.DateTime.Now.Year, Convert.ToInt16(month));
            for (int i = 0; i < days; i++)
            {
                if (cityinfo == "")
                    cityinfo = (i + 1).ToString();
                else
                    cityinfo += "," + (i + 1);
            }
            return cityinfo;
        }


        public string getJWDayinfo(string mon)
        {
            //string sql = string.Format("select monthname from Base_Month order by monthname");
            int month = Convert.ToInt16(mon.Substring(0, mon.Length - 1));
            int days = System.DateTime.DaysInMonth(System.DateTime.Now.Year, Convert.ToInt16(month));
            try
            {
                unit_allchildren = ManageProvider.Provider.Current().CompanyId;
                getchild(unit_allchildren);
                string cityinfo = "";
               // DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < days; i++)
                {
                    string sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and month(SendDate)='" + month + "' and day(SendDate) ='"+(i+1)+"'  and unit_id in('" + unit_allchildren + "')");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = (i+1)+"," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + (i + 1) + "," + dt2.Rows[0][0].ToString();
                }
                return cityinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string getHCDayinfo(string mon)
        {
            //string sql = string.Format("select monthname from Base_Month order by monthname");
            int month = Convert.ToInt16(mon.Substring(0, mon.Length - 1));
            int days = System.DateTime.DaysInMonth(System.DateTime.Now.Year, Convert.ToInt16(month));
            try
            {
                unit_allchildren = ManageProvider.Provider.Current().CompanyId;
                getchild(unit_allchildren);
                string cityinfo = "";
                // DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < days; i++)
                {
                    string sql2 = string.Format("select count(apply_id) from JW_Apply where year(adddate)=year(getdate()) and month(adddate)='" + month + "' and day(adddate) ='" + (i + 1) + "'  and unit_id in('" + unit_allchildren + "')");
                    DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                    if (cityinfo == "")
                        cityinfo = (i + 1) + "," + dt2.Rows[0][0].ToString();
                    else
                        cityinfo += "|" + (i + 1) + "," + dt2.Rows[0][0].ToString();
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
            string user_id = ManageProvider.Provider.Current().UserId;
            string sql;
            if (type == "1")//法警部门负责人审核
                sql = string.Format("select a.state from JW_Apply a join Base_PoliceArea p on a.PoliceArea_id=p.PoliceArea_id where a.state in('1') and (Charindex('" + user_id + "',DepLeader)>0) union all select state from JW_PoliceApply where state in('1') and unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',DepLeader)>0)");
            else if (type == "2")//警务领导审批
                sql = string.Format("select a.state from JW_Apply a join Base_PoliceArea p on a.PoliceArea_id=p.PoliceArea_id where a.state in('2') and (Charindex('" + user_id + "',UnitLeader)>0) union all select state from JW_PoliceApply where state in('2') and unit_id in(select unit_id from Base_PoliceArea where Charindex('" + user_id + "',UnitLeader)>0)");
            else if (type == "3")//警务执勤
                sql = string.Format("select state from JW_SendPolice where user_id='" + user_id + "' and state in('0')");
            else if (type == "4")//自侦退回
                sql = string.Format("select state from JW_Apply where type='1' and adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "5")//刑检退回
                sql = string.Format("select state from JW_Apply where type='2' and adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "6")//指定居所退回
                sql = string.Format("select state from JW_Apply where type='3' and adduser_id='" + user_id + "' and state in('-1','-2')");
            else if (type == "7")//派警确认退回
                sql = string.Format("select state from JW_SendPolice where SendUser_id='" + user_id + "' and state in('-1')");
            else if (type == "8")//退回
                sql = string.Format("select state from JW_PoliceApply where adduser_id='" + user_id + "' and state in('-1','-2')");
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

        //////public string Getjwtypeinfo2()//房间状态
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

        //////            if (dt.Rows[i]["FullName"].ToString() == "保护犯罪现场")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo = "保护现场," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "执行传唤")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|执行传唤," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "执行拘传")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|执行拘传," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "协助执行指定居所监视居住")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|协助监居," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "协助执行拘留、逮捕")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|协助拘捕," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "参与追捕在逃或者脱逃的犯罪嫌疑人")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|参与追捕," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "参与搜查任务")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|参与搜查," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "提押犯罪嫌疑人被告人或罪犯")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|提押人员," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "看管犯罪嫌疑人被告人或罪犯")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|看管人员," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "送达法律文书")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|送达文书," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "保护检察人员安全")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|保护人员," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "办公、办案、控申接待场所执勤")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|场所执勤," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "参与处置突发事件任务")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|参与处突," + dt2.Rows[0][0].ToString();
        //////            }
        //////            else if (dt.Rows[i]["FullName"].ToString() == "完成其他任务")
        //////            {
        //////                string sql2 = string.Format("select count(*) from JW_SendPolice s join JW_PoliceApply a on (s.Object_id=a.apply_id and s.type='4') where a.tasktype_id='" + dt.Rows[i]["code"].ToString() + "' and s.unit_id in('" + unit_allchildren + "')");
        //////                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
        //////                cityinfo += "|其他任务," + dt2.Rows[0][0].ToString();
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
        /// 获取房间的状态
        /// </summary>
        /// <param name="type">警务区类型</param>
        /// <param name="unitId">当前登录用户的单位主键</param>
        /// <param name="roomType">加载除了指定的其他全部的房间类型</param>
        /// <returns></returns>
        public string GetRoomState(string type, string unit_id, string user_id)
        {
            if (unit_id == null || unit_id.ToString() == "")
            {
                unit_id = ManageProvider.Provider.Current().CompanyId;
            }

            string sql = "";
            string typeName = "";
            if (type == "1")
            {
                typeName = "自侦办案区";
            }
            else if (type == "6")
            {
                typeName = "指定居所";
            }
            else if (type == "2")
            {
                typeName = "刑检办案区";
            }
            else if (type == "-1")
            {
                typeName = "其他办案法警";
            }

            if (type == "1" || type == "2")//自侦办案区,刑检办案区
            {
                sql = string.Format(@"
                    select room.room_id,room.RoomName,isnull(usenow.isend,1) isend,usenow.username,usenow.apply_id,room.PoliceArea_id from
                        (select br.room_id ,bpa.AreaType, br.RoomName,br.unit_id,RoomCode,bpa.PoliceArea_id from Base_Room br
                        left join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id and  bpa.unit_id=br.unit_id 
                        where br.unit_id='{1}' and br.state=1 
                        and bpa.AreaType='{0}') room
                        left  join 
                        (select ud.room_id,min(ud.isend) isend,max(ja.userName) userName,min(ja.apply_id) apply_id  from  JW_Usedetail ud 
                        join jw_apply ja on ja.apply_id=ud.apply_id where ud.isend=0 group by ud.room_id ) usenow
                        on room.room_id=usenow.Room_id order by room.RoomCode
                      "
                       , type
                       , unit_id
                        );
            }
            else if (type == "6")//指定居所
            {
                sql = string.Format(@"
                    select room.room_id,room.RoomName,usenow.enddate ,isnull(usenow.userName,'') userName ,isnull(usenow.state,99) state ,usenow.apply_id from
                        (select br.room_id ,bpa.AreaType, br.RoomName,br.unit_id,RoomCode from Base_Room br
                        left join Base_PoliceArea bpa on bpa.PoliceArea_id=br.PoliceArea_id and  bpa.unit_id=br.unit_id 
                        where br.unit_id='{1}' and br.state=1 
                        and bpa.AreaType='{0}') room
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
                       );

            }
            else if (type == "-1")//其他办案
            {
                sql = string.Format(@"
                        select bu.RealName,case jpa.tasktype_id 
                        when 1 then '保护犯罪现场'
                        when 2 then '执行传唤'
                        when 3 then '执行拘传'
                        when 5 then '协助执行拘留、逮捕'
                        when 6 then '参与追捕在逃或者逃脱的犯罪嫌疑人'
                        when 7 then '参与搜查任务'
                        when 8 then '提押犯罪嫌疑人被告人或罪犯'
                        when 10 then '送达法律文书'
                        when 11 then '保护检察人员'
                        when 13 then '参与处置突发事件任务'
                        when 14 then '完成其他任务'
                        end  tasktype_id
                        from JW_SendPolice jsp
                        left join Base_user bu on bu.UserId =jsp.user_id 
                        left join JW_policeapply jpa on jpa.apply_id= jsp.object_id  
                        where jsp.unit_id='{0}' and jsp.state=1  and jsp.type in (4,5)
                        and jpa.tasktype_id not in (4,9,12)
                    "
                      , unit_id
                       );

            }
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);      //所有房间
                string returnHtml = string.Empty;
                int heighta = 0;
                int ai = 0;
                if (dt.Rows.Count == 0)
                {
                    heighta = 0;
                }
                else
                {
                   
                    ai =   (int)Math.Ceiling((decimal)(dt.Rows.Count + 1) / 5);
                    heighta = ai * 82 + 40;

                }
                    //if (dt.Rows.Count > 0 && dt.Rows.Count < 5)
                    //{
                    //    heighta = 115;
                    //}
                    //else if (dt.Rows.Count >= 5 && dt.Rows.Count < 10)
                    //{
                    //    heighta = 197;
                    //}
                    //else if (dt.Rows.Count >= 10 && dt.Rows.Count < 15)
                    //{
                    //    heighta = 279;
                    //}
                    //else if (dt.Rows.Count >= 15 && dt.Rows.Count < 20)
                    //{
                    //    heighta = 361;
                    //}
                    //else if (dt.Rows.Count >= 20 && dt.Rows.Count < 25)
                    //{
                    //    heighta = 443;
                    //}
                    //else if (dt.Rows.Count >= 25 && dt.Rows.Count < 30)
                    //{
                    //    heighta = 525;
                    //}
                    //else if (dt.Rows.Count >= 30 && dt.Rows.Count < 35)
                    //{
                    //    heighta = 607;
                    //}
                    //else if (dt.Rows.Count >= 35)
                    //{
                    //    heighta = 689;
                    //}
                returnHtml += "<div style=\" width:100%; height:" + heighta + "px;\">";
                //returnHtml += "<td>"+typeName+"</td>";
                if (type == "1") //自侦办案区
                {
                    string policeAreaId = dt.Rows[0]["PoliceArea_id"].ToString();

                    string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "1");


                    //returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>自侦办案区</h7></div>";
                    returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 25%; text-align:left;\">自侦办案区</th><th style=\"width: 10%; text-align:right;\">值班领导:</th><td style=\"width: 15%; text-align:left;\">" +
                                  "<span id='leaderUser1'>" + usersMsg.Split('|')[0] + "</span>" +
                                  "</td><th style=\"width: 10%; text-align:right;\">值班人员:</th><td style=\"width: 15%; text-align:left;\">" +
                                  "<span id='fjUser1'>"+usersMsg.Split('|')[1]+"</span>" +
                                  "</td><td style=\"width: 25%; text-align:center;\"><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                  "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','1');" +
                                  ">开始执勤</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                  "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','2');" +
                                  ">交接班</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                  "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','3');" +
                                  ">结束执勤</button></td></tr></table></div>";
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (dt.Rows[i]["isend"].ToString() == "0")//房间在使用中
                        {
                            returnHtml += "<div  align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:10px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>" + dt.Rows[i]["userName"].ToString() + "</div>";
                        }
                        else//房间没在使用
                        {
                            returnHtml += "<div  align='left' style=\"cursor:hand;background-color:Green;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div style=\"text-align:center;margin-top:15px;font-weight:bold\">空闲中……</div>";
                        }
                        returnHtml += "</div>";
                    }
                    //增加法警值班室
                    //获取法警
                    string policeName = "";
                    //                    string sql1 = string.Format(@"
                    //                                select distinct bu.RealName from jw_sendpolice sp 
                    //                                join base_user bu on bu.UserId=sp.User_id
                    //                                where state in('1','2') and (sp.type='1' or (sp.type in('4','5') and Object_id in(select apply_id from JW_PoliceApply where tasktype_id='12')))");
                    if (dt.Rows.Count > 0)//有房间的，才增加值班室
                    {
                        string sql1 = string.Format(@"
select distinct bu.RealName from jw_sendpolice sp 
join base_user bu on bu.UserId=sp.User_id 
join JW_Apply ja on ja.apply_id=sp.Object_id 
join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id
where sp.state in('1','2') and (sp.type='1' or (sp.type in('4','5') and Object_id in(select apply_id from JW_PoliceApply where tasktype_id='12'))) and bpa.Unit_id='{0}'
", unit_id);

                        DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                        policeName = "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr>";
                        for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                        {
                            policeName += "<td><img src='../../Content/Images/Icon16/user_policeman(2).png'/><br/>" + dt1.Rows[j]["RealName"].ToString() + "</td>";
                        }
                        policeName += "</div></tr></table>";
                        returnHtml += "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;法警值班室</div>" + policeName + "</div>";
                    }
                }

                else if (type == "2") //刑检办案区
                {
                    string policeAreaId = dt.Rows[0]["PoliceArea_id"].ToString();

                    string usersMsg = GetOnDutyUsersMsg(unit_id, policeAreaId, "1");
                    //returnHtml +="<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>刑检办案区</h7></div>";
                    returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 25%; text-align:left;\">刑检办案区</th><th style=\"width: 10%; text-align:right;\">值班领导:</th><td style=\"width: 15%; text-align:left;\">" +
                                 "<span id='leaderUser1'>" + usersMsg.Split('|')[0] + "</span>" +
                                 "</td><th style=\"width: 10%; text-align:right;\">值班人员:</th><td style=\"width: 15%; text-align:left;\">" +
                                 "<span id='fjUser1'>" + usersMsg.Split('|')[1] + "</span>" +
                                 "</td><td style=\"width: 25%; text-align:center;\"><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" " +
                                 "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','1');" +
                                 ">开始执勤</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                 "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','2');" +
                                 ">交接班</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" " +
                                 "onclick=ActionDuty('" + unit_id + "','" + user_id + "','" + policeAreaId + "','1','3');" +
                                 ">结束执勤</button></td></tr></table></div>";


                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (dt.Rows[i]["isend"].ToString() == "0") //房间在使用中
                        {
                            returnHtml +=
                                "<div  align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" +
                                dt.Rows[i]["RoomName"].ToString() +
                                "</div><div style=\"text-align:center;margin-top:10px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>" +
                                dt.Rows[i]["userName"].ToString() + "</div>";
                        }
                        else //房间没在使用
                        {
                            returnHtml +=
                                "<div  align='left' style=\"cursor:hand;background-color:Green;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" +
                                dt.Rows[i]["RoomName"].ToString() +
                                "</div><div style=\"text-align:center;margin-top:15px;font-weight:bold\">空闲中……</div>";
                        }
                        returnHtml += "</div>";
                    }
                    //增加法警值班室
                    //获取法警
                    string policeName = "";
                    //                    string sql1 = string.Format(@"
                    //                                select distinct bu.RealName from jw_sendpolice sp 
                    //                                join base_user bu on bu.UserId=sp.User_id
                    //                                where state in('1','2') and (sp.type='2')"
                    //                    );
                    if (dt.Rows.Count > 0) //有房间的，才增加值班室
                    {
                        string sql1 = string.Format(@"
select distinct bu.RealName from jw_sendpolice sp 
join base_user bu on bu.UserId=sp.User_id 
join JW_Apply ja on ja.apply_id=sp.Object_id 
join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id
where sp.state in('1','2') and sp.type='2' and bpa.Unit_id='{0}'
", unit_id);


                        DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                        policeName =
                            "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr>";
                        for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                        {
                            policeName += "<td><img src='../../Content/Images/Icon16/user_policeman(2).png'/><br/>" +
                                          dt1.Rows[j]["RealName"].ToString() + "</td>";
                        }
                        policeName += "</div></tr></table>";
                        returnHtml +=
                            "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;法警值班室</div>" +
                            policeName + "</div>";
                    }
                }

                else if (type == "6") //指定居所
                {
                   
                    //returnHtml +="<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>指定居所</h7></div>";
                    returnHtml += "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height:38px;\"><table style=\"width: 100%;\"><tr><th style=\"width: 25%; text-align:left;\">指定居所</th><th style=\"width: 10%; text-align:right;\">值班领导:</th><td style=\"width: 15%; text-align:left;\">" +
                                 "<span id='leaderUser1'></span>" +
                                 "</td><th style=\"width: 10%; text-align:right;\">值班人员:</th><td style=\"width: 15%; text-align:left;\">" +
                                 "<span id='fjUser1'></span>" +
                                 "</td><td style=\"width: 25%; text-align:center;\"><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px; margin-left:5px;\" >开始执勤</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" >交接班</button><button class=\"btn btn-xs btn btn-xs btn-pink\" style=\"width:40px;line-height:14px;margin-top:0px;margin-left:5px;\" >结束执勤</button></td></tr></table></div>";


                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        string stateName = "";

                        if (dt.Rows[i]["userName"].ToString() != "") //房间在使用中
                        {
                            switch (dt.Rows[i]["state"].ToString())
                            {
                                case "1":
                                    stateName = "正常居住中";
                                    break;
                                case "2":
                                    stateName = "外出提审中";
                                    break;
                                case "3":
                                    stateName = "就医中";
                                    break;
                                case "4":
                                    stateName = "外出就医中";
                                    break;
                                case "5":
                                    stateName = "会见中";
                                    break;
                                case "7":
                                    stateName = "其他外出中";
                                    break;
                                case "-999":
                                    stateName = "使用中";
                                    break;
                                default:
                                    break;
                            }


                            //returnHtml += "<div align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:120px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</div><div><img src='../../Content/Images/Icon32/user_firefighter.png' width='18px' height='7px'  />&nbsp;&nbsp;" + dt.Rows[i]["userName"].ToString() + "：" + stateName + "</div><div>" + policeName + "</div>";
                            returnHtml +=
                                "<div align='left' style=\"cursor:hand;background-color:#842B00;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" +
                                dt.Rows[i]["RoomName"].ToString() +
                                "</div><div style=\"text-align:center;margin-top:10px;font-weight:bold\"><img src='../../Content/Images/Icon16/bullet_user.png'/>&nbsp;&nbsp;" +
                                dt.Rows[i]["userName"].ToString() +
                                "</div><div style=\"text-align:center;font-weight:bold\">" + stateName + "</div>";
                        }
                        else //房间没在使用
                        {
                            returnHtml +=
                                "<div align='left' style=\"cursor:hand;background-color:Green;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;" +
                                dt.Rows[i]["RoomName"].ToString() +
                                "</div><div style=\"text-align:center;margin-top:15px;font-weight:bold\">空闲中……</div>";
                        }
                        returnHtml += "</div>";
                    }
                    //增加法警值班室
                    //获取法警
                    string policeName = "";
                    //                    string sql1 = string.Format(@"
                    //                                select distinct bu.RealName from jw_sendpolice sp 
                    //                                join base_user bu on bu.UserId=sp.User_id
                    //                                where state in('1','2') and (sp.type='3' or (sp.type in('4','5') and Object_id in(select apply_id from JW_PoliceApply where tasktype_id='4')))"
                    //                    );
                    if (dt.Rows.Count > 0) //有房间的，才增加值班室
                    {
                        string sql1 = string.Format(@"select distinct bu.RealName from jw_sendpolice sp 
                                join base_user bu on bu.UserId=sp.User_id 
                                join JW_Apply ja on ja.apply_id=sp.Object_id 
								join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id
                                where sp.state in('1','2') and (sp.type='3' or (sp.type in('4','5') and Object_id in(select apply_id from JW_PoliceApply where tasktype_id='4'))) and bpa.Unit_id='{0}'",
                            unit_id);



                        DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);
                        policeName =
                            "<div style=\"text-align:center;margin-top:10px;font-weight:bold;\"><table width='95%'><tr>";
                        for (int j = 0; j <= dt1.Rows.Count - 1; j++)
                        {
                            policeName += "<td><img src='../../Content/Images/Icon16/user_policeman(2).png'/><br/>" +
                                          dt1.Rows[j]["RealName"].ToString() + "</td>";
                        }
                        policeName += "</div></tr></table>";
                        returnHtml +=
                            "<div align='left' style=\"cursor:hand;background-color:Blue;color:white; width:125px; height:80px; float:left; margin-right:5px; margin-bottom:2px;\"><div> &nbsp;法警值班室</div>" +
                            policeName + "</div>";
                    }
                }

                else if (type == "-1") //其他办案
                {

                    returnHtml +=
                        "<div class=\"widget-header widget-header-flat widget-header-small\" style=\"height: 10px;\"><h7>其他执勤法警 (鼠标悬停法警姓名显示执勤任务)</h7></div>";
                    returnHtml +=
                        "<div style=\"text-align:left;margin-top:5px;margin-left:5px;font-weight:bold;\"><table ><tr>";

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        //  returnHtml += "<div  align='left' ><img src=\"/Content/Images/Icon16/user_female.png\"><input type='txt' readonly='readonly' style='width:45px;' value='" + dt.Rows[i]["RealName"].ToString() + "'  title='" + dt.Rows[i]["tasktype_id"].ToString() + "'/>";
                        returnHtml += "<td style=\"width:50px\" title='" + dt.Rows[i]["tasktype_id"].ToString() +
                                      "'>&nbsp;&nbsp;&nbsp;<img src='../../Content/Images/Icon16/user_policeman(2).png'/><br/>" +
                                      dt.Rows[i]["RealName"].ToString() + "</td>";
                    }
                    returnHtml += "</tr></table></div>";
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
                string sql = string.Format("select top 10 DailyReport_id,b.unit,r.adddate from JW_DailyReport r join Base_Unit b on b.Base_Unit_id=r.unit_id where r.unit_id in('" + unit_allparent + "') order by adddate desc");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (cityinfo == "")
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
        /// 全省市院数据包括省院
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
                    string sql = string.Format(@"select unit,case when Base_Unit_id='" + parent_company_id + @"' then 1 else 0 end as toLast,parent_unit_id from base_unit 
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
                        if (dt.Rows[i]["parent_unit_id"].ToString() == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                            builder.Append(dt.Rows[i][0].ToString().Substring(0, dt.Rows[i][0].ToString().Length - 1) + ",");
                        else
                            builder.Append(dt.Rows[i][0].ToString() + ",");
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
                                   select a.companyid,a.y,b.code as color,case when a.companyid='" + parent_company_id + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid,count(Unit_id) as y,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join jw_sendpolice b on a. Base_Unit_id=b.Unit_id and type<6
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + parent_company_id + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + parent_company_id + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='颜色'
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
                        string sql2 = string.Format("select count(*) from JW_SendPolice where unit_id='" + companyid + "' and  type < 6");
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"color\":\"" + colors[0] + "\",\"tolast\":1},");

                        DataTable dt = Repository().FindTableBySql(sql.ToString());
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            sql2 = string.Format("select count(*) from JW_SendPolice where type <6 and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"color\":\"" + colors[i % 9] + "\",\"tolast\":0},");
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

        public string GetALLCityData(string companyid)
        {
            string[] colors = { "#2f7ed8", "#0d233a", "#8bbc21", "#910000", "#1aadce", "#492970", "#f28f43", "#77a1e5", "#c42525", "#a6c96a" };
            string sql = string.Format("select * from Base_unit where parent_unit_id in('" + companyid + "') order by sortcode");
            try
            {
                StringBuilder cityinfo = new StringBuilder();
                string sql2 = string.Format("select count(*) from JW_SendPolice where unit_id='" + companyid + "'");
                DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                cityinfo.Append("[{\"companyid\":\"" + companyid + "\",\"y\":" + dt2.Rows[0][0].ToString() + ",\"color\":\"" + colors[0] + "\",\"tolast\":1},");

                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql2 = string.Format("select count(*) from JW_SendPolice where type <6 and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                    DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                    cityinfo.Append("{\"companyid\":\"" + dt.Rows[i]["Base_Unit_id"].ToString() + "\",\"y\":" + dt3.Rows[0][0].ToString() + ",\"color\":\"" + colors[i % 9] + "\",\"tolast\":0},");
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

        public DataTable GetCityData(string companyid, string toLast)
        {
            //'colors['++']' 
            string sql_last = @"
    if object_id(N'tempdb..#b',N'U') is not null
	    drop table #b
 
select bdl.code,bdl.FullName,bdl.Remark into #b from dbo.Base_DataDictionary bd left join 
	dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='tasktype'

select  '" + companyid + @"'as companyid,aaa.code,aaa.remark,aaa.counts as y,bbb.code as color from 
                                        (
                                         select aa.code,aa.remark,isnull(bb.counts,0)as counts,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY aa.code)% 9)as sort from 
                                            #b aa left join 
	                                        (
		                                        select code,max(remark)as remark,sum(counts)as counts from (
						                                        select a.code,max(a.remark)as remark,count(SendPolice_id)as counts  from #b a 
						                                        right join JW_PoliceApply b on b.tasktype_id = a.code
						                                        right join JW_SendPolice c on c.Object_id=b.apply_id  
							                                        where b.unit_id ='" + companyid + @"' and (c.type=4 or c.type=5)
						                                        group by a.code
						                                        union all
						                                        select a.code,max(a.remark)as remark,count(SendPolice_id)as counts from #b a 
						                                        right join JW_OnDuty b on b.tasktype_id = a.code
						                                        right join JW_SendPolice d on d.Object_id=b.OnDuty_id 
						                                        where d.unit_id ='" + companyid + @"' and (d.type=0 or d.type=1 or d.type=2 or d.type=3)
						                                        group by a.code
			                                        ) tmp1 group by code 
	                                        )bb on aa.code=bb.code where aa.code>0
                                        )aaa 
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='颜色'
                                    )bbb
                                   on aaa.sort = bbb.fullname   order by code";

            string sql = string.Format(@"
                                   select a.companyid,a.y,b.code as color,case when a.companyid='" + companyid + @"' then 1 else 0 end as toLast from (
	                                    select a.Base_Unit_id as companyid,count(Unit_id) as y,convert(varchar(5),ROW_NUMBER()OVER(ORDER BY max(a.sortcode))% 9)as sort
	                                    from base_unit a  
	                                    left join jw_sendpolice b on a. Base_Unit_id=b.Unit_id and type<6
	                                    where Base_Unit_id in (select Base_Unit_id from Base_Unit 
	                                    where Base_Unit_id = '" + companyid + @"' or parent_unit_id in(select Base_Unit_id from Base_Unit where Base_Unit_id = '" + companyid + @"')) 
	                                    group by a.Base_Unit_id 
                                    ) a
                                    left join 
                                    (	
	                                    select bdl.code,bdl.fullname from dbo.Base_DataDictionary bd left join 
	                                    dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='颜色'
                                    )b 
                                    on a.sort = b.fullname
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

        public string GetAllTasktype()
        {
            StringBuilder builder = new StringBuilder();
            string sql = string.Format(@"select bdl.Remark from dbo.Base_DataDictionary bd left join 
	                dbo.Base_DataDictionaryDetail bdl on bd.datadictionaryid= bdl.datadictionaryid where bd.code='tasktype' and bdl.code>'0'   order by bdl.code");
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

        public string GetCityUnitList(string companyid)
        {
            try
            {
                string sql_p = "select * from base_unit where parent_unit_id ='" + companyid + "'";

                DataTable dt_p = Repository().FindTableBySql(sql_p.ToString());
                if (dt_p.Rows.Count == 0)
                {
                    return GetAllTasktype();
                }
                StringBuilder builder = new StringBuilder();
                string sql = string.Format(@"select unit,case when Base_Unit_id='" + companyid + @"' then 1 else 0 end as toLast,parent_unit_id from base_unit 
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
                    if (dt.Rows[i]["parent_unit_id"].ToString() == "e2c79c56-5b58-4c62-b2a9-3bb7492c")
                        builder.Append(dt.Rows[i][0].ToString().Substring(0, dt.Rows[i][0].ToString().Length - 1) + ",");
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

        public string SubmitFormOnDuty(string dutyType, string fjUsers_ids, JW_OnDuty jwOnDuty)
        {
            //1.先处理jw_onduty 
            string onDuty_id = Guid.NewGuid().ToString();
            string sqlJWOnDutyInsert = string.Empty;
            if (dutyType == "1")
            {
                //开始执勤
                sqlJWOnDutyInsert = string.Format(@"insert into JW_OnDuty(
                                                                OnDuty_id,unit_id,PoliceArea_id,adduser_id,adddate,tasktype_id,DutyUser_id,SendDate,type,detail
                                                                ) values(
                                                                @OnDuty_id,@unit_id,@PoliceArea_id,@adduser_id,getdate(),@tasktype_id,@DutyUser_id,@SendDate,@type,@detail
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
                    new SqlParameter("@type",jwOnDuty.type), 
                    new SqlParameter("@detail",jwOnDuty.detail)
                };
                try
                {
                    SqlHelper.ExecuteNonQuery(sqlJWOnDutyInsert, CommandType.Text, parsJWOnDutyInsert);
                }
                catch (Exception exception)
                {
                    return "1|0|值班登记成功，派警失败！";
                }


            }
            else if (dutyType == "2")
            {
                //交接班
            }
            else if (dutyType == "3")
            {
                //结束执勤
            }


            //jw_onduty表的数据插入完了之后，开始处理jw_sendpolice
            string sqlGetUnit = string.Format(@" select * from Base_Unit where Base_Unit_id='{0}' ", jwOnDuty.unit_id);
            string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

            //拿到数据库中当前单位的最大的流水号
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
                //没有选择其他法警
                fjUsers_ids = jwOnDuty.DutyUser_id;
            }
            else
            {
                //选择了其他法警
                fjUsers_ids = jwOnDuty.DutyUser_id + "," + fjUsers_ids;
            }


            //拿到令之后，再添加新的数据
            if (!fjUsers_ids.Contains(","))
            {
                //只选择了一个人
                sb.AppendFormat(
                    @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}',getdate(),'{3}','{4}','{5}','{6}'); "
                    , jwOnDuty.unit_id
                    , unitShotName + "检警派【" + code + "】号"
                    , jwOnDuty.DutyUser_id
                    , "1"
                    , onDuty_id
                    , fjUsers_ids
                    , "0"
                    );
            }
            else
            {
                //选择了多个人
                string[] ids = fjUsers_ids.Split(',');

                for (int i = 0; i < ids.Length; i++)
                {
                    sb.AppendFormat(
                      @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}',getdate(),'{3}','{4}','{5}','{6}'); "
                      , jwOnDuty.unit_id
                      , unitShotName + "检警派【" + (Convert.ToInt32(code) + i) + "】号"
                      , jwOnDuty.DutyUser_id
                      , "1"
                      , onDuty_id
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
                SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
                return "1|1|值班登记成功，派警成功";
            }
            catch (Exception)
            {
                return "1|0|值班登记成功，派警失败";
            }


            return "";
        }


        public string GetOnDutyUsersMsg(string unit_id, string policeArea_id, string policeAreaType)
        {
            string sqlLeaderUser =
                string.Format(
                    @" select top 1 *,bu.RealName from JW_OnDuty jod  join Base_User bu on jod.DutyUser_id=bu.UserId  where jod.unit_id='{0}' and jod.PoliceArea_id='{1}' and jod.type ='{2}' order by jod.SendDate desc ",
                    unit_id, policeArea_id, policeAreaType);

            DataTable dtLeaderUser = SqlHelper.DataTable(sqlLeaderUser, CommandType.Text);
            if (dtLeaderUser.Rows.Count <= 0)
            {
                return "|";
            }
            else
            {
                string sqlFjUser = string.Format(@"  select *,bu.RealName from JW_SendPolice jsp join Base_User bu on jsp.user_id=bu.UserId  where jsp.Object_id='{0}' and jsp.user_id<>'{1}'", dtLeaderUser.Rows[0]["OnDuty_id"], dtLeaderUser.Rows[0]["DutyUser_id"]);

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
        public string GetOnCaseInfo(string type, string unit_id, string user_id)
        {
            string sql = "";
            string strRes = "";
            if (unit_id != "e2c79c56-5b58-4c62-b2a9-3bb7492c")//sheng
            {
                sql = "select unit,Base_Unit_id from Base_unit where Base_Unit_id='" + unit_id + "' or parent_unit_id='" + unit_id + "' order by sortcode";
                DataTable dtunit = SqlHelper.DataTable(sql, CommandType.Text);
                if (dtunit.Rows.Count > 1)
                {
                    for (int i = 0; i <= dtunit.Rows.Count - 1; i++)
                    {
                        sql = "select count(*) from JW_Apply where type='" + type + "' and state in('4') and PoliceArea_id in(select PoliceArea_id from Base_policearea where unit_id='" + dtunit.Rows[i]["Base_Unit_id"].ToString() + "')";
                        DataTable dtLeaderUser = SqlHelper.DataTable(sql, CommandType.Text);
                        if(dtLeaderUser.Rows[0][0].ToString()!="0")
                        {
                            if (strRes == "")
                                strRes = dtunit.Rows[i]["unit"].ToString() + ":" + dtLeaderUser.Rows[0][0].ToString()+"个";
                            else
                                strRes += ";" + dtunit.Rows[i]["unit"].ToString() + ":" + dtLeaderUser.Rows[0][0].ToString() + "个";
                        }
                    }
                    return strRes;
                }
                else//区院不需要
                    return "";
            }
            else//省院不需要
                return "";
        }
        public string getVideoType(string type)
        {
            string sql = "";
            string strRes = "";
            if (type == "1")//质量
            {
                sql = "select count(*) as num,case PictureQuality when 1 then '高清' when 2 then '标清' when 3 then '模糊' when 4 then '其他' end as PictureQuality2 from Base_MonitorChannels group by PictureQuality order by PictureQuality";
                DataTable dtunit = SqlHelper.DataTable(sql, CommandType.Text);
                for (int i = 0; i <= dtunit.Rows.Count - 1; i++)
                {
                    if (strRes == "")
                        strRes = dtunit.Rows[i]["PictureQuality2"].ToString() + "," + dtunit.Rows[i]["num"].ToString();
                    else
                        strRes += "|" + dtunit.Rows[i]["PictureQuality2"].ToString() + "," + dtunit.Rows[i]["num"].ToString();
                }
                return strRes;
            }
            else
                return "0,0";
        }

        public string getundutymonthinfo()
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
                    string sql2 = string.Format("select count(*) from XC_Main where year(xc_datetime)=year(getdate()) and month(xc_datetime)='" + dt.Rows[i]["monthname"].ToString() + "'and xc_res=1  and xc_unit_id in('" + unit_allchildren + "') and xc_type='1'");
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

        public string getdutymonthinfo()
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
                    string sql2 = string.Format("select count(*) from XC_Main where year(xc_datetime)=year(getdate()) and month(xc_datetime)='" + dt.Rows[i]["monthname"].ToString() + "' and xc_unit_id in('" + unit_allchildren + "') and xc_type='1'");
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

        public string getVideocount(int type)
        {
           
           
            try
            {
                string sql = "";
                switch (type)
                {
                    case 1:
                        sql = @"select count(MonitorApplication_id) from Base_MonitorApplication where object_id in(select RoomType_id from Base_RoomType where bigtype in('自侦办案区','刑检办案区'))";
                        break;
                    case 2:
                       
                        break;
                    case 3:
                        break;
                    case 4:
                        sql = @"select count(MonitorApplication_id) from Base_MonitorApplication where object_id in(select RoomType_id from Base_RoomType where bigtype in('控申为民服务中心'))";
                        break;
                    case 5:
                        sql = @"select count(MonitorApplication_id) from Base_MonitorApplication where object_id in(select RoomType_id from Base_RoomType where bigtype in('机关重要场所'))";
                        break;
                    default:
                        break;
                }
                unit_allchildren = ManageProvider.Provider.Current().CompanyId;
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
                else
                    return "0";
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string getcityname()
        {
            string cityinfo = "";
            unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            if (unit_allchildren == "e2c79c56-5b58-4c62-b2a9-3bb7492c")//sheng
            {
                cityinfo = "省院";
                string sql = string.Format("select unit from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                    {
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3);
                    }
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 2);
                }
            }
            else
            {
                string sql = string.Format("select unit from Base_unit where Base_Unit_id='" + unit_allchildren + "' or parent_unit_id in('" + unit_allchildren + "') order by sortcode");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (cityinfo=="")
                        cityinfo = dt.Rows[i]["unit"].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString();
                }
            }
            return cityinfo;
        }

        
        public string getBAZXL1(string type)
        {
            unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            //getchild(unit_allchildren);
            if (unit_allchildren == "e2c79c56-5b58-4c62-b2a9-3bb7492c")//sheng
            {
                string sql = string.Format("select * from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
                try
                {
                    string cityinfo = "";
                    string sql2 = "";
                    if (type == "1")//自侦办案区,改为根据usedetail办案表读取//20180726自侦和刑检合并
                        sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and year(adddate)=year(getdate())  and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    else if (type == "2")//控申接待
                        sql2 = string.Format("select count(*) from CheckIn_LF where  year(check_time)=year(getdate()) and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c'))");
                    else if (type == "3")//其他场所
                        sql2 = string.Format("select count(*) from Base_MonitorApplication a join Base_RoomType t on a.object_id=t.RoomType_id join Base_MonitorChannels c on c.MonitorChannels_id=a.MonitorChannels_id join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id where bigtype='机关重要场所' and s.unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    else if (type == "4")//履职
                        sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2','3') and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    if (sql2 != "")
                    {
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        if (dt2.Rows[0][0].ToString() != "0")
                            cityinfo = "省院," + dt2.Rows[0][0].ToString() + "," + unit_allchildren;
                    }
                    else
                        cityinfo = "省院,0" + "," + unit_allchildren;

                    DataTable dt = Repository().FindTableBySql(sql.ToString());
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (type == "1")//自侦办案区
                            sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and year(adddate)=year(getdate())  and (Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "2")//控申接待
                            sql2 = string.Format("select count(*) from CheckIn_LF where year(check_time)=year(getdate()) and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "3")//其他场所
                            sql2 = string.Format("select count(*) from Base_MonitorApplication a join Base_RoomType t on a.object_id=t.RoomType_id join Base_MonitorChannels c on c.MonitorChannels_id=a.MonitorChannels_id join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id where bigtype='机关重要场所' and s.unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                        else if (type == "4")//履职
                            sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2','3') and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                        if (sql2 != "")
                        {
                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                            {
                                if (dt3.Rows[0][0].ToString() != "0")
                                {
                                    if (cityinfo == "")
                                        cityinfo = dt.Rows[i]["unit"].ToString().Substring(0, 4) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                    else
                                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 4) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                }
                            }
                            else
                            {
                                if (dt3.Rows[0][0].ToString() != "0")
                                {
                                    if (cityinfo == "")
                                        cityinfo = dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                    else
                                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                }

                            }
                        }
                        else
                        {
                            if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                                cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 4) + ",1" + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                            else
                                cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + ",1" + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                        }

                    }
                    if (cityinfo != "")
                        return cityinfo;
                    else
                        return ",0";
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                string sqlcity = string.Format("select Base_Unit_id,unit from Base_unit where Base_Unit_id='" + unit_allchildren + "' or parent_unit_id ='" + unit_allchildren + "' order by code");
                DataTable dtcity = Repository().FindTableBySql(sqlcity.ToString());
                string cityinfo = "";
                for (int j = 0; j <= dtcity.Rows.Count - 1; j++)//shi
                {
                    string sql2 = "";
                    if (type == "1")//自侦办案区
                        sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and year(adddate)=year(getdate()) and Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "'");
                    else if (type == "2")//控申接待
                        sql2 = string.Format("select count(*) from CheckIn_LF where year(check_time)=year(getdate()) and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                    else if (type == "3")//其他场所
                        sql2 = "select count(*) from Base_MonitorApplication a join Base_RoomType t on a.object_id=t.RoomType_id join Base_MonitorChannels c on c.MonitorChannels_id=a.MonitorChannels_id join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id where bigtype='机关重要场所' and s.unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "' ";
                    else if (type == "4")//履职
                        sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2','3') and Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "'");
                    if (sql2 != "")
                    {
                        DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                        if (dt3.Rows[0][0].ToString() != "0")
                        {
                            if (cityinfo == "")
                                cityinfo = dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                            else
                                cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                        }
                    }
                    else
                    {
                        if (cityinfo == "")
                            cityinfo = dtcity.Rows[j]["unit"].ToString() + ",1";
                        else
                            cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + ",1";
                    }

                }


                if (cityinfo != "")
                    return cityinfo;
                else
                    return ",0";
            }

        }

        #region Lwl

        public string getBAZXL1(string type, string unit_allchildren)
        {
          //  unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            //getchild(unit_allchildren);
            if (unit_allchildren == "e2c79c56-5b58-4c62-b2a9-3bb7492c")//sheng
            {
                string sql = string.Format("select * from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
                try
                {
                    string cityinfo = "";
                    string sql2 = "";
                    if (type == "1")//自侦办案区,改为根据usedetail办案表读取//20180726自侦和刑检合并
                        sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and year(adddate)=year(getdate())  and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    else if (type == "2")//控申接待
                        sql2 = string.Format("select count(*) from CheckIn_LF where  year(check_time)=year(getdate()) and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c'))");
                    else if (type == "3")//其他场所
                        sql2 = string.Format("select count(*) from Base_MonitorApplication a join Base_RoomType t on a.object_id=t.RoomType_id join Base_MonitorChannels c on c.MonitorChannels_id=a.MonitorChannels_id join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id where bigtype='机关重要场所' and s.unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    else if (type == "4")//履职
                        sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2','3') and unit_id='e2c79c56-5b58-4c62-b2a9-3bb7492c'");
                    if (sql2 != "")
                    {
                        DataTable dt2 = Repository().FindTableBySql(sql2.ToString());
                        if (dt2.Rows[0][0].ToString() != "0")
                            cityinfo = "省院," + dt2.Rows[0][0].ToString() + "," + unit_allchildren;
                    }
                    else
                        cityinfo = "省院,0" + "," + unit_allchildren;

                    DataTable dt = Repository().FindTableBySql(sql.ToString());
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        if (type == "1")//自侦办案区
                            sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and year(adddate)=year(getdate())  and (Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "2")//控申接待
                            sql2 = string.Format("select count(*) from CheckIn_LF where year(check_time)=year(getdate()) and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "'))");
                        else if (type == "3")//其他场所
                            sql2 = string.Format("select count(*) from Base_MonitorApplication a join Base_RoomType t on a.object_id=t.RoomType_id join Base_MonitorChannels c on c.MonitorChannels_id=a.MonitorChannels_id join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id where bigtype='机关重要场所' and s.unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                        else if (type == "4")//履职
                            sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2','3') and Unit_id in (select Base_Unit_id from Base_Unit where Base_Unit_id='" + dt.Rows[i]["Base_Unit_id"].ToString() + "' or parent_unit_id ='" + dt.Rows[i]["Base_Unit_id"].ToString() + "')");
                        if (sql2 != "")
                        {
                            DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                            if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                            {
                                if (dt3.Rows[0][0].ToString() != "0")
                                {
                                    if (cityinfo == "")
                                        cityinfo = dt.Rows[i]["unit"].ToString().Substring(0, 4) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                    else
                                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 4) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                }
                            }
                            else
                            {
                                if (dt3.Rows[0][0].ToString() != "0")
                                {
                                    if (cityinfo == "")
                                        cityinfo = dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                    else
                                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + "," + dt3.Rows[0][0].ToString() + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                                }

                            }
                        }
                        else
                        {
                            if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                                cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 4) + ",1" + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                            else
                                cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3) + ",1" + "," + dt.Rows[i]["Base_Unit_id"].ToString();
                        }

                    }
                    if (cityinfo != "")
                        return cityinfo;
                    else
                        return ",0";
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                string sqlcity = string.Format("select Base_Unit_id,unit from Base_unit where Base_Unit_id='" + unit_allchildren + "' or parent_unit_id ='" + unit_allchildren + "' order by code");
                DataTable dtcity = Repository().FindTableBySql(sqlcity.ToString());
                string cityinfo = "";
                for (int j = 0; j <= dtcity.Rows.Count - 1; j++)//shi
                {
                    string sql2 = "";
                    if (type == "1")//自侦办案区
                        sql2 = string.Format("select count(*) from JW_Apply where type in('1','2') and year(adddate)=year(getdate()) and Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "'");
                    else if (type == "2")//控申接待
                        sql2 = string.Format("select count(*) from CheckIn_LF where year(check_time)=year(getdate()) and PoliceArea_id in (select PoliceArea_id from Base_PoliceArea where Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "')");
                    else if (type == "3")//其他场所
                        sql2 = "select count(*) from Base_MonitorApplication a join Base_RoomType t on a.object_id=t.RoomType_id join Base_MonitorChannels c on c.MonitorChannels_id=a.MonitorChannels_id join Base_MonitorServer s on s.MonitorServer_id=c.MonitorServer_id where bigtype='机关重要场所' and s.unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "' ";
                    else if (type == "4")//履职
                        sql2 = string.Format("select count(*) from JW_SendPolice where year(SendDate)=year(getdate()) and state in('1','2','3') and Unit_id='" + dtcity.Rows[j]["Base_Unit_id"].ToString() + "'");
                    if (sql2 != "")
                    {
                        DataTable dt3 = Repository().FindTableBySql(sql2.ToString());
                        if (dt3.Rows[0][0].ToString() != "0")
                        {
                            if (cityinfo == "")
                                cityinfo = dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                            else
                                cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + "," + dt3.Rows[0][0].ToString();
                        }
                    }
                    else
                    {
                        if (cityinfo == "")
                            cityinfo = dtcity.Rows[j]["unit"].ToString() + ",1";
                        else
                            cityinfo += "|" + dtcity.Rows[j]["unit"].ToString() + ",1";
                    }

                }


                if (cityinfo != "")
                    return cityinfo;
                else
                    return ",0";
            }

        }
        public string getcityname(string unit_allchildren)
        {
            string cityinfo = "";
            //unit_allchildren = ManageProvider.Provider.Current().CompanyId;
            if (unit_allchildren == "e2c79c56-5b58-4c62-b2a9-3bb7492c")//sheng
            {
                cityinfo = "省院";
                string sql = string.Format("select unit from Base_unit where parent_unit_id in('e2c79c56-5b58-4c62-b2a9-3bb7492c') order by sortcode");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (dt.Rows[i]["unit"].ToString() == "连云港市院")
                    {
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 3);
                    }
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString().Substring(0, 2);
                }
            }
            else
            {
                string sql = string.Format("select unit from Base_unit where Base_Unit_id='" + unit_allchildren + "' or parent_unit_id in('" + unit_allchildren + "') order by sortcode");
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (cityinfo == "")
                        cityinfo = dt.Rows[i]["unit"].ToString();
                    else
                        cityinfo += "|" + dt.Rows[i]["unit"].ToString();
                }
            }
            return cityinfo;
        }
        #endregion

    }
}
//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.DataAccess;
using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System;

namespace LeaRun.Business
{
    /// <summary>
    /// Base_Unit
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.06 09:43</date>
    /// </author>
    /// </summary>
    public class Base_UnitBll : RepositoryFactory<Base_Unit>
    {
        /// <summary>
        /// ��ȡ��˾�б�
        /// </summary>
        /// <returns></returns>
        public List<Base_Unit> GetList()
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

        public List<Base_Unit> GetListNologin()
        {
            StringBuilder WhereSql = new StringBuilder();

            WhereSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString());
        }

        public List<Base_Unit> GetUnit(string unitid)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder WhereSql = new StringBuilder();

            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //WhereSql.Append(" AND ( [Base_Unit_id] IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                //WhereSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //WhereSql.Append(" ) )");
            }
            if (!string.IsNullOrEmpty(unitid))
            {
                WhereSql.Append(" AND Base_Unit_id = @unitid");
                parameter.Add(DbFactory.CreateDbParameter("@unitid", unitid));
            }
            WhereSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }

        public List<Base_Unit> GetUnitWithNologin(string unitid)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder WhereSql = new StringBuilder();


            if (!string.IsNullOrEmpty(unitid))
            {
                WhereSql.Append(" AND Base_Unit_id = @unitid");
                parameter.Add(DbFactory.CreateDbParameter("@unitid", unitid));
            }
            WhereSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
        /* begin lwl*/
        public List<Base_Unit> GetFirstUnitList()
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder WhereSql = new StringBuilder();
            WhereSql.Append("  and parent_unit_id IN('0','e2c79c56-5b58-4c62-b2a9-3bb7492c')");
            WhereSql.Append(" ORDER BY Base_Unit_id DESC ");
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
        public int GetSortCode(string base_unit_id)
        {
            string strSql = "select  * from base_unit where base_unit_id ='" + base_unit_id + "'";
             DataTable  dt= DataFactory.Database().FindTableBySql(strSql);
             if (dt.Rows.Count > 0)
             { 
               DataRow r =dt.Rows[0];
               int sortcode = int.Parse(r["sortcode"].ToString());
               return sortcode;
             }
            //List<DbParameter> parameter = new List<DbParameter>();
            //Base_Unit m= Repository().FindList(sqlStr.ToString(), parameter.ToArray())[0];
            //int sortcode = Convert.ToInt32(m.sortcode);
            return 0;
        }


        /// <summary>
        /// ��ͼһ���б�
        /// </summary>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        public List<ChartMapData> GetFristListMap(string MessageType)
        {
            List<ChartMapData> list = new List<ChartMapData>();
            /*
             ��ȡʡԺ��Ϣ
             ����Ĭ�ϰ참����
             */
            List<ChartMapData> list1 = GetMapListInof(MessageType, "'0'");
            List<ChartMapData> list2 = GetMapListInof(MessageType, "'e2c79c56-5b58-4c62-b2a9-3bb7492c'");

            /*��ȡ���еؼ�������λ*/
            DataTable allCityUnit_dt = GetSubUnitTable("'e2c79c56-5b58-4c62-b2a9-3bb7492c'");
            foreach (DataRow item in allCityUnit_dt.Rows)
            {
                string unitId = item["unitId"].ToString();
                string unitname = item["name"].ToString();
                int sortcode = int.Parse(item["sortcode"].ToString());
                /*�ؼ�������λ�������¼�ͳ������*/
                int subNum = GetSubUnitTongJi(MessageType, "'" + unitId + "'");
                ChartMapData obj = list2.Find(p => p.id == unitId);
                if (obj != null)
                {
                    for (int i = 0; i < list2.Count; i++)
                    {
                        var cityunitId = list2[i].id;
                        if (unitId == cityunitId)
                        {
                            list2[i].value = (int.Parse(list2[i].value) + subNum).ToString();
                            break;
                        }
                    }
                }
                else
                {
                    ChartMapData newobj = new ChartMapData();
                    newobj.id = unitId;
                    newobj.name = unitname;
                    newobj.sortcode = sortcode;
                    newobj.value = subNum.ToString();
                    list2.Add(newobj);
                }


            }

            list.Clear();
            /**/
            foreach (var item in list1)
            {
                list.Add(item);
            }

            foreach (var item in list2)
            {
                list.Add(item);
            }
            list = PaiXun(list);
            return list;
        }

        public List<ChartMapData> PaiXun(List<ChartMapData> list)
        {
            ChartMapData obj =new ChartMapData();
            for (int i = 0; i < list.Count; i++)
            {
                
                for (int j = i+1; j < list.Count; j++)
                {
                   
                    if (list[i].sortcode > list[j].sortcode)
                    {
                        obj = list[i];
                        list[i] = list[j];
                        list[j] = obj;
                    }

                }
            }
            return list;
        }
        /// <summary>
        /// ��ͼ�����б�
        /// </summary>
        /// <param name="MessageType"></param>
        /// <param name="parentUnit_id"></param>
        /// <returns></returns>
        public List<ChartMapData> GetSubListMap(string MessageType, string parentUnit_id)
        {
            List<ChartMapData> list = new List<ChartMapData>();
            List<ChartMapData> subList = GetMapListInof(MessageType, parentUnit_id);
            foreach (var item in subList)
            {
                list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// ��Ӧ��ǰ��ͼչʾ���б����Ϣ
        /// </summary>
        /// <param name="MessageType"></param>
        /// <param name="parentUnit_id"></param>
        /// <returns></returns>
        public List<ChartMapData> GetMapListInof(string MessageType, string parentUnit_id)
        {
            DataTable dt = new DataTable();
            string bigtype = "'����참��','�̼�참��'";
            StringBuilder strSql = new StringBuilder();
            switch (MessageType)
            {
                case "���꣨Ϊ��":
                    {
                        bigtype = "'����Ϊ���������'";
                        break;
                    }
                case "����Ϊ���������":
                    {
                        bigtype = "'����참��','�̼�참��'";
                        break;
                    }
                case "���ذ���":
                    {
                        bigtype = "'������Ҫ����'";
                        break;
                    }
            }

            strSql.Append("SELECT max(u.sortcode) as  sortcode,MAX(u.Base_Unit_id) AS unitId, MAX(SUBSTRING(u.unit,0,LEN(u.unit))) AS name,COUNT(u.Base_Unit_id) AS value ");
            strSql.Append("FROM dbo.Base_Unit AS u , dbo.Base_MonitorServer AS s,");
            strSql.Append("dbo.Base_MonitorChannels AS c ,dbo.Base_MonitorApplication AS a,");
            strSql.Append(" Base_RoomType AS t ");
            strSql.Append("  WHERE u.Base_Unit_id =s.Unit_id  AND s.MonitorServer_id=c.MonitorServer_id ");
            strSql.Append("AND c.MonitorChannels_id=a.MonitorChannels_id  AND t.RoomType_id=a.Object_id ");
            strSql.Append(" AND t.bigtype IN (" + MessageType + ") ");
            strSql.Append(" AND u.parent_unit_id IN(" + parentUnit_id + ") ");
            strSql.Append(" GROUP BY u.Base_Unit_id ");
            strSql.Append(" ORDER BY u.Base_Unit_id ");

            dt = SqlHelper.DataTable(strSql.ToString(), CommandType.Text);
            List<ChartMapData> list = new List<ChartMapData>();
            dt.DefaultView.Sort = "sortcode asc";
            dt = dt.DefaultView.ToTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ChartMapData m = new ChartMapData();
                DataRow r = dt.Rows[i];
                m.id = r["unitId"].ToString();
                m.name = r["name"].ToString();
                m.value = r["value"].ToString();
                m.sortcode = int.Parse(r["sortcode"].ToString());
                list.Add(m);
            }

            return list;
        }
        /// <summary>
        /// lwlѲ�������ͼ��Ϣ
        /// </summary>
        /// <param name="parentUnit_id"></param>
        /// <returns></returns>
        public DataTable GetMapInofXunChaWenti(string parentUnit_id)
        {
            Base_MapBll bll = new Base_MapBll();
            DataTable dt = new DataTable();
            DataTable allCityUnit_dt = GetSubUnitTable(parentUnit_id);
            dt = allCityUnit_dt;
            dt.Columns.Add("value", typeof(Int32));
            foreach (DataRow item in dt.Rows)
            {
                string unitId = item["unitId"].ToString();
                string unitname = item["name"].ToString();
                int value = bll.GetXunChaWenTiNum(unitId, 1);
                item["value"] = value;
                
            }
            return dt;
        }
        /// <summary>
        /// map��ͼʹ�� 
        /// һ���Ի�ȡ��ͼ�����ȫ����Ϣ
        /// </summary>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        public DataTable GetMapInof(string MessageType, string parentUnit_id)
        {
            DataTable dt = new DataTable();
            string bigtype = "'����참��','�̼�참��'";
            StringBuilder strSql = new StringBuilder();
            switch (MessageType)
            {
                case "���꣨Ϊ��":
                    {
                        bigtype = "'����Ϊ���������'";
                        break;
                    }
                case "����Ϊ���������":
                    {
                        bigtype = "'����참��','�̼�참��'";
                        break;
                    }
                case "���ذ���":
                    {
                        bigtype = "'������Ҫ����'";
                        break;
                    }
            }

            strSql.Append("SELECT max(u.sortcode) as  sortcode,MAX(u.Base_Unit_id) AS unitId, MAX(SUBSTRING(u.unit,0,LEN(u.unit))) AS name,COUNT(u.Base_Unit_id) AS value ");
            strSql.Append("FROM dbo.Base_Unit AS u , dbo.Base_MonitorServer AS s,");
            strSql.Append("dbo.Base_MonitorChannels AS c ,dbo.Base_MonitorApplication AS a,");
            strSql.Append(" Base_RoomType AS t ");
            strSql.Append("  WHERE u.Base_Unit_id =s.Unit_id  AND s.MonitorServer_id=c.MonitorServer_id ");
            strSql.Append("AND c.MonitorChannels_id=a.MonitorChannels_id  AND t.RoomType_id=a.Object_id ");
            strSql.Append(" AND t.bigtype IN (" + MessageType + ") ");
            if (parentUnit_id != "")
            {
                strSql.Append(" AND u.parent_unit_id IN(" + parentUnit_id + ") ");
            }

            strSql.Append(" GROUP BY u.Base_Unit_id ");
            strSql.Append(" ORDER BY u.Base_Unit_id ");

            dt = SqlHelper.DataTable(strSql.ToString(), CommandType.Text);
            /*��ȡ���еؼ�������λ*/
            DataTable allCityUnit_dt = GetSubUnitTable("'e2c79c56-5b58-4c62-b2a9-3bb7492c'");

            foreach (DataRow item in allCityUnit_dt.Rows)
            {
                string unitId = item["unitId"].ToString();
                string unitname = item["name"].ToString();
                /*�ؼ�������λ�������¼�ͳ������*/
                int subNum = GetSubUnitTongJi(MessageType, "'" + unitId + "'");
                if (subNum > 0)
                {
                    if (UnitIsExit(dt, unitId))
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DataRow r = dt.Rows[i];
                            string cityunitId = r["unitId"].ToString();
                            if (unitId == cityunitId)
                            {
                                dt.Rows[i]["value"] = int.Parse(r["value"].ToString()) + subNum;
                            }
                        }
                    }
                    else
                    {
                        DataRow r = dt.NewRow();
                        r["unitId"] = unitId;
                        r["name"] = unitname;
                        r["value"] = subNum;
                        dt.Rows.Add(r);
                    }

                }
                // item["value"] = int.Parse(item["value"].ToString()) +subNum;
            }
            dt.DefaultView.Sort = "unitId asc ";
            dt = dt.DefaultView.ToTable();
            return dt;
        }

        #region lwl
        
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">���ϲ�ѯ��datatable</param>
        /// <param name="Base_Unit_id"></param>
        /// <returns></returns>
        private bool UnitIsExit(DataTable dt, string Base_Unit_id)
        {
            bool flag = false;
            foreach (DataRow item in dt.Rows)


                if (item["unitId"].ToString().Trim() == Base_Unit_id)
                {
                    flag = true;
                    break;
                }
            return flag;
        }

        /// <summary>
        /// ��ȡ��һ��������λͳ����Ϣ
        /// </summary>
        /// <returns></returns>
        public int GetSubUnitTongJi(string MessageType, string parentUnit_id)
        {
            string bigtype = "'����참��','�̼�참��'";
            StringBuilder strSql = new StringBuilder();
            switch (MessageType)
            {
                case "���꣨Ϊ��":
                    {
                        bigtype = "'����Ϊ���������'";
                        break;
                    }
                case "����Ϊ���������":
                    {
                        bigtype = "'����참��','�̼�참��'";
                        break;
                    }
                case "���ذ���":
                    {
                        bigtype = "'������Ҫ����'";
                        break;
                    }
            }

            DataTable dt = new DataTable();
            strSql.Append("SELECT MAX(u.Base_Unit_id) AS unitId, MAX(SUBSTRING(u.unit,0,LEN(u.unit))) AS name,COUNT(u.Base_Unit_id) AS value ");
            strSql.Append("FROM dbo.Base_Unit AS u , dbo.Base_MonitorServer AS s,");
            strSql.Append("dbo.Base_MonitorChannels AS c ,dbo.Base_MonitorApplication AS a,");
            strSql.Append(" Base_RoomType AS t ");
            strSql.Append("  WHERE u.Base_Unit_id =s.Unit_id  AND s.MonitorServer_id=c.MonitorServer_id ");
            strSql.Append("AND c.MonitorChannels_id=a.MonitorChannels_id  AND t.RoomType_id=a.Object_id ");
            strSql.Append(" AND t.bigtype IN (" + MessageType + ") ");
            if (parentUnit_id != "")
            {
                strSql.Append(" AND u.parent_unit_id IN(" + parentUnit_id + ") ");
            }

            strSql.Append(" GROUP BY u.parent_unit_id ");
            dt = SqlHelper.DataTable(strSql.ToString(), CommandType.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["value"].ToString());
            }
            else
            {
                return 0;
            }


        }
        /// <summary>
        /// ��ȡ����������������λ
        /// </summary>
        /// <param name="parent_unit_id"></param>
        /// <returns></returns>
        public DataTable GetSubUnitTable(string parent_unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            DataTable dt = new DataTable();
            strSql.Append(" SELECT  sortcode,Base_Unit_id AS unitId ,SUBSTRING(unit, 0, LEN(unit)) AS name ");
            strSql.Append(" FROM    Base_Unit ");
            strSql.Append(" WHERE   parent_unit_id IN ( " + parent_unit_id + " ) ");
            strSql.Append(" order by sortcode ");
            dt = SqlHelper.DataTable(strSql.ToString(), CommandType.Text);
            return dt;
        }
        /// <summary>
        /// lwl��ȡ��ͬͨ����ͳ����Ϣ
        /// </summary>
        /// <returns></returns>
        public List<TongDaoInfor> GetTongdaoList()
        {
            /*����Ϊ���������=���꣨Ϊ��
             (����참��+�̼�참��)= �참����
              ������Ҫ����=���ذ���*/
            StringBuilder strSql = new StringBuilder();
            DataTable dt = new DataTable();
            List<TongDaoInfor> list = new List<TongDaoInfor>();

            strSql.Append("  SELECT t.bigtype ,COUNT(u.Base_Unit_id) AS num   ");
            strSql.Append("FROM dbo.Base_Unit AS u , dbo.Base_MonitorServer AS s, ");
            strSql.Append("  dbo.Base_MonitorChannels AS c ,dbo.Base_MonitorApplication AS a, ");
            strSql.Append(" Base_RoomType AS t ");
            strSql.Append(" WHERE u.Base_Unit_id =s.Unit_id  AND s.MonitorServer_id=c.MonitorServer_id ");
            strSql.Append("  AND c.MonitorChannels_id=a.MonitorChannels_id  AND t.RoomType_id=a.Object_id ");
            strSql.Append(" AND t.bigtype IN ('����참��','�̼�참��','����Ϊ���������','������Ҫ����','����') ");
            strSql.Append(" GROUP BY t.bigtype  ");
            strSql.Append("  ORDER BY t.bigtype ");
            dt = SqlHelper.DataTable(strSql.ToString(), CommandType.Text);
            /*�참����*/
            TongDaoInfor bananzhongxin = new TongDaoInfor();
            /*���꣨Ϊ��*/
            TongDaoInfor shenkongweimin = new TongDaoInfor();
            /*���ذ���*/
            TongDaoInfor jiguanbanan = new TongDaoInfor();
            /*����*/
            TongDaoInfor jiansuo = new TongDaoInfor();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    string bigtype = item["bigtype"].ToString();
                    if (bigtype == "����Ϊ���������")
                    {
                        shenkongweimin.Num = item["num"].ToString();
                        shenkongweimin.TypeName = "���꣨Ϊ��";
                        list.Add(shenkongweimin);
                    }
                    if (bigtype == "����참��" || bigtype == "�̼�참��")
                    {
                        if (Exit_TongDao(list, "�참����"))
                        {
                            bananzhongxin = Get_TongDai(list, "�참����");
                            bananzhongxin.Num = (int.Parse(bananzhongxin.Num) + int.Parse(item["num"].ToString())).ToString();
                            list.Add(bananzhongxin);
                        }
                        else
                        {
                            bananzhongxin.TypeName = "�참����";
                            bananzhongxin.Num = item["num"].ToString();
                            list.Add(bananzhongxin);
                        }
                    }
                    if (bigtype == "������Ҫ����")
                    {
                        jiguanbanan.Num = item["num"].ToString();
                        jiguanbanan.TypeName = "���ذ���";
                        list.Add(jiguanbanan);
                    }
                    if (bigtype == "����")
                    {
                        jiansuo.Num = item["num"].ToString();
                        jiansuo.TypeName = "����";
                        list.Add(jiansuo);
                    }
                }
            }
            return list;
        }

        private TongDaoInfor Get_TongDai(List<TongDaoInfor> list, string str)
        {
            foreach (TongDaoInfor item in list)
            {
                if (item.TypeName == str)
                {
                    return item;
                }
            }
            return null;
        }
        private bool Exit_TongDao(List<TongDaoInfor> list, string str)
        {
            foreach (TongDaoInfor item in list)
            {
                if (item.TypeName == str)
                {
                    return true;
                }
            }
            return false;
        }
        /*end begin*/


        public DataTable GetList_quanxian(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (unit_id != Share.UNIT_ID_JS)//��ʡԺѡ�񱾵�λ���¼���λ
            {
                strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --����
                                                    u.unit as name ,	          --��λ����
                                                    '0' AS parent_id ,--�ϼ���λID
                                                    uf.unit as parent_name,		  --�ϼ�����
                                                    u.sortcode ,				  --�����ֶ�
                                                    u.code,   					  --Code�ֶ�
                                                    'Unit' AS Sort                --�����ֶ�
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id
                                        ) T WHERE 1=1 ");

                if (!string.IsNullOrEmpty(unit_id))
                {
                    strSql.Append(" AND t.dep_id in (select base_unit_id from base_unit where base_unit_ID=@unit_id or parent_unit_id =@unit_id) ");
                    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", ManageProvider.Provider.Current().CompanyId));
                }
            }
            else//ʡԺѡ��ȫ����λ
            {
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
                if (!string.IsNullOrEmpty(unit_id))
                {

                }
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

        public List<Base_Unit> GetChildUnit(string unitid)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder WhereSql = new StringBuilder();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //WhereSql.Append(" AND ( [Base_Unit_id] IN ( SELECT ResourceId FROM Base_DataScopePermission WHERE");
                //WhereSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //WhereSql.Append(" ) )");
            }
            if (!string.IsNullOrEmpty(unitid))
            {
                WhereSql.Append(" AND parent_unit_id = @unitid");
                parameter.Add(DbFactory.CreateDbParameter("@unitid", unitid));
            }
            WhereSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
        public List<Base_Unit> GetChildUnitWithNologin(string unitid)
        {
            List<DbParameter> parameter = new List<DbParameter>();
            StringBuilder WhereSql = new StringBuilder();

            if (!string.IsNullOrEmpty(unitid))
            {
                WhereSql.Append(" AND parent_unit_id = @unitid");
                parameter.Add(DbFactory.CreateDbParameter("@unitid", unitid));
            }
            WhereSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }

        public string CreateCityFormTableNologin(string unitid)
        {
            List<Base_Unit> ListData = this.GetUnitWithNologin(unitid);
            StringBuilder FormTable = new StringBuilder();
            foreach (Base_Unit entity in ListData)
            {
                FormTable.Append(" <a onclick=\"SetTopLevel('" + entity.Base_Unit_id + "','" + entity.unit + "')\"  href='#' style='font-weight:bold;'>" + entity.unit + "</a><br> ");
            }

            ListData = this.GetChildUnitWithNologin(unitid);
            foreach (Base_Unit entity in ListData)
            {

                if ((entity.unit.ToString()).Length == 4)
                {
                    FormTable.Append(" <a onclick=\"SetNextLevel('" + entity.Base_Unit_id + "','" + entity.unit + "')\" class='input_city' href='#' style='padding-right:12px;'>" + entity.unit + "</a>");
                }
                else
                {
                    FormTable.Append(" <a onclick=\"SetNextLevel('" + entity.Base_Unit_id + "','" + entity.unit + "')\" class='input_city' href='#'>" + entity.unit + "</a> ");
                }

            }

            return FormTable.ToString();
        }
        public string CreateCityFormTable(string unitid)
        {
            List<Base_Unit> ListData = this.GetUnit(unitid);
            StringBuilder FormTable = new StringBuilder();
            foreach (Base_Unit entity in ListData)
            {
                FormTable.Append(" <a onclick=\"SetTopLevel('" + entity.Base_Unit_id + "','" + entity.unit + "')\"  href='#' style='font-weight:bold;'>" + entity.unit + "</a><br> ");
            }

            ListData = this.GetChildUnit(unitid);
            foreach (Base_Unit entity in ListData)
            {

                if ((entity.unit.ToString()).Length == 4)
                {
                    FormTable.Append(" <a onclick=\"SetNextLevel('" + entity.Base_Unit_id + "','" + entity.unit + "')\" class='input_city' href='#' style='padding-right:12px;'>" + entity.unit + "</a>");
                }
                else
                {
                    FormTable.Append(" <a onclick=\"SetNextLevel('" + entity.Base_Unit_id + "','" + entity.unit + "')\" class='input_city' href='#'>" + entity.unit + "</a> ");
                }

            }

            return FormTable.ToString();
        }
    }
    /// <summary>
    /// ��Ϣͨ��
    /// </summary>
    public class TongDaoInfor
    {
        public string TypeName { get; set; }
        public string Num { get; set; }
    }
    /*lwl*/
    public class ChartMapData
    {
        public string name { get; set; }
        public string value { get; set; }
        public string id { get; set; }
        public int sortcode { get; set; }
    }
    /*lwl*/
}
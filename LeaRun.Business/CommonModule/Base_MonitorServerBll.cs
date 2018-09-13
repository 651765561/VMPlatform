//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace LeaRun.Business
{
    public class Base_MonitorServerBll : RepositoryFactory<Base_MonitorServer>
    {
        public DataTable GetMonitorTypeList()
        {
            string sql = string.Format(@" select DataDictionaryDetailId,d.Code  from 
Base_DataDictionaryDetail d
join Base_DataDictionary m on d.DataDictionaryId=m.DataDictionaryId 
where m.Code='设备类别' ");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int AddMonitorServer(Base_MonitorServer monitorServer, string type)
        {
            StringBuilder sb = new StringBuilder();
            if (type == "add")
            {
                string sql = string.Format(@"
insert into 
Base_MonitorServer(
MonitorServer_id
,ServerName
,ServerCode
,ServerIP
,ServerUser
,ServerPSW
,ServerPort
,type
,Unit_id
,domain_id
,PlatKind
)
values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') "
                    , monitorServer.MonitorServer_id
                    , monitorServer.ServerName
                    , monitorServer.ServerCode
                    , monitorServer.ServerIP
                    , monitorServer.ServerUser
                    , monitorServer.ServerPSW
                    , monitorServer.ServerPort
                    , monitorServer.type
                    , monitorServer.Unit_id
                    , monitorServer.domain_id
                    , monitorServer.PlatKind
                    );
                sb.Append(sql);
            }
            else
            {
                string sql = string.Format(@"
update Base_MonitorServer 
set 
ServerName='{0}'
,ServerCode='{1}'
,ServerIP='{2}'
,ServerUser='{3}'
,ServerPSW='{4}'
,ServerPort='{5}'
,type='{6}'
,Unit_id='{7}'
,domain_id='{8}'
,PlatKind='{10}'
where MonitorServer_id='{9}'
"
                    , monitorServer.ServerName
                    , monitorServer.ServerCode
                    , monitorServer.ServerIP
                    , monitorServer.ServerUser
                    , monitorServer.ServerPSW
                    , monitorServer.ServerPort
                    , monitorServer.type
                    , monitorServer.Unit_id
                    , monitorServer.domain_id
                    , monitorServer.MonitorServer_id
                    , monitorServer.PlatKind
 );
                sb.Append(sql);
            }

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

        public string GridPageDeviceJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                string sqlLoadAll = string.Empty;
                if (!string.IsNullOrEmpty(unit_id))
                {
                    sqlWhere = string.Format(" where ms.Unit_id='{0}'", unit_id);
                    sqlLoadAll = string.Format(" select * from Base_MonitorServer where Unit_id='{0}' ", unit_id);
                }
                else
                {
                    sqlLoadAll = string.Format(" select * from Base_MonitorServer ");
                }

                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" 
select * from ( 
select ROW_NUMBER() over(order by MonitorServer_id) rowNumber 
,ms.MonitorServer_id
,ms.ServerName
,ms.ServerCode
,ms.ServerIP
,ms.ServerUser
,ms.ServerPSW
,ms.ServerPort 
,ms.PlatKind 
,mt.code type 
,u.unit unit_id 
from Base_MonitorServer ms 
join Base_DataDictionaryDetail mt on ms.type=mt.DataDictionaryDetailId
join Base_Unit u on ms.Unit_id=u.Base_Unit_id  {4}
) as a  
where rowNumber between {0} and {1}  
order by {2} {3}  "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , sqlWhere
                        );
                DataTable dt = Repository().FindTableBySql(sqlLoad);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dtAll.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                    page = jqgridparam.page, //当前页码
                    records = dtAll.Rows.Count, //总记录数
                    costtime = CommonHelper.TimerEnd(watch), //查询消耗的毫秒数
                    rows = dt
                };
                return JsonData.ToJson();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int DeleteDevice(string keyValue)
        {
            //删除设备之前判断该设备中有没有通道
            string sqlCheckChannel = string.Format(@"
select count(*) from 
Base_MonitorChannels
where MonitorServer_id='{0}'
"
                , keyValue
                );

            int channelCount = Repository().FindCountBySql(sqlCheckChannel);
            if (channelCount > 0)
            {
                return -100;//表示当前设备中有通道，不予以删除
            }

            StringBuilder sb = new StringBuilder();
            string sql = string.Format(@"
delete 
Base_MonitorServer
where MonitorServer_id='{0}' 
"
                , keyValue
                );
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

        public string GetUnitByServerId(string serverId)
        {
            string sql = string.Format(@"
                                        select u.unit from Base_MonitorServer ms
                                        join Base_Unit u on ms.Unit_id=u.Base_Unit_id
                                        where MonitorServer_id='{0}'
                                        "
                                        , serverId);
            try
            {
                DataTable dt = Repository().FindTableBySql(sql.ToString());
                return dt.Rows[0]["unit"].ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
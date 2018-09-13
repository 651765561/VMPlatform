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
    public class Base_MonitorChannelsBll : RepositoryFactory<Base_MonitorChannels>
    {
        public int AddMonitorChannel(Base_MonitorChannels channel)
        {
            StringBuilder sb = new StringBuilder();
            string sql = string.Empty;
            if (channel.MonitorChannels_id == string.Empty)
            {
                sql = string.Format(@"
insert into 
Base_MonitorChannels(
MonitorChannels_id
,MonitorServer_id
,ChannelName
,ChannelCode
,Channels
,State
,PictureQuality
,manufactory
)values('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}') "
                    , Guid.NewGuid().ToString()
                    , channel.MonitorServer_id
                    , channel.ChannelName
                    , channel.ChannelCode
                    , channel.Channels
                    ,channel.State
                    ,channel.PictureQuality
                    , channel.manufactory
                    );
            }
            else
            {
                sql = string.Format(@"update Base_MonitorChannels set 
                             MonitorServer_id='{0}'
                            ,ChannelName='{1}'
                            ,ChannelCode='{2}'
                            ,Channels='{3}'
                            ,State={4}
                            ,PictureQuality={5}
                            ,manufactory='{7}'
                        where 
                            MonitorChannels_id='{6}'"
                    , channel.MonitorServer_id
                    , channel.ChannelName
                    , channel.ChannelCode
                    , channel.Channels
                    ,channel.State
                    ,channel.PictureQuality
                    , channel.MonitorChannels_id
                     , channel.manufactory
                    );
            }

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

        public string GridPageChannelsJson(string ParameterJson, string monitorserver_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlWhere = string.Empty;
                string sqlLoadAll = string.Empty;
                if (!string.IsNullOrEmpty(monitorserver_id))
                {
                    sqlWhere = string.Format(" where mc.MonitorServer_id='{0}' ", monitorserver_id);
                    sqlLoadAll = string.Format(" select * from Base_MonitorChannels where MonitorServer_id='{0}' ", monitorserver_id);
                }
                else
                {
                    sqlLoadAll = string.Format(" select * from Base_MonitorChannels ");
                }

                DataTable dtAll = Repository().FindTableBySql(sqlLoadAll);
                string sqlLoad =
                    string.Format(
                        @" 
select * from ( 
select ROW_NUMBER() over(order by channelname) rowNumber 
,mc.MonitorChannels_id 
,mc.MonitorServer_id 
,ms.ServerName monitorserver_name 
,mc.ChannelName 
,mc.ChannelCode 
,mc.Channels 
,mc.State
,mc.PictureQuality
,mc.manufactory
from Base_MonitorChannels mc 
join Base_MonitorServer ms on mc.MonitorServer_id=ms.MonitorServer_id 
 {4} 
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

        public int DeleteChannel(string keyValue)
        {
            //删除通道之前先判断设备应用里是否有关联
            string sqlCheckMonitorApplication = string.Format(@"
select count(*) from 
Base_MonitorApplication
where MonitorChannels_id='{0}'
"
                , keyValue
                );

            int applicationCount = Repository().FindCountBySql(sqlCheckMonitorApplication);
            if (applicationCount > 0)
            {
                return -100;//表示当前设备中有通道，不予以删除
            }
            StringBuilder sb = new StringBuilder();
            string sql = string.Format(@"
delete 
Base_MonitorChannels
where MonitorChannels_id='{0}' 
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



        public string SetServer(string channelId)
        {
            string sql = string.Format(@"select s.* from Base_MonitorChannels c
join Base_MonitorServer s on c.MonitorServer_id=s.MonitorServer_id 
where MonitorChannels_id='{0}'", channelId);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count <= 0)
                {
                    return "|";
                }
                else
                {
                    return dt.Rows[0]["MonitorServer_id"] + "|" + dt.Rows[0]["ServerName"];
                }
            }
            catch (Exception)
            {
                return "|";
            }
        }
    }
}
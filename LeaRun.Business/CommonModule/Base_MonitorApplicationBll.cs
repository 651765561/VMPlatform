using LeaRun.DataAccess;
using LeaRun.Utilities;
using LeaRun.Entity.CommonModule;
using LeaRun.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LeaRun.Business
{
    public partial class Base_MonitorApplicationBll : RepositoryFactory<Base_MonitorApplication>
    {
        public DataTable GetDeviceInfo(string id)
        {
            StringBuilder strSql = new StringBuilder();
//            strSql.Append(@"select d.code,channelname,channelcode,channels,servername,servercode,serveruser,serverip,serverpsw,serverport,domain_id,streamserverip from base_monitorapplication a left join dbo.Base_MonitorChannels b on a.MonitorChannels_id=b.MonitorChannels_id
//                                left join base_monitorserver c on c.MonitorServer_id=b.MonitorServer_id
//                                left join base_unit bt on c.Unit_id=bt.Base_Unit_id 
//                                left join dbo.Base_DataDictionaryDetail d on d.DataDictionaryDetailId=c.type where MonitorApplication_id=@MonitorApplication_id  ");
            strSql.Append(@"select d.code,channelname,channelcode,channels,servername,servercode,serveruser,serverip,serverpsw,serverport,domain_id,streamserverip,
                    jy.unitname,jy.depname,cc.name as casename ,jy.username,(PlatKind+MonitorApplication_id)as MonitorApplication_id,manufactory from base_monitorapplication a left join dbo.Base_MonitorChannels b on a.MonitorChannels_id=b.MonitorChannels_id
                                left join base_monitorserver c on c.MonitorServer_id=b.MonitorServer_id
                                left join base_unit bt on c.Unit_id=bt.Base_Unit_id 
                                left join dbo.Base_DataDictionaryDetail d on d.DataDictionaryDetailId=c.type 
                                left join JW_usedetail u on u.room_id =a.room_id and u.isend =0 
                                left join JW_Apply  jy on jy.apply_id = u.apply_id
                                left join case_caseinfo cc on cc.case_id = jy.case_Id
                                where PlatKind+MonitorApplication_id=@MonitorApplication_id ");
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@MonitorApplication_id", id));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        public DataTable GetListType(string unitid,string type)
        {
            StringBuilder strSql = new StringBuilder();
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{

            //}
            if (!string.IsNullOrEmpty(unitid))
            {
                strSql.Append(@"
                    select RoomType_id as id,'0' as parentid,case when bb.name is null then aa.name else bb.name end as name,'bkind'as type,type as roomtype from base_roomtype aa
                    left join (select b.Name as name1,b.Name+'('+CONVERT(varchar(20),COUNT(b.Name))+')' as name  from Base_MonitorApplication a                                   
								 left join Base_RoomType b on a.Object_id=b.RoomType_id
                                 left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                                 left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id                   
                                 where br.Unit_id =@unit_id
                                 group by b.Name)  bb on bb.name1=aa.Name
                     where aa.action='"+type+@"' 
                    union all
                    select bm.room_id as id ,bm.RoomType_id as parentid,bm.RoomName as name,'aroom' as [type],b.type as roomtype from Base_MonitorApplication a left join Base_RoomType b on a.Object_id=b.RoomType_id
                    left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                    left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id
                    left join base_room bm on bm.room_id = a.room_id
                    where br.Unit_id =@unit_id group by b.type, bm.room_id,bm.RoomName,bm.RoomType_id
                    union all
                    select a.MonitorApplication_id as id,case when (a.room_id ='' or a.room_id is null) then object_id else room_id end as parentid ,bs.ChannelName as name,'channel' as type,b.type as roomtype from Base_MonitorApplication a left join Base_RoomType b on a.Object_id=b.RoomType_id
                    left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                    left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id
                    where Unit_id =@unit_id order by roomtype,name");

                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unitid));

                return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
            }
            else
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }

        public DataTable GetList(string unitid)
        {
            StringBuilder strSql = new StringBuilder();
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{

            //}
            if (!string.IsNullOrEmpty(unitid))
            {
                strSql.Append(@"
                    select PlatKind as id,'0' as parentid,platkind as name,'bplatkind'as type,convert(varchar(30),max(type)) as roomtype,'true' as isParent,platkind
                ,'pIcon01' as iconSkin,'true' as [open]
from base_monitorserver where unit_id =@unit_id group by platkind
					union all
                    select PlatKind+RoomType_id as id,PlatKind as parentid,case when bb.name is null then aa.name else bb.name end as name,'bkind'as type,convert(varchar(30),type) as roomtype,'true' as isParent,platkind
                    ,'pIcon02' as iconSkin,'false' as [open]
                    from base_roomtype aa
                    right join (select b.Name as name1,b.Name+'('+CONVERT(varchar(20),COUNT(b.Name))+')' as name,PlatKind  from Base_MonitorApplication a                                   
								 left join Base_RoomType b on a.Object_id=b.RoomType_id
                                 left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                                 left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id                   
                                 where br.Unit_id =@unit_id and bigtype<>'其他2'
                                 group by b.Name,PlatKind)  bb on bb.name1=aa.Name
                    union all
                    select PlatKind+bm.room_id as id ,PlatKind+bm.RoomType_id as parentid,bm.RoomName as name,'aroom' as [type],convert(varchar(30),b.type) as roomtype,'true' as isParent,platkind
                     ,'pIcon02' as iconSkin,'false' as [open]
                    from Base_MonitorApplication a left join Base_RoomType b on a.Object_id=b.RoomType_id
                    left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                    left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id
                    left join base_room bm on bm.room_id = a.room_id
                    where br.Unit_id =@unit_id and a.room_id <> '' group by PlatKind,b.type, bm.room_id,bm.RoomName,bm.RoomType_id
                    union all
                    select PlatKind+a.MonitorApplication_id as id,case when (a.room_id ='' or a.room_id is null) then PlatKind+object_id else PlatKind+room_id end as parentid ,bs.ChannelName as name,'channel' as type,convert(varchar(30),b.type) as roomtype,'false' as isParent,platkind 
                    ,case when isnull(bs.state,0) =0 then 'icon01' else 'icon02' end as iconSkin ,'false' as [open]
                    from Base_MonitorApplication a left join Base_RoomType b on a.Object_id=b.RoomType_id
                    left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                    left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id
                    where br.Unit_id =@unit_id and bigtype<>'其他2' order  by roomtype,name");

                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unitid));

                return Repository().FindTableBySqlNoLower(strSql.ToString(), parameter.ToArray());
            }
            else
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }

        public DataTable GetListByType(string unitid,string type)
        {
            StringBuilder strSql = new StringBuilder();
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{

            //}
            if (!string.IsNullOrEmpty(unitid))
            {
                strSql.Append(@"
                    select PlatKind as id,'0' as parentid,platkind as name,'bplatkind'as type,convert(varchar(30),max(a.type)) as roomtype,'true' as isParent,platkind
                ,'pIcon01' as iconSkin,'true' as [open]
from base_monitorserver a
left join Base_MonitorChannels bs on a.MonitorServer_id=bs.MonitorServer_id
left join  Base_MonitorApplication bn on bn.MonitorChannels_id=bs.MonitorChannels_id
left join Base_RoomType b on bn.Object_id=b.RoomType_id
where unit_id =@unit_id and bn.Object_id='" + type + @"' group by platkind
					union all
                    select PlatKind+RoomType_id as id,PlatKind as parentid,case when bb.name is null then aa.name else bb.name end as name,'bkind'as type,convert(varchar(30),type) as roomtype,'true' as isParent,platkind
                    ,'pIcon02' as iconSkin,'false' as [open]
                    from base_roomtype aa
                    right join (select b.Name as name1,b.Name+'('+CONVERT(varchar(20),COUNT(b.Name))+')' as name,PlatKind  from Base_MonitorApplication a                                   
								 left join Base_RoomType b on a.Object_id=b.RoomType_id
                                 left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                                 left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id                   
                                 where br.Unit_id =@unit_id and a.object_id ='" + type + @"'
                                 group by b.Name,PlatKind)  bb on bb.name1=aa.Name
                    union all
                    select PlatKind+bm.room_id as id ,PlatKind+bm.RoomType_id as parentid,bm.RoomName as name,'aroom' as [type],convert(varchar(30),b.type) as roomtype,'true' as isParent,platkind
                     ,'pIcon02' as iconSkin,'false' as [open]
                    from Base_MonitorApplication a left join Base_RoomType b on a.Object_id=b.RoomType_id
                    left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                    left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id
                    left join base_room bm on bm.room_id = a.room_id
                    where a.object_id ='" +type+@"' and br.Unit_id =@unit_id and a.room_id <> '' group by PlatKind,b.type, bm.room_id,bm.RoomName,bm.RoomType_id
                    union all
                    select PlatKind+a.MonitorApplication_id as id,case when (a.room_id ='' or a.room_id is null) then PlatKind+object_id else PlatKind+room_id end as parentid ,bs.ChannelName as name,'channel' as type,convert(varchar(30),b.type) as roomtype,'false' as isParent,platkind 
                    ,case when isnull(bs.state,0) =0 then 'icon01' else 'icon02' end as iconSkin ,'false' as [open]
                    from Base_MonitorApplication a 
                    left join Base_RoomType b on a.Object_id=b.RoomType_id
                    left join Base_MonitorChannels bs on bs.MonitorChannels_id=a.MonitorChannels_id
                    left join base_monitorserver br on br.MonitorServer_id=bs.MonitorServer_id
                    where br.Unit_id =@unit_id and a.object_id ='" + type + @"' order by roomtype,name");

                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unitid));

                return Repository().FindTableBySqlNoLower(strSql.ToString(), parameter.ToArray());
            }
            else
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }

        public DataTable GetMapData()
        {
            string sql = string.Format(@" select substring(a.unit,0,LEN(a.unit)) as name,COUNT(MonitorChannels_id) as value from Base_Unit a
	                                    left join Base_MonitorServer b on a.Base_Unit_id=b.Unit_id
	                                    left join Base_MonitorChannels c on b.MonitorServer_id =c.MonitorServer_id
	                                    group by a.Base_Unit_id,a.unit");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetMapData(string Base_Unit_id)
        {
            string sql = string.Format(@" declare @a varchar(36)	                                    
select @a=Base_Unit_id from Base_Unit where substring(unit,0,LEN(unit))='"+Base_Unit_id+@"'

select substring(a.unit,0,LEN(a.unit)) as name,COUNT(MonitorChannels_id) as value from Base_Unit a
	                                    left join Base_MonitorServer b on a.Base_Unit_id=b.Unit_id
	                                    left join Base_MonitorChannels c on b.MonitorServer_id =c.MonitorServer_id
	                                    where parent_unit_id in (select b from dbo.GetChildUnit(@a))
	                                    group by a.Base_Unit_id,a.unit");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 加载单位列表数据
        /// </summary>
        /// <returns></returns>
        public DataTable LoadUnitData()
        {
            string sql = string.Format(@" select Base_Unit_id,unit,parent_unit_id from Base_Unit order by sortcode ");
            try
            {
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 加载应用类别数据
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable LoadAppTypeData(string unitId)
        {
            string sql = string.Format(@"
select '1' appTypeId, '办案房间' appTypeName, '0' appTypePId ,'true' isParent
union all
select '2' appTypeId, '监控类别' appTypeName, '0' appTypePId,'true' isParent
union all
select RoomType_id appTypeId,Name appTypeName,case when type=1 then '1' when type=2 then '2' end as appTypePId,
case when type=1 then 'true' when type=2 then 'false' end as isParent  from Base_RoomType
union all
select Room_id appTypeId,RoomName appTypeName,RoomType_id appTypePId,'false' isParent from Base_Room where Unit_id='{0}'
"
                , unitId
                );
            try
            {
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 异步加载设备和通道数据
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="isParent"></param>
        /// <returns></returns>
        public DataTable LoadServerChannels(string pId, bool isParent, string treeType, string txtAppTypeIdPName, string baseUnitId)
        {
            //获取Ajax通道的是哪棵树
            string type = string.Empty;
            switch (treeType)
            {
                case "txCoder":
                    type = "特写编码器";
                    break;
                case "qjCoder":
                    type = "全景编码器";
                    break;
                case "jmCoder":
                    type = "解码器";
                    break;
                case "sxCoder":
                    type = "审讯主机";
                    break;
                case "jkServer":
                    type = "监控服务";
                    break;
                case "hfServer":
                    type = "回放服务";
                    break;
                default:
                    type = "监控点";
                    break;
            }
            //获取房间的主键  和  房间类别的主键  
            string appTypeSplit0 = txtAppTypeIdPName.Split(',')[0];
            string appTypeSplit2 = txtAppTypeIdPName.Split(',')[2];
            string room_id = string.Empty; //房间的主键
            string object_id = string.Empty; //房间类别的主键
            if (appTypeSplit2 == "2")
            {
                //点击的是  监控类别
                room_id = String.Empty;
                object_id = appTypeSplit0;
            }
            else
            {
                //点击的是  办案房间
                room_id = appTypeSplit0;
                object_id = appTypeSplit2;
            }
            string sql = string.Empty;
            if (pId == "0")
            {
                if (type != "审讯主机")
                {
                    #region 第一次加载非审讯主机
                    //                    sql = string.Format(@"
                    //
                    //select MonitorServer_id id,ServerName name,'0' pid,'true' isParent,'false' checked from Base_MonitorServer 
                    //where Unit_id='{3}' 
                    //union all 
                    //select 
                    //aa.MonitorChannels_id id
                    //,aa.ChannelName name 
                    //,aa.MonitorServer_id pid
                    //,'false' isParent
                    //,case when bb.Room_id='{1}' and bb.Object_id='{2}' and bb.type='{0}' then 'true' else 'false' end as checked
                    // from (
                    //--拿到,所有被选中的设备下的树
                    //select mc.* from Base_MonitorChannels mc
                    //join 
                    //--指定房间,指定类别,指定树,下的 设备有没有被选中
                    //(select mc.MonitorServer_id from Base_MonitorApplication ma join Base_MonitorChannels mc on ma.MonitorChannels_id=mc.MonitorChannels_id
                    //where Room_id='{1}' and Object_id='{2}' and type='{0}' group by mc.MonitorServer_id) b 
                    //on mc.MonitorServer_id=b.MonitorServer_id) aa
                    //left join
                    //--找到哪些通道被选中了
                    //(select * from Base_MonitorApplication where Room_id='{1}' and Object_id='{2}' and type='{0}') bb
                    //on aa.MonitorChannels_id=bb.MonitorChannels_id order by checked desc
                    //"
                    //                                    , type
                    //                                    , room_id
                    //                                    , object_id
                    //                                    , baseUnitId
                    //                                    );

                    sql = string.Format(@"
select MonitorServer_id id,ServerName name,'0' pid,'true' isParent,'false' checked from Base_MonitorServer 
where Unit_id='{3}' 
union all 
select 
aa.MonitorChannels_id id
,aa.ChannelName name 
,aa.MonitorServer_id pid
,'false' isParent
,case when bb.Room_id='{1}' and bb.Object_id='{2}' and bb.type='{0}' then 'true' else 'false' end as checked
 from (
--拿到,所有被选中的设备下的树
select mc.* from Base_MonitorChannels mc
join 
--指定房间,指定类别,指定树,下的 设备有没有被选中
(select mc.MonitorServer_id from Base_MonitorApplication ma join Base_MonitorChannels mc on ma.MonitorChannels_id=mc.MonitorChannels_id
join Base_MonitorServer ms on ms.MonitorServer_id=mc.MonitorServer_id
where Room_id='{1}' and Object_id='{2}' and ma.type='{0}' and Unit_id='{3}' group by mc.MonitorServer_id) b 
on mc.MonitorServer_id=b.MonitorServer_id) aa
left join
--找到哪些通道被选中了
(select * from Base_MonitorApplication where Room_id='{1}' and Object_id='{2}' and type='{0}') bb
on aa.MonitorChannels_id=bb.MonitorChannels_id order by checked desc,name

", type, room_id, object_id, baseUnitId);

                    #endregion
                }
                else
                {
                    #region 第一个加载 审讯主机
                    sql = string.Format(@"
select 
a.MonitorServer_id id,a.ServerName name, '0' pid, 'true' isParent 
,case when b.MonitorApplication_id is NULL then 'false' else 'true' end as checked 
 from  
(select * from Base_MonitorServer where Unit_id='{3}' ) a 
left join
(select * from Base_MonitorApplication where Room_id='{1}' and Object_id='{2}' and type='{0}') b 
on a.MonitorServer_id=b.MonitorChannels_id 
"
                                     , type
                                    , room_id
                                    , object_id
                                    , baseUnitId
                                    );
                    #endregion
                }
            }
            else
            {
                #region 非第一次加载
                sql = string.Format(@"
          select 
            ca.MonitorChannels_id id
            ,ca.ChannelName name
            ,ca.MonitorServer_id pid
            ,'{0}' isParent
            ,case when ca.MonitorApplication_id is NULL then 'false' else 'true' end as checked
          from 
          (
            select mc.MonitorChannels_id,mc.MonitorServer_id,mc.ChannelName,mc.ChannelCode,ma.MonitorApplication_id from 
            Base_MonitorChannels mc 
            left join 
          (
            select MonitorApplication_id,MonitorChannels_id from Base_MonitorApplication 
            where Room_id='{1}' 
            and Object_id='{2}' 
            and type='{3}'
          ) ma 
            on mc.MonitorChannels_id=ma.MonitorChannels_id
          where mc.MonitorServer_id='{4}'
            ) ca 
          order by ca.MonitorApplication_id desc,ca.ChannelCode,ca.ChannelName
"
                           , isParent
                           , room_id
                           , object_id
                           , type
                           , pId
                           );
                #endregion
            }
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
        /// 保存数据
        /// </summary>
        /// <param name="appTypeIdPName">选中的应用类别的主键及其父级的名称</param>
        /// <param name="txTxtNodeIds">选中的特写编码器的主键</param>
        /// <param name="qjTxtNodeIds">选中的全景编码器的主键</param>
        /// <param name="jmTxtNodeIds">选中的解码器的主键</param>
        /// <param name="sxTxtNodeId">选中的审讯主机的主键</param>
        /// <param name="jkTxtNodeIds">选中的监控服务的主键</param>
        /// <param name="hfTxtNodeIds">选中的回放服务的主键</param>
        /// <param name="jkPointTxtNodeIds">选中的监控点的主键</param>
        /// <returns></returns>
        public string SaveData(string unit_id, string appTypeIdPName, string txTxtNodeIds, string qjTxtNodeIds, string jmTxtNodeIds,
            string sxTxtNodeId, string jkTxtNodeIds, string hfTxtNodeIds, string jkPointTxtNodeIds)
        {
            #region 删除数据库数据
            //一、先进行删除数据的操作
            //1.获取选中的房间的主键，以及 房间类别的主键
            string appTypeSplit0 = appTypeIdPName.Split(',')[0];
            string appTypeSplit2 = appTypeIdPName.Split(',')[2];
            string room_id = string.Empty; //房间的主键
            string object_id = string.Empty; //房间类别的主键
            if (appTypeSplit2 == "2")
            {
                //点击的是  监控类别
                room_id = String.Empty;
                object_id = appTypeSplit0;
            }
            else
            {
                //点击的是  办案房间
                room_id = appTypeSplit0;
                object_id = appTypeSplit2;
            }

            //2.获取对应类型所选择的通道
            //2.1特写编码器
            List<string> txNodeList = GetNodes(txTxtNodeIds);
            //2.2全景编码器
            List<string> qjNodeList = GetNodes(qjTxtNodeIds);
            //2.3解码器
            List<string> jmNodeList = GetNodes(jmTxtNodeIds);
            //2.4审讯主机
            List<string> sxNodeList = GetNodes(sxTxtNodeId);
            //2.5监控服务
            List<string> jkNodeList = GetNodes(jkTxtNodeIds);
            //2.6回放服务
            List<string> hfNodeList = GetNodes(hfTxtNodeIds);
            //2.7监控点
            List<string> jkPointNodeList = GetNodes(jkPointTxtNodeIds);

            //3.组装删除的sql
            string sqlDel = string.Empty;
            StringBuilder sbDel = new StringBuilder();

            if (appTypeSplit2 == "2")
            {
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, txNodeList, "特写编码器"));
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, qjNodeList, "全景编码器"));
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, jmNodeList, "解码器"));
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, sxNodeList, "审讯主机"));
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, jkNodeList, "监控服务"));
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, hfNodeList, "回放服务"));
                sbDel.Append(GetDelSqlWithUnit(unit_id, room_id, object_id, jkPointNodeList, "监控点"));
            }
            else
            {
                sbDel.Append(GetDelSql(room_id, object_id, txNodeList, "特写编码器"));
                sbDel.Append(GetDelSql(room_id, object_id, qjNodeList, "全景编码器"));
                sbDel.Append(GetDelSql(room_id, object_id, jmNodeList, "解码器"));
                sbDel.Append(GetDelSql(room_id, object_id, sxNodeList, "审讯主机"));
                sbDel.Append(GetDelSql(room_id, object_id, jkNodeList, "监控服务"));
                sbDel.Append(GetDelSql(room_id, object_id, hfNodeList, "回放服务"));
                sbDel.Append(GetDelSql(room_id, object_id, jkPointNodeList, "监控点"));
            }

            sqlDel = string.Format(@"
                                    declare @errorNumber int
                                    set @errorNumber=0
                                    begin tran 
                                    {0} 
                                    set @errorNumber =@@ERROR 
                                    if(@errorNumber>0) 
	                                    begin 
		                                    rollback tran 
	                                    end 
                                    else 
	                                    begin 
		                                    commit tran 
	                                    end 
                                    "
                                        , sbDel.ToString()
                                );
            #endregion

            try
            {
                SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
            }
            catch (Exception ex)
            {
                return "delError";
            }

            #region 插入数据库数据
            //二、进行插入操作
            //4.组装插入的sql
            string sqlInsert = string.Empty;
            StringBuilder sbInsert = new StringBuilder();
            sbInsert.Append(GetInsertSql(room_id, object_id, txNodeList, "特写编码器"));
            sbInsert.Append(GetInsertSql(room_id, object_id, qjNodeList, "全景编码器"));
            sbInsert.Append(GetInsertSql(room_id, object_id, jmNodeList, "解码器"));
            sbInsert.Append(GetInsertSql(room_id, object_id, sxNodeList, "审讯主机"));
            sbInsert.Append(GetInsertSql(room_id, object_id, jkNodeList, "监控服务"));
            sbInsert.Append(GetInsertSql(room_id, object_id, hfNodeList, "回放服务"));
            sbInsert.Append(GetInsertSql(room_id, object_id, jkPointNodeList, "监控点"));

            sqlInsert = string.Format(@"
                                    declare @errorNumber int
                                    set @errorNumber=0
                                    begin tran 
                                    {0} 
                                    set @errorNumber =@@ERROR 
                                    if(@errorNumber>0) 
	                                    begin 
		                                    rollback tran 
	                                    end 
                                    else 
	                                    begin 
		                                    commit tran 
	                                    end 
                                    "
                                        , sbInsert.ToString()
                                );
            #endregion

            try
            {
                SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
            }
            catch (Exception)
            {
                return "insertError";
            }
            return "saveSuccess";
        }

        /// <summary>
        /// 组装插入的sql
        /// </summary>
        /// <param name="room_id"></param>
        /// <param name="object_id"></param>
        /// <param name="nodesList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetInsertSql(string room_id, string object_id, List<string> nodesList, string type)
        {
            string sql = string.Empty;
            if (nodesList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string node in nodesList)
                {
                    sb.AppendFormat(@"
                                        insert into 
                                        Base_MonitorApplication
                                        (
                                            MonitorApplication_id
                                            ,MonitorChannels_id
                                            ,Room_id
                                            ,Object_id
                                            ,type
                                        ) values(
                                            NEWID()
                                            ,'{0}'
                                            ,'{1}'
                                            ,'{2}'
                                            ,'{3}'
                                        );
                                        "
                                            , node
                                            , room_id
                                            , object_id
                                            , type
                                );
                }
                sql = sb.ToString();
            }
            return sql;
        }

        /// <summary>
        /// 组装删除的sql
        /// </summary>
        /// <param name="room_id"></param>
        /// <param name="object_id"></param>
        /// <param name="nodesList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDelSql(string room_id, string object_id, List<string> nodesList, string type)
        {
            string sql = string.Empty;

            sql = string.Format(@" delete Base_MonitorApplication where Room_id='{0}' and Object_id='{1}' and type='{2}'; ", room_id, object_id, type);

            return sql;
        }

        /// <summary>
        /// 如果删除的是监控类别的话，那么需要判断删除的是哪个单位下的监控类别
        /// </summary>
        /// <param name="unit_id"></param>
        /// <param name="room_id"></param>
        /// <param name="object_id"></param>
        /// <param name="nodesList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDelSqlWithUnit(string unit_id, string room_id, string object_id, List<string> nodesList,
            string type)
        {
            string sql = string.Empty;

            sql = string.Format(@" delete Base_MonitorApplication where Room_id='{0}' and Object_id='{1}' and type='{2}' and MonitorChannels_id in(  select mc.MonitorChannels_id from Base_MonitorChannels  mc left join Base_MonitorServer ms on mc.MonitorServer_id=ms.MonitorServer_id  where ms.Unit_id='{3}'); ", room_id, object_id, type, unit_id);

            return sql;
        }

        /// <summary>
        /// 根据传入的字符串分割组成集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<string> GetNodes(string str)
        {
            List<string> nodeList = new List<string>();
            if (str != string.Empty)
            {
                //选择了通道
                str = str.Substring(0, str.Length - 1);
                if (str.Contains(","))
                {
                    string[] ids = str.Split(',');
                    foreach (string id in ids)
                    {
                        nodeList.Add(id);
                    }
                }
                else
                {
                    nodeList.Add(str);
                }
            }
            return nodeList;
        }
    }
}

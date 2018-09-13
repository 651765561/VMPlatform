using LeaRun.Repository;
using System;
using System.Collections.Generic;
using LeaRun.Entity;
using LeaRun.Utilities;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.Common;
using LeaRun.DataAccess;
using System.Diagnostics;

namespace LeaRun.Business
{
public  class Base_CaseDiscussionBll : RepositoryFactory<Base_Meeting>
    {

        public DataTable GetPageList(ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@" select * from ( 
                                           select 
                                               m.meetingid as meetingid ,
                                                   m.Meeting_id as Meeting_id   ,     --主键                                                   
                                                    m.unit_id as unit_id,	          --组会单位ID
                                                    c.unit as unit_name,			  --组会单位名称
                                                    m.adduser_id as adduser_id,		  --组会人员ID
                                                    u.RealName as adduser_name,       --组会人员名称
                                                    m.adddate as adddate,             --组会日期
                                                    m.name as name ,                  --会议名称
                                                    m.type as type,                   --会议类型
                                                    m.Room_id as room_id  ,           --房间ID
                                                    m.startdate as startdate,         --开始时间
                                                    m.enddate as enddate,             --结束时间
                                                    m.userid as userid,               --会场参会人员的Id
                                                    m.username as username,    	      --会场参会人员
                                                    m.QJid as QJid,                   --会场全景ID
                                                    m.LDid as LDid,                   --领导ID
                                                    m.ZhCid as ZhCid,                 --主持人ID
                                                    m.screenmodel as screenmodel  ,   --窗口模式
                                                    m.state                           --状态
                                          FROM      Base_Meeting m
                                                    LEFT JOIN Base_Unit c ON c.Base_Unit_id = m.Unit_id
                                                    LEFT JOIN Base_User u ON u.UserId = m.adduser_id
                                        ) a  where 1=1 and state=0 and type=2    ");
            strSql.Append(" AND adduser_id = @user_id");
            parameter.Add(DbFactory.CreateDbParameter("@user_id", ManageProvider.Provider.Current().UserId));
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }

        /// <summary>
        /// 根据当前单位的主键获取单位的相关信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetCurrentUserUnit(string unitId)
        {
            string sql = string.Format(@" 
                select 
                * 
                from Base_Unit 
                where 
                Base_Unit_id='{0}' 
                "
                , unitId
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

        //新增或者编辑一条数据
        public int AddMeeting(Base_Meeting base_Meeting, string type)
        {
            StringBuilder sb = new StringBuilder();
            if (type == "add")
            {

                string sql = string.Format(@" 
insert into  Base_Meeting( adduser_id ,adddate ,name,type,Room_id ,startdate ,enddate ,username ,QJid ,LDid,ZhCid ,screenmodel ,state ,unit_id , userid  , meetingid)values('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11},{12},'{13}','{14}','{15}') "
                    , base_Meeting.adduser_id
                    , base_Meeting.adddate
                    , base_Meeting.name
                    , 2
                    , base_Meeting.Room_id
                    , base_Meeting.startdate
                    , base_Meeting.enddate
                    , base_Meeting.username
                    , base_Meeting.QJid
                    , base_Meeting.LDid
                    , base_Meeting.ZhCid
                    , 8
                    , base_Meeting.state
                    , base_Meeting.unit_id
                    , base_Meeting.userid
                    , base_Meeting.meetingid
                    );
                sb.Append(sql);
            }
            else
            {
                string sql = string.Format(@"
update Base_Meeting set 
name='{0}'
,adddate='{1}'
,startdate='{2}'
,enddate='{3}'
,userid='{4}'
,username='{5}'
,QJid='{6}'
,LDid='{7}'
,ZhCid='{8}'
,state={9}
where meetingid='{10}'
"
                                   , base_Meeting.name

                                   , base_Meeting.adddate
                                   , base_Meeting.startdate
                                   , base_Meeting.enddate
                                   , base_Meeting.userid
                                   , base_Meeting.username
                                   , base_Meeting.QJid
                                   , base_Meeting.LDid
                                   , base_Meeting.ZhCid
                                   , base_Meeting.state
                                   , base_Meeting.meetingid
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
        
        public int DeleteCaseDiscussion(string keyValue)
        {

            StringBuilder sb = new StringBuilder();
            string sql = string.Format(@"delete Base_Meeting where meetingid='{0}' ", keyValue);
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

        /// <summary>
        /// 获取当前所选人
        /// </summary>
        /// <param name="keyValue"></param> //主键
        /// <param name="Id"></param> //修改哪个类别的人员："userid":参会人员；"QJid":会场全景人员；"LDid":领导人员；"ZHCid"：主持人
        /// <returns></returns>
        public string LoadSelectUsers(string keyValue, string Id)
        {
            string sql = string.Format(@" select * from Base_Meeting where meetingid='{0}' ", keyValue);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string user_ids = dt.Rows[0][Id].ToString();
                    if (user_ids.Contains(","))
                    {
                        //多人
                        string[] ids = user_ids.Split(',');
                        List<string> nameList = new List<string>();

                        foreach (string id in ids)
                        {
                            string sqlGetName = string.Format(@" select * from Base_User where UserId='{0}' ", id);
                            DataTable dtName = SqlHelper.DataTable(sqlGetName, CommandType.Text);
                            nameList.Add(dtName.Rows[0]["UserId"].ToString() + "," + dtName.Rows[0]["RealName"].ToString());
                        }
                        string strids = string.Empty;
                        string strnames = string.Empty;
                        foreach (string n in nameList)
                        {
                            strids += n.Split(',')[0] + ",";
                            strnames += n.Split(',')[1] + ",";
                        }
                        return strids.Substring(0, strids.Length - 1) + "|" + strnames.Substring(0, strnames.Length - 1);
                    }
                    else
                    {
                        //一个人
                        string sqlGetName = string.Format(@" select * from Base_User where UserId='{0}' ", user_ids);
                        DataTable dtName = SqlHelper.DataTable(sqlGetName, CommandType.Text);
                        return dtName.Rows[0]["UserId"].ToString() + "|" + dtName.Rows[0]["RealName"];
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

    }
}


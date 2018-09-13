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
using LeaRun.Repository;

namespace LeaRun.Business
{
  public  class DiscussionBll : RepositoryFactory<Base_Meeting>
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
          strSql.Append(" AND ( @user_id  in(QJid,LDid,ZhCid) or userid like '%" + ManageProvider.Provider.Current().UserId + "%' )");
          parameter.Add(DbFactory.CreateDbParameter("@user_id", ManageProvider.Provider.Current().UserId));
          //strSql.Append(" AND adduser_id = @user_id");
          //parameter.Add(DbFactory.CreateDbParameter("@user_id", ManageProvider.Provider.Current().UserId));
          return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
      }

      public DataTable GetCurrentUser(string userId)
      {
          string sql = string.Format(@" 
                select 
                * 
                from Base_User 
                where 
                UserId='{0}' 
                "
              , userId
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
    }
}

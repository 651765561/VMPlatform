using LeaRun.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Entity.CommonModule;
using LeaRun.Utilities;
using System.Diagnostics;
using LeaRun.Entity;
using System.Data.Common;
using LeaRun.DataAccess;

namespace LeaRun.Business
{
  public  class Query_GroupMeetingBll : RepositoryFactory<Base_Meeting>
    {

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
      public string GridPageApplyJson(string ParameterJson, JqGridParam jqgridparam)
        {
           
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                     string.Format(
                         @" 
                                           select 
                                          ROW_NUMBER() over(order by meetingid) rowNumber ,
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
                                     where  m.type=1 and m.unit_id='{0}' "
                                  , ManageProvider.Provider.Current().CompanyId

                         );

                string sql =
                string.Format(
                    @" select * from ( 
                                        {4}
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                    , (pageIndex - 1) * pageSize + 1
                    , pageIndex * pageSize
                    , jqgridparam.sidx
                    , jqgridparam.sord
                    , sqlTotal
                    );

                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                    page = jqgridparam.page, //当前页码
                    records = dt.Rows.Count, //总记录数
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


      /// <summary>
      /// 列表加载
      /// </summary>
      /// <param name="ParameterJson"></param>
      /// <param name="unit_id"></param>
      /// <param name="jqgridparam"></param>
      /// <returns></returns>
      public string GridPageJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string adduser_name, string name, string datestart, string dateend, string state, string contianssubordinateunit)
      {

          try
          {
              List<DbParameter> parameter = new List<DbParameter>();
              int pageIndex = jqgridparam.page;
              int pageSize = jqgridparam.rows;
              Stopwatch watch = CommonHelper.TimerStart();
              string sqlTotal =
                string.Format(
                   @" 
                                           select 
                                                    ROW_NUMBER() over(order by meetingid) rowNumber ,
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
                                       where 1=1 and type=1   ");

              //if (unit_id != "")
              //{
              //    sqlTotal = sqlTotal + " and m.unit_id ='" + unit_id + "'";
              //}
              if (unit_id != "")//单位ID
              {
                  if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                  {
                      if (contianssubordinateunit == "1")
                      {
                          sqlTotal = sqlTotal + " and (m.unit_id ='" + unit_id + "' or (m.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                      }
                      else
                      {
                          sqlTotal = sqlTotal + " and m.unit_id ='" + unit_id + "'";
                      }
                  }
                  else
                  {
                      if (contianssubordinateunit == "1")
                      {

                      }
                      else
                      {
                          sqlTotal = sqlTotal + " and m.unit_id ='" + unit_id + "'";
                      }
                  }
              }
              else
              {
                  if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                  {
                      sqlTotal = sqlTotal + " and (m.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (m.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                  }
                  else
                  {
                     
                  }
              }

              if (adduser_name != "")
              {
                  sqlTotal = sqlTotal + " and u.RealName like '%" + adduser_name.Trim() + "%'";
              }
              if (name != "")
              {
                  sqlTotal = sqlTotal + " and m.name like '%" + name.Trim() + "%'";
              }
              if (datestart != "")
              {
                  sqlTotal = sqlTotal + " and  m.adddate> '" + datestart + "'";
              }
              if (dateend != "")
              {
                  sqlTotal = sqlTotal + " and  m.adddate< '" + dateend + "'";
              }
              if (state != "")
              {
                  sqlTotal = sqlTotal + " and m.state = " + state;
              }

              string sql =
              string.Format(
                  @" select * from ( 
                                        {4}
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                  , (pageIndex - 1) * pageSize + 1
                  , pageIndex * pageSize
                  , jqgridparam.sidx
                  , jqgridparam.sord
                  , sqlTotal
                  );
              DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);

              var JsonData = new
              {
                  total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数               
                  page = jqgridparam.page, //当前页码
                  records = dt.Rows.Count, //总记录数
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

      /// <summary>
      /// 本单位及下级单位
      /// </summary>
      /// <param name="unit_id">单位ID</param>
      /// <returns></returns>
      public DataTable GetList_quanxian(string unit_id)
      {
          StringBuilder strSql = new StringBuilder();
          List<DbParameter> parameter = new List<DbParameter>();
          if (unit_id != Share.UNIT_ID_JS)//非省院选择本单位及下级单位
          {
              strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    '0' AS parent_id ,--上级单位ID
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit uf ON u.parent_unit_id = uf.Base_Unit_id
                                        ) T WHERE 1=1 ");

              if (!string.IsNullOrEmpty(unit_id))
              {
                  strSql.Append(" AND t.dep_id in (select base_unit_id from base_unit where base_unit_ID=@unit_id or parent_unit_id =@unit_id) ");
                  parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
              }
          }
          else//省院选择全部单位
          {
              strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id as dep_id ,    --主键
                                                    u.unit as name ,	          --单位名称
                                                    ISNULL(uf.Base_Unit_id,0) AS parent_id ,--上级单位ID
                                                    uf.unit as parent_name,		  --上级部门
                                                    u.sortcode ,				  --排序字段
                                                    u.code,   					  --Code字段
                                                    'Unit' AS Sort                --分类字段
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

      /// <summary>
      /// 获得登陆者信息
      /// </summary>
      /// <param name="unitId"></param>
      /// <returns></returns>
      public DataTable GetLoginUserId(string UserId)
      {
          string sql = string.Format(@" 
                select 
                UNIT.Base_Unit_id, UNIT.unit,DEP.Dep_id,DEP.Name,USE1.UserId,USE1.RealName,Telephone
                from Base_User USE1
                join Base_Unit UNIT ON UNIT.Base_Unit_id=USE1.CompanyId
                join Base_Department DEP  ON DEP.Dep_id=USE1.dep_id
                WHERE USE1.UserId='{0}' 
                "
              , UserId
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


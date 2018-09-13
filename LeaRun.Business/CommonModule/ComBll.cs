using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Diagnostics;

namespace LeaRun.Business
{
    public partial class ComBll
    {
        /// <summary>
        /// 加载所有单位的列表
        /// </summary>
        /// <returns></returns>
        public DataTable LoadUnitData(string unitId)
        {
            string where = string.Empty;
            if (unitId != "0")
            {
                where = string.Format(@" where  Base_Unit_id='{0}' ", unitId);
            }
            string sql = string.Format(@" select Base_Unit_id,unit,parent_unit_id from Base_Unit {0} order by sortcode ", where);
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
        /// 加载指定单位的部门信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable LoadDepData(string unitId)
        {
            string sql = string.Format(@" select Dep_id,Name,Parent_id from Base_Department where unit_id='{0}' ", unitId);
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
        /// 加载指定部门的人员列表
        /// </summary>
        /// <param name="depId"></param>
        /// <returns></returns>
        public DataTable LoadUserData(string depId, string unitId)
        {
            //string sql = string.Format(@" select UserId,RealName,0 pId,'false' isParent from Base_User where dep_id='{0}' ", depId);


            //下面加载出需要调上来的警
            string sql = string.Empty;
            //判断当前的这个部门是否包含'法警'字样
            string sqlCheckDep =
                string.Format(
                    @"select * from Base_Department where Dep_id='{0}' and Name like '%法警%'",depId);
            DataTable dtCheckDep = SqlHelper.DataTable(sqlCheckDep, CommandType.Text);
            if (dtCheckDep.Rows.Count > 0)
            {
                sql = string.Format(@"
select UserId,RealName,0 pId,'false' isParent from Base_User where dep_id='{0}' 
and UserId not in (select jsp.user_id UserId from JW_SendPolice jsp 
join Base_User bu on jsp.user_id=bu.UserId 
join JW_OrderPolice jop on jsp.Object_id=jop.orderpolice_id 
where jop.state<>5 and jsp.state>0 and jop.from_unit_id='{1}')
and UserId not in (select jsp.user_id UserId from JW_SendPolice jsp
join Base_User bu on jsp.user_id=bu.UserId
join JW_AssignPolice jap on jsp.Object_id=jap.callpolice_id
where jap.state<>5  and jsp.state>0 and jap.from_unit_id='{1}')

union all
select jsp.user_id UserId,bu.RealName,0 pId,'false' isParent from JW_SendPolice jsp 
join Base_User bu on jsp.user_id=bu.UserId 
join JW_OrderPolice jop on jsp.Object_id=jop.orderpolice_id 
where jop.state<>5  and jsp.state>0 and jop.to_unit_id='{1}'
union all
select jsp.user_id UserId,bu.RealName,0 pId,'false' isParent from JW_SendPolice jsp
join Base_User bu on jsp.user_id=bu.UserId
join JW_AssignPolice jap on jsp.Object_id=jap.callpolice_id
where jap.state<>5  and jsp.state>0 and jap.to_unit_id='{1}'

                                        ", depId, unitId);
            }
            else
            {
                sql = string.Format(@" select UserId,RealName,0 pId,'false' isParent from Base_User where dep_id='{0}' ", depId);
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
        /// 加载指定的房间列表
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public DataTable LoadPoliceAreaData(string unitId, string policeAreaType)
        {
            string where = string.Empty;
            if (policeAreaType != "0")
            {
                if (policeAreaType.Contains(","))
                {
                    //需要加载多个办案区
                    string[] areasType = policeAreaType.Split(',');
                    where += string.Format(@" and ( AreaType= '{0}' ", areasType[0]);
                    for (int i = 1; i < areasType.Length; i++)
                    {
                        where += string.Format(@" or AreaType = '{0}' ", areasType[i]);
                    }
                    where += " ) ";
                }
                else
                {
                    //只需要加载一个办案区
                    where = string.Format(@" and AreaType='{0}' ", policeAreaType);
                }
            }
            string sql = string.Format(@" select PoliceArea_id,AreaName,0 pId from Base_PoliceArea where unit_id='{0}' {1} ", unitId, where);
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
        /// 加载派警列表
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridSendPoliceJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql =
                    string.Format(
                        @" select * from ( 
select ROW_NUMBER() over(order by jsp.SendDate) rowNumber, bu.unit unitname,buser.RealName sendpolicename,jsp.SendDate,ur.RealName fjname,jw.startdate,jw.enddate 
,case when jsp.type=1 then '办案区' when jsp.type=2 then '刑检办案区' when jsp.type=3 then '指定居所'  when jsp.type=4 then '用警申请' when jsp.type=5 then '直接派警'  when jsp.type=6 then '调警令' else '' end as type
from JW_SendPolice jsp 
join Base_Unit bu on jsp.Unit_id=bu.Base_Unit_id 
join Base_User buser on jsp.SendUser_id=buser.UserId 
join Base_User ur on jsp.user_id=ur.UserId 
join JW_Apply jw on jsp.Object_id=jw.apply_id
where jsp.Unit_id='{4}' and jsp.state in (1,2,3)
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , unit_id
                        );
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dt.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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
        /// 加载默认的人员列表
        /// </summary>
        /// <param name="depId"></param>
        /// <returns></returns>
        public DataTable LoadUsers(string unit_id)
        {
            string sql = string.Format(@" 
              select bu.UserId,bu.RealName,0 pId,'false' isParent from Base_User bu
              join Base_Department bd on bu.dep_id = bd.Dep_id
              where bu.CompanyId='{0}' and bu.Enabled='1' and bd.Name like '法警%' and bd.state='1' 
        union all
              select bu.UserId,bu.RealName,0 pId,'false' isParent from JW_SendPolice sp 
              join JW_AssignPolice ap on sp.Object_id=ap.callpolice_id 
              join Base_User bu on sp.user_id=bu.UserId
              where sp.type=6 and sp.state=1 and ap.to_unit_id='{0}' and ap.state=3
        union all
              select bu.UserId,bu.RealName,0 pId,'false' isParent from JW_SendPolice sp 
              join JW_OrderPolice op on sp.Object_id=op.orderpolice_id 
              join Base_User bu on sp.user_id=bu.UserId
              where sp.type=7 and sp.state=1 and op.from_unit_id='{0}' and op.state=3
", unit_id);
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
        /// 加载案由数据
        /// </summary>
        /// <returns></returns>
        public DataTable LoadBriefData()
        {
            string sql = string.Format(@"select * from Case_CaseBrief order by code");
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 默认加载当前登录人员所在单位中的，包含 法警 字样的部门中所有的人
        /// </summary>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public DataTable LoadDefauleUserData(string unit_id)
        {
            string sql = string.Format(@"
                                        select u.UserId UserId,u.RealName RealName,0 pId, 'false' isParent from Base_User u where dep_id in 
                                    (select bd.Dep_id from Base_Department bd
                                    join Base_Unit bu on bd.unit_id=bu.Base_Unit_id
                                    where 
                                    bu.Base_Unit_id='{0}'
                                    and bd.Name like '%法警%')
                                        ", unit_id);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Utilities;
using System.Diagnostics;
using System.Data;
using LeaRun.Repository;

namespace LeaRun.Business.CommonModule
{
    public partial class SendPoliceConfirmBll
    {
        /// <summary>
        /// 加载警务派警确认列表
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="user_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageJson(string ParameterJson, string user_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal = string.Format(@"
--select ROW_NUMBER() over(order by SendPolice_id) rowNumber, sp.*,unit.unit Unit_name,us.RealName user_name,userr.RealName SendUser_name from JW_SendPolice sp
--left join Base_Unit unit on sp.Unit_id=unit.Base_Unit_id
--left join Base_User us on sp.user_id=us.UserId 
--left join Base_User userr on sp.SendUser_id=userr.UserId
--where sp.user_id='{0}' and sp.state <> 3

select ROW_NUMBER() over(order by SendPolice_id) rowNumber ,* from ( 
select  sp.*,unit.unit Unit_name,us.RealName user_name,userr.RealName SendUser_name,pa.tel_lianxi cb_tel
from JW_SendPolice sp
left join Base_Unit unit on sp.Unit_id=unit.Base_Unit_id
left join Base_User us on sp.user_id=us.UserId 
left join Base_User userr on sp.SendUser_id=userr.UserId 
left join JW_PoliceApply pa on sp.Object_id=pa.apply_id 
where sp.user_id='{0}' and sp.state <> 3 and sp.type=4

union all 

select  sp.*,unit.unit Unit_name,us.RealName user_name,userr.RealName SendUser_name,us.Telephone cb_tel
from JW_SendPolice sp
left join Base_Unit unit on sp.Unit_id=unit.Base_Unit_id
left join Base_User us on sp.user_id=us.UserId 
left join Base_User userr on sp.SendUser_id=userr.UserId 
--left join JW_PoliceApply pa on sp.Object_id=pa.apply_id 
where sp.user_id='{0}' and sp.state <> 3 and sp.type<>4 and sp.type<>8) as aa  
                ", user_id);
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
        /// 回退
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int BackSendPolice(string keyValue, string backReason)
        {
            string sql = string.Format(@" update JW_SendPolice set BackReason='{0}',state=-1 where SendPolice_id='{1}' ", backReason, keyValue);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 确认派警
        /// </summary>
        /// <returns></returns>
        public string SubmitSendPolice(string sendPolice_id)
        {
            string sql = string.Format(@" update JW_SendPolice set state=1 where SendPolice_id='{0}' ", sendPolice_id);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r > 0)
                {
                    return "成功确定";
                }
                else
                {
                    return "数据异常，确定失败";
                }

            }
            catch (Exception ex)
            {
                return "数据异常，确定失败";
            }
        }

        /// <summary>
        /// 加载回退原因
        /// </summary>
        /// <param name="sendPolice_id"></param>
        /// <returns></returns>
        public string LoadBackReason(string sendPolice_id)
        {
            string sql = string.Format(@" select * from JW_SendPolice where SendPolice_id='{0}' ", sendPolice_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BackReason"] != DBNull.Value)
                {
                    return dt.Rows[0]["BackReason"].ToString();
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

        /// <summary>
        /// 加载已经确定的法警
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string ConfirmPoliceName(string apply_id)
        {
            string sql = string.Format(@"select bu.RealName from JW_SendPolice jsp 
                                join Base_User bu on jsp.user_id=bu.UserId 
                                where jsp.state in (1,2,3) and jsp.Object_id='{0}'", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count <= 0)
                {
                    return string.Empty;
                }
                else
                {
                    //有数据
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append(row["RealName"] + ",");
                    }
                    return sb.ToString().Substring(0, sb.ToString().Length - 1);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}

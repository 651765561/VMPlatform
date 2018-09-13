//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using LeaRun.DataAccess;
using System;
using System.Data.SqlClient;
using LeaRun.Entity;
using System.Diagnostics;
using Microsoft.Win32;


namespace LeaRun.Business
{
    public class JW_OrderPoliceBll : RepositoryFactory<JW_OrderPolice>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, string unit_id, JqGridParam jqgridparam)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal = string.Format(@"

                select 
                                            ROW_NUMBER() over(order by ja.userdate) rowNumber
                                            , ja.* ,CONVERT(varchar(10), ja.userdate, 120) AS userdate1,u.unit,ja.state as state1 
                                           from JW_OrderPolice ja  left join base_unit u on ja.from_unit_id=u.base_unit_id
                                            
                                            where ja.from_unit_id in (select base_unit_id from base_unit where base_unit_id='{0}' or parent_unit_id='{0}' ) 
                                            and ja.adduser='{1}' 
                
                ", unit_id, ManageProvider.Provider.Current().UserId);
                string sql =
                    string.Format(
                        @" select * from ( 
                                        {4}
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3}  "
                        , (pageIndex - 1) * pageSize + 1
                        , pageIndex * pageSize
                        , jqgridparam.sidx
                        , jqgridparam.sord
                        , sqlTotal
                        );
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal,CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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
        /// 删除调警令
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteJWPoliceApply(string apply_id)
        {
            string sql = string.Format(@"update JW_SendPolice set state=-2 where object_id='" + apply_id + "'; delete JW_OrderPolice where orderpolice_id ='" + apply_id + "'");
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
        /// 单位列表树
        /// </summary>
        /// <param name="unit_id">单位ID,暂时不用</param>
        /// <returns></returns>
        public DataTable GetList(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                                FROM    ( 
                                           SELECT   u.Base_Unit_id  ,                   --主键
                                                    u.unit AS Name ,	                --单位名称
                                                    isnull(u2.Base_Unit_id,'0') as parent_unit_id,  --上级部门ID
                                                    u2.unit as parent_name ,	        --上级单位名称
                                                    u.sortcode ,				        --排序字段
                                                    u.code,   					        --Code字段
                                                    'Unit' AS Sort                      --分类字段
                                          FROM      Base_Unit u
                                                    LEFT JOIN Base_Unit u2 ON u.parent_unit_id = u2.Base_Unit_id
                                        ) T WHERE 1=1 ");
            List<DbParameter> parameter = new List<DbParameter>();
            //if (!string.IsNullOrEmpty(unit_id))
            //{
            //    strSql.Append(" AND unit_id = @unit_id");
            //    parameter.Add(LeaRun.DataAccess.DbFactory.CreateDbParameter("@unit_id", unit_id));
            //}
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
        /// 更新jw_orderpolice表中的orderpoliceno
        /// </summary>
        /// <param name="keyValue"></param>
        public void UpdateOrderpoliceNo(string keyValue)
        {
            //2.先拿到当前单位的简称
            string sqlGetUnit = string.Format(@" select p.from_unit_id,u.longname from JW_OrderPolice p join base_unit u on p.from_unit_id=u.Base_Unit_id where p.orderpolice_id='{0}' ", keyValue);
            string unitID = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["from_unit_id"].ToString();
            string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

            //3.拿到数据库中当前单位的最大的流水号
            string orderpoliceNo = string.Empty;
            string sqlGetCode =
                string.Format(
                    @" select MAX(CONVERT(int,SUBSTRING(replace(orderpoliceNo,'&nbsp;',''),6,7))) code from JW_OrderPolice where from_unit_id='{0}' ",
                    unitID);
            DataTable dtCode = SqlHelper.DataTable(sqlGetCode, CommandType.Text);
            if (dtCode == null || dtCode.Rows.Count <= 0 || dtCode.Rows[0]["code"].ToString() == string.Empty || dtCode.Rows[0]["code"].ToString() == "0")
            {
                orderpoliceNo = DateTime.Now.Year.ToString() + "001";
            }
            else
            {
                string year = dtCode.Rows[0]["code"].ToString().Substring(0, 4);
                string co = dtCode.Rows[0]["code"].ToString().Substring(4, 3);
                if (year == DateTime.Now.Year.ToString())
                {
                    orderpoliceNo = (Convert.ToInt32(year + co) + 1).ToString();
                }
                else
                {
                    orderpoliceNo = DateTime.Now.Year.ToString() + "001";
                }
            }

            string sqlUpdate = string.Format(@"
                    UPDATE JW_OrderPolice set orderpoliceNo='{1}'  where  orderpolice_id='{0}';
                    "
                  , keyValue
                  , unitShotName + "检警令【" + orderpoliceNo + "】号"
                  );
            int r = SqlHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text);
        }

        /// <summary>
        /// 新增派警表
        /// </summary>
        /// <param name="keyValue">派警表中Object_id（存放 用警表对应ID）</param>
        /// <param name="sendPolice_id">警员ID，用逗号分隔</param>
        /// <returns></returns>
        public string SubmitJW_YJ(string keyValue, string sendPolice_id)
        {
            if (sendPolice_id != "" && sendPolice_id != "&nbsp;")
            {
                //对派警的处理

                #region 删除
                //1.先删除原来的数据
                string sqlDel = string.Format(@"
                    delete JW_SendPolice where  Object_id='{0}';
                    "
                    , keyValue
                    );
                sqlDel = string.Format(@"
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
                    , sqlDel.ToString()
                    );

                try
                {
                    int r2 = SqlHelper.ExecuteNonQuery(sqlDel, CommandType.Text);
                }
                catch (Exception)
                {
                    return "数据异常，派警失败";
                }

                //2.先拿到当前单位的简称
                string sqlGetUnit = string.Format(@" select p.from_unit_id,u.longname from JW_OrderPolice p join base_unit u on p.from_unit_id=u.Base_Unit_id where p.orderpolice_id='{0}' ", keyValue);
                string unitID = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["from_unit_id"].ToString();
                string unitShotName = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["longname"].ToString();

                //3.拿到数据库中当前单位的最大的流水号
                string code = string.Empty;
                string sqlGetCode =
                    string.Format(
                        @" select MAX(CONVERT(int,SUBSTRING(SendCode,6,7))) code from JW_SendPolice where Unit_id='{0}' ",
                        unitID);
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

                #endregion

                #region 添加

                StringBuilder sb = new StringBuilder();
                //再添加新的数据
                if (!sendPolice_id.Contains(","))
                {
                    //只选择了一个人
                    sb.AppendFormat(
                        @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                        , unitID
                        , unitShotName + "检警令【" + code + "】号"
                        , ManageProvider.Provider.Current().UserId
                        , DateTime.Now
                        , "7"
                        , keyValue
                        , sendPolice_id
                        , "0"
                        );

                }
                else
                {
                    //选择了多个人
                    string[] ids = sendPolice_id.Split(',');

                    for (int i = 0; i < ids.Length; i++)
                    {
                        sb.AppendFormat(
                          @" insert into JW_SendPolice(SendPolice_id,Unit_id,SendCode,SendUser_id,SendDate,type,Object_id,user_id,state) values(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'); "
                          , unitID
                          , unitShotName + "检警令【" + (Convert.ToInt32(code) + i) + "】号"
                          , ManageProvider.Provider.Current().UserId
                          , DateTime.Now
                          , "7"
                          , keyValue
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
                    int r3 = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text);
                }
                catch (Exception)
                {
                    return "数据异常，派警失败";
                }
                #endregion
            }
            return "审批成功";
        }

        /// <summary>
        /// 更新派警表内OBJECT_ID为 用警表对应ID的 STATE 为5实施完成
        /// </summary>
        /// <param name="keyValue">用警表对应ID</param>
        /// <returns></returns>
        public int UpdatePJState(string keyValue, int PYstate)
        {
            string sql = string.Format(@" update JW_SendPolice set state=1 where object_id ='{0}' ;update JW_OrderPolice set state=" + PYstate + " where orderpolice_id ='{0}' ; ", keyValue);
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 对调警令的处理
        /// </summary>
        /// <param name="jwOrderPolice"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public int SubmitFormMy(JW_OrderPolice jwOrderPolice, string submitType)
        {
            if (submitType == "add")
            {
                string sqlInsert = string.Format(@"insert into JW_OrderPolice(orderpolice_id,to_unit_id,from_unit_id,userdate,userAddress,usernum,userman,userwoman,car,equipment,contacts,contactsTel,addUser,addDate,state,reason,remark,days,renwu) values(@orderpolice_id,@to_unit_id,@from_unit_id,@userdate,@userAddress,@usernum,@userman,@userwoman,@car,@equipment,@contacts,@contactsTel,@addUser,@addDate,@state,@reason,@remark,@days,@renwu)");
                SqlParameter[] pars = new SqlParameter[]
                    {
                        new SqlParameter("@orderpolice_id",jwOrderPolice.orderpolice_id), 
                        new SqlParameter("@to_unit_id",jwOrderPolice.to_unit_id), 
                        new SqlParameter("@from_unit_id",jwOrderPolice.from_unit_id), 
                        new SqlParameter("@userdate",jwOrderPolice.userdate), 
                        new SqlParameter("@userAddress",jwOrderPolice.userAddress), 
                        new SqlParameter("@usernum",jwOrderPolice.usernum), 
                        new SqlParameter("@userman",jwOrderPolice.userman), 
                        new SqlParameter("@userwoman",jwOrderPolice.userwoman), 
                        new SqlParameter("@car",jwOrderPolice.car), 
                        new SqlParameter("@equipment",jwOrderPolice.equipment), 
                        new SqlParameter("@contacts",jwOrderPolice.contacts), 
                        new SqlParameter("@contactsTel",jwOrderPolice.contactsTel), 
                        new SqlParameter("@addUser",jwOrderPolice.addUser), 
                        new SqlParameter("@addDate",jwOrderPolice.addDate), 
                        new SqlParameter("@state",jwOrderPolice.state), 
                        new SqlParameter("@reason",jwOrderPolice.reason), 
                        new SqlParameter("@remark",jwOrderPolice.remark), 
                        new SqlParameter("@days",jwOrderPolice.days), 
                        new SqlParameter("@renwu",jwOrderPolice.renwu)  
                    };
                int r = SqlHelper.ExecuteNonQuery(sqlInsert, CommandType.Text, pars);
                UpdateOrderpoliceNo(jwOrderPolice.orderpolice_id);
                return r;
            }
            else
            {
                string sqlUpdate = string.Format(@"update JW_OrderPolice set to_unit_id=@to_unit_id,from_unit_id=@from_unit_id,userdate=@userdate,userAddress=@userAddress,usernum=@usernum,userman=@userman,userwoman=@userwoman,car=@car,equipment=@equipment,contacts=@contacts,contactsTel=@contactsTel,addUser=@addUser,addDate=@addDate,state=@state,reason=@reason,remark=@remark,days=@days,renwu=@renwu where orderpolice_id=@orderpolice_id");
                SqlParameter[] pars = new SqlParameter[]
                    {
                        new SqlParameter("@orderpolice_id",jwOrderPolice.orderpolice_id), 
                        new SqlParameter("@to_unit_id",jwOrderPolice.to_unit_id), 
                        new SqlParameter("@from_unit_id",jwOrderPolice.from_unit_id), 
                        new SqlParameter("@userdate",jwOrderPolice.userdate), 
                        new SqlParameter("@userAddress",jwOrderPolice.userAddress), 
                        new SqlParameter("@usernum",jwOrderPolice.usernum), 
                        new SqlParameter("@userman",jwOrderPolice.userman), 
                        new SqlParameter("@userwoman",jwOrderPolice.userwoman), 
                        new SqlParameter("@car",jwOrderPolice.car), 
                        new SqlParameter("@equipment",jwOrderPolice.equipment), 
                        new SqlParameter("@contacts",jwOrderPolice.contacts), 
                        new SqlParameter("@contactsTel",jwOrderPolice.contactsTel), 
                        new SqlParameter("@addUser",jwOrderPolice.addUser), 
                        new SqlParameter("@addDate",jwOrderPolice.addDate), 
                        new SqlParameter("@state",jwOrderPolice.state), 
                        new SqlParameter("@reason",jwOrderPolice.reason), 
                        new SqlParameter("@remark",jwOrderPolice.remark), 
                        new SqlParameter("@days",jwOrderPolice.days), 
                        new SqlParameter("@renwu",jwOrderPolice.renwu)  
                    };
                int r = SqlHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, pars);
                UpdateOrderpoliceNo(jwOrderPolice.orderpolice_id);
                return r;
            }
        }

        public int SubmitFormMySendPolice(JW_OrderPolice jwOrderPolice, string sendPolice_id)
        {
            string sqlCheckState = string.Format(@"select * from JW_OrderPolice where orderpolice_id='{0}'", jwOrderPolice.orderpolice_id);
            DataTable dt = SqlHelper.DataTable(sqlCheckState, CommandType.Text);
            if (dt.Rows[0]["state"].ToString() == "2")
            {
                //待分管领导审批
                return 10002;
            }
            if (dt.Rows[0]["state"].ToString() == "-1")
            {
                //法警部门负责人审核退回
                return -10001;
            }
            if (dt.Rows[0]["state"].ToString() == "-2")
            {
                //分管领导审核退回
                return -10002;
            }

            SubmitJW_YJ(jwOrderPolice.orderpolice_id, sendPolice_id);
            string sqlUpdate = string.Format(@" update JW_OrderPolice set FJUser_id=@FJUser_id,FJDetail=@FJDetail,FJDate=@FJDate,state=@state where orderpolice_id=@orderpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@FJUser_id",jwOrderPolice.FJUser_id), 
                new SqlParameter("@FJDetail",jwOrderPolice.FJDetail), 
                new SqlParameter("@FJDate",jwOrderPolice.FJDate),
                new SqlParameter("@state",jwOrderPolice.state), 
                new SqlParameter("@orderpolice_id",jwOrderPolice.orderpolice_id)
            };

            int r = SqlHelper.ExecuteNonQuery(sqlUpdate, CommandType.Text, pars);
            return r;
        }

        public string LoadOrderPoliceSend(string keyValue)
        {
            string user_id = string.Empty;
            string user_name = string.Empty;
            string sql = string.Format(@"
                                            select jsp.*,bu.RealName from JW_SendPolice jsp 
                                            join Base_User bu on jsp.user_id=bu.UserId 
                                            where jsp.Object_id='{0}' and jsp.type=7 and jsp.state in (0,1,2,3)
            ", keyValue);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                user_id = dt.Rows[0]["user_id"].ToString();
                user_name = dt.Rows[0]["RealName"].ToString();
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    user_id += "," + dt.Rows[i]["user_id"];
                    user_name += "," + dt.Rows[i]["RealName"];
                }
                return user_id + "|" + user_name;
            }
            catch (Exception)
            {
                return "|";
            }
        }

        public int SubmitFormSendPoliceConfirm(string orderPoliceId, string sendPoliceId)
        {
            string sql = string.Format(@" 
                                        update JW_OrderPolice set state=4 where orderpolice_id='{0}';
                                        update JW_SendPolice set state=1 where SendPolice_id='{1}';

                                        ", orderPoliceId, sendPoliceId);

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

        public int SubmitFormMyPostFGLD(JW_OrderPolice jwOrderPolice)
        {
            string sql = string.Format(@" update JW_OrderPolice set FJUser_id=@FJUser_id,FJDate=@FJDate,FJDetail=@FJDetail,state=@state where orderpolice_id=@orderpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@FJUser_id",jwOrderPolice.FJUser_id), 
                new SqlParameter("@FJDate",jwOrderPolice.FJDate), 
                new SqlParameter("@FJDetail",jwOrderPolice.FJDetail), 
                new SqlParameter("@state",jwOrderPolice.state), 
                new SqlParameter("@orderpolice_id",jwOrderPolice.orderpolice_id)
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int SubmitFormMySendPoliceJW(JW_OrderPolice jwOrderPolice)
        {
            string sql = string.Format(@" update JW_OrderPolice set LeaderUser_id=@LeaderUser_id,LeaderDate=@LeaderDate,LeaderDetail=@LeaderDetail,state=@state where orderpolice_id=@orderpolice_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@LeaderUser_id",jwOrderPolice.LeaderUser_id), 
                new SqlParameter("@LeaderDate",jwOrderPolice.LeaderDate), 
                new SqlParameter("@LeaderDetail",jwOrderPolice.LeaderDetail), 
                new SqlParameter("@state",jwOrderPolice.state), 
                new SqlParameter("@orderpolice_id",jwOrderPolice.orderpolice_id)
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 结束调警令
        /// </summary>
        /// <param name="keyValue">调警令表对应OrderPolice_id</param>
        /// <returns></returns>
        public int EndOrderPolice(string keyValue)
        {
            int r = 0;
            string sqlSubmit = "update JW_OrderPolice set state=5 where OrderPolice_id='" + keyValue + "';";
            r = SqlHelper.ExecuteNonQuery(sqlSubmit, CommandType.Text);
          
            return r;

        }
    }
}
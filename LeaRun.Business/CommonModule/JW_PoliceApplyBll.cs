//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

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


namespace LeaRun.Business
{
    /// <summary>
    /// JW_PoliceApply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.18 10:00</date>
    /// </author>
    /// </summary>
    public class JW_PoliceApplyBll : RepositoryFactory<JW_PoliceApply>
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
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by apply_id) rowNumber
                                            , * 
                                        from JW_PoliceApply 
                                            where unit_id='{0}' and state<>'5' and adduser_id='{1}' 
                                        ) as a  
                                       "
                        , unit_id
                         , ManageProvider.Provider.Current().UserId
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


        /// <summary>
        /// 获得登陆者信息（仅适用于首页房间图上的单位数，调警令和调警申请中的法警看到的应该是被调单位的树和房间图）
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetLoginUserIdOnlyForIndex(string UserId)
        {

            string sql = "";

            //=====================================处理调警人员开始============================================================================================================

            // 处理调警令的法警---->处理方式：临时改变unit_id   （调警令:sp.type=7 ,  法警本人已确认:sp.state=1 , 调警令待执行状态:：op.state=4）
            string sqlOrderPolice = string.Format(@"
                                 select to_unit_id from JW_SendPolice sp 
                                 join JW_OrderPolice op on sp.Object_id=op.orderpolice_id 
                                 where sp.type=7 and sp.state=1 and op.from_unit_id=(select companyid from base_user where userid='{0}') and op.state=4
                                 and sp.user_id='{0}'
                       ", UserId);
            DataTable dtOrderPolice = SqlHelper.DataTable(sqlOrderPolice, CommandType.Text);

            // 处理调警申请中的法警---->处理方式：临时改变unit_id   （调警令:sp.type=6 ,  法警本人已确认:sp.state=1 , 调警申请待执行状态:：ap.state=4）
            string sqlAssignPolice = string.Format(@"
                                 select to_unit_id from JW_SendPolice sp 
                                  join JW_AssignPolice ap on sp.Object_id=ap.callpolice_id 
                                  join Base_User bu on sp.user_id=bu.UserId
                                  where sp.type=6 and sp.state=1 and ap.from_unit_id=(select companyid from base_user where userid='{0}') and ap.state=4
                                  and sp.user_id='{0}'
                       ", UserId);
            DataTable dtAssignPolice = SqlHelper.DataTable(sqlAssignPolice, CommandType.Text);

            //=====================================处理调警人员结束============================================================================================================


            if (dtOrderPolice != null && dtOrderPolice.Rows.Count > 0)//调警令中法警
            {
                sql = string.Format(@" 
                                select top 1  
                                '{0}', (select unit from base_unit where base_unit_id='{0}'),'!@#$','','{1}','',''
                                from Base_Unit
                                "
                                , dtOrderPolice.Rows[0][0].ToString()
                                , UserId
                                );
            }
            else if (dtAssignPolice != null && dtAssignPolice.Rows.Count > 0)//调警申请中法警
            {
                sql = string.Format(@" 
                                select top 1  
                                '{0}', (select unit from base_unit where base_unit_id='{0}'),'!@#$','','{1}','',''
                                from Base_Unit
                                "
                                , dtAssignPolice.Rows[0][0].ToString()
                                , UserId
                                );
            }
            else
            { 
                 sql = string.Format(@" 
                                select 
                                UNIT.Base_Unit_id, UNIT.unit,DEP.Dep_id,DEP.Name,USE1.UserId,USE1.RealName,Telephone
                                from Base_User USE1
                                join Base_Unit UNIT ON UNIT.Base_Unit_id=USE1.CompanyId
                                join Base_Department DEP  ON DEP.Dep_id=USE1.dep_id
                                WHERE USE1.UserId='{0}' 
                                "
                                , UserId
                                );
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
        /// 获得登陆者信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetLoginUserIdInArea(string UserId, string areaType)
        {
            string sql = string.Format(@" 
                select top 1 bpa.PoliceArea_id,AreaName
                from Base_User USE1
                join Base_Unit UNIT ON UNIT.Base_Unit_id=USE1.CompanyId
                join Base_policearea bpa  ON bpa.unit_id=UNIT.Base_Unit_id
                WHERE USE1.UserId='{0}' and bpa.areatype='{1}'
                "
                , UserId
                , areaType
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
        /// 获得登陆者信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetApplyLianxi(string KeyValue)
        {
            string sql = string.Format(@" 
                select pa.user_lianxi,pa.tel_lianxi  from JW_PoliceApply pa WHERE pa. apply_id='{0}' "
                , KeyValue
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
        /// 获得登陆者信息上级单位
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetLoginUserIdFatherUnit(string UnitId)
        {
            string sql = string.Format(@" 
                select 
                UNITFATHER.Base_Unit_id, UNITFATHER.unit
                from  Base_Unit UNIT 
                LEFT JOIN Base_Unit UNITFATHER 
                ON UNIT.parent_unit_id=UNITFATHER.Base_Unit_id
                WHERE UNIT.Base_Unit_id='{0}' 
                "
                , UnitId
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
        /// 获得回退理由
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetBackReason(string object_id)
        {
            string sql = string.Format(@" 
                select 
                BackReason
                from JW_SendPolice
                
                WHERE object_id='{0}' 
                "
                , object_id
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
        /// 获得单位ID获得name
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetUnitName(string unit_id)
        {
            string sql = string.Format(@" 
                select 
                UNIT.unit
                from Base_Unit UNIT 
                WHERE UNIT.BASE_UNIT_ID='{0}' 
                "
                , unit_id
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
        /// 删除用警申请表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteJWPoliceApply(string apply_id)
        {
            string sql = string.Format(@"update JW_SendPolice set state=-2 where object_id='" + apply_id + "'; delete JW_PoliceApply where apply_id ='" + apply_id + "'");
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
                string sqlGetUnit = string.Format(@" select p.unit_id,u.longname from JW_PoliceApply p join base_unit u on p.unit_id=u.Base_Unit_id where p.apply_id='{0}' ", keyValue);
                string unitID = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["unit_id"].ToString();
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
                        , unitShotName + "检警派【" + code + "】号"
                        , ManageProvider.Provider.Current().UserId
                        , DateTime.Now
                        , "4"
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
                          , unitShotName + "检警派【" + (Convert.ToInt32(code) + i) + "】号"
                          , ManageProvider.Provider.Current().UserId
                          , DateTime.Now
                          , "4"
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
        /// 新增派警表 直接派警
        /// </summary>
        /// <param name="keyValue">派警表中Object_id（存放 用警表对应ID）</param>
        /// <param name="sendPolice_id">警员ID，用逗号分隔</param>
        /// <returns></returns>
        public string SubmitJW_ZJPJ(string keyValue, string sendPolice_id)
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
                string sqlGetUnit = string.Format(@" select p.unit_id,u.longname from JW_PoliceApply p join base_unit u on p.unit_id=u.Base_Unit_id where p.apply_id='{0}' ", keyValue);
                string unitID = SqlHelper.DataTable(sqlGetUnit, CommandType.Text).Rows[0]["unit_id"].ToString();
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
                        , unitShotName + "检警派【" + code + "】号"
                        , ManageProvider.Provider.Current().UserId
                        , DateTime.Now
                        , "5"
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
                          , unitShotName + "检警派【" + (Convert.ToInt32(code) + i) + "】号"
                          , ManageProvider.Provider.Current().UserId
                          , DateTime.Now
                          , "5"
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
            string sql = string.Format(@" update JW_SendPolice set state=" + PYstate + " where object_id ='{0}' and state=1; ", keyValue);
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
        /// 派警编辑取消任务
        /// </summary>
        /// <param name="sendPolice_id"></param>
        /// <param name="state">-2:派警取消任务;  -1 法警确认退回,1:法警确认</param>
        /// <returns></returns>
        public string CancelSendPolice(string sendPolice_id, int state, string backreason)
        {
            string type = "";
            if (state == 1)
            {
                type = "确认";
            }
            if (state == -1)
            {
                type = "退回";
            }
            if (state == -2)
            {
                type = "取消任务";
            }


            string sql = string.Format(@" update JW_SendPolice set state=" + state + ",backreason='" + backreason + "' where SendPolice_id='{0}' ", sendPolice_id);

            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r > 0)
                {
                    return type + "成功";
                }
                else
                {
                    return "数据异常，" + type + "失败";
                }
            }
            catch (Exception)
            {
                return "数据异常，" + type + "失败";

            }
        }

         /// <summary>
        /// 直接派警编辑取消任务
        /// </summary>
        /// <param name="object_id"></param>
        /// <returns></returns>
        public string CancelSendPolice(string object_id)
        {


            string sql = string.Format(@" update JW_SendPolice set state=-2 where object_id='{0}';update JW_PoliceApply set state=0 where apply_id='{0}' ", object_id);

            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                if (r > 0)
                {
                    return  "取消任务成功";
                }
                else
                {
                    return "数据异常，取消任务失败";
                }
            }
            catch (Exception)
            {
                return "数据异常，取消任务失败";

            }
        }
        


        /// <summary>
        ///  查询关联审批表
        /// </summary>
        /// <returns></returns>
        public DataTable CaseInfoListJson(int object_id)
        {
            string sql = "";

            if (object_id == 9 || object_id == 12)
            {
                sql = " select ja.apply_id as case_id,bpa.AreaName+'---'+ja.userName as name  from JW_Apply ja join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id where ja.unit_id='" + ManageProvider.Provider.Current().CompanyId + "'";
                sql = sql + " and ja.state not in (0,5)";
                sql = sql + " and ja.type in (1,2) ";
            }
            else if (object_id == 4)
            {
                sql = " select ja.apply_id as case_id,bpa.AreaName+'---'+ja.userName as name from JW_Apply  ja join Base_PoliceArea bpa on ja.PoliceArea_id=bpa.PoliceArea_id where ja.unit_id='" + ManageProvider.Provider.Current().CompanyId + "'";
                sql = sql + " and ja.state not in (0,5)";
                sql = sql + " and ja.type in (3) ";
            }
            else//绑定案件
            {
                sql += sql + " select cci.case_id,name as name from JW_PoliceApply ja join Case_caseinfo cci on ja.object_id_"+object_id+"=cci.case_id  ";
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
        /// 获取当前已经保存的最新的数据，用警部门
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string GetPoliceApply(string apply_id)
        {
            string sql = String.Format(@"select * from  JW_PoliceApply where apply_id='{0}'", apply_id);
            DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
            if (dt.Rows.Count <= 0)
            {
                return "|";
            }
            else
            {
                return dt.Rows[0]["dep_id"].ToString() + "|" + dt.Rows[0]["depname"].ToString();
            }

        }

        /// <summary>
        /// 获得案件涉案人关联信息
        /// </summary>
        /// <param name="case_id"></param>
        /// <returns></returns>
        public DataTable GetCaseInfo(string case_id)
        {
            string sql = string.Format(@" 
                select cb.name,case cb.sex when '男' then '1' else '2' end sex,cb.age,cb.work,cb.homeaddress,u.RealName
                from Case_caseinfo cc
                join Case_Byinquest cb ON cb.case_id=cc.case_id
                join base_user u ON u.userid=cc.adduser_id
                WHERE cc.case_id='{0}' 
                "
                , case_id
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using LeaRun.Utilities;
using System.Diagnostics;

namespace LeaRun.Business
{
    public class Case_ByinquestBll : RepositoryFactory<Case_Byinquest>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageByinquestJson(string ParameterJson, JqGridParam jqgridparam, string case_id)
        {
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();

                string sqltotal = string.Format(@"
                              select ROW_NUMBER() over(order by Byinquest_id) rowNumber
                                       ,* from Case_Byinquest where case_id='{0}'
                    ", case_id);


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
                        , sqltotal
                        );
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);



                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqltotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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
        /// 表单提交
        /// </summary>
        /// <param name="caseByinquest"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public int SubmitByinquestForm(Case_Byinquest caseByinquest, string submitType)
        {
            string sql = string.Empty;
            if (submitType == "add")
            {
                //新增
                sql =
                    string.Format(
                        @"insert into Case_Byinquest(Byinquest_id,unit_id,dep_id,case_id,adduser_id,adddate,type,name,othername,sex,age,code,nation,work,post,workcall,industry,mobile,homeaddress,homecall,liveaddress,isCommunist,isNPCmember,isCPPCCmembers,isOther,img,state) values(@Byinquest_id,@unit_id,@dep_id,@case_id,@adduser_id,@adddate,@type,@name,@othername,@sex,@age,@code,@nation,@work,@post,@workcall,@industry,@mobile,@homeaddress,@homecall,@liveaddress,@isCommunist,@isNPCmember,@isCPPCCmembers,@isOther,@img,@state)");
            }
            else
            {
                //编辑
                sql = string.Format(@"update Case_Byinquest set unit_id=@unit_id,dep_id=@dep_id,case_id=@case_id,adduser_id=@adduser_id,adddate=@adddate,type=@type,name=@name,othername=@othername,sex=@sex,age=@age,code=@code,nation=@nation,work=@work,post=@post,workcall=@workcall,industry=@industry,mobile=@mobile,homeaddress=@homeaddress,homecall=@homecall,liveaddress=@liveaddress,isCommunist=@isCommunist,isNPCmember=@isNPCmember,isCPPCCmembers=@isCPPCCmembers,isOther=@isOther,img=@img,state=@state where Byinquest_id=@Byinquest_id");


            }
            SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@Byinquest_id",caseByinquest.Byinquest_id), 
                    new SqlParameter("@unit_id",caseByinquest.unit_id), 
                    new SqlParameter("@dep_id",caseByinquest.dep_id), 
                    new SqlParameter("@case_id",caseByinquest.case_id), 
                    new SqlParameter("@adduser_id",caseByinquest.adduser_id), 
                    new SqlParameter("@adddate",caseByinquest.adddate), 
                    new SqlParameter("@type",caseByinquest.type), 
                    new SqlParameter("@name",caseByinquest.name), 
                    new SqlParameter("@othername",caseByinquest.othername), 
                    new SqlParameter("@sex",caseByinquest.sex), 
                    new SqlParameter("@age",caseByinquest.age), 
                    new SqlParameter("@code",caseByinquest.code), 
                    new SqlParameter("@nation",caseByinquest.nation), 
                    new SqlParameter("@work",caseByinquest.work), 
                    new SqlParameter("@post",caseByinquest.post), 
                    new SqlParameter("@workcall",caseByinquest.workcall), 
                    new SqlParameter("@industry",caseByinquest.industry), 
                    new SqlParameter("@mobile",caseByinquest.mobile), 
                    new SqlParameter("@homeaddress",caseByinquest.homeaddress), 
                    new SqlParameter("@homecall",caseByinquest.homecall), 
                    new SqlParameter("@liveaddress",caseByinquest.liveaddress), 
                    new SqlParameter("@isCommunist",caseByinquest.isCommunist), 
                    new SqlParameter("@isNPCmember",caseByinquest.isNPCmember), 
                    new SqlParameter("@isCPPCCmembers",caseByinquest.isCPPCCmembers), 
                    new SqlParameter("@isOther",caseByinquest.isOther), 
                    new SqlParameter("@img",caseByinquest.img), 
                    new SqlParameter("@state",caseByinquest.state)
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
        /// 删除一个数据
        /// </summary>
        /// <param name="case_id"></param>
        /// <returns></returns>
        public int DeleteByinquest(string byinquest_id)
        {
            string sql = string.Format(@"delete Case_Byinquest where Byinquest_id = '{0}'", byinquest_id);
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
    }
}

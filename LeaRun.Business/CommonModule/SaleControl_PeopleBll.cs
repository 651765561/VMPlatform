//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using LeaRun.DataAccess;
using System;

namespace LeaRun.Business
{
    /// <summary>
    /// SaleControl_People
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 10:05</date>
    /// </author>
    /// </summary>
    public class SaleControl_PeopleBll : RepositoryFactory<SaleControl_People>
    {

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetSale(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select om.operationmain_id,r.name as roomName,p.designation,p.name as peopleName,CONVERT(varchar(20), om.adddate, 120)  adddate,om.saletype,om.cash,u.RealName  from Operation_Main om";
                sql = sql + " left join base_room r on om.room_id=r.room_id";
                sql = sql + " left join people p on om.People_id=p.People_id";
                sql = sql + " left join Base_User u on  u.UserId=om.adduser";
                sql = sql + " where om.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                sql = sql + " order by operationmain_id";

                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];//Repository().FindTableBySql(sql);

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
        /// 番号验证
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable checkDesignation(string KeyValue)
        {
            string sql = string.Format(@" 
                select 
                r.name ,p.name as roomName ,p.account,
                isnull(p.limit-(select isnull(sum(cash),0) from Operation_Main where People_id=p.People_id and CONVERT(varchar(7), adddate, 120)=CONVERT(varchar(7), getdate(), 120) group by People_id  ),p.limit) as limit
                from people p
                join Base_room r ON r.room_id=p.room_id
                WHERE p.designation='{0}' and p.state=1 and p.room_id in (select room_id from base_room where user_id='{1}')
                "
                , KeyValue
                , ManageProvider.Provider.Current().UserId
                );
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text,sql ).Tables[0] ;
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 商品验证
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable checkGoods(string KeyValue, string designation)
        {
            string sql = string.Format(@" 
                select 
                g.name ,g.standand,g.unit,g.price
                from base_goods g
                WHERE g.state=1 and 
                g.shortcode='{0}' 
                and g.goods_id not in (select goods_id from People_NoGoods where people_id=(select people_id from  People  where  designation ='{1}'))
                and g.goods_id in (select goods_id from Base_GoodsAreaRelation where Area_id=(select r.Area_id from  people p join base_room r on r.room_id=p.room_id where  designation ='{1}'))
                ", KeyValue
                , designation
                , designation
                ) ;
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text,sql ).Tables [0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }




        //新增个人销售主表
        public int Buy(string operationmain_id, string designation,decimal cash, string saletype)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert Operation_Main (operationmain_id,room_id,people_id,moneytype_id,");
            strSql.Append("adduser,adddate,cash,state,saletype)");
            strSql.Append(" values ('" + operationmain_id + "',(select room_id from people where designation='" + designation + "'),(select people_id from people where designation='" + designation + "'),6,");
            strSql.Append("'" + ManageProvider.Provider.Current().UserId + "',getdate(),"+cash+",0," + saletype + ");");
            strSql.Append("update people set account=account-" + cash + " where people_id=(select people_id from people where designation='" + designation + "')");
            return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }


        //新增个人销售子表
        public int BuyDetail(string operationmain_id, string goodsCode, string goodschoosenum, string outprice)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert Operation_Detail (OperationDetail_id,operationmain_id,goods_id,goodschoosenum,outprice,state)");
            strSql.Append(" values ('" + Guid.NewGuid().ToString() + "','" + operationmain_id + "' ,(select goods_id from Base_Goods where shortcode='" + goodsCode + "')," + goodschoosenum + "," + outprice + ",1)");
            return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }
        
    }
}
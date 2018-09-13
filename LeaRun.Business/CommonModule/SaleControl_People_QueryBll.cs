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
    /// SaleControl_People_Query
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 10:05</date>
    /// </author>
    /// </summary>
    public class SaleControl_People_QueryBll : RepositoryFactory<SaleControl_People_Query>
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
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetSaleQuery(string ParameterJson, JqGridParam jqgridparam, string designation,string area_id, string room_id, string goodscode, string startdate, string enddate, string goodstype)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select distinct om.operationmain_id,r.name as roomName,p.designation,p.name as peopleName,CONVERT(varchar(20), om.adddate, 120)  adddate,om.saletype,om.cash,u.RealName  from Operation_Main om";
                sql = sql + " left join base_room r on om.room_id=r.room_id";
                sql = sql + " left join people p on om.People_id=p.People_id";
                sql = sql + " left join Operation_Detail od on om.operationmain_id=od.operationmain_id";
                sql = sql + " left join base_goods g on  g.goods_id=od.goods_id";
                sql = sql + " left join Base_User u on  u.UserId=om.adduser";
                sql = sql + " where om.room_id in (select room_id from base_room where user_id='" + user_id + "')";

                if (designation != "")//番号
                {
                    sql = sql + " and p.designation like '%" + designation.Trim() + "%'";
                }
                if (area_id != "")//监区
                {
                    sql = sql + " and r.area_id='" + area_id + "'";
                }
                if (room_id != "")//监室
                {
                    sql = sql + " and p.room_id='" + room_id + "'";
                }
                if (goodscode != "")//商品简码
                {
                    sql = sql + " and g.shortcode like '%" + goodscode.Trim() + "%'";
                }
                if (startdate != "")//销售日期开始
                {
                    sql = sql + " and  om.adddate> '" + startdate + "'";
                }
                if (enddate != "")//销售日期结束
                {
                    sql = sql + " and  om.adddate< '" + enddate + "'";
                }
                if (goodstype != "")//商品类型
                {
                    sql = sql + " and  g.goodstype_id= '" + goodstype + "'";
                }
               //  sql = sql + " order by om.adddate " + jqgridparam.sord;


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
        /// 列表加载明细TITLE
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetSaleDetailTitle(string KeyValue)
        {
            string sql = "select (p.name + '(' + p.designation + ')') AS peopleName, r.name AS roomName,om.cash,";
            sql = sql + " CONVERT(varchar(10), om.adddate, 120) AS adddate,u.RealName AS policeName,";
            sql = sql + " case  CONVERT(nvarchar(1),om.saletype) when '1' then '普通销售' when '2' then '三无销售' when '0' then '作废/删除' when '-1' then '普通红单' when '-2' then '三无红单' end AS saletype";
            sql = sql + " from Operation_Detail od";
            sql = sql + " left join Operation_Main om on od.operationmain_id=om.operationmain_id";
            sql = sql + " left join people p on om.People_id=p.People_id";
            sql = sql + " left join base_room r on om.room_id=r.room_id";
            sql = sql + " left join Base_User u on om.adduser=u.UserId";

            sql = sql + " where od.operationmain_id='" + KeyValue + "'";
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// 列表加载明细
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetSaleDetail(string ParameterJson, JqGridParam jqgridparam,string KeyValue)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select g.shortcode ,g.name,g.name ,g.price,case ISNULL(od.goodsnum,'') when '' then od.goodschoosenum else od.goodsnum end as goodsnum from Operation_Detail od";
                sql = sql + " left join base_goods g on od.goods_id=g.goods_id";
                sql = sql + " where od.operationmain_id='"+KeyValue+"'";
              

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


        //获得打印的内容数据
        public DataTable GetPintStr(string KeyValue)
        {
            try
            {
                 string sql = "select g.code ,g.name,g.name ,g.price,case ISNULL(od.goodsnum,'') when '' then od.goodschoosenum else od.goodsnum end as goodsnum from Operation_Detail od";
                 sql = sql + " left join base_goods g on od.goods_id=g.goods_id";
                 sql = sql + " where od.operationmain_id='"+KeyValue+"'";
                return  DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];//Repository().FindTableBySql(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }
        //获得打印的内容数据(ALL)
        public DataTable PrintDetailALL(string KeyValue)
        {
            try
            {
                string user_id = ManageProvider.Provider.Current().UserId;

                string sql = @"  select om.operationmain_id,r.name as roomName,p.designation,p.name as peopleName,CONVERT(varchar(20), om.adddate, 120)  
                                 adddate,om.saletype,om.cash,u.RealName,op.outprice,g.name,g.unit,g.price,g.standand,g.code,g.standand,op.goodsnum
                                 from Operation_Main om left join base_room r on om.room_id=r.room_id 
                                 left join people p on om.People_id=p.People_id left join Base_User u on 
                                 u.UserId=om.adduser 
                                 left join Operation_Detail op on om.operationmain_id=op.operationmain_id
                                 left join Base_Goods g  on op.goods_id=g.goods_id 
                                 where om.room_id in
                                 (select room_id from base_room where user_id='6cf6743a-b7c5-4ec3-ba40-f333ebd3de01')
                                 and om.operationmain_id in(" + KeyValue+@")  
                                 order by operationmain_id";
                return DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];//Repository().FindTableBySql(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获得打印的表头和结束数据
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public DataTable GetPeopleInfo(string KeyValue)
        {
            StringBuilder str = new StringBuilder();
            str.Append(" select ");
            str.Append(" op.operationmain_id, pe.people_id,pe.code,pe.designation,pe.name,(op.cash+op.account)money,op.account,op.cash,Convert(char(10),OP.adddate,120)as adddate,OP.adduser,us.RealName,pe.room_id,b.Code as Bcode,b.name as Bname ");
            str.Append(" from ");
            str.Append(" Operation_Main op left join people pe on op.People_id=pe.People_id left join base_room  b on pe.room_id=b.room_id left join base_user us on OP.adduser=us.userId ");
            str.Append(" where op.operationmain_id ='" + KeyValue + "'");
            return DbHelper.GetDataSet(CommandType.Text, str.ToString()).Tables[0];
        }

       
    }
}
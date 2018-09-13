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
    /// SaleControl_ChangeSend
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 10:05</date>
    /// </author>
    /// </summary>
    public class SaleControl_ChangeSendBll : RepositoryFactory<SaleControl_ChangeSend>
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
                string sql = "select om.operationmain_id,r.name as roomName,p.designation,p.name as peopleName,CONVERT(varchar(20), om.adddate, 120)  adddate,om.saletype,om.cash,u.RealName,u.RealName  from Operation_Main om";
                sql = sql + " left join base_room r on om.room_id=r.room_id";
                sql = sql + " left join people p on om.People_id=p.People_id";
                sql = sql + " left join Base_User u on  u.UserId=om.actuser";
                sql = sql + " where om.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                sql = sql + " and om.room_id <> p.room_id and  om.state=0 and p.state = 1 ";//换监室 and 商品未发放 and 在押
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
                sql = sql + " left join Base_User u on  u.UserId=om.actuser";
                sql = sql + " where om.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                sql = sql + " and om.room_id <> p.room_id and  om.state=0 and p.state = 1 ";//换监室 and 商品未发放 and 在押

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

        
    }
}
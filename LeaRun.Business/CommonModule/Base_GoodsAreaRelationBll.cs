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
using System.Data;
using System;
using System.Diagnostics;
using LeaRun.DataAccess;

namespace LeaRun.Business
{
    /// <summary>
    /// Base_GoodsAreaRelation
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.16 14:27</date>
    /// </author>
    /// </summary>
    public class Base_GoodsAreaRelationBll : RepositoryFactory<Base_GoodsAreaRelation>
    {

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageJsonMyQuery(string ParameterJson, JqGridParam jqgridparam)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;

            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "SELECT Base_Goods.goods_id ,Base_Goods.code,Base_Goods.shortcode,Base_Goods.name,Base_Goods.shortname, " +
              "Base_Goods.standand,Base_Goods.unit,Base_GoodsType.name As goodstypename,Base_Goods.price,Base_Goods.islimit,Base_Goods.imgurl,Base_Goods.filetype,Base_Goods.limitnum,Base_Goods.state" +
              " FROM Base_Goods" +
              " LEFT JOIN Base_GoodsType" +
              " ON Base_Goods.goodstype_id=Base_GoodsType.goodstype_id";
                

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
        //新增关联
        public string update(string areaName, string goodsId)
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                string sql = "if  ( not exists(select goods_id from Base_GoodsAreaRelation where Area_id=(select Area_id from Base_Area where name='" + areaName + "')  and goods_id ='" + goodsId + "' ) )"
                + " insert into Base_GoodsAreaRelation(GoodsAreaRelation_id,Area_id,goods_id) values('" + id + "',(select Area_id from Base_Area where name='" + areaName + "'),'" + goodsId + "') ";
                
                DataTable dt = Repository().FindTableBySql(sql);
                return "success";
            }
            catch (Exception)
            {
                return null;
            }

        }
        //删除关联
        public string  deleteMy(string areaName, string goodsId) 
        {
            try
            {
                string id = Guid.NewGuid().ToString();
                string sql = "if  (  exists(select goods_id from Base_GoodsAreaRelation where Area_id=(select Area_id from Base_Area where name='" + areaName + "')  and goods_id ='" + goodsId + "' ) )"
                + " delete from Base_GoodsAreaRelation where Area_id=(select Area_id from Base_Area where name='" + areaName + "')  and goods_id ='" + goodsId + "' ";

                DataTable dt = Repository().FindTableBySql(sql);
                return "success";
            }
            catch (Exception)
            {
                return null;
            }
        }
        //获取关联
        public DataTable getGoodsAreaRelation(string areaName)
        {
            try
            {
                string sql = "select goods_id from Base_GoodsAreaRelation where Area_id = (select Area_id from Base_Area where  name = '"+ areaName +"') ";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable getArea()
        {
            try
            {
                string sql = "select name from Base_Area ";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
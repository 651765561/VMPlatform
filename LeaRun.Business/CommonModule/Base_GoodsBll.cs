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
using System;
using System.Diagnostics;
using System.Data;
using LeaRun.DataAccess;

namespace LeaRun.Business
{
    /// <summary>
    /// Base_Goods
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.13 08:51</date>
    /// </author>
    /// </summary>
    public class Base_GoodsBll : RepositoryFactory<Base_Goods>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetGoods(string ParameterJson, JqGridParam jqgridparam,string queryMessage)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
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
                // sql = sql + " order by Base_Room.room_id";

                if (queryMessage != "" && queryMessage!=null)
                {
                    sql = sql + " where Base_Goods.code like  '%" + queryMessage +
                        "%' or Base_Goods.shortcode like '%" + queryMessage +
                        "%' or Base_Goods.name like '%" + queryMessage +
                        "%' or Base_Goods.unit like  '%" + queryMessage +
                        "%' or Base_Goods.price like '%" + queryMessage +
                        "%' or Base_GoodsType.name like '%" + queryMessage +"%'";
                        
                }

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
        //添加商品
        public int AddGoods(Base_Goods goods, string strKeyValue)
        {
            if (strKeyValue == "")//新增
            {
                //string sql =
                //     string.Format(@"insert into Base_Room(Name,Code,Area_id,User_id ) values (@name,@code,@areaid,@userid)");
                string id = Guid.NewGuid().ToString();
                string sql = "insert into Base_Goods(goods_id,code,shortcode,name,shortname,standand,unit,goodstype_id,price,state,islimit)" +
                    "values ('" + id + "', '" +
                    goods.code + "','" + 
                    goods.shortcode + "','"+ 
                    goods.name  +  "','"+
                    goods.shortname  +  "','"+
                    goods.standand  +  "','"+
                    goods.unit  +  "',"+
                    "(select goodstype_id from Base_GoodsType where name='" + goods.goodstype_id + "'),'" +
                    goods.price +  "','"+
                    goods.state  +  "','"+
                    goods.islimit  +  "')";
                                                         
                //SqlParameter[] pars = new SqlParameter[]
                //{
                //    new SqlParameter("@name",room.Name),
                //    new SqlParameter("@code",room.Code),
                //    new SqlParameter("@areaid",room.Area_id),
                //    new SqlParameter("@userid",room.User_id),


                //};

                try
                {

                    int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else
            {
                string sql = "update Base_Goods set code='" + goods.code +
                    "',shortcode='" + goods.shortcode +
                    "',name='" + goods.name +
                    "',shortname='" + goods.shortname +
                    "',standand='" + goods.standand +
                    "',unit='" + goods.unit +
                    "',goodstype_id=(select goodstype_id from Base_GoodsType where name='" + goods.goodstype_id + "')" +
                    ",price='" + goods.price +
                    "',islimit='" + goods.islimit +
                    "',limitnum='" + goods.limitnum +
                    "',state='" + goods.state +                  
                    "' where Base_Goods.goods_id='" + strKeyValue + "'";

                //string.Format(@"update Base_Room set Name=@name,Code=@code,Area_id=@areaid,User_id=@userid where rom_id_id=@KeyValue");

                //SqlParameter[] pars = new SqlParameter[]
                //{
                //    new SqlParameter("@name",room.Name),
                //    new SqlParameter("@code",room.Code),
                //    new SqlParameter("@areaid",room.Area_id),
                //    new SqlParameter("@userid",room.User_id),

                //    new SqlParameter("@KeyValue",strKeyValue)

                //};

                try
                {
                    int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        //获取指定行数据并获取从表相应数据
        public DataTable getRowData(string KeyValue)
        {
            // Stopwatch watch = CommonHelper.TimerStart();
            try
            {
                string sql = "SELECT Base_Goods.goods_id ,Base_Goods.code,Base_Goods.shortcode,Base_Goods.name,Base_Goods.shortname, " +
                "Base_Goods.standand,Base_Goods.unit,Base_GoodsType.name As goodstypename,Base_Goods.price,Base_Goods.islimit,Base_Goods.imgurl,Base_Goods.filetype,Base_Goods.limitnum,Base_Goods.state" +
                " FROM Base_Goods" +
                " LEFT JOIN Base_GoodsType" +
                " ON Base_Goods.goodstype_id=Base_GoodsType.goodstype_id"+
                " where Base_Goods.goods_id='"+ KeyValue +"'";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }

            catch (Exception)
            {
                return null;
            }


        }
       
        public DataTable getType()
        {
            try
            {
                string sql = "select name from Base_GoodsType ";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DataTable getState()
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
        public DataTable getIsLimit()
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

        //根据id删除数据
        public int del(string KeyValue)
        {
            string sql = string.Format(@" delete from Base_Goods where goods_id ='" + KeyValue + "'");
            try
            {
                int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //上传图片
        public int upload(string KeyValue,string path)
        {
            string sql = "update  Base_Goods set imgurl='" + path + "',filetype='jpg' where goods_id='" + KeyValue + "'";
            try
            {
                int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        //获取图片路径
        public DataTable getImageUrl(string KeyValue)
        {
        
               try
            {
                string sql = "select imgurl  from Base_Goods where goods_id='" + KeyValue + "'";
                DataTable dt = Repository().FindTableBySql(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取对应商品类型
        /// </summary>
        /// <param name="sql1"></param>
        /// <returns></returns>
        public DataTable Getgoodstype(string sql1)
        {
            try
            {
              
            //    DataTable dt = Repository().FindTableBySql(sql1);
                return DbHelper.GetDataSet(CommandType.Text, sql1).Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public int ExecuteSql(string sql)
        {
            try
            {
                return DbHelper.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception)
            {

                return 0;
            }
         
        }
    }
}
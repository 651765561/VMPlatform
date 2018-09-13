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
    /// SaleControl_Room
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 10:05</date>
    /// </author>
    /// </summary>
    public class SaleControl_RoomBll : RepositoryFactory<SaleControl_Room>
    {

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetQuery(string ParameterJson, JqGridParam jqgridparam)
        {

            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select orm.OperationRoomMain_id,orm.room_id,orm.adduser,CONVERT(varchar(20), orm.adddate, 120)  adddate,";
                sql = sql + " case orm.state when 0 then '待销售' when 1 then '待发放' when 2 then '已发放' end state,r.Name as roomName,u.RealName from Operation_RoomMain orm";
                sql = sql + " left join base_room r on orm.room_id=r.room_id";
                sql = sql + " left join Base_User u on  u.UserId=orm.adduser";
                sql = sql + " and orm.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                sql = sql + " order by adddate";


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
        /// 新增监室销售初始(jqgrid方式)
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string AddRoomSale(string ParameterJson, JqGridParam jqgridparam)
        {

            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select p.People_id,p.code,p.name,p.room_id,r.Name as roomName,account,";
                sql = sql + " p.limit-(select isnull(sum(cash),0) from Operation_Main where People_id=p.People_id and CONVERT(varchar(7), adddate, 120)=CONVERT(varchar(7), getdate(), 120) group by People_id  ) ";//,p.People_id,p.code，p.room_id,
                sql = sql + " as canuse from People p";
                sql = sql + " left join base_room r on p.room_id=r.room_id";
                sql = sql + " and p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                sql = sql + " where 1=2 ";
                sql = sql + " order by People_id";

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
        /// 新增监室销售(jqgrid方式)
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string AddRoomSaleParam(string ParameterJson, JqGridParam jqgridparam, string room_id)
        {

            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select p.People_id,p.code,p.name,p.room_id,r.Name as roomName,account,";
                sql = sql + " p.limit-(select isnull(sum(cash),0) from Operation_Main where People_id=p.People_id and CONVERT(varchar(7), adddate, 120)=CONVERT(varchar(7), getdate(), 120) group by People_id  ) ";//,p.People_id,p.code，p.room_id,
                sql = sql + " as canuse from People p";
                sql = sql + " left join base_room r on p.room_id=r.room_id";
                sql = sql + " where p.room_id='" + room_id + "'";
                sql = sql + " and p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
                sql = sql + " order by People_id";

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
        /// 新增监室销售(动态表格方式--空)
        /// </summary>
        public string AddRoomSaleTable(string room_id)
        {
            string user_id = ManageProvider.Provider.Current().UserId;

            string sqlPeople = "";
            sqlPeople = "select r.Name as roomName,p.designation,p.name,p.account,";
            sqlPeople = sqlPeople + " isnull(p.limit-(select isnull(sum(cash),0) from Operation_Main where People_id=p.People_id and CONVERT(varchar(7), adddate, 120)=CONVERT(varchar(7), getdate(), 120) group by People_id  ),p.limit) ";//,p.People_id,p.code，p.room_id,
            sqlPeople = sqlPeople + " as canuse from People p";
            sqlPeople = sqlPeople + " left join base_room r on p.room_id=r.room_id";
            sqlPeople = sqlPeople + " where p.room_id='" + room_id + "' and p.state=1";
            sqlPeople = sqlPeople + " and p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
            sqlPeople = sqlPeople + " order by People_id";
            DataTable dtPeople = DbHelper.GetDataSet(CommandType.Text, sqlPeople).Tables[0];    

            string sqlGoods = "";
            sqlGoods = "select g.name,g.price,g.goods_id from base_goods g";
            sqlGoods = sqlGoods + " inner join Base_GoodsAreaRelation r on g.goods_id=r.goods_id and  Area_id = (select Area_id from base_room where room_id='"+room_id+"')";
            //sqlGoods = sqlGoods + " where p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
            sqlGoods = sqlGoods + " order by g.goods_id";
           
            try
            {
                DataTable dtGoods = DbHelper.GetDataSet(CommandType.Text, sqlGoods).Tables[0];    
                string returnHtml = string.Empty;
                returnHtml += "<table id='tableSale' border=1>";
                //==========================================画表格头开始==================================================================
                returnHtml += "<tr>";

                returnHtml += "<td style='height:25px;text-align:center; width:50px'>监室";
                returnHtml += "</td>";
                returnHtml += "<td style='height:25px;text-align:center; width:50px'>番号";
                returnHtml += "</td>";
                returnHtml += "<td style='height:25px;text-align:center;width:50px'>姓名";
                returnHtml += "</td>";
                returnHtml += "<td style='height:25px;text-align:center;width:50px'>余额";
                returnHtml += "</td>";
                returnHtml += "<td style='height:25px;text-align:center;width:50px'>可用额";
                returnHtml += "</td>";
                for (int i = 0; i <= dtGoods.Rows.Count - 1; i++)
                {
                    //returnHtml += "<td  align='left' style=\"cursor:hand;background-color:#842B00;color:white\"><b> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</b></br></br>" + dt.Rows[i]["userName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + policeName;
                    //returnHtml += "</td>";
                    returnHtml += "<td style='height:25px;text-align:center;width:50px'>" + dtGoods.Rows[i]["name"].ToString() + "<br/>￥" + dtGoods.Rows[i]["price"].ToString();
                    returnHtml += "</td>";
                }
               
                returnHtml += "</tr>";
                //==========================================画表格头结束==================================================================
               
                //==========================================画表格身体开始==================================================================
                string sqlNoGoods = "";
                DataTable dtNoGoods = null;
                for (int i = 0; i <= dtPeople.Rows.Count - 1; i++)
                {
                   

                    returnHtml += "<tr>";
                    for (int j = 0; j <= dtPeople.Columns.Count - 1; j++)
                    {
                        //returnHtml += "<td  align='left' style=\"cursor:hand;background-color:#842B00;color:white\"><b> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</b></br></br>" + dt.Rows[i]["userName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + policeName;
                        //returnHtml += "</td>";
                        returnHtml += "<td >" + dtPeople.Rows[i][j].ToString();
                        returnHtml += "</td>";
                    }
                    for (int k = 0; k <= dtGoods.Rows.Count - 1; k++)
                    {
                        sqlNoGoods = "";
                        sqlNoGoods = "select people_id from People_NoGoods where people_id=(select people_id from people where designation='" + dtPeople.Rows[i]["designation"].ToString() + "')";
                        sqlNoGoods = sqlNoGoods + " and goods_id ='" + dtGoods.Rows[k]["goods_id"].ToString() + "'";
                        dtNoGoods = DbHelper.GetDataSet(CommandType.Text, sqlNoGoods).Tables[0];
                        if (dtNoGoods == null || dtNoGoods.Rows.Count==0)
                        {
                            returnHtml += "<td ><input type='text' style='width:50px' '/>";
                            returnHtml += "</td>";
                        }
                        else
                        {
                            returnHtml += "<td ><input type='text' style='width:50px; text-align=center' value='X' readonly='readonly'/>";
                            returnHtml += "</td>";
                        }
                    }
                    returnHtml += "</tr>";
                }
                //==========================================画表格身体结束=================================================================
                returnHtml += "</table>";    
                  
                return returnHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 查看监室销售(动态表格方式--头信息)
        /// </summary>
        /// <param name="OperationRoomMain_id"></param>
        /// <returns></returns>
        public DataTable AddRoomSaleTableForDetailTital(string OperationRoomMain_id)
        {
            string sql = string.Format(@" 
                select 
                r.name,u.realName,CONVERT(varchar(10), adddate, 120) adddate
                from Operation_RoomMain orm
                left join base_room r on r.room_id=orm.room_id
                left join base_user u on u.userid=orm.adduser 
                WHERE orm.OperationRoomMain_id='{0}' 
                "
                , OperationRoomMain_id
                );
            try
            {
                DataTable dt = DbHelper.GetDataSet( CommandType.Text,sql).Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 查看监室销售(动态表格方式--有数据)
        /// </summary>
        public string AddRoomSaleTableForDetail(string OperationRoomMain_id)
        {

            string sqlPeople = "";
            sqlPeople = "select p.People_id,max(p.name) name from Operation_RoomMain orm";
            sqlPeople = sqlPeople + " inner join Operation_Main om on orm.OperationRoomMain_id=om.OperationRoomMain_id";
            sqlPeople = sqlPeople + " inner join people p on om.people_id=p.people_id";
            sqlPeople = sqlPeople + " where orm.OperationRoomMain_id='" + OperationRoomMain_id + "' ";
            sqlPeople = sqlPeople + " group by p.People_id";
            sqlPeople = sqlPeople + " order by p.People_id";
            DataTable dtPeople = DbHelper.GetDataSet(CommandType.Text, sqlPeople).Tables[0];


            string sqlGoods = "";
            sqlGoods = "select g.goods_id,max(g.name) name, max(g.price) price from Operation_RoomMain orm";
            sqlGoods = sqlGoods + " inner join Operation_Main om on orm.OperationRoomMain_id=om.OperationRoomMain_id";
            sqlGoods = sqlGoods + " inner join Operation_Detail od on om.OperationMain_id=od.OperationMain_id";
            sqlGoods = sqlGoods + " inner join base_goods g on od.goods_id=g.goods_id";
            sqlGoods = sqlGoods + " where orm.OperationRoomMain_id='" + OperationRoomMain_id + "' ";
            sqlGoods = sqlGoods + " group by g.goods_id";
            sqlGoods = sqlGoods + " order by g.goods_id";
            string sqlPeopleGoods = "";
            DataTable dtPeopleGoods = null;
            try
            {
                DataTable dtGoods = DbHelper.GetDataSet(CommandType.Text, sqlGoods).Tables[0];    
                string returnHtml = string.Empty;
                returnHtml += "<table id='tableSale' border=1>";
                //==========================================画表格头开始==================================================================
                returnHtml += "<tr>";
                returnHtml += "<td style='height:25px;text-align:center;width:50px'>姓名";
                returnHtml += "</td>";
              
                for (int i = 0; i <= dtGoods.Rows.Count - 1; i++)
                {
                    //returnHtml += "<td  align='left' style=\"cursor:hand;background-color:#842B00;color:white\"><b> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</b></br></br>" + dt.Rows[i]["userName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + policeName;
                    //returnHtml += "</td>";
                    returnHtml += "<td style='height:25px;text-align:center;width:50px'>" + dtGoods.Rows[i]["name"].ToString() + "<br/>￥" + dtGoods.Rows[i]["price"].ToString();
                    returnHtml += "</td>";
                }

                returnHtml += "</tr>";
                //==========================================画表格头结束==================================================================

                //==========================================画表格身体开始==================================================================

                for (int i = 0; i <= dtPeople.Rows.Count - 1; i++)
                {
                    returnHtml += "<tr>";
                    for (int j = 1; j <= dtPeople.Columns.Count - 1; j++)//去掉第一列
                    {
                        //returnHtml += "<td  align='left' style=\"cursor:hand;background-color:#842B00;color:white\"><b> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</b></br></br>" + dt.Rows[i]["userName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + policeName;
                        //returnHtml += "</td>";
                        returnHtml += "<td >" + dtPeople.Rows[i][j].ToString();
                        returnHtml += "</td>";
                    }
                    for (int k = 0; k <= dtGoods.Rows.Count - 1; k++)
                    {
                        sqlPeopleGoods = "select isnull(goodsnum,0) from Operation_RoomMain orm";
                        sqlPeopleGoods = sqlPeopleGoods + " inner join Operation_Main om on orm.OperationRoomMain_id=om.OperationRoomMain_id";
                        sqlPeopleGoods = sqlPeopleGoods + " inner join Operation_Detail od on om.OperationMain_id=od.OperationMain_id";
                        sqlPeopleGoods = sqlPeopleGoods + " inner join base_goods g on od.goods_id=g.goods_id";
                        sqlPeopleGoods = sqlPeopleGoods + " where orm.OperationRoomMain_id='" + OperationRoomMain_id + "' ";
                        sqlPeopleGoods = sqlPeopleGoods + " and od.goods_id='" + dtGoods.Rows[k]["goods_id"].ToString () + "' ";
                        sqlPeopleGoods = sqlPeopleGoods + " and om.people_id='" + dtPeople.Rows[i]["people_id"].ToString() + "' ";
                        dtPeopleGoods = DbHelper.GetDataSet(CommandType.Text, sqlPeopleGoods).Tables[0];    
                
                        //returnHtml += "<td  align='left' style=\"cursor:hand;background-color:#842B00;color:white\"><b> &nbsp;" + dt.Rows[i]["RoomName"].ToString() + "</b></br></br>" + dt.Rows[i]["userName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + policeName;
                        //returnHtml += "</td>";
                        if ((dtPeopleGoods != null) && dtPeopleGoods.Rows.Count>0 && Convert.ToInt32(dtPeopleGoods.Rows[0][0]) > 0)
                        {
                            returnHtml += "<td style='text-align=center'>" + dtPeopleGoods.Rows[0][0].ToString();
                            returnHtml += "</td>";
                        }
                        else 
                        {
                            returnHtml += "<td style='width:50px; text-align=center'>&nbsp;";
                            returnHtml += "</td>";
                        }
                    }
                    returnHtml += "</tr>";
                }
                //==========================================画表格身体结束=================================================================
                returnHtml += "</table>";

                return returnHtml;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //新增监室销售表对应销售主表
        public int RoomBuy(string OperationRoomMain_id, string room_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert Operation_RoomMain (OperationRoomMain_id,room_id,adduser,adddate,state)");
            strSql.Append(" values ('" + OperationRoomMain_id + "','" + room_id + "','" + ManageProvider.Provider.Current().UserId + "',getdate(),1);");
            return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }



        //新增监室销售表对应销售主表
        public int Buy(string operationmain_id, string designation, decimal cash, string OperationRoomMain_id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert Operation_Main (operationmain_id,room_id,people_id,moneytype_id,");
            strSql.Append("adduser,adddate,cash,account,state,saletype,OperationRoomMain_id)");
            strSql.Append(" values ('" + operationmain_id + "',(select room_id from people where designation='" + designation + "'),(select people_id from people where designation='" + designation + "'),6,");
            strSql.Append("'" + ManageProvider.Provider.Current().UserId + "',getdate()," + cash + ",(select account-" + cash + " from people where people_id=(select people_id from people where designation='" + designation + "') ),0,0,'" + OperationRoomMain_id + "');");
            strSql.Append("update people set account=account-" + cash + " where people_id=(select people_id from people where designation='" + designation + "')");
            return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }

        //新增监室销售表对应销售子表
        public int BuyDetail(string operationmain_id, string goodsName, string goodschoosenum, string outprice)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert Operation_Detail (OperationDetail_id,operationmain_id,goods_id,goodschoosenum,goodsnum,outprice,state)");
            strSql.Append(" values ('" + Guid.NewGuid().ToString() + "','" + operationmain_id + "' ,(select top 1 goods_id from Base_Goods where name='" + goodsName + "')," + goodschoosenum + "," + goodschoosenum + "," + outprice + ",1)");
            return DbHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString());
        }

        /// <summary>
        /// 获取打印的所有商品名称
        /// </summary>
        /// <returns></returns>
        public DataTable GetPintStr()
        {
            StringBuilder str = new StringBuilder();
            str.Append(" select name,price from Base_Goods ");
            return DbHelper.GetDataSet(CommandType.Text, str.ToString()).Tables[0];
        }
        /// <summary>
        ///  需要打印的单据的人
        /// </summary>
        /// <returns></returns>
        public DataTable GetPintStrR()
        {
            StringBuilder str = new StringBuilder();
            str.Append(" select ro.Code,p.designation,p.name from People p left join  Base_Room ro  on p.room_id=ro.room_id where ro.Code is not null");
            return DbHelper.GetDataSet(CommandType.Text, str.ToString()).Tables[0];
        }
    }
}
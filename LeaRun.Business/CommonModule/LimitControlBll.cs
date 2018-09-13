//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
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
using LeaRun.DataAccess;
using System.Data.Common;

namespace LeaRun.Business
{
    /// <summary>
    /// base_area
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.03 14:55</date>
    /// </author>
    /// </summary>
    public class LimitControlBll : RepositoryFactory<Base_Area>
    {
        /// <summary>
        /// 绑定房间或对象信息
        /// </summary>
        /// <param name="type">room：房间信息；people：对象信息</param>
        /// <param name="room_id"></param>
        /// <returns></returns>
        public DataTable GetData(string user_id,string type,string key_id)
        {
            string sql = "";
            if (type == "area")
            {

                sql = " select Area_id as code,name from base_area";

            }
            else if (type == "room")
            {

                sql = " select room_id as code,name from base_room where User_id='" + user_id + "' and area_id='" + key_id + "'";
               
            }
            else
            {
                sql = " select People_id as code ,name from people where room_id ='" + key_id + "'";
            }
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text,sql).Tables [0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }



        /// <summary>
        /// 更新限额
        /// </summary>
        /// <param name="user_id">民警ID</param>
        /// <param name="type">房间或对象</param>
        /// <param name="dLimit"></param>
        /// <returns></returns>
        public int UpdateLimit(string updateObject, string type, double dLimit)
        {
            string sql = "";
            if (type == "room")
            {
                sql = " update people set limit=" + dLimit + "  where room_id in  ( select room_id from base_room where User_id='" + updateObject + "')";
            }
            else
            {
                sql = " update people set limit=" + dLimit + "  where people_id ='" + updateObject + "'";
            }
           

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
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeaRun.Repository;
using LeaRun.Entity;
using System.Data;
using System.Data.SqlClient;

namespace LeaRun.Business
{
    public partial class JW_GoodsMain_XJBll : RepositoryFactory<JW_GoodsMain>
    {
        /// <summary>
        /// 加载主表信息
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public DataTable SetGoodsMain(string apply_id)
        {
            string sql = string.Format(@" select * from JW_GoodsMain where apply_id='{0}'", apply_id);
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
        /// 提交主表信息
        /// </summary>
        /// <param name="jwGoodsMain"></param>
        /// <returns></returns>
        public int SubmitGoodsMain(JW_GoodsMain jwGoodsMain)
        {
            //拿到初始需要的数据
            string sqlSelectApply = string.Format(@"select * from JW_Apply where apply_id='{0}'", jwGoodsMain.apply_id);
            try
            {
                DataTable dtSelectApply = SqlHelper.DataTable(sqlSelectApply, CommandType.Text);
                //判断主表中是否有当前申请单的记录，
                string sqlCheckMain = string.Format(@"select * from JW_GoodsMain where apply_id='{0}'", jwGoodsMain.apply_id);
                DataTable dtCheckMain = SqlHelper.DataTable(sqlCheckMain, CommandType.Text);

                if (dtCheckMain.Rows.Count <= 0)
                {
                    //没有数据
                    string sqlInsertMain = string.Format(@"insert into JW_GoodsMain(
                                            goodsmain_id,unit_id,PoliceArea_id,apply_id,getuser_id,getDate,LockersNum,LockersPsw,backuser_id,backDate,detail,state
                                )values(
                                            NEWID(),@unit_id,@PoliceArea_id,@apply_id,@getuser_id,@getDate,@LockersNum,@LockersPsw,@backuser_id,@backDate,@detail,@state
                                )");
                    SqlParameter[] pars = new SqlParameter[]
                    {
                        new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                        new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                        new SqlParameter("@apply_id",jwGoodsMain.apply_id),
                        new SqlParameter("@getuser_id",jwGoodsMain.getuser_id), 
                        new SqlParameter("@getDate",jwGoodsMain.getDate==null?(object)DBNull.Value:jwGoodsMain.getDate), 
                        new SqlParameter("@LockersNum",jwGoodsMain.LockersNum),
                        new SqlParameter("@LockersPsw",jwGoodsMain.LockersPsw), 
                        new SqlParameter("@detail",jwGoodsMain.detail),
                        new SqlParameter("@backuser_id",jwGoodsMain.backDate==null?(object)DBNull.Value:jwGoodsMain.backuser_id), 
                        new SqlParameter("@backDate",jwGoodsMain.backDate==null?(object)DBNull.Value:jwGoodsMain.backDate), 
                        new SqlParameter("@state",jwGoodsMain.backDate==null?1:2)
                    };
                    int r = SqlHelper.ExecuteNonQuery(sqlInsertMain, CommandType.Text, pars);
                    return r;
                }
                else
                {
                    //有数据
                    string sqlUpdateMain = string.Format(@"update JW_GoodsMain set 
                                                 unit_id=@unit_id
                                                ,PoliceArea_id=@PoliceArea_id
                                                ,apply_id=@apply_id
                                                ,getuser_id=@getuser_id
                                                ,getDate=@getDate
                                                ,LockersNum=@LockersNum
                                                ,LockersPsw=@LockersPsw
                                                ,backuser_id=@backuser_id
                                                ,backDate=@backDate
                                                ,detail=@detail
                                                ,state=@state 
                                            where
                                                goodsmain_id=@goodsmain_id
                                            ");
                    SqlParameter[] pars = new SqlParameter[]
                    {
                        new SqlParameter("@unit_id",dtSelectApply.Rows[0]["unit_id"]),
                        new SqlParameter("@PoliceArea_id",dtSelectApply.Rows[0]["PoliceArea_id"]), 
                        new SqlParameter("@apply_id",jwGoodsMain.apply_id),
                        new SqlParameter("@getuser_id",jwGoodsMain.getuser_id), 
                        new SqlParameter("@getDate",jwGoodsMain.getDate==null?(object)DBNull.Value:jwGoodsMain.getDate), 
                        new SqlParameter("@LockersNum",jwGoodsMain.LockersNum),
                        new SqlParameter("@LockersPsw",jwGoodsMain.LockersPsw), 
                        new SqlParameter("@detail",jwGoodsMain.detail),
                        new SqlParameter("@backuser_id",jwGoodsMain.backDate==null?(object)DBNull.Value:jwGoodsMain.backuser_id), 
                        new SqlParameter("@backDate",jwGoodsMain.backDate==null?(object)DBNull.Value:jwGoodsMain.backDate), 
                        new SqlParameter("@state",jwGoodsMain.backDate==null?1:2),
                        new SqlParameter("@goodsmain_id",dtCheckMain.Rows[0]["goodsmain_id"])
                    };
                    int r = SqlHelper.ExecuteNonQuery(sqlUpdateMain, CommandType.Text, pars);
                    return r;
                }
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// 检测当前申请单中的物品寄存的状态
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string CheckState(string apply_id)
        {
            string sql = string.Format(@"select * from JW_GoodsMain where apply_id='{0}'", apply_id);
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                if (dt.Rows.Count <= 0)
                {
                    return "2";
                }
                else
                {
                    return dt.Rows[0]["state"].ToString();
                }
            }
            catch (Exception)
            {
                return "2";
            }
        }
    }
}

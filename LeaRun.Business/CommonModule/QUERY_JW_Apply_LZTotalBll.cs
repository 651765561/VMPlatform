//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using LeaRun.Business.CommonModule;

namespace LeaRun.Business
{
    /// <summary>
    /// QUERY_JW_Apply_LZTotal
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.23 14:10</date>
    /// </author>
    /// </summary>
    public class QUERY_JW_Apply_LZTotalBll : RepositoryFactory<QUERY_JW_Apply_LZTotal>
    {
        public DataTable GetUnit_name(string unit_id)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("SELECT unit from base_unit where base_unit_id='" + unit_id + "'");
          
            return Repository().FindTableBySql(strSql.ToString());
        }

        //获得数据
        public DataTable GetData(string unit_id, string applydatestart, string applydateend)
        {
            
          
            DataTable dt1=new DataTable ();
            //================================头部开始=============================================================
            StringBuilder strHead = new StringBuilder();
            strHead.Append(" select unitAll.unit  ");
            for (int i = 1; i < 15; i++)
            {
                strHead.Append(", isnull(user" + i + ".user_count,0) user_count" + i);
                strHead.Append(", isnull(object" + i + ".object_count,0) object_count" + i);
            }
            strHead.Append(", unitAll.sortcode   ");
            strHead.Append(" from  ");
            //================================头部结束============================================================
            //================================取得所有单位开始====================================================
            StringBuilder strUnitAll = new StringBuilder();
            if (unit_id != "")//单位ID
            {
                if (unit_id != Share.UNIT_ID_JS)//非省院选择本单位及下级单位
                {
                    strUnitAll.Append(" ( select base_unit_id,unit,sortcode from base_unit where base_unit_ID='" + unit_id + "' or parent_unit_id ='" + unit_id + "') unitAll ");
                }
                else//省院选择全部单位
                {
                    strUnitAll.Append("( select base_unit_id,unit,sortcode from base_unit where len(code)<=5) unitAll ");
                }
            }
            else
            {
                strUnitAll.Append("( select base_unit_id,unit,sortcode from base_unit ) unitAll");
            }
            //================================取得所有单位结束====================================================
            //================================取得次开始====================================================
            StringBuilder strGetObject= new StringBuilder();
            for (int i = 1; i < 15; i++)
            {
                strGetObject.Append(" left join ");
                strGetObject.Append("( ");


                //strGetObject.Append(" select count(1) object_count, pa.unit_id unit_id from JW_PoliceApply pa ");
                //strGetObject.Append(" where pa.tasktype_id="+i);
                //strGetObject.Append(" group by pa.unit_id ");
                strGetObject.Append(" select unit_id ,sum(object_count) object_count from (");
                strGetObject.Append(" select c.unit_id,isnull(sum(case when Datediff(day,c.actdate,c.enddate)=0  then 1 ");
                strGetObject.Append(" else Datediff(day,c.actdate,c.enddate) end ),0) as object_count  from JW_PoliceApply b ");
                strGetObject.Append(" join JW_SendPolice c on c.Object_id=b.apply_id ");
                strGetObject.Append(" where c.type in (4,5) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetObject.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetObject.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetObject.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetObject.Append(" union all");
                strGetObject.Append(" select c.unit_id,isnull(sum(case when Datediff(day,c.actdate,c.enddate)=0  then 1 ");
                strGetObject.Append(" else Datediff(day,c.actdate,c.enddate) end ),0)as object_count  from  jw_onduty b");
                strGetObject.Append(" join JW_SendPolice c on c.Object_id=b.OnDuty_id ");
                strGetObject.Append(" where c.type in (1,2,3,8,9) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetObject.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetObject.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetObject.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetObject.Append(" ) temp");
                strGetObject.Append(" group by temp.unit_id");
               

                strGetObject.Append(" ) object" + i );
                strGetObject.Append(" on unitAll.base_unit_id=object"+i+".Unit_id ");
            }
            //================================取得次结束====================================================
            //================================取得人开始====================================================
            StringBuilder strGetUser = new StringBuilder();
            for (int i = 1; i < 15; i++)
            {
                strGetUser.Append(" left join ");
                strGetUser.Append("( ");

                strGetUser.Append(" select unit_id ,sum(user_count) user_count from (");
                strGetUser.Append(" select c.unit_id,count(1) as user_count  from JW_PoliceApply b ");
                strGetUser.Append(" join JW_SendPolice c on c.Object_id=b.apply_id ");
                strGetUser.Append(" where c.type in (4,5) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetUser.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetUser.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetUser.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetUser.Append(" union all");
                strGetUser.Append(" select c.unit_id,count(1) as user_count  from  jw_onduty b");
                strGetUser.Append(" join JW_SendPolice c on c.Object_id=b.OnDuty_id ");
                strGetUser.Append(" where c.type in (1,2,3,8,9) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetUser.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetUser.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetUser.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetUser.Append(" ) temp");
                strGetUser.Append(" group by temp.unit_id");
                strGetUser.Append(" ) user" + i);
                strGetUser.Append(" on unitAll.base_unit_id=user" + i + ".Unit_id ");
            }
            //================================取得人结束====================================================
            strGetUser.Append(" union select '总计',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,99999 from base_unit");
                    
            string sql = strHead.ToString() + strUnitAll.ToString() + strGetObject.ToString() + strGetUser.ToString() ;
            sql = sql + " order by unitAll.sortcode";
      
            dt1 = Repository().FindTableBySql(sql);
            dt1.TableName = "dt1"; 
          
         

          
            //ds.Tables.Add (dt1.Copy() );
            //ds.Tables.Add(dt2.Copy());
            return dt1;
        }


        //获得明细数据
        public DataTable GetDataDetail(string unit_name, string applydatestart, string applydateend)
        {


            DataTable dt1 = new DataTable();
            //================================头部开始=============================================================
            StringBuilder strHead = new StringBuilder();
            strHead.Append(" select unitAll.unit  ");
            for (int i = 1; i < 15; i++)
            {
                strHead.Append(", isnull(user" + i + ".user_count,0) user_count" + i);
                strHead.Append(", isnull(object" + i + ".object_count,0) object_count" + i);
            }
            strHead.Append(", unitAll.sortcode   ");
            strHead.Append(" from  ");
            //================================头部结束============================================================
            //================================取得所有单位开始====================================================
            StringBuilder strUnitAll = new StringBuilder();
            strUnitAll.Append(" ( select base_unit_id,unit,sortcode from base_unit where parent_unit_id =(select base_unit_id from base_unit where unit='" + unit_name + "') ) unitAll ");
               
            //================================取得所有单位结束====================================================
            //================================取得次开始====================================================
            StringBuilder strGetObject = new StringBuilder();
            for (int i = 1; i < 15; i++)
            {
                strGetObject.Append(" left join ");
                strGetObject.Append("( ");


                //strGetObject.Append(" select count(1) object_count, pa.unit_id unit_id from JW_PoliceApply pa ");
                //strGetObject.Append(" where pa.tasktype_id="+i);
                //strGetObject.Append(" group by pa.unit_id ");
                strGetObject.Append(" select unit_id ,sum(object_count) object_count from (");
                strGetObject.Append(" select c.unit_id,isnull(sum(case when Datediff(day,c.actdate,c.enddate)=0  then 1 ");
                strGetObject.Append(" else Datediff(day,c.actdate,c.enddate) end ),0) as object_count  from JW_PoliceApply b ");
                strGetObject.Append(" join JW_SendPolice c on c.Object_id=b.apply_id ");
                strGetObject.Append(" where c.type in (4,5) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetObject.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetObject.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetObject.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetObject.Append(" union all");
                strGetObject.Append(" select c.unit_id,isnull(sum(case when Datediff(day,c.actdate,c.enddate)=0  then 1 ");
                strGetObject.Append(" else Datediff(day,c.actdate,c.enddate) end ),0)as object_count  from  jw_onduty b");
                strGetObject.Append(" join JW_SendPolice c on c.Object_id=b.OnDuty_id ");
                strGetObject.Append(" where c.type in (1,2,3,8,9) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetObject.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetObject.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetObject.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetObject.Append(" ) temp");
                strGetObject.Append(" group by temp.unit_id");


                strGetObject.Append(" ) object" + i);
                strGetObject.Append(" on unitAll.base_unit_id=object" + i + ".Unit_id ");
            }
            //================================取得次结束====================================================
            //================================取得人开始====================================================
            StringBuilder strGetUser = new StringBuilder();
            for (int i = 1; i < 15; i++)
            {
                strGetUser.Append(" left join ");
                strGetUser.Append("( ");

                strGetUser.Append(" select unit_id ,sum(user_count) user_count from (");
                strGetUser.Append(" select c.unit_id,count(1) as user_count  from JW_PoliceApply b ");
                strGetUser.Append(" join JW_SendPolice c on c.Object_id=b.apply_id ");
                strGetUser.Append(" where c.type in (4,5) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetUser.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetUser.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetUser.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetUser.Append(" union all");
                strGetUser.Append(" select c.unit_id,count(1) as user_count  from  jw_onduty b");
                strGetUser.Append(" join JW_SendPolice c on c.Object_id=b.OnDuty_id ");
                strGetUser.Append(" where c.type in (1,2,3,8,9) and c.state =3 and c.actdate is not null ");
                if (applydatestart != "")//来访开始日期
                {
                    strGetUser.Append("  and  c.SendDate> '" + applydatestart + "'");
                }
                if (applydateend != "")//来访结束日期
                {
                    strGetUser.Append("  and  c.SendDate< '" + applydateend + "'");
                }
                strGetUser.Append(" and b.tasktype_id =" + i + " group by c.unit_id ");
                strGetUser.Append(" ) temp");
                strGetUser.Append(" group by temp.unit_id");
                strGetUser.Append(" ) user" + i);
                strGetUser.Append(" on unitAll.base_unit_id=user" + i + ".Unit_id ");
            }
            //================================取得人结束====================================================
            strGetUser.Append(" union select '总计',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,99999 from base_unit");

            string sql = strHead.ToString() + strUnitAll.ToString() + strGetObject.ToString() + strGetUser.ToString();
            sql = sql + " order by unitAll.sortcode";

            dt1 = Repository().FindTableBySql(sql);
            dt1.TableName = "dt1";




            //ds.Tables.Add (dt1.Copy() );
            //ds.Tables.Add(dt2.Copy());
            return dt1;
        }


        //是否有权限点柱状图
        public string CheckUnit(string unit_id)
        {

            if (ManageProvider.Provider.Current().CompanyId == "e2c79c56-5b58-4c62-b2a9-3bb7492c")//登陆者是省院的人都可以看
            {
                return "1";
            }
            else//登陆者不是省院的人
            {
                string sql = string.Format(@" 
                    select 
                    1 from Base_unit 
                    WHERE  base_unit_id='{0}'  and parent_unit_id='{1}'
                    "
                    , unit_id
                    , ManageProvider.Provider.Current().CompanyId
                    );
               
                 DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);

                 string sql1 = string.Format(@" 
                    select 
                    1 from Base_unit 
                    WHERE  base_unit_id='{1}'  and parent_unit_id='{0}'
                    "
                    , unit_id
                    , ManageProvider.Provider.Current().CompanyId
                    );

                 DataTable dt1 = SqlHelper.DataTable(sql1, CommandType.Text);

                if (unit_id == "e2c79c56-5b58-4c62-b2a9-3bb7492c") //登陆者不是省院的人 不能看省院的
                {
                    return "0";
                }
                else if (unit_id == ManageProvider.Provider.Current().CompanyId)//登陆者不是省院的人 能看本单位市院的
                {
                    return "1";
                }
                else if (unit_id != ManageProvider.Provider.Current().CompanyId )//登陆者不是省院的人 不能看其他单位市院的,但能看下级区县院的
                {
                    if (dt != null && dt.Rows.Count > 0 || dt1 != null && dt1.Rows.Count > 0)
                    {
                        return "1";
                    }
                    else
                    {
                       return "0";
                    }
                    
                }
            }
            return "0";

         

////            string sql = string.Format(@" 
////                select 
////                1 from Base_unit 
////                WHERE '{1}'='e2c79c56-5b58-4c62-b2a9-3bb7492c'  or   USE1.UserId='{0}' 
////                "
////                , unit_id
////                , ManageProvider.Provider.Current().CompanyId
////                );
////            try
////            {
////                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
////                return dt;
////            }
////            catch (Exception)
////            {
////                return null;
////            }
        }
    }
}
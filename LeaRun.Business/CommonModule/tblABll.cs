//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2018
// Software Developers @ Learun 2018
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using LeaRun.DataAccess;
using System.IO;
using System;
using System.Data.Common;

namespace LeaRun.Business
{
    /// <summary>
    /// tblA
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.08.31 15:37</date>
    /// </author>
    /// </summary>
    public class tblABll : RepositoryFactory<tblA>
    {

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="ParentName">所属模块</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable Search(string name, string ParentName, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT ROW_NUMBER() over(order by Adddate desc) as RowNum1,[ID],[Name],[ParentName],[Status],[Adddate],[AddUser],[Filetype] FROM [tblA] where 1=1");
            if (!string.IsNullOrEmpty(name))
            {
                strSql.Append(" and name like '%"+name+"%' ");
            }

            if (!string.IsNullOrEmpty(ParentName))
            {
                strSql.Append(" and ParentName like '%" + ParentName + @"%'");
            }

            

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 当前名称是否重复（修改或新增不可以重复）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExitsByName(string name, string id)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  Count(1) as num FROM [tblA]
                         where id!='"+id+@"' and name='"+name+@"'");
            int num =int.Parse( Repository().FindTableBySql(strSql.ToString()).Rows[0][0].ToString());
            return num > 0;
           
            
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void SubmitData(string KeyValue, string FilePath, tblA model)
        {
            IDatabase database = DataFactory.Database();
           
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            byte[] imgBytesIn = new byte[fs.Length];
            fs.Read(imgBytesIn, 0, Convert.ToInt32(fs.Length));
            fs.Close();
         
            if (!string.IsNullOrEmpty(KeyValue))
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update tblA set ");
                strSql.Append("Name=@Name,");
                strSql.Append("ParentName=@ParentName,");
                strSql.Append("Record=@Record,");
                strSql.Append("Status=@Status,");
                strSql.Append("Adddate=@Adddate,");
                strSql.Append("AddUser=@AddUser,");
                strSql.Append("Filetype=@Filetype");
                strSql.Append(" where ID=@ID");
                SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.VarChar,50),
					new SqlParameter("@ParentName", SqlDbType.VarChar,50),
					new SqlParameter("@Record", SqlDbType.Image),
					new SqlParameter("@Status", SqlDbType.Int,4),
					new SqlParameter("@Adddate", SqlDbType.DateTime),
					new SqlParameter("@AddUser", SqlDbType.Int,4),
					new SqlParameter("@Filetype", SqlDbType.VarChar,50),
					new SqlParameter("@ID", SqlDbType.Int,4)};
                parameters[0].Value = model.Name;
                parameters[1].Value = model.ParentName;
                parameters[2].Value = imgBytesIn;
                parameters[3].Value = model.Status;
                parameters[4].Value = DateTime.Now;
                parameters[5].Value = model.AddUser;
                parameters[6].Value = model.Filetype;
                parameters[7].Value = KeyValue;

                database.ExecuteBySql(strSql, parameters);
                database.Commit();
            }
            else
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into tblA(");
                strSql.Append("Name,ParentName,Record,Status,Adddate,AddUser,Filetype)");
                strSql.Append(" values (");
                strSql.Append("@Name,@ParentName,@Record,@Status,@Adddate,@AddUser,@Filetype)");
                strSql.Append(";select @@IDENTITY");
                SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.VarChar,50),
					new SqlParameter("@ParentName", SqlDbType.VarChar,50),
					new SqlParameter("@Record", SqlDbType.Image),
					new SqlParameter("@Status", SqlDbType.Int,4),
					new SqlParameter("@Adddate", SqlDbType.DateTime),
					new SqlParameter("@AddUser", SqlDbType.Int,4),
					new SqlParameter("@Filetype", SqlDbType.VarChar,50)};
                parameters[0].Value = model.Name;
                parameters[1].Value = model.ParentName;
                parameters[2].Value = imgBytesIn;
                parameters[3].Value = model.Status;
                parameters[4].Value = DateTime.Now;
                parameters[5].Value = model.AddUser;
                parameters[6].Value = model.Filetype;

                database.ExecuteBySql(strSql, parameters);
                database.Commit();
            }
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
            }
        }
        public int DeleteByID(string id)
        {
            //
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"delete FROM [tblA] where id=" + id);
            return Repository().ExecuteBySql(strSql);
        }
        public DataRow GetModelII(string id)
        {
            //

            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"select * from [tblA] where id=" + id);
            DataTable dt = Repository().FindTableBySql(strSql.ToString());
            DataRow r =null;
            if (dt.Rows.Count > 0)
            {
                r = dt.Rows[0];
            }
           
            return r;
        }

    }
}
//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================

using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeaRun.Entity
{
    /// <summary>
    /// Base_Department
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.08 10:05</date>
    /// </author>
    /// </summary>
    [Description("Base_Department")]
    [PrimaryKey("Dep_id")]
    public class Base_Department : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Dep_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Dep_id")]
        public string Dep_id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        [DisplayName("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Parent_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Parent_id")]
        public string Parent_id { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// SortCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("SortCode")]
        public string SortCode { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        /// <returns></returns>
        [DisplayName("Code")]
        public string Code { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Dep_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Dep_id = KeyValue;
        }
        #endregion
    }
}
//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2018
// Software Developers @ Learun 2018
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
    /// tblA
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.08.31 15:37</date>
    /// </author>
    /// </summary>
    [Description("tblA")]
    [PrimaryKey("ID")]
    public class tblA : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// ID
        /// </summary>
        /// <returns></returns>
        [DisplayName("ID")]
        public int? ID { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        [DisplayName("Name")]
        public string Name { get; set; }
        /// <summary>
        /// ParentName
        /// </summary>
        /// <returns></returns>
        [DisplayName("ParentName")]
        public string ParentName { get; set; }
        /// <summary>
        /// Record
        /// </summary>
        /// <returns></returns>
        [DisplayName("Record")]
        public string Record { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        /// <returns></returns>
        [DisplayName("Status")]
        public int? Status { get; set; }
        /// <summary>
        /// Adddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("Adddate")]
        public DateTime? Adddate { get; set; }
        /// <summary>
        /// AddUser
        /// </summary>
        /// <returns></returns>
        [DisplayName("AddUser")]
        public int? AddUser { get; set; }
        /// <summary>
        /// Filetype
        /// </summary>
        /// <returns></returns>
        [DisplayName("Filetype")]
        public string Filetype { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            //this.ID = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            //this.ID = KeyValue;
                                            }
        #endregion
    }
}
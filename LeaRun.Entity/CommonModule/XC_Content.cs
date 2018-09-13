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
    /// XC_Content
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.08.01 14:11</date>
    /// </author>
    /// </summary>
    [Description("XC_Content")]
    [PrimaryKey("xc_content_id")]
    public class XC_Content : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// xc_content_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("xc_content_id")]
        public string xc_content_id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// detail
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        /// <summary>
        /// orders
        /// </summary>
        /// <returns></returns>
        [DisplayName("orders")]
        public int? orders { get; set; }
        /// <summary>
        /// place
        /// </summary>
        /// <returns></returns>
        [DisplayName("place")]
        public string place { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.xc_content_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.xc_content_id = KeyValue;
                                            }
        #endregion
    }
}
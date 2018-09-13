//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
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
    /// Base_MoneyType
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.10 09:35</date>
    /// </author>
    /// </summary>
    [Description("Base_MoneyType")]
    [PrimaryKey("MoneyType_id")]
    public class Base_MoneyType : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// MoneyType_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MoneyType_id")]
        public string MoneyType_id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// code
        /// </summary>
        /// <returns></returns>
        [DisplayName("code")]
        public string code { get; set; }
        /// <summary>
        /// orderby
        /// </summary>
        /// <returns></returns>
        [DisplayName("orderby")]
        public int? orderby { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MoneyType_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.MoneyType_id = KeyValue;
                                            }
        #endregion
    }
}
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
    /// People_Money
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 11:53</date>
    /// </author>
    /// </summary>
    [Description("People_Money")]
    [PrimaryKey("peoplemoney_id")]
    public class People_Money : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// peoplemoney_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("peoplemoney_id")]
        public string peoplemoney_id { get; set; }
        /// <summary>
        /// people_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("people_id")]
        public string people_id { get; set; }
        /// <summary>
        /// designation
        /// </summary>
        /// <returns></returns>
        [DisplayName("designation")]
        public string designation { get; set; }
        /// <summary>
        /// addUser
        /// </summary>
        /// <returns></returns>
        [DisplayName("addUser")]
        public string addUser { get; set; }
        /// <summary>
        /// adddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("adddate")]
        public DateTime? adddate { get; set; }
        /// <summary>
        /// MoneyType_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MoneyType_id")]
        public string MoneyType_id { get; set; }
        /// <summary>
        /// account
        /// </summary>
        /// <returns></returns>
        [DisplayName("account")]
        public decimal? account { get; set; }
        /// <summary>
        /// design
        /// </summary>
        /// <returns></returns>
        [DisplayName("design")]
        public string design { get; set; }
        /// <summary>
        /// checker
        /// </summary>
        /// <returns></returns>
        [DisplayName("checker")]
        public string checker { get; set; }
        /// <summary>
        /// checkdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkdate")]
        public DateTime? checkdate { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.peoplemoney_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.peoplemoney_id = KeyValue;
                                            }
        #endregion
    }
}
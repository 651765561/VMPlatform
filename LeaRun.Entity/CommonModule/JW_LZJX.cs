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
    /// JW_LZJX
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.20 16:56</date>
    /// </author>
    /// </summary>
    [Description("JW_LZJX")]
    [PrimaryKey("LZJX_id")]
    public class JW_LZJX : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// LZJX_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("LZJX_id")]
        public string LZJX_id { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// adduser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("adduser_id")]
        public string adduser_id { get; set; }
        /// <summary>
        /// adddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("adddate")]
        public DateTime? adddate { get; set; }
        /// <summary>
        /// itemCount
        /// </summary>
        /// <returns></returns>
        [DisplayName("itemCount")]
        public string itemCount { get; set; }
        /// <summary>
        /// item1
        /// </summary>
        /// <returns></returns>
        [DisplayName("item1")]
        public string item1 { get; set; }
        /// <summary>
        /// value1
        /// </summary>
        /// <returns></returns>
        [DisplayName("value1")]
        public string value1 { get; set; }
        /// <summary>
        /// item2
        /// </summary>
        /// <returns></returns>
        [DisplayName("item2")]
        public string item2 { get; set; }
        /// <summary>
        /// value2
        /// </summary>
        /// <returns></returns>
        [DisplayName("value2")]
        public string value2 { get; set; }
        /// <summary>
        /// item3
        /// </summary>
        /// <returns></returns>
        [DisplayName("item3")]
        public string item3 { get; set; }
        /// <summary>
        /// value3
        /// </summary>
        /// <returns></returns>
        [DisplayName("value3")]
        public string value3 { get; set; }
        /// <summary>
        /// item4
        /// </summary>
        /// <returns></returns>
        [DisplayName("item4")]
        public string item4 { get; set; }
        /// <summary>
        /// value4
        /// </summary>
        /// <returns></returns>
        [DisplayName("value4")]
        public string value4 { get; set; }
        /// <summary>
        /// item5
        /// </summary>
        /// <returns></returns>
        [DisplayName("item5")]
        public string item5 { get; set; }
        /// <summary>
        /// value5
        /// </summary>
        /// <returns></returns>
        [DisplayName("value5")]
        public string value5 { get; set; }
        /// <summary>
        /// item6
        /// </summary>
        /// <returns></returns>
        [DisplayName("item6")]
        public string item6 { get; set; }
        /// <summary>
        /// value6
        /// </summary>
        /// <returns></returns>
        [DisplayName("value6")]
        public string value6 { get; set; }
        /// <summary>
        /// item7
        /// </summary>
        /// <returns></returns>
        [DisplayName("item7")]
        public string item7 { get; set; }
        /// <summary>
        /// value7
        /// </summary>
        /// <returns></returns>
        [DisplayName("value7")]
        public string value7 { get; set; }
        /// <summary>
        /// item8
        /// </summary>
        /// <returns></returns>
        [DisplayName("item8")]
        public string item8 { get; set; }
        /// <summary>
        /// value8
        /// </summary>
        /// <returns></returns>
        [DisplayName("value8")]
        public string value8 { get; set; }
        /// <summary>
        /// item9
        /// </summary>
        /// <returns></returns>
        [DisplayName("item9")]
        public string item9 { get; set; }
        /// <summary>
        /// value9
        /// </summary>
        /// <returns></returns>
        [DisplayName("value9")]
        public string value9 { get; set; }
        /// <summary>
        /// item10
        /// </summary>
        /// <returns></returns>
        [DisplayName("item10")]
        public string item10 { get; set; }
        /// <summary>
        /// value10
        /// </summary>
        /// <returns></returns>
        [DisplayName("value10")]
        public string value10 { get; set; }
        /// <summary>
        /// item11
        /// </summary>
        /// <returns></returns>
        [DisplayName("item11")]
        public string item11 { get; set; }
        /// <summary>
        /// value11
        /// </summary>
        /// <returns></returns>
        [DisplayName("value11")]
        public string value11 { get; set; }
        /// <summary>
        /// item12
        /// </summary>
        /// <returns></returns>
        [DisplayName("item12")]
        public string item12 { get; set; }
        /// <summary>
        /// value12
        /// </summary>
        /// <returns></returns>
        [DisplayName("value12")]
        public string value12 { get; set; }
        /// <summary>
        /// item13
        /// </summary>
        /// <returns></returns>
        [DisplayName("item13")]
        public string item13 { get; set; }
        /// <summary>
        /// value13
        /// </summary>
        /// <returns></returns>
        [DisplayName("value13")]
        public string value13 { get; set; }
        /// <summary>
        /// item14
        /// </summary>
        /// <returns></returns>
        [DisplayName("item14")]
        public string item14 { get; set; }
        /// <summary>
        /// value14
        /// </summary>
        /// <returns></returns>
        [DisplayName("value14")]
        public string value14 { get; set; }
        /// <summary>
        /// item15
        /// </summary>
        /// <returns></returns>
        [DisplayName("item15")]
        public string item15 { get; set; }
        /// <summary>
        /// value15
        /// </summary>
        /// <returns></returns>
        [DisplayName("value15")]
        public string value15 { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.LZJX_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.LZJX_id = KeyValue;
                                            }
        #endregion
    }
}
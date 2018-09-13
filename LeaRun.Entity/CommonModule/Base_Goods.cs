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
    /// Base_Goods
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.13 08:51</date>
    /// </author>
    /// </summary>
    [Description("Base_Goods")]
    [PrimaryKey("goods_id")]
    public class Base_Goods : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// goods_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goods_id")]
        public string goods_id { get; set; }
        /// <summary>
        /// code
        /// </summary>
        /// <returns></returns>
        [DisplayName("code")]
        public string code { get; set; }
        /// <summary>
        /// shortcode
        /// </summary>
        /// <returns></returns>
        [DisplayName("shortcode")]
        public string shortcode { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// shortname
        /// </summary>
        /// <returns></returns>
        [DisplayName("shortname")]
        public string shortname { get; set; }
        /// <summary>
        /// standand
        /// </summary>
        /// <returns></returns>
        [DisplayName("standand")]
        public string standand { get; set; }
        /// <summary>
        /// unit
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit")]
        public string unit { get; set; }
        /// <summary>
        /// goodstype_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goodstype_id")]
        public string goodstype_id { get; set; }
        /// <summary>
        /// price
        /// </summary>
        /// <returns></returns>
        [DisplayName("price")]
        public decimal? price { get; set; }
        /// <summary>
        /// islimit
        /// </summary>
        /// <returns></returns>
        [DisplayName("islimit")]
        public int? islimit { get; set; }
        /// <summary>
        /// imgurl
        /// </summary>
        /// <returns></returns>
        [DisplayName("imgurl")]
        public string imgurl { get; set; }
        /// <summary>
        /// filetype
        /// </summary>
        /// <returns></returns>
        [DisplayName("filetype")]
        public string filetype { get; set; }
        /// <summary>
        /// limitnum
        /// </summary>
        /// <returns></returns>
        [DisplayName("limitnum")]
        public int? limitnum { get; set; }
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
            this.goods_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.goods_id = KeyValue;
                                            }
        #endregion
    }
}
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
    /// Base_GoodsAreaRelation
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.16 14:27</date>
    /// </author>
    /// </summary>
    [Description("Base_GoodsAreaRelation")]
    [PrimaryKey("GoodsAreaRelation_id")]
    public class Base_GoodsAreaRelation : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// GoodsAreaRelation_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("GoodsAreaRelation_id")]
        public string GoodsAreaRelation_id { get; set; }
        /// <summary>
        /// Area_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Area_id")]
        public string Area_id { get; set; }
        /// <summary>
        /// goods_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goods_id")]
        public string goods_id { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.GoodsAreaRelation_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.GoodsAreaRelation_id = KeyValue;
                                            }
        #endregion
    }
}
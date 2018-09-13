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
    /// SaleControl_People_Query
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.17 10:05</date>
    /// </author>
    /// </summary>
    [Description("SaleControl_People_Query")]
    [PrimaryKey("OperationDetail_id")]
    public class SaleControl_People_Query : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// OperationDetail_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("OperationDetail_id")]
        public string OperationDetail_id { get; set; }
        /// <summary>
        /// operationmain_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("operationmain_id")]
        public string operationmain_id { get; set; }
        /// <summary>
        /// goods_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goods_id")]
        public string goods_id { get; set; }
        /// <summary>
        /// goodschoosenum
        /// </summary>
        /// <returns></returns>
        [DisplayName("goodschoosenum")]
        public int? goodschoosenum { get; set; }
        /// <summary>
        /// goodsnum
        /// </summary>
        /// <returns></returns>
        [DisplayName("goodsnum")]
        public int? goodsnum { get; set; }
        /// <summary>
        /// batchno
        /// </summary>
        /// <returns></returns>
        [DisplayName("batchno")]
        public string batchno { get; set; }
        /// <summary>
        /// enddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("enddate")]
        public DateTime? enddate { get; set; }
        /// <summary>
        /// outprice
        /// </summary>
        /// <returns></returns>
        [DisplayName("outprice")]
        public decimal? outprice { get; set; }
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
            this.OperationDetail_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.OperationDetail_id = KeyValue;
                                            }
        #endregion
    }
}
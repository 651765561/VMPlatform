using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;

namespace LeaRun.Entity
{
    [Description("JW_GoodsDetail")]
    [PrimaryKey("goodsdetail_id")]
    public class JW_GoodsDetail : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// goodsdetail_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goodsdetail_id")]
        public string goodsdetail_id { get; set; }
        /// <summary>
        /// goodsmain_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goodsmain_id")]
        public string goodsmain_id { get; set; }



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
        public DateTime adddate { get; set; }






        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// num
        /// </summary>
        /// <returns></returns>
        [DisplayName("num")]
        public string num { get; set; }
        /// <summary>
        /// treatment
        /// </summary>
        /// <returns></returns>
        [DisplayName("treatment")]
        public string treatment { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.goodsdetail_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.goodsdetail_id = KeyValue;
        }
        #endregion
    }
}

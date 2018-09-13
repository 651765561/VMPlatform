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
    /// XC_Daliy
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.04.19 11:19</date>
    /// </author>
    /// </summary>
    [Description("XC_Daliy")]
    [PrimaryKey("xc_daliy_id")]
    public class XC_Daliy : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// xc_daliy_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("xc_daliy_id")]
        public string xc_daliy_id { get; set; }
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
        /// reportYear
        /// </summary>
        /// <returns></returns>
        [DisplayName("reportYear")]
        public string reportYear { get; set; }
        /// <summary>
        /// reportNum
        /// </summary>
        /// <returns></returns>
        [DisplayName("reportNum")]
        public string reportNum { get; set; }
        /// <summary>
        /// reportAllNum
        /// </summary>
        /// <returns></returns>
        [DisplayName("reportAllNum")]
        public string reportAllNum { get; set; }
        /// <summary>
        /// basicinfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("basicinfo")]
        public string basicinfo { get; set; }
        /// <summary>
        /// xcinfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("xcinfo")]
        public string xcinfo { get; set; }
        /// <summary>
        /// operationinfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("operationinfo")]
        public string operationinfo { get; set; }
        /// <summary>
        /// videoinfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("videoinfo")]
        public string videoinfo { get; set; }
        /// <summary>
        /// submit
        /// </summary>
        /// <returns></returns>
        [DisplayName("submit")]
        public string submit { get; set; }
        /// <summary>
        /// deliver
        /// </summary>
        /// <returns></returns>
        [DisplayName("deliver")]
        public string deliver { get; set; }
        /// <summary>
        /// editing
        /// </summary>
        /// <returns></returns>
        [DisplayName("editing")]
        public string editing { get; set; }
        /// <summary>
        /// review
        /// </summary>
        /// <returns></returns>
        [DisplayName("review")]
        public string review { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.xc_daliy_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.xc_daliy_id = KeyValue;
                                            }
        #endregion
    }
}
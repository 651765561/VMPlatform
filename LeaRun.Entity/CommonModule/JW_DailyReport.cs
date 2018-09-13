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
    /// JW_DailyReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.21 14:06</date>
    /// </author>
    /// </summary>
    [Description("JW_DailyReport")]
    [PrimaryKey("DailyReport_id")]
    public class JW_DailyReport : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// DailyReport_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("DailyReport_id")]
        public string DailyReport_id { get; set; }
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
        /// importantinfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("importantinfo")]
        public string importantinfo { get; set; }
        /// <summary>
        /// dailyinfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("dailyinfo")]
        public string dailyinfo { get; set; }
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
        /// <summary>
        /// issue
        /// </summary>
        /// <returns></returns>
        [DisplayName("issue")]
        public string issue { get; set; }
        /// <summary>
        /// needReport
        /// </summary>
        /// <returns></returns>
        [DisplayName("needReport")]
        public int? needReport { get; set; }

        /// <summary>
        /// needPublish
        /// </summary>
        /// <returns></returns>
        [DisplayName("needPublish")]
        public int? needPublish { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.DailyReport_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.DailyReport_id = KeyValue;
                                            }
        #endregion
    }
}
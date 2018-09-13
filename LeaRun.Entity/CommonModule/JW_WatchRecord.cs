using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;

namespace LeaRun.Entity
{
    /// <summary>
    /// JW_WatchRecord
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.26 13:04</date>
    /// </author>
    /// </summary>
    [Description("JW_WatchRecord")]
    [PrimaryKey("watchrecord_id")]
    public class JW_WatchRecord : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// watchrecord_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("watchrecord_id")]
        public string watchrecord_id { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// PoliceArea_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("PoliceArea_id")]
        public string PoliceArea_id { get; set; }
        /// <summary>
        /// apply_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("apply_id")]
        public string apply_id { get; set; }
        /// <summary>
        /// adduser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("adduser_id")]
        public string adduser_id { get; set; }
        /// <summary>
        /// addDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("addDate")]
        public DateTime? addDate { get; set; }
        /// <summary>
        /// watchuser
        /// </summary>
        /// <returns></returns>
        [DisplayName("watchuser")]
        public string watchuser { get; set; }
        /// <summary>
        /// depname
        /// </summary>
        /// <returns></returns>
        [DisplayName("depname")]
        public string depname { get; set; }
        /// <summary>
        /// watchplace
        /// </summary>
        /// <returns></returns>
        [DisplayName("watchplace")]
        public string watchplace { get; set; }
        /// <summary>
        /// startdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("startdate")]
        public DateTime? startdate { get; set; }
        /// <summary>
        /// enddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("enddate")]
        public DateTime? enddate { get; set; }
        /// <summary>
        /// dutydetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("dutydetail")]
        public string dutydetail { get; set; }
        /// <summary>
        /// attention
        /// </summary>
        /// <returns></returns>
        [DisplayName("attention")]
        public string attention { get; set; }
        /// <summary>
        /// successor
        /// </summary>
        /// <returns></returns>
        [DisplayName("successor")]
        public string successor { get; set; }
        /// <summary>
        /// depname2
        /// </summary>
        /// <returns></returns>
        [DisplayName("depname2")]
        public string depname2 { get; set; }
        /// <summary>
        /// depname2
        /// </summary>
        /// <returns></returns>
        [DisplayName("signing")]
        public string signing { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.watchrecord_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.watchrecord_id = KeyValue;
        }
        #endregion
    }
}

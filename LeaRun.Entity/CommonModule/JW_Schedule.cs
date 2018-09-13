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
    /// JW_Schedule
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.03.07 17:10</date>
    /// </author>
    /// </summary>
    [Description("JW_Schedule")]
    [PrimaryKey("Schedule_id")]
    public class JW_Schedule : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Schedule_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Schedule_id")]
        public string Schedule_id { get; set; }
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
        /// DutyUser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("DutyUser_id")]
        public string DutyUser_id { get; set; }
        /// <summary>
        /// user_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("user_id")]
        public string user_id { get; set; }
        /// <summary>
        /// user_name
        /// </summary>
        /// <returns></returns>
        [DisplayName("user_name")]
        public string user_name { get; set; }
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
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
        /// <summary>
        /// detail
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Schedule_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Schedule_id = KeyValue;
        }
        #endregion
    }
}

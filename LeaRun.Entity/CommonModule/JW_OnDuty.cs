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
    /// JW_OnDuty
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.12 10:54</date>
    /// </author>
    /// </summary>
    [Description("JW_OnDuty")]
    [PrimaryKey("OnDuty_id")]
    public class JW_OnDuty : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// OnDuty_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("OnDuty_id")]
        public string OnDuty_id { get; set; }
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
        /// tasktype_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("tasktype_id")]
        public string tasktype_id { get; set; }
        /// <summary>
        /// DutyUser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("DutyUser_id")]
        public string DutyUser_id { get; set; }
        /// <summary>
        /// SendDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("SendDate")]
        public DateTime? SendDate { get; set; }
        /// <summary>
        /// OldDutyUser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("OldDutyUser_id")]
        public string OldDutyUser_id { get; set; }
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
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int state { get; set; }
        /// <summary>
        /// Object_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Object_id")]
        public string Object_id { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OnDuty_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.OnDuty_id = KeyValue;
        }
        #endregion
    }
}

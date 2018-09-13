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
    /// JW_DutyRecord
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.26 13:02</date>
    /// </author>
    /// </summary>
    [Description("JW_DutyRecord")]
    [PrimaryKey("dutyrecord_id")]
    public class JW_DutyRecord : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// dutyrecord_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("dutyrecord_id")]
        public string dutyrecord_id { get; set; }
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
        /// dutyuser
        /// </summary>
        /// <returns></returns>
        [DisplayName("dutyuser")]
        public string dutyuser { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.dutyrecord_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.dutyrecord_id = KeyValue;
        }
        #endregion
    }
}

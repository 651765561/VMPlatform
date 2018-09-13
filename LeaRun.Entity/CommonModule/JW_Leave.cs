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
    /// JW_Leave
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.28 14:35</date>
    /// </author>
    /// </summary>
    [Description("JW_Leave")]
    [PrimaryKey("leave_id")]
    public class JW_Leave : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// leave_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("leave_id")]
        public string leave_id { get; set; }
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
        /// item
        /// </summary>
        /// <returns></returns>
        [DisplayName("item")]
        public string item { get; set; }
        /// <summary>
        /// detail
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        /// <summary>
        /// sarqm
        /// </summary>
        /// <returns></returns>
        [DisplayName("sarqm")]
        public string sarqm { get; set; }
        /// <summary>
        /// fjqm
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjqm")]
        public string fjqm { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.leave_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.leave_id = KeyValue;
        }
        #endregion
    }
}

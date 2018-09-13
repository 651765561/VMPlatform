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
    /// JW_Apply_room
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.22 16:21</date>
    /// </author>
    /// </summary>
    [Description("JW_Apply_room")]
    [PrimaryKey("apply_room_id")]
    public class JW_Apply_room : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// apply_room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("apply_room_id")]
        public string apply_room_id { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
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
        /// adddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("adddate")]
        public DateTime? adddate { get; set; }
        /// <summary>
        /// Room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Room_id")]
        public string Room_id { get; set; }
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
        /// note
        /// </summary>
        /// <returns></returns>
        [DisplayName("note")]
        public string note { get; set; }
        /// <summary>
        /// timetip
        /// </summary>
        /// <returns></returns>
        [DisplayName("timetip")]
        public string timetip { get; set; }
        /// <summary>
        /// downloadedtime
        /// </summary>
        /// <returns></returns>
        [DisplayName("downloadedtime")]
        public DateTime? downloadedtime { get; set; }
        /// <summary>
        /// hasdownloaded
        /// </summary>
        /// <returns></returns>
        [DisplayName("hasdownloaded")]
        public int? hasdownloaded { get; set; }
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
            this.apply_room_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.apply_room_id = KeyValue;
        }
        #endregion
    }
}

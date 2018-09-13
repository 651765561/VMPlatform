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
    /// JW_Apply_detail
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.22 16:58</date>
    /// </author>
    /// </summary>
    [Description("JW_Apply_detail")]
    [PrimaryKey("Apply_detail_id")]
    public class JW_Apply_detail : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Apply_detail_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Apply_detail_id")]
        public string Apply_detail_id { get; set; }
        /// <summary>
        /// Apply_room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Apply_room_id")]
        public string Apply_room_id { get; set; }
        /// <summary>
        /// Room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Room_id")]
        public string Room_id { get; set; }
        /// <summary>
        /// outtime
        /// </summary>
        /// <returns></returns>
        [DisplayName("outtime")]
        public DateTime? outtime { get; set; }
        /// <summary>
        /// inttime
        /// </summary>
        /// <returns></returns>
        [DisplayName("intime")]
        public DateTime? intime { get; set; }
        /// <summary>
        /// record_type
        /// </summary>
        /// <returns></returns>
        [DisplayName("record_type")]
        public int? record_type { get; set; }
        /// <summary>
        /// cmaker
        /// </summary>
        /// <returns></returns>
        [DisplayName("cmaker")]
        public string cmaker { get; set; }
        /// <summary>
        /// cmakerBack
        /// </summary>
        /// <returns></returns>
        [DisplayName("cmakerBack")]
        public string cmakerBack { get; set; }
        /// <summary>
        /// detail
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        /// <summary>
        /// adduser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("adduser_id")]
        public string adduser_id { get; set; }
        /// <summary>
        /// addate
        /// </summary>
        /// <returns></returns>
        [DisplayName("addate")]
        public DateTime? addate { get; set; }
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
        /// examreason
        /// </summary>
        /// <returns></returns>
        [DisplayName("examreason")]
        public string examreason { get; set; }
        /// <summary>
        /// medicalhistory 
        /// </summary>
        /// <returns></returns>
        [DisplayName("medicalhistory ")]
        public string medicalhistory { get; set; }
        /// <summary>
        /// temperature 
        /// </summary>
        /// <returns></returns>
        [DisplayName("temperature ")]
        public string temperature { get; set; }
        /// <summary>
        /// blood 
        /// </summary>
        /// <returns></returns>
        [DisplayName("blood ")]
        public string blood { get; set; }
        /// <summary>
        /// heartrate 
        /// </summary>
        /// <returns></returns>
        [DisplayName("heartrate ")]
        public string heartrate { get; set; }
        /// <summary>
        /// breathing 
        /// </summary>
        /// <returns></returns>
        [DisplayName("breathing ")]
        public string breathing { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Apply_detail_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Apply_detail_id = KeyValue;
        }
        #endregion
    }
}

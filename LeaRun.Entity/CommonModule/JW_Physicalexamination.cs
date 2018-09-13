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
    /// JW_Physicalexamination
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.21 09:40</date>
    /// </author>
    /// </summary>
    [Description("JW_Physicalexamination")]
    [PrimaryKey("exam_id")]
    public class JW_Physicalexamination : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// exam_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("exam_id")]
        public string exam_id { get; set; }
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
        /// addDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("addDate")]
        public DateTime? addDate { get; set; }
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
        /// examusername
        /// </summary>
        /// <returns></returns>
        [DisplayName("examusername")]
        public string examusername { get; set; }
        /// <summary>
        /// examDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("examDate")]
        public DateTime? examDate { get; set; }
        /// <summary>
        /// examplace
        /// </summary>
        /// <returns></returns>
        [DisplayName("examplace")]
        public string examplace { get; set; }
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
        [DisplayName("medicalhistory")]
        public string medicalhistory { get; set; }
        /// <summary>
        /// temperature
        /// </summary>
        /// <returns></returns>
        [DisplayName("temperature")]
        public string temperature { get; set; }
        /// <summary>
        /// blood
        /// </summary>
        /// <returns></returns>
        [DisplayName("blood")]
        public string blood { get; set; }
        /// <summary>
        /// heartrate
        /// </summary>
        /// <returns></returns>
        [DisplayName("heartrate")]
        public string heartrate { get; set; }
        /// <summary>
        /// breathing
        /// </summary>
        /// <returns></returns>
        [DisplayName("breathing")]
        public string breathing { get; set; }
        /// <summary>
        /// examdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("examdetail")]
        public string examdetail { get; set; }
        /// <summary>
        /// signimg
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
            this.exam_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.exam_id = KeyValue;
        }
        #endregion
    }
}

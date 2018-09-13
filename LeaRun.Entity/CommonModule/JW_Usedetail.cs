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
    /// JW_Usedetail
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.01 14:01</date>
    /// </author>
    /// </summary>
    [Description("JW_Usedetail")]
    [PrimaryKey("Usedetail_id")]
    public class JW_Usedetail : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Usedetail_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Usedetail_id")]
        public string Usedetail_id { get; set; }
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
        /// room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("room_id")]
        public string room_id { get; set; }
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
        /// timeoutstate
        /// </summary>
        /// <returns></returns>
        [DisplayName("timeoutstate")]
        public int? timeoutstate { get; set; }
        /// <summary>
        /// downloadtime
        /// </summary>
        /// <returns></returns>
        [DisplayName("downloadtime")]
        public DateTime? downloadtime { get; set; }
        /// <summary>
        /// isend
        /// </summary>
        /// <returns></returns>
        [DisplayName("isend")]
        public int? isend { get; set; }
        /// <summary>
        /// quxiang
        /// </summary>
        /// <returns></returns>
        [DisplayName("quxiang")]
        public string quxiang { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Usedetail_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Usedetail_id = KeyValue;
        }
        #endregion
    }
}

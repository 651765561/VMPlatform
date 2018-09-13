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
    /// JW_Apply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.17 11:59</date>
    /// </author>
    /// </summary>
    [Description("JW_Apply")]
    [PrimaryKey("apply_id")]
    public class QUERY_JW_Apply_XJ : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// apply_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("apply_id")]
        public string apply_id { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
        /// <summary>
        /// case_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("case_id")]
        public string case_id { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// unitname
        /// </summary>
        /// <returns></returns>
        [DisplayName("unitname")]
        public string unitname { get; set; }
        /// <summary>
        /// dep_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("dep_id")]
        public string dep_id { get; set; }
        /// <summary>
        /// depname
        /// </summary>
        /// <returns></returns>
        [DisplayName("depname")]
        public string depname { get; set; }
        /// <summary>
        /// asker_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("asker_id")]
        public string asker_id { get; set; }
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
        /// docCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("docCode")]
        public string docCode { get; set; }
        /// <summary>
        /// roomdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("roomdetail")]
        public string roomdetail { get; set; }
        /// <summary>
        /// userName
        /// </summary>
        /// <returns></returns>
        [DisplayName("userName")]
        public string userName { get; set; }
        /// <summary>
        /// userSex
        /// </summary>
        /// <returns></returns>
        [DisplayName("userSex")]
        public string userSex { get; set; }
        /// <summary>
        /// userAge
        /// </summary>
        /// <returns></returns>
        [DisplayName("userAge")]
        public string userAge { get; set; }
        /// <summary>
        /// userCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("userCode")]
        public string userCode { get; set; }
        /// <summary>
        /// userNation
        /// </summary>
        /// <returns></returns>
        [DisplayName("userNation")]
        public string userNation { get; set; }
        /// <summary>
        /// userType
        /// </summary>
        /// <returns></returns>
        [DisplayName("userType")]
        public string userType { get; set; }
        /// <summary>
        /// userbiref
        /// </summary>
        /// <returns></returns>
        [DisplayName("userbiref")]
        public string userbiref { get; set; }
        /// <summary>
        /// userPoliticalstatus
        /// </summary>
        /// <returns></returns>
        [DisplayName("userPoliticalstatus")]
        public string userPoliticalstatus { get; set; }
        /// <summary>
        /// userIsNPCmember
        /// </summary>
        /// <returns></returns>
        [DisplayName("userIsNPCmember")]
        public int? userIsNPCmember { get; set; }
        /// <summary>
        /// userEducation
        /// </summary>
        /// <returns></returns>
        [DisplayName("userEducation")]
        public string userEducation { get; set; }
        /// <summary>
        /// userWork
        /// </summary>
        /// <returns></returns>
        [DisplayName("userWork")]
        public string userWork { get; set; }
        /// <summary>
        /// userDuty
        /// </summary>
        /// <returns></returns>
        [DisplayName("userDuty")]
        public string userDuty { get; set; }
        /// <summary>
        /// userHome
        /// </summary>
        /// <returns></returns>
        [DisplayName("userHome")]
        public string userHome { get; set; }
        /// <summary>
        /// userHealthy
        /// </summary>
        /// <returns></returns>
        [DisplayName("userHealthy")]
        public string userHealthy { get; set; }
        /// <summary>
        /// remark
        /// </summary>
        /// <returns></returns>
        [DisplayName("remark")]
        public string remark { get; set; }
        /// <summary>
        /// usercardcode
        /// </summary>
        /// <returns></returns>
        [DisplayName("usercardcode")]
        public string usercardcode { get; set; }
        /// <summary>
        /// detail
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        /// <summary>
        /// ApprovalDetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("ApprovalDetail")]
        public string ApprovalDetail { get; set; }
        /// <summary>
        /// fjuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjuser_id")]
        public string fjuser_id { get; set; }
        /// <summary>
        /// fjdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjdate")]
        public DateTime? fjdate { get; set; }
        /// <summary>
        /// fjdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjdetail")]
        public string fjdetail { get; set; }
        /// <summary>
        /// leaderuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("leaderuser_id")]
        public string leaderuser_id { get; set; }
        /// <summary>
        /// leaderdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("leaderdate")]
        public DateTime? leaderdate { get; set; }
        /// <summary>
        /// leaderdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("leaderdetail")]
        public string leaderdetail { get; set; }
        /// <summary>
        /// fact_indate
        /// </summary>
        /// <returns></returns>
        [DisplayName("fact_indate")]
        public DateTime? fact_indate { get; set; }
        /// <summary>
        /// fact_outdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("fact_outdate")]
        public DateTime? fact_outdate { get; set; }
        /// <summary>
        /// Whereabouts
        /// </summary>
        /// <returns></returns>
        [DisplayName("Whereabouts")]
        public string Whereabouts { get; set; }
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
            this.apply_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.apply_id = KeyValue;
                                            }
        #endregion
    }
}
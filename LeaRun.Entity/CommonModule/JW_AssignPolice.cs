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
    /// JW_AssignPolice
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.28 10:49</date>
    /// </author>
    /// </summary>
    [Description("JW_AssignPolice")]
    [PrimaryKey("callpolice_id")]
    public class JW_AssignPolice : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// callpolice_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("callpolice_id")]
        public string callpolice_id { get; set; }
        /// <summary>
        /// to_unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("to_unit_id")]
        public string to_unit_id { get; set; }
        /// <summary>
        /// from_unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("from_unit_id")]
        public string from_unit_id { get; set; }
        /// <summary>
        /// userdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("userdate")]
        public DateTime? userdate { get; set; }
        /// <summary>
        /// userAddress
        /// </summary>
        /// <returns></returns>
        [DisplayName("userAddress")]
        public string userAddress { get; set; }
        /// <summary>
        /// usernum
        /// </summary>
        /// <returns></returns>
        [DisplayName("usernum")]
        public string usernum { get; set; }
        /// <summary>
        /// userman
        /// </summary>
        /// <returns></returns>
        [DisplayName("userman")]
        public string userman { get; set; }
        /// <summary>
        /// userwoman
        /// </summary>
        /// <returns></returns>
        [DisplayName("userwoman")]
        public string userwoman { get; set; }
        /// <summary>
        /// car
        /// </summary>
        /// <returns></returns>
        [DisplayName("car")]
        public string car { get; set; }
        /// <summary>
        /// equipment
        /// </summary>
        /// <returns></returns>
        [DisplayName("equipment")]
        public string equipment { get; set; }
        /// <summary>
        /// commander
        /// </summary>
        /// <returns></returns>
        [DisplayName("commander")]
        public string commander { get; set; }
        /// <summary>
        /// commanderTel
        /// </summary>
        /// <returns></returns>
        [DisplayName("commanderTel")]
        public string commanderTel { get; set; }
        /// <summary>
        /// contacts
        /// </summary>
        /// <returns></returns>
        [DisplayName("contacts")]
        public string contacts { get; set; }
        /// <summary>
        /// contactsTel
        /// </summary>
        /// <returns></returns>
        [DisplayName("contactsTel")]
        public string contactsTel { get; set; }
        /// <summary>
        /// addUser
        /// </summary>
        /// <returns></returns>
        [DisplayName("addUser")]
        public string addUser { get; set; }
        /// <summary>
        /// addDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("addDate")]
        public DateTime? addDate { get; set; }
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
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// reason
        /// </summary>
        /// <returns></returns>
        [DisplayName("reason")]
        public string reason { get; set; }
        /// <summary>
        /// remark
        /// </summary>
        /// <returns></returns>
        [DisplayName("remark")]
        public string remark { get; set; }
        /// <summary>
        /// days
        /// </summary>
        /// <returns></returns>
        [DisplayName("days")]
        public string days { get; set; }
        /// <summary>
        /// renwu
        /// </summary>
        /// <returns></returns>
        [DisplayName("renwu")]
        public string renwu { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.callpolice_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.callpolice_id = KeyValue;
                                            }
        #endregion
    }
}
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
    /// JW_OrderPolice
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.02 09:52</date>
    /// </author>
    /// </summary>
    [Description("JW_OrderPolice")]
    [PrimaryKey("orderpolice_id")]
    public class JW_OrderPolice : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// orderpolice_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("orderpolice_id")]
        public string orderpolice_id { get; set; }
        /// <summary>
        /// orderpoliceNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("orderpoliceNo")]
        public string orderpoliceNo { get; set; }
        /// <summary>
        /// from_unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("from_unit_id")]
        public string from_unit_id { get; set; }
        /// <summary>
        /// to_unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("to_unit_id")]
        public string to_unit_id { get; set; }
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
        /// orderUser
        /// </summary>
        /// <returns></returns>
        [DisplayName("orderUser")]
        public string orderUser { get; set; }
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
        /// <summary>
        /// FJUser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("FJUser_id")]
        public string FJUser_id { get; set; }
        /// <summary>
        /// FJDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("FJDate")]
        public DateTime? FJDate { get; set; }
        /// <summary>
        /// FJDetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("FJDetail")]
        public string FJDetail { get; set; }
        /// <summary>
        /// LeaderUser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("LeaderUser_id")]
        public string LeaderUser_id { get; set; }
        /// <summary>
        /// LeaderDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("LeaderDate")]
        public DateTime? LeaderDate { get; set; }
        /// <summary>
        /// LeaderDetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("LeaderDetail")]
        public string LeaderDetail { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.orderpolice_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.orderpolice_id = KeyValue;
                                            }
        #endregion
    }
}
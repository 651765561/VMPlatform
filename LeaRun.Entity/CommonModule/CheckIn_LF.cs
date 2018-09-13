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
    /// CheckIn_LF
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.25 09:42</date>
    /// </author>
    /// </summary>
    [Description("CheckIn_LF")]
    [PrimaryKey("checkIn_LF_Id")]
    public class CheckIn_LF : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// checkIn_LF_Id
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkIn_LF_Id")]
        public string checkIn_LF_Id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// sex
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex")]
        public string sex { get; set; }
        /// <summary>
        /// sfz_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("sfz_id")]
        public string sfz_id { get; set; }
        /// <summary>
        /// checkInfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkInfo")]
        public string checkInfo { get; set; }
        /// <summary>
        /// goods
        /// </summary>
        /// <returns></returns>
        [DisplayName("goods")]
        public string goods { get; set; }
        /// <summary>
        /// check_time
        /// </summary>
        /// <returns></returns>
        [DisplayName("check_time")]
        public DateTime? check_time { get; set; }
        /// <summary>
        /// remarks
        /// </summary>
        /// <returns></returns>
        [DisplayName("remarks")]
        public string remarks { get; set; }
        /// <summary>
        /// address
        /// </summary>
        /// <returns></returns>
        [DisplayName("address")]
        public string address { get; set; }
        /// <summary>
        /// tel
        /// </summary>
        /// <returns></returns>
        [DisplayName("tel")]
        public string tel { get; set; }
        /// <summary>
        /// unit
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit")]
        public string unit { get; set; }
        /// <summary>
        /// photo
        /// </summary>
        /// <returns></returns>
        [DisplayName("photo")]
        public string photo { get; set; }

         /// <summary>
        /// PoliceArea_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("PoliceArea_id")]
        public string PoliceArea_id { get; set; }

        /// <summary>
        /// alertLever
        /// </summary>
        /// <returns></returns>
        [DisplayName("alertLever")]
        public string alertLever { get; set; }
        /// <summary>
        /// adduser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("adduser_id")]
        public string adduser_id { get; set; }
        /// <summary>
        /// chuli
        /// </summary>
        /// <returns></returns>
        [DisplayName("chuli")]
        public string chuli { get; set; }
        /// <summary>
        /// department
        /// </summary>
        /// <returns></returns>
        [DisplayName("department")]
        public string department { get; set; }
        /// <summary>
        /// info
        /// </summary>
        /// <returns></returns>
        [DisplayName("info")]
        public string info { get; set; }
        /// <summary>
        /// userJD
        /// </summary>
        /// <returns></returns>
        [DisplayName("userJD")]
        public string userJD { get; set; }
       
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.checkIn_LF_Id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.checkIn_LF_Id = KeyValue;
                                            }
        #endregion
    }
}
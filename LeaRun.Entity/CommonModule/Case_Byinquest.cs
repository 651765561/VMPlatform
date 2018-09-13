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
    /// Case_Byinquest
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.17 16:17</date>
    /// </author>
    /// </summary>
    [Description("Case_Byinquest")]
    [PrimaryKey("Byinquest_id")]
    public class Case_Byinquest : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Byinquest_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Byinquest_id")]
        public string Byinquest_id { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// dep_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("dep_id")]
        public string dep_id { get; set; }
        /// <summary>
        /// case_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("case_id")]
        public string case_id { get; set; }
        /// <summary>
        /// outcode
        /// </summary>
        /// <returns></returns>
        [DisplayName("outcode")]
        public string outcode { get; set; }
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
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public string type { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// othername
        /// </summary>
        /// <returns></returns>
        [DisplayName("othername")]
        public string othername { get; set; }
        /// <summary>
        /// sex
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex")]
        public string sex { get; set; }
        /// <summary>
        /// age
        /// </summary>
        /// <returns></returns>
        [DisplayName("age")]
        public string age { get; set; }
        /// <summary>
        /// code
        /// </summary>
        /// <returns></returns>
        [DisplayName("code")]
        public string code { get; set; }
        /// <summary>
        /// nation
        /// </summary>
        /// <returns></returns>
        [DisplayName("nation")]
        public string nation { get; set; }
        /// <summary>
        /// work
        /// </summary>
        /// <returns></returns>
        [DisplayName("work")]
        public string work { get; set; }
        /// <summary>
        /// post
        /// </summary>
        /// <returns></returns>
        [DisplayName("post")]
        public string post { get; set; }
        /// <summary>
        /// workcall
        /// </summary>
        /// <returns></returns>
        [DisplayName("workcall")]
        public string workcall { get; set; }
        /// <summary>
        /// industry
        /// </summary>
        /// <returns></returns>
        [DisplayName("industry")]
        public string industry { get; set; }
        /// <summary>
        /// mobile
        /// </summary>
        /// <returns></returns>
        [DisplayName("mobile")]
        public string mobile { get; set; }
        /// <summary>
        /// homeaddress
        /// </summary>
        /// <returns></returns>
        [DisplayName("homeaddress")]
        public string homeaddress { get; set; }
        /// <summary>
        /// homeacall
        /// </summary>
        /// <returns></returns>
        [DisplayName("homecall")]
        public string homecall { get; set; }
        /// <summary>
        /// liveaddress
        /// </summary>
        /// <returns></returns>
        [DisplayName("liveaddress")]
        public string liveaddress { get; set; }
        /// <summary>
        /// isCommunist
        /// </summary>
        /// <returns></returns>
        [DisplayName("isCommunist")]
        public string isCommunist { get; set; }
        /// <summary>
        /// isNPCmember
        /// </summary>
        /// <returns></returns>
        [DisplayName("isNPCmember")]
        public string isNPCmember { get; set; }
        /// <summary>
        /// isCPPCCmembers
        /// </summary>
        /// <returns></returns>
        [DisplayName("isCPPCCmembers")]
        public string isCPPCCmembers { get; set; }
        /// <summary>
        /// isOther
        /// </summary>
        /// <returns></returns>
        [DisplayName("isOther")]
        public string isOther { get; set; }
        /// <summary>
        /// img
        /// </summary>
        /// <returns></returns>
        [DisplayName("img")]
        public string img { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public string state { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Byinquest_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Byinquest_id = KeyValue;
        }
        #endregion
    }
}

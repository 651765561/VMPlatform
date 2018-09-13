using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace LeaRun.Entity
{
    [Description("Base_Meeting")]
    [PrimaryKey("meetingid")]
    public class Base_Meeting : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("主键")]
        public string meetingid { get; set; }
        /// <summary>
        /// 会议id
        /// </summary>
        /// <returns></returns>
        [DisplayName("会议id")]
        public int? Meeting_id { get; set; }
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
        /// adddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("adddate")]
        public DateTime? adddate { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
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
        /// username
        /// </summary>
        /// <returns></returns>
        [DisplayName("username")]
        public string username { get; set; }
        /// <summary>
        /// QJid
        /// </summary>
        /// <returns></returns>
        [DisplayName("QJid")]
        public string QJid { get; set; }
        /// <summary>
        /// LDid
        /// </summary>
        /// <returns></returns>
        [DisplayName("LDid")]
        public string LDid { get; set; }
        /// <summary>
        /// ZhCid
        /// </summary>
        /// <returns></returns>
        [DisplayName("ZhCid")]
        public string ZhCid { get; set; }
        /// <summary>
        /// screenmodel
        /// </summary>
        /// <returns></returns>
        [DisplayName("screenmodel")]
        public int? screenmodel { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// userid
        /// </summary>
        /// <returns></returns>
        [DisplayName("userid")]
        public string userid { get; set; }
        #endregion


        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.meetingid = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.meetingid = KeyValue;
        }
        #endregion
    }
}

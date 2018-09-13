//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
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
    /// People_Clear
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.02.04 18:23</date>
    /// </author>
    /// </summary>
    [Description("People_Clear")]
    [PrimaryKey("People_id")]
    public class People_Clear : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// People_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("People_id")]
        public string People_id { get; set; }
        /// <summary>
        /// code
        /// </summary>
        /// <returns></returns>
        [DisplayName("code")]
        public string code { get; set; }
        /// <summary>
        /// designation
        /// </summary>
        /// <returns></returns>
        [DisplayName("designation")]
        public string designation { get; set; }
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
        /// selftype
        /// </summary>
        /// <returns></returns>
        [DisplayName("selftype")]
        public string selftype { get; set; }
        /// <summary>
        /// selfmoney
        /// </summary>
        /// <returns></returns>
        [DisplayName("selfmoney")]
        public decimal? selfmoney { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
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
        /// outuser
        /// </summary>
        /// <returns></returns>
        [DisplayName("outuser")]
        public string outuser { get; set; }
        /// <summary>
        /// outdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("outdate")]
        public DateTime? outdate { get; set; }
        /// <summary>
        /// room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("room_id")]
        public string room_id { get; set; }
        /// <summary>
        /// adduser
        /// </summary>
        /// <returns></returns>
        [DisplayName("adduser")]
        public string adduser { get; set; }
        /// <summary>
        /// adddate
        /// </summary>
        /// <returns></returns>
        [DisplayName("adddate")]
        public DateTime? adddate { get; set; }
        /// <summary>
        /// account
        /// </summary>
        /// <returns></returns>
        [DisplayName("account")]
        public decimal? account { get; set; }
        /// <summary>
        /// note
        /// </summary>
        /// <returns></returns>
        [DisplayName("note")]
        public string note { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// limit
        /// </summary>
        /// <returns></returns>
        [DisplayName("limit")]
        public decimal? limit { get; set; }
        /// <summary>
        /// punish
        /// </summary>
        /// <returns></returns>
        [DisplayName("punish")]
        public string punish { get; set; }
        /// <summary>
        /// cur_limit
        /// </summary>
        /// <returns></returns>
        [DisplayName("cur_limit")]
        public decimal? cur_limit { get; set; }
        /// <summary>
        /// cur_date
        /// </summary>
        /// <returns></returns>
        [DisplayName("cur_date")]
        public DateTime? cur_date { get; set; }
        /// <summary>
        /// psw
        /// </summary>
        /// <returns></returns>
        [DisplayName("psw")]
        public string psw { get; set; }
        /// <summary>
        /// istnb
        /// </summary>
        /// <returns></returns>
        [DisplayName("istnb")]
        public int? istnb { get; set; }
        /// <summary>
        /// isgxy
        /// </summary>
        /// <returns></returns>
        [DisplayName("isgxy")]
        public int? isgxy { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.People_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.People_id = KeyValue;
                                            }
        #endregion
    }
}
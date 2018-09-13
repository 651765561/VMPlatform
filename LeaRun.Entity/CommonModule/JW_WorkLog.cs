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
    /// JW_WorkLog
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.05.17 10:50</date>
    /// </author>
    /// </summary>
    [Description("JW_WorkLog")]
    [PrimaryKey("JW_WorkLog_id")]
    public class JW_WorkLog : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// JW_WorkLog_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("JW_WorkLog_id")]
        public string JW_WorkLog_id { get; set; }
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
        /// user_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("user_id")]
        public string user_id { get; set; }
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
        /// content
        /// </summary>
        /// <returns></returns>
        [DisplayName("content")]
        public string content { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.JW_WorkLog_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.JW_WorkLog_id = KeyValue;
                                            }
        #endregion
    }
}
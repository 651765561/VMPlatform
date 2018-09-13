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
    /// CheckIn_ZQ
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.25 13:16</date>
    /// </author>
    /// </summary>
    [Description("CheckIn_ZQ")]
    [PrimaryKey("checkIn_ZQ_Id")]
    public class CheckIn_ZQ : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// checkIn_ZQ_Id
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkIn_ZQ_Id")]
        public string checkIn_ZQ_Id { get; set; }
        /// <summary>
        /// PoliceArea_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("PoliceArea_id")]
        public string PoliceArea_id { get; set; }
        /// <summary>
        /// userId
        /// </summary>
        /// <returns></returns>
        [DisplayName("userId")]
        public string userId { get; set; }
        /// <summary>
        /// userName
        /// </summary>
        /// <returns></returns>
        [DisplayName("userName")]
        public string userName { get; set; }
        /// <summary>
        /// startTime
        /// </summary>
        /// <returns></returns>
        [DisplayName("startTime")]
        public DateTime? startTime { get; set; }
        /// <summary>
        /// endTime
        /// </summary>
        /// <returns></returns>
        [DisplayName("endTime")]
        public DateTime? endTime { get; set; }
        /// <summary>
        /// matters
        /// </summary>
        /// <returns></returns>
        [DisplayName("matters")]
        public string matters { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.checkIn_ZQ_Id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.checkIn_ZQ_Id = KeyValue;
                                            }
        #endregion
    }
}
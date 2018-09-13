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
    /// CaseBreakRoles
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.07 10:51</date>
    /// </author>
    /// </summary>
    [Description("CaseBreakRoles")]
    [PrimaryKey("breakroles_id")]
    public class CaseBreakRoles : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// breakroles_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("breakroles_id")]
        public string breakroles_id { get; set; }
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
        /// watchuser
        /// </summary>
        /// <returns></returns>
        [DisplayName("watchuser")]
        public string watchuser { get; set; }
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
        /// detail
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        /// <summary>
        /// treatment
        /// </summary>
        /// <returns></returns>
        [DisplayName("treatment")]
        public string treatment { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.breakroles_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.breakroles_id = KeyValue;
                                            }
        #endregion
    }
}
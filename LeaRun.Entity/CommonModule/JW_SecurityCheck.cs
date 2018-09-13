using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;

namespace LeaRun.Entity
{
    [Description("JW_SecurityCheck")]
    [PrimaryKey("SecurityCheck_id")]
    public class JW_SecurityCheck : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// SecurityCheck_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("SecurityCheck_id")]
        public string SecurityCheck_id { get; set; }
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
        /// checkuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkuser_id")]
        public string checkuser_id { get; set; }
        /// <summary>
        /// checkDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkDate")]
        public DateTime? checkDate { get; set; }
        /// <summary>
        /// checkplace
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkplace")]
        public string checkplace { get; set; }
        /// <summary>
        /// cardcode
        /// </summary>
        /// <returns></returns>
        [DisplayName("cardcode")]
        public string cardcode { get; set; }
        /// <summary>
        /// checkmethod
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkmethod")]
        public string checkmethod { get; set; }
        /// <summary>
        /// checkdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("checkdetail")]
        public string checkdetail { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SecurityCheck_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.SecurityCheck_id = KeyValue;
        }
        #endregion
    }
}

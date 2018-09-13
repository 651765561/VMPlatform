using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;

namespace LeaRun.Entity
{
    [Description("JW_GoodsMain")]
    [PrimaryKey("goodsmain_id")]
    public class JW_GoodsMain : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// goodsmain_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("goodsmain_id")]
        public string goodsmain_id { get; set; }
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
        /// getuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("getuser_id")]
        public string getuser_id { get; set; }
        /// <summary>
        /// getDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("getDate")]
        public DateTime? getDate { get; set; }
        /// <summary>
        /// sarqm
        /// </summary>
        /// <returns></returns>
        [DisplayName("sarqm")]
        public string sarqm { get; set; }
        /// <summary>
        /// barqm
        /// </summary>
        /// <returns></returns>
        [DisplayName("barqm")]
        public string barqm { get; set; }
        /// <summary>
        /// zqfjqm
        /// </summary>
        /// <returns></returns>
        [DisplayName("zqfjqm")]
        public string zqfjqm { get; set; }
        /// <summary>
        /// LockersNum
        /// </summary>
        /// <returns></returns>
        [DisplayName("LockersNum")]
        public string LockersNum { get; set; }
        /// <summary>
        /// LockersPsw
        /// </summary>
        /// <returns></returns>
        [DisplayName("LockersPsw")]
        public string LockersPsw { get; set; }
        /// <summary>
        /// backuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("backuser_id")]
        public string backuser_id { get; set; }
        /// <summary>
        /// backDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("backDate")]
        public DateTime? backDate { get; set; }
        /// <summary>
        /// sarqm2
        /// </summary>
        /// <returns></returns>
        [DisplayName("sarqm2")]
        public string sarqm2 { get; set; }
        /// <summary>
        /// barqm2
        /// </summary>
        /// <returns></returns>
        [DisplayName("barqm2")]
        public string barqm2 { get; set; }
        /// <summary>
        /// zqfjqm2
        /// </summary>
        /// <returns></returns>
        [DisplayName("zqfjqm2")]
        public string zqfjqm2 { get; set; }
        /// <summary>
        /// zqfjqm2
        /// </summary>
        /// <returns></returns>
        [DisplayName("detail")]
        public string detail { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.goodsmain_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.goodsmain_id = KeyValue;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;

namespace LeaRun.Entity
{
    [Description("Case_caseinfo")]
    [PrimaryKey("case_id")]
    public class Case_caseinfo : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// case_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("case_id")]
        public string case_id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// CaseCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("CaseCode")]
        public string CaseCode { get; set; }
        /// <summary>
        /// Brief
        /// </summary>
        /// <returns></returns>
        [DisplayName("Brief")]
        public string Brief { get; set; }
        /// <summary>
        /// acceptnumber
        /// </summary>
        /// <returns></returns>
        [DisplayName("acceptnumber")]
        public string acceptnumber { get; set; }
        /// <summary>
        /// acceptdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("acceptdate")]
        public DateTime? acceptdate { get; set; }
        /// <summary>
        /// casetype
        /// </summary>
        /// <returns></returns>
        [DisplayName("casetype")]
        public string casetype { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// unitname
        /// </summary>
        /// <returns></returns>
        [DisplayName("unitname")]
        public string unitname { get; set; }
        /// <summary>
        /// dep_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("dep_id")]
        public string dep_id { get; set; }
        /// <summary>
        /// depname
        /// </summary>
        /// <returns></returns>
        [DisplayName("depname")]
        public string depname { get; set; }
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
        /// State
        /// </summary>
        /// <returns></returns>
        [DisplayName("State")]
        public int? State { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.case_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.case_id = KeyValue;
        }
        #endregion
    }
}

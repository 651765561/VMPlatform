using LeaRun.DataAccess.Attributes;
using LeaRun.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LeaRun.Entity.CommonModule
{
    /// <summary>
    /// Base_MonitorApplication
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.19 16:17</date>
    /// </author>
    /// </summary>
    [Description("Base_MonitorApplication")]
    [PrimaryKey("MonitorApplication_id")]
    public class Base_MonitorApplication : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// MonitorApplication_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MonitorApplication_id")]
        public string MonitorApplication_id { get; set; }
        /// <summary>
        /// MonitorChannels_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MonitorChannels_id")]
        public string MonitorChannels_id { get; set; }
        /// <summary>
        /// Room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Room_id")]
        public string Room_id { get; set; }
        /// <summary>
        /// Object_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Object_id")]
        public string Object_id { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public string type { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MonitorApplication_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.MonitorApplication_id = KeyValue;
        }
        #endregion
    }
}

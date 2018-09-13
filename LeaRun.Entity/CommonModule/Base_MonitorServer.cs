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
    /// Base_MonitorServer
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.13 17:27</date>
    /// </author>
    /// </summary>
    [Description("Base_MonitorServer")]
    [PrimaryKey("MonitorServer_id")]
    public class Base_MonitorServer : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// MonitorServer_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MonitorServer_id")]
        public string MonitorServer_id { get; set; }
        /// <summary>
        /// ServerName
        /// </summary>
        /// <returns></returns>
        [DisplayName("ServerName")]
        public string ServerName { get; set; }
        /// <summary>
        /// ServerCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("ServerCode")]
        public string ServerCode { get; set; }
        /// <summary>
        /// ServerIP
        /// </summary>
        /// <returns></returns>
        [DisplayName("ServerIP")]
        public string ServerIP { get; set; }
        /// <summary>
        /// ServerUser
        /// </summary>
        /// <returns></returns>
        [DisplayName("ServerUser")]
        public string ServerUser { get; set; }
        /// <summary>
        /// ServerPSW
        /// </summary>
        /// <returns></returns>
        [DisplayName("ServerPSW")]
        public string ServerPSW { get; set; }
        /// <summary>
        /// ServerPort
        /// </summary>
        /// <returns></returns>
        [DisplayName("ServerPort")]
        public string ServerPort { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public string type { get; set; }
        /// <summary>
        /// Unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Unit_id")]
        public string Unit_id { get; set; }
        /// <summary>
        /// domain_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("domain_id")]
        public string domain_id { get; set; }

        /// <summary>
        /// PlatKind
        /// </summary>
        /// <returns></returns>
        [DisplayName("PlatKind")]
        public string PlatKind { get; set; }
        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MonitorServer_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.MonitorServer_id = KeyValue;
        }
        #endregion
    }
}
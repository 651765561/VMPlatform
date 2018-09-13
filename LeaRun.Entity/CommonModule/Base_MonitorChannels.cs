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
    /// Base_MonitorChannels
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.13 17:28</date>
    /// </author>
    /// </summary>
    [Description("Base_MonitorChannels")]
    [PrimaryKey("MonitorChannels_id")]
    public class Base_MonitorChannels : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// MonitorChannels_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MonitorChannels_id")]
        public string MonitorChannels_id { get; set; }
        /// <summary>
        /// MonitorServer_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("MonitorServer_id")]
        public string MonitorServer_id { get; set; }
        /// <summary>
        /// ChannelName
        /// </summary>
        /// <returns></returns>
        [DisplayName("ChannelName")]
        public string ChannelName { get; set; }
        /// <summary>
        /// ChannelCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("ChannelCode")]
        public string ChannelCode { get; set; }
        /// <summary>
        /// Channels
        /// </summary>
        /// <returns></returns>
        [DisplayName("Channels")]
        public string Channels { get; set; }
        /// <summary>
        /// State
        /// </summary>
        /// <returns></returns>
        [DisplayName("State")]
        public int? State { get; set; }
        /// <summary>
        /// PictureQuality
        /// </summary>
        /// <returns></returns>
        [DisplayName("PictureQuality")]
        public int? PictureQuality { get; set; }

        /// <summary>
        /// manufactory
        /// </summary>
        /// <returns></returns>
        [DisplayName("manufactory")]
        public string manufactory { get; set; }


        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MonitorChannels_id = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.MonitorChannels_id = KeyValue;
        }
        #endregion
    }
}
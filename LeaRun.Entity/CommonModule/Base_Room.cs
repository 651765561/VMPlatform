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
    /// Base_Room
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.12 10:54</date>
    /// </author>
    /// </summary>
    [Description("Base_Room")]
    [PrimaryKey("Room_id")]
    public class Base_Room : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// Room_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Room_id")]
        public string Room_id { get; set; }
        /// <summary>
        /// RoomName
        /// </summary>
        /// <returns></returns>
        [DisplayName("RoomName")]
        public string RoomName { get; set; }
        /// <summary>
        /// RoomCode
        /// </summary>
        /// <returns></returns>
        [DisplayName("RoomCode")]
        public string RoomCode { get; set; }
        /// <summary>
        /// RoomType_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("RoomType_id")]
        public string RoomType_id { get; set; }
        /// <summary>
        /// PoliceArea_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("PoliceArea_id")]
        public string PoliceArea_id { get; set; }
        /// <summary>
        /// Unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Unit_id")]
        public string Unit_id { get; set; }
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
            this.Room_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Room_id = KeyValue;
                                            }
        #endregion
    }
}
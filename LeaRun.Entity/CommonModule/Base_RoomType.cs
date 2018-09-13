//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2018
// Software Developers @ Learun 2018
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
    /// Base_RoomType
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.08.30 10:18</date>
    /// </author>
    /// </summary>
    [Description("Base_RoomType")]
    [PrimaryKey("RoomType_id")]
    public class Base_RoomType : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// RoomType_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("RoomType_id")]
        public string RoomType_id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        [DisplayName("Name")]
        public string Name { get; set; }
        /// <summary>
        /// type
        /// </summary>
        /// <returns></returns>
        [DisplayName("type")]
        public int? type { get; set; }
        /// <summary>
        /// action
        /// </summary>
        /// <returns></returns>
        [DisplayName("action")]
        public string action { get; set; }
        /// <summary>
        /// orders
        /// </summary>
        /// <returns></returns>
        [DisplayName("orders")]
        public int? orders { get; set; }
        /// <summary>
        /// bigtype
        /// </summary>
        /// <returns></returns>
        [DisplayName("bigtype")]
        public string bigtype { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.RoomType_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.RoomType_id = KeyValue;
                                            }
        #endregion
    }
}
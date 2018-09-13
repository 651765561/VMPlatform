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
    /// Base_PoliceArea
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.18 13:09</date>
    /// </author>
    /// </summary>
    [Description("Base_PoliceArea")]
    [PrimaryKey("PoliceArea_id")]
    public class Base_PoliceArea : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// PoliceArea_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("PoliceArea_id")]
        public string PoliceArea_id { get; set; }
        /// <summary>
        /// AreaName
        /// </summary>
        /// <returns></returns>
        [DisplayName("AreaName")]
        public string AreaName { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        /// <returns></returns>
        [DisplayName("Code")]
        public string Code { get; set; }
        /// <summary>
        /// AreaType
        /// </summary>
        /// <returns></returns>
        [DisplayName("AreaType")]
        public string AreaType { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        /// <returns></returns>
        [DisplayName("Address")]
        public string Address { get; set; }
        /// <summary>
        /// BuildDate
        /// </summary>
        /// <returns></returns>
        [DisplayName("BuildDate")]
        public DateTime? BuildDate { get; set; }
        /// <summary>
        /// YearExamine
        /// </summary>
        /// <returns></returns>
        [DisplayName("YearExamine")]
        public DateTime? YearExamine { get; set; }
        /// <summary>
        /// RoomInfo
        /// </summary>
        /// <returns></returns>
        [DisplayName("RoomInfo")]
        public string RoomInfo { get; set; }
        /// <summary>
        /// DepLeader
        /// </summary>
        /// <returns></returns>
        [DisplayName("DepLeader")]
        public string DepLeader { get; set; }
        /// <summary>
        /// UnitLeader
        /// </summary>
        /// <returns></returns>
        [DisplayName("UnitLeader")]
        public string UnitLeader { get; set; }
        /// <summary>
        /// unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_id")]
        public string unit_id { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        [DisplayName("SortCode")]
        public string SortCode { get; set; }
        /// <summary>
        /// SortCode
        /// </summary>
        /// <returns></returns>

        
        #endregion



        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.PoliceArea_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.PoliceArea_id = KeyValue;
                                            }
        #endregion
    }
}
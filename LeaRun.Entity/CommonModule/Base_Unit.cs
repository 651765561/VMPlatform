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
    /// Base_Unit
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.09.06 09:43</date>
    /// </author>
    /// </summary>
    [Description("Base_Unit")]
    [PrimaryKey("Base_Unit_id")]
    public class Base_Unit : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("Base_Unit_id")]
        public string Base_Unit_id { get; set; }
        /// <summary>
        /// unit
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit")]
        public string unit { get; set; }
        /// <summary>
        /// longname
        /// </summary>
        /// <returns></returns>
        [DisplayName("longname")]
        public string longname { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// parent_unit_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("parent_unit_id")]
        public string parent_unit_id { get; set; }
        /// <summary>
        /// orders
        /// </summary>
        /// <returns></returns>
        [DisplayName("sortcode")]
        public int? sortcode { get; set; }
        /// <summary>
        /// regcode
        /// </summary>
        /// <returns></returns>
        [DisplayName("regcode")]
        public string regcode { get; set; }
   
        /// <summary>
        /// 单位编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("code")]
        public string code { get; set; }

        
        /// <summary>
        /// 流媒体地址
        /// </summary>
        /// <returns></returns>
        [DisplayName("streamserverip")]
        public string streamserverip { get; set; }
        
          /// <summary>
        /// 单位平台地址
        /// </summary>
        /// <returns></returns>
        [DisplayName("webURL")]
        public string webURL { get; set; }
        
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Base_Unit_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Base_Unit_id = KeyValue;
                                            }
        #endregion
    }
}
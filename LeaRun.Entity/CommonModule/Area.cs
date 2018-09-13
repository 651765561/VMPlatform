//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
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
    /// Base_Area
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.03 14:55</date>
    /// </author>
    /// </summary>
    [Description("Area")]
    [PrimaryKey("Area_id")]
    public class Base_Area : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// Area_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("Area_id")]
        public string Area_id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        /// <returns></returns>
        [DisplayName("name")]
        public string name { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        /// <returns></returns>
        [DisplayName("Code")]
        public string Code { get; set; }
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.Area_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Area_id = KeyValue;
                                            }
        #endregion
    }
}
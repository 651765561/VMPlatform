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
    /// JW_PoliceApplyAction
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.20 15:11</date>
    /// </author>
    /// </summary>
    [Description("JW_PoliceApplyAction")]
    [PrimaryKey("apply_id")]
    public class JW_PoliceApplyAction : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// apply_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("apply_id")]
        public string apply_id { get; set; }
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
        /// tasktype_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("tasktype_id")]
        public string tasktype_id { get; set; }
        /// <summary>
        /// usedate
        /// </summary>
        /// <returns></returns>
        [DisplayName("usedate")]
        public DateTime? usedate { get; set; }
        /// <summary>
        /// useadd
        /// </summary>
        /// <returns></returns>
        [DisplayName("useadd")]
        public string useadd { get; set; }
        /// <summary>
        /// usenum
        /// </summary>
        /// <returns></returns>
        [DisplayName("usenum")]
        public string usenum { get; set; }
        /// <summary>
        /// useman
        /// </summary>
        /// <returns></returns>
        [DisplayName("useman")]
        public string useman { get; set; }
        /// <summary>
        /// usewoman
        /// </summary>
        /// <returns></returns>
        [DisplayName("usewoman")]
        public string usewoman { get; set; }
        /// <summary>
        /// armsorder
        /// </summary>
        /// <returns></returns>
        [DisplayName("armsorder")]
        public string armsorder { get; set; }
        /// <summary>
        /// otherorder
        /// </summary>
        /// <returns></returns>
        [DisplayName("otherorder")]
        public string otherorder { get; set; }
        /// <summary>
        /// applydetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("applydetail")]
        public string applydetail { get; set; }
        /// <summary>
        /// ApprovalDetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("ApprovalDetail")]
        public string ApprovalDetail { get; set; }
        /// <summary>
        /// fjuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjuser_id")]
        public string fjuser_id { get; set; }
        /// <summary>
        /// fjdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjdate")]
        public DateTime? fjdate { get; set; }
        /// <summary>
        /// fjdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("fjdetail")]
        public string fjdetail { get; set; }
        /// <summary>
        /// leaderuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("leaderuser_id")]
        public string leaderuser_id { get; set; }
        /// <summary>
        /// leaderdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("leaderdate")]
        public DateTime? leaderdate { get; set; }
        /// <summary>
        /// leaderdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("leaderdetail")]
        public string leaderdetail { get; set; }
        /// <summary>
        /// actuser_id
        /// </summary>
        /// <returns></returns>
        [DisplayName("actuser_id")]
        public string actuser_id { get; set; }
        /// <summary>
        /// actdate
        /// </summary>
        /// <returns></returns>
        [DisplayName("actdate")]
        public DateTime? actdate { get; set; }
        /// <summary>
        /// actdetail
        /// </summary>
        /// <returns></returns>
        [DisplayName("actdetail")]
        public string actdetail { get; set; }
        /// <summary>
        /// state
        /// </summary>
        /// <returns></returns>
        [DisplayName("state")]
        public int? state { get; set; }
        /// <summary>
        /// name_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_2")]
        public string name_2 { get; set; }
        /// <summary>
        /// sex_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_2")]
        public string sex_2 { get; set; }
        /// <summary>
        /// age_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_2")]
        public string age_2 { get; set; }
        /// <summary>
        /// workunit_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("workunit_2")]
        public string workunit_2 { get; set; }
        /// <summary>
        /// address_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_2")]
        public string address_2 { get; set; }
        /// <summary>
        /// content_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("content_2")]
        public string content_2 { get; set; }
        /// <summary>
        /// contractors_2
        /// </summary>
        /// <returns></returns>
        [DisplayName("contractors_2")]
        public string contractors_2 { get; set; }
        /// <summary>
        /// name_3
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_3")]
        public string name_3 { get; set; }
        /// <summary>
        /// sex_3
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_3")]
        public string sex_3 { get; set; }
        /// <summary>
        /// age_3
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_3")]
        public string age_3 { get; set; }
        /// <summary>
        /// workunit_3
        /// </summary>
        /// <returns></returns>
        [DisplayName("workunit_3")]
        public string workunit_3 { get; set; }
        /// <summary>
        /// address_3
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_3")]
        public string address_3 { get; set; }
        /// <summary>
        /// id_3
        /// </summary>
        /// <returns></returns>
        [DisplayName("id_3")]
        public string id_3 { get; set; }
        /// <summary>
        /// name_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_4")]
        public string name_4 { get; set; }
        /// <summary>
        /// sex_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_4")]
        public string sex_4 { get; set; }
        /// <summary>
        /// age_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_4")]
        public string age_4 { get; set; }
        /// <summary>
        /// workunit_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("workunit_4")]
        public string workunit_4 { get; set; }
        /// <summary>
        /// address_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_4")]
        public string address_4 { get; set; }
        /// <summary>
        /// id_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("id_4")]
        public string id_4 { get; set; }
        /// <summary>
        /// physical_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("physical_4")]
        public string physical_4 { get; set; }
        /// <summary>
        /// unit_4
        /// </summary>
        /// <returns></returns>
        [DisplayName("unit_4")]
        public string unit_4 { get; set; }
        /// <summary>
        /// name_5
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_5")]
        public string name_5 { get; set; }
        /// <summary>
        /// sex_5
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_5")]
        public string sex_5 { get; set; }
        /// <summary>
        /// age_5
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_5")]
        public string age_5 { get; set; }
        /// <summary>
        /// workunit_5
        /// </summary>
        /// <returns></returns>
        [DisplayName("workunit_5")]
        public string workunit_5 { get; set; }
        /// <summary>
        /// address_5
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_5")]
        public string address_5 { get; set; }
        /// <summary>
        /// id_5
        /// </summary>
        /// <returns></returns>
        [DisplayName("id_5")]
        public string id_5 { get; set; }
        /// <summary>
        /// name_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_6")]
        public string name_6 { get; set; }
        /// <summary>
        /// sex_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_6")]
        public string sex_6 { get; set; }
        /// <summary>
        /// age_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_6")]
        public string age_6 { get; set; }
        /// <summary>
        /// workunit_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("workunit_6")]
        public string workunit_6 { get; set; }
        /// <summary>
        /// address_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_6")]
        public string address_6 { get; set; }
        /// <summary>
        /// id_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("id_6")]
        public string id_6 { get; set; }
        /// <summary>
        /// features_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("features_6")]
        public string features_6 { get; set; }
        /// <summary>
        /// contact_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("contact_6")]
        public string contact_6 { get; set; }
        /// <summary>
        /// hidingplace_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("hidingplace_6")]
        public string hidingplace_6 { get; set; }
        /// <summary>
        /// weapon_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("weapon_6")]
        public string weapon_6 { get; set; }
        /// <summary>
        /// contractors_unit_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("contractors_unit_6")]
        public string contractors_unit_6 { get; set; }
        /// <summary>
        /// contractors_address_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("contractors_address_6")]
        public string contractors_address_6 { get; set; }
        /// <summary>
        /// contractors_phone_6
        /// </summary>
        /// <returns></returns>
        [DisplayName("contractors_phone_6")]
        public string contractors_phone_6 { get; set; }
        /// <summary>
        /// name_7
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_7")]
        public string name_7 { get; set; }
        /// <summary>
        /// sex_7
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_7")]
        public string sex_7 { get; set; }
        /// <summary>
        /// age_7
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_7")]
        public string age_7 { get; set; }
        /// <summary>
        /// workunit_7
        /// </summary>
        /// <returns></returns>
        [DisplayName("workunit_7")]
        public string workunit_7 { get; set; }
        /// <summary>
        /// address_7
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_7")]
        public string address_7 { get; set; }
        /// <summary>
        /// search_content_7
        /// </summary>
        /// <returns></returns>
        [DisplayName("search_content_7")]
        public string search_content_7 { get; set; }
        /// <summary>
        /// name_8
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_8")]
        public string name_8 { get; set; }
        /// <summary>
        /// sex_8
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_8")]
        public string sex_8 { get; set; }
        /// <summary>
        /// age_8
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_8")]
        public string age_8 { get; set; }
        /// <summary>
        /// id_8
        /// </summary>
        /// <returns></returns>
        [DisplayName("id_8")]
        public string id_8 { get; set; }
        /// <summary>
        /// charge_8
        /// </summary>
        /// <returns></returns>
        [DisplayName("charge_8")]
        public string charge_8 { get; set; }
        /// <summary>
        /// way_8
        /// </summary>
        /// <returns></returns>
        [DisplayName("way_8")]
        public string way_8 { get; set; }
        /// <summary>
        /// place_9
        /// </summary>
        /// <returns></returns>
        [DisplayName("place_9")]
        public string place_9 { get; set; }
        /// <summary>
        /// name_9
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_9")]
        public string name_9 { get; set; }
        /// <summary>
        /// sex_9
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_9")]
        public string sex_9 { get; set; }
        /// <summary>
        /// age_9
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_9")]
        public string age_9 { get; set; }
        /// <summary>
        /// physical_9
        /// </summary>
        /// <returns></returns>
        [DisplayName("physical_9")]
        public string physical_9 { get; set; }
        /// <summary>
        /// name_10
        /// </summary>
        /// <returns></returns>
        [DisplayName("name_10")]
        public string name_10 { get; set; }
        /// <summary>
        /// sex_10
        /// </summary>
        /// <returns></returns>
        [DisplayName("sex_10")]
        public string sex_10 { get; set; }
        /// <summary>
        /// age_10
        /// </summary>
        /// <returns></returns>
        [DisplayName("age_10")]
        public string age_10 { get; set; }
        /// <summary>
        /// contact_10
        /// </summary>
        /// <returns></returns>
        [DisplayName("contact_10")]
        public string contact_10 { get; set; }
        /// <summary>
        /// address_10
        /// </summary>
        /// <returns></returns>
        [DisplayName("address_10")]
        public string address_10 { get; set; }
        /// <summary>
        /// time_10
        /// </summary>
        /// <returns></returns>
        [DisplayName("time_10")]
        public DateTime? time_10 { get; set; }
        /// <summary>
        /// case_nature_11
        /// </summary>
        /// <returns></returns>
        [DisplayName("case_nature_11")]
        public string case_nature_11 { get; set; }
        /// <summary>
        /// criminal_num_11
        /// </summary>
        /// <returns></returns>
        [DisplayName("criminal_num_11")]
        public string criminal_num_11 { get; set; }
        /// <summary>
        /// court_11
        /// </summary>
        /// <returns></returns>
        [DisplayName("court_11")]
        public string court_11 { get; set; }
        /// <summary>
        /// police_num_11
        /// </summary>
        /// <returns></returns>
        [DisplayName("police_num_11")]
        public string police_num_11 { get; set; }
        /// <summary>
        /// increase_num_13
        /// </summary>
        /// <returns></returns>
        [DisplayName("increase_num_13")]
        public string increase_num_13 { get; set; }
        /// <summary>
        /// nature_13
        /// </summary>
        /// <returns></returns>
        [DisplayName("nature_13")]
        public string nature_13 { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.apply_id = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.apply_id = KeyValue;
                                            }
        #endregion
    }
}
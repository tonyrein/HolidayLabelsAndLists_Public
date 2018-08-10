using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class BagLabelInfo_DAO
    {
        public int Id { get; set; }
        public int year { get; set; }
        public string family_id { get; set; }
        public string family_name { get; set; }
        public string family_members { get; set; }
        public string request_type { get; set; }
        public string donor_code { get; set; }
        public string donor_name { get; set; }
    }

    public class Donor_DAO
    {
        public int Id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }

    public class GiftLabelInfo_DAO
    {
        public int Id { get; set; }
        public int year { get; set; }
        public string family_id { get; set; }
        public string family_name { get; set; }
        public string child_name { get; set; }
        public string child_gender { get; set; }
        public int child_age { get; set; }
        public string request_type { get; set; }
        public string request_detail { get; set; }
        public string donor_code { get; set; }
        public string donor_name { get; set; }
    }

    public class ServicesHouseholdEnrollment_DAO
    {
        public int Id { get; set; }
        public string service_type { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public string head_of_household { get; set; }
        public string family_id { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state_or_province { get; set; }
        public string postal_code { get; set; }
    }

    /// <summary>
    /// used to prepare ParticipantSummaryLabel docs
    /// </summary>
    class FamiliesAndKids
    {
         public ServicesHouseholdEnrollment_DAO dao { get; set; }
         public string[] kids { get; set; }
         public int gift_card_count { get; set; }
     }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Classes in the file Models.cs are mostly just "bags of
/// properties," with some logic in the setters to validate
/// the input.
/// 
/// In addition, the Donor class transforms input strings
/// using methods from Utils.TextUtils and from the standard
/// library to insure that donor names and donor codes are
/// in a consistent format.
/// 
/// Month, day and year in ServicesHouseholdEnrollment are NOT
/// checked to see if they represent valid dates -- the code
/// simply checks that the month is between 1 and 12, the day
/// between 1 and 31, and the year >= 2000.
/// </summary>
namespace DAO
{
    public class BagLabelInfo
    {
        //private BagLabelInfo_DAO _dao = new BagLabelInfo_DAO();
        public BagLabelInfo_DAO dao { get; set; }
        //public BagLabelInfo_DAO dao { get { return _dao; } }
        public int year
        {
            get { return dao.year; }
            set
            {
                if (value < 2000) throw new ArgumentException("bag_label_info year must be >= 2000.");
                dao.year = value;
            }
        }
        public string family_id { get { return dao.family_id; } set { dao.family_id = value; } }
        public string family_name { get { return dao.family_name; } set { dao.family_name = value; } }
        public string family_members { get { return dao.family_members; } set { dao.family_members = value; } } 
        public string request_type { get { return dao.request_type; } set { dao.request_type = value; } }
        public string donor_code { get { return dao.donor_code; } set { dao.donor_code = value; } }
        public string donor_name { get { return dao.donor_name; } set { dao.donor_name = value; } }
        public BagLabelInfo()
        {
            this.dao = new BagLabelInfo_DAO();
        }
        public BagLabelInfo(BagLabelInfo_DAO dao)
        {
            this.dao = dao;
        }
    }

    public class Donor
    {
        public Donor_DAO dao { get; set; }
        //public Donor_DAO dao { get { return _dao; } }
        public static string MakeDonorName(string s)
        {
            string scr = Utils.TextUtils.CleanString(s);
            return scr.Replace('_', ' ');
        }
        public static string MakeDonorCode(string s)
        {
            return Utils.TextUtils.CleanString(s).ToUpper();
        }

        public string code
        {
            get { return dao.code; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    dao.code = Donor.MakeDonorCode(value);
                else // generate from name
                    dao.code = MakeDonorCode(name);
            }
        }
        public string name
        {
            get { return dao.name; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    dao.name = Donor.MakeDonorName(value);
                }
                else
                {
                    throw new ArgumentException("Attempted to create a Donor without a name.");
                }
            }
        }
        public Donor()
        {
            this.dao = new Donor_DAO();
        }
        public Donor(string _cd, string _nm)
            : this()
        {
            name = _nm;
            code = _cd;
        }

        public Donor(Donor_DAO dao)
        {
            this.dao = dao;
        }

        /// <summary>
        /// Constructor given only a donor name.
        /// Use the name to construct a code (done
        /// in the code property setter).
        /// </summary>
        /// <param name="_nm"></param>
        public Donor(string _nm)
            : this(_nm, _nm)
        {
        }
}
    public class GiftLabelInfo
    {
        public GiftLabelInfo_DAO dao { get; set; }
        //public GiftLabelInfo_DAO dao { get { return _dao; } }
        private static string[] GENDER_VALUES = new string[] { "F", "M", "NotSpecified" };
        private int _year;
        private string _child_gender;
        private int _child_age;
        public int year
        {
            get { return dao.year; }
            set
            {
                if (value < 2000) throw new ArgumentException("gift_label_info year must be >= 2000.");
                dao.year = value;
            }
        }
        public string family_id { get { return dao.family_id; } set { dao.family_id = value; } }
        public string family_name { get { return dao.family_name; } set { dao.family_name = value; } }
        public string child_name { get { return dao.child_name; } set { dao.child_name = value; } }
        public string child_gender
        {
            get { return dao.child_gender; }
            set
            {
                if (!GENDER_VALUES.Contains(value))
                    throw new ArgumentException("gift_label_info child gender must be F, M, or NotSpecified");
                dao.child_gender = value;
            }
        }
        public int child_age
        {
            get { return dao.child_age; }
            set
            {
                if (value < -1)
                    throw new ArgumentException("gift_label_info child age must >= -1");
                dao.child_age = value;
            }
        }
        public string request_type { get { return dao.request_type; } set { dao.request_type = value; } }
        public string request_detail { get { return dao.request_detail; } set { dao.request_detail = value; } }
        public string donor_code { get { return dao.donor_code; } set { dao.donor_code = value; } }
        public string donor_name { get { return dao.donor_name; } set { dao.donor_name = value; } }

        public GiftLabelInfo()
        {
            this.dao = new GiftLabelInfo_DAO();
        }
        public GiftLabelInfo(GiftLabelInfo_DAO dao)
        {
            this.dao = dao;
        }
    }

    public class ServicesHouseholdEnrollment
    {
        public ServicesHouseholdEnrollment_DAO dao { get; set; }
        //public ServicesHouseholdEnrollment_DAO dao { get { return _dao; } }
        public string service_type { get { return dao.service_type; } set { dao.service_type = value; } }
        public int year
        {
            get { return dao.year; }
            set
            {
                if (value < 2000) throw new ArgumentException("services_household_enrollment year must be >= 2000.");
                dao.year = value;
            }
        }
        public int month
        {
            get { return dao.month; }
            set
            {
                if (value < 1 || value > 12) throw new ArgumentException("services_household_enrollment month must be between 1 and 12.");
                dao.month = value;
            }
        }
        public int day
        {
            get { return dao.day; }
            set
            {
                if (value < 1 || value > 31) throw new ArgumentException("services_household_enrollment day must be between 1 and 31.");
                dao.day = value;
            }
        }
        public string head_of_household { get { return dao.head_of_household; } set { dao.head_of_household = value; } }
        public string family_id { get { return dao.family_id; } set { dao.family_id = value; } }
        public string phone { get { return dao.phone; } set { dao.phone = value; } }
        public string address { get { return dao.address; } set { dao.address = value; } }
        public string city { get { return dao.city; } set { dao.city = value; } }
        public string state_or_province { get { return dao.state_or_province; } set { dao.state_or_province = value; } }
        public string postal_code { get { return dao.postal_code; } set { dao.postal_code = value; } }

        public ServicesHouseholdEnrollment()
        {
            this.dao = new ServicesHouseholdEnrollment_DAO();
        }
        public ServicesHouseholdEnrollment(ServicesHouseholdEnrollment_DAO dao)
        {
            this.dao = dao;
        }
    }

}

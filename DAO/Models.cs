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
        public int Id { get; set; }
        private int _year;
        public string family_id { get; set; }
        public string family_name { get; set; }
        public string family_members { get; set; }
        public string request_type { get; set; }
        public string donor_code { get; set;  }
        public string donor_name { get; set; }
        public int year
        {
            get { return _year; }
            set
            {
                if (value < 2000) throw new ArgumentException("bag_label_info year must be >= 2000.");
                _year = value;
            }
        }
        public BagLabelInfo()
        {
            this.year = DateTime.Now.Year;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public BagLabelInfo(BagLabelInfo other)
        {
            this.donor_code = other.donor_code;
            this.donor_name = other.donor_name;
            this.family_id = other.family_id;
            this.family_members = other.family_members;
            this.family_name = other.family_name;
            this.request_type = other.request_type;
            this.year = other.year;
        }
    }

    public class Donor
    {
        public int Id { get; set; }
        private string _code;
        private string _name;
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
            get { return _code; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    _code = Donor.MakeDonorCode(value);
                else // generate from name
                    _code = MakeDonorCode(name);
            }
        }
        public string name
        {
            get { return _name; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    _name = Donor.MakeDonorName(value);
                }
                else
                {
                    throw new ArgumentException("Attempted to create a Donor without a name.");
                }
            }
        }
        public Donor()
        {
        }

        /// <summary>
        /// Construct a Donor from a code and a name
        /// </summary>
        /// <param name="_cd"></param>
        /// <param name="_nm"></param>
        public Donor(string _cd, string _nm)
            : this()
        {
            name = _nm;
            code = _cd;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public Donor(Donor other)
            : this(other.code , other.name )
        {
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
        public int Id { get; set; }
        private int _year;
        private string _child_gender;
        private int _child_age;
        private static string[] GENDER_VALUES = new string[] { "F", "M", "NotSpecified" };
        public int year
        {
            get { return _year; }
            set
            {
                if (value < 2000)
                    throw new ArgumentException("gift_label_info year must be >= 2000.");
                else
                    _year = value;
            }
        }
        public string family_id { get; set; }
        public string family_name { get; set; }
        public string child_name { get; set; }
        public string child_gender
        {
            get { return _child_gender; }
            set
            {
                if (!GENDER_VALUES.Contains(value))
                    throw new ArgumentException("gift_label_info child gender must be F, M, or NotSpecified");
                else
                    _child_gender = value;
            }
        }
        public int child_age
        {
            get { return _child_age; }
            set
            {
                if (value < -1)
                    throw new ArgumentException("gift_label_info child age must >= -1");
                else
                    _child_age = value;
            }
        }
        public string request_type { get; set; }
        public string request_detail { get; set; }
        public string donor_code { get; set; }
        public string donor_name { get; set; }

        public GiftLabelInfo()
        {
            this.year = DateTime.Now.Year;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public GiftLabelInfo(GiftLabelInfo other)
        {
            this.child_age = other.child_age;
            this.child_gender = other.child_gender;
            this.child_name = other.child_name;
            this.donor_code = other.donor_code;
            this.donor_name = other.donor_name;
            this.family_id = other.family_id;
            this.family_name = other.family_name;
            this.request_detail = this.request_detail;
            this.request_type = other.request_type;
            this.year = other.year;
        }
    }

    public class ServicesHouseholdEnrollment
    {
        public int Id { get; set; }
        private int _year;
        private int _month;
        private int _day;
        public string service_type { get; set; }
        public int year
        {
            get { return _year; }
            set
            {
                if (value < 2000)
                    throw new ArgumentException("services_household_enrollment year must be >= 2000.");
                else
                    _year = value;
            }
        }
        public int month
        {
            get { return _month; }
            set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentException("services_household_enrollment month must be between 1 and 12.");
                else
                    _month = value;
            }
        }
        public int day
        {
            get { return _day; }
            set
            {
                if (value < 1 || value > 31)
                    throw new ArgumentException("services_household_enrollment day must be between 1 and 31.");
                else
                    _day = value;
            }
        }
        public string head_of_household { get; set; }
        public string family_id { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state_or_province { get; set; }
        public string postal_code { get; set; }

        public ServicesHouseholdEnrollment()
        {
            this.year = DateTime.Now.Year;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param></param>
        public ServicesHouseholdEnrollment(ServicesHouseholdEnrollment other)
        {
            this.address = other.address;
            this.city = other.city;
            this.day = other.day;
            this.family_id = other.family_id;
            this.head_of_household = other.head_of_household;
            this.month = other.month;
            this.phone = other.phone;
            this.postal_code = other.postal_code;
            this.service_type = other.service_type;
            this.state_or_province = other.state_or_province;
            this.year = other.year;
        }
    }

}

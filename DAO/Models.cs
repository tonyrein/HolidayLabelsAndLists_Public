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
        private int _year;
        public int year
        {
            get { return _year; }
            set
            {
                if (value < 2000) throw new ArgumentException("bag_label_info year must be >= 2000.");
                _year = value;
            }
        }
        public string family_id { get; set; }
        public string family_name { get; set; }
        public string family_members { get; set; }
        public string request_type { get; set; }
        public string donor_code { get; set; }
        public string donor_name { get; set; }
    }
    public class Donor
    {
        public static string MakeDonorName(string s)
        {
            string scr = Utils.TextUtils.CleanString(s);
            return scr.Replace('_', ' ');
        }
        public static string MakeDonorCode(string s)
        {
            return Utils.TextUtils.CleanString(s).ToUpper();
        }

        private string _code;
        private string _name;
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
                if (! String.IsNullOrWhiteSpace(value))
                {
                    _name = Donor.MakeDonorName(value);
                }
                else
                {
                    throw new ArgumentException("Attempted to create a Donor without a name.");
                }
            }
        }
        public Donor() { }
        public Donor(string _cd, string _nm)
        {
            name = _nm;
            code = _cd;
        }
    }
    public class GiftLabelInfo
    {
        private static string[] GENDER_VALUES = new string[] { "F", "M", "NotSpecified" };
        private int _year;
        private string _child_gender;
        private int _child_age;
        public int year
        {
            get { return _year; }
            set
            {
                if (value < 2000) throw new ArgumentException("gift_label_info year must be >= 2000.");
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
                _child_age = value;
            }
        }
        public string request_type { get; set; }
        public string request_detail { get; set; }
        public string donor_code { get; set; }
        public string donor_name { get; set; }
    }
    public class ServicesHouseholdEnrollment
    {
        private int _year;
        private int _month;
        private int _day;
        public string service_type { get; set; }
        public int year
        {
            get { return _year; }
            set
            {
                if (value < 2000) throw new ArgumentException("services_household_enrollment year must be >= 2000.");
                _year = value;
            }
        }
        public int month
        {
            get { return _month; }
            set
            {
                if (value < 1 || value > 12) throw new ArgumentException("services_household_enrollment month must be between 1 and 12.");
                _month = value;
            }
        }
        public int day
        {
            get { return _day; }
            set
            {
                if (value < 1 || value > 31) throw new ArgumentException("services_household_enrollment day must be between 1 and 31.");
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
        public int gift_card_count { get; set; }
    }

}

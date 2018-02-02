using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public enum Gender { F, M, NotSpecified };
    public enum RequestType {  Toys, Clothing, Other };


    public class DTO
    {
        protected long? id_from_string(string s)
        {
            long? retLong = null;
            long j;
            bool res = long.TryParse(s, out j);
            if (res) { retLong = j; }
            return retLong;
        }
    }

    public class Donor : DTO
    {
        public long? Id { get; set;  }
        public string Name { get; set; }
        public string Code { get; set; }

        // Parameterless constructor to allow for
        // xml serialization:
        public Donor()
        {
        }
        // Constructor. Use Name for Code if Code not given.
        public Donor(long? id, string name, string code = "")
        {
            this.Id = id;
            this.Name = name;
            if (String.IsNullOrWhiteSpace(code))
            {
                this.Code = name;
            }
            else
            {
                this.Code = code;
            }
        }
        // Copy constructor:n-- will we need this?
        public Donor(Donor otherDonor)
            : this(otherDonor.Id, otherDonor.Name, otherDonor.Code)
        {
        }
        public override string ToString()
        {
            return String.Format("Donor ID: {0} Name: {1}, Code: {2}", this.Id, this.Name, this.Code);
        }

    } // end class Donor

    public class Child : DTO
    {
        public long? Id { get; set;  }
        public string FirstName { get; set;  }
        public string VESTA_Id { get; set;  }
        public Gender Gender { get; set;  }
        // Parameterless constructor to allow for
        // xml serialization:
        private Child()
        {
        }
        public Child(long? id, string first_name, string vesta_id, Gender _gender)
        {
            this.Id = id;
            this.FirstName = first_name;
            this.VESTA_Id = vesta_id;
            this.Gender = _gender;
        }
        // Construct from array of string. Assume
        // strings are in the order id, first name, vesta id, gender
        public Child(string[] fields)
        {
            this.Id = this.id_from_string(fields[0]);
            this.FirstName = fields[1];
            this.VESTA_Id = fields[2];
            this.Gender = (Gender)Enum.Parse(typeof(Gender), fields[3]);
        }
        // Copy constructor-- will we need this?
        public Child(Child otherChild)
            : this(otherChild.Id, otherChild.FirstName, otherChild.VESTA_Id, otherChild.Gender)
        {
        }
        public override string ToString()
        {
            return String.Format("Child ID: {0} First Name: {1}, Gender: {2}, VESTA_ID {3}",
                this.Id, this.FirstName, this.Gender, this.VESTA_Id);
        }
    }
    public class HeadOfHousehold : DTO
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string VESTA_Id { get; set; }
        // Parameterless constructor to allow for
        // xml serialization:
        public HeadOfHousehold()
        {
        }
        public HeadOfHousehold(long? id, string name, string phone,
            string street, string city, string state, string zip, string vesta_id)
        {
            this.Id = id;
            this.Name = name;
            this.Phone = phone;
            this.Street = street;
            this.City = city;
            this.State = state;
            this.Zip = zip;
            this.VESTA_Id = vesta_id;
        }
        public override string ToString()
        {
            return String.Format("Head of Household ID: {0} Name: {1}, Phone {2}, VESTA_ID {3}",
                this.Id, this.Name, this.Phone, this.VESTA_Id);
        }
    }
    public class ServicesEnrollment : DTO
    {
        public long? Id { get; set; }
        public string Service { get; set; } // "Adopt a family," "Holiday food basket," etc.
        public DateTime DateEnrolled { get; set; }
        public int ChildsAge { get; set; }
        public Child Child { get; set; }
        public HeadOfHousehold HeadOfHousehold { get; set; }
        // Parameterless constructor to allow for
        // xml serialization:
        public ServicesEnrollment()
        {
        }
    }
    public class LabelEvent_Bag : DTO
    {
        public long? Id { get; set;  }
        public RequestType RequestType { get; set; }
        public int Year { get; set; }
        public string ChildrensNames { get; set; }
        public Donor Donor { get; set; }
        public HeadOfHousehold HeadOfHousehold { get; set; }
        // Parameterless constructor to allow for
        // xml serialization:
        public LabelEvent_Bag()
        {
        }
    }
    public class LabelEvent_Gift : DTO
    {
        public long? Id { get; set; }
        public RequestType RequestType { get; set; }
        public int Year { get; set; }
        public string RequestDetail { get; set; }
        public int ChildsAge { get; set; }
        public Donor Donor { get; set; }
        public HeadOfHousehold HeadOfHousehold { get; set; }
        public Child Child { get; set;  }
        // Parameterless constructor to allow for
        // xml serialization:
        public LabelEvent_Gift()
        {
        }
    }

}

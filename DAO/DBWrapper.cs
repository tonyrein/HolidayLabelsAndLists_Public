using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace DAO
{
    /// <summary>
    /// Wrapper around data structures used for temporary
    /// (in-RAM) storage.
    /// 
    /// A DBWrapper object contains Lists of instances of the classes
    /// defined in Models.cs:
    ///  - BagLabelInfo objects
    ///  - Donor objects
    ///  - GiftLabelInfo objects
    ///  - ServicesHouseholdEnrollment objects
    /// </summary>
    public class DBWrapper
    {
        public List<BagLabelInfo> BliList { get; set; }
        public List<Donor> DonorList { get; set; }
        public List<GiftLabelInfo> GliList { get; set; }
        public List<ServicesHouseholdEnrollment> HoEnrList { get; set; }

        /// <summary>
        /// Initialize this DBWrapper's data lists to empty lists.
        /// Then add a default set of donors from the values supplied by
        /// the client.
        /// </summary>
        public DBWrapper()
        {
            BliList = new List<BagLabelInfo>();
            DonorList = new List<Donor>();
            GliList = new List<GiftLabelInfo>();
            HoEnrList = new List<ServicesHouseholdEnrollment>();
            foreach(string[] sa in initial_donors)
            {
                DonorList.Add(new Donor(sa[0], sa[1]));
            }

        }

        /// <summary>
        /// Load data from persistent store.
        /// </summary>
        /// <returns></returns>
        public int Load()
        {
            int retInt = 0;
            return retInt;
        }

        /// <summary>
        /// Save data to persistent store
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            int retInt = 0;
            return retInt;
        }
        /// <summary>
        /// Remove old data in preparation for a new
        /// processing run. Save any new Donors for
        /// the life of the main form.
        /// </summary>
        public void Clean()
        {
            BliList.Clear();
            GliList.Clear();
            HoEnrList.Clear();
        }

        /// <summary>
        /// Is there already an object in the list that matches the
        /// passed-in one?
        /// 
        /// Two BagLabelInfo objects match if they have the same year, request type,
        /// and family id.
        /// 
        /// </summary>
        /// <param name="bli"></param>
        /// <returns></returns>
        public BagLabelInfo MatchingBagLabelInfo(BagLabelInfo bli)
        {
            return BliList.FirstOrDefault
                (b => (
                    b.year == bli.year &&
                    b.request_type == bli.request_type &&
                    b.family_id == bli.family_id
                    )
                );
        }

        /// <summary>
        /// Is there a Donor with the given name?
        /// 
        /// NOTE: Match is NOT case-sensetive -- "SMITH" matches "smith"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Donor DonorForDonorName(string name)
        {
            string s = Donor.MakeDonorName(name);
            return DonorList.FirstOrDefault
                (d => d.name.ToUpper() == s.ToUpper());
        }

      
        /// <summary>
        /// Is there already an object in the list that matches
        /// the passed-in one?
        /// 
        /// Two Donor objects match if their name properties
        /// are the same.
        /// 
        /// NOTE: There is no need to consider case here, since
        /// the setter for the Donor.name property "canonicalizes"
        /// the name.
        /// 
        /// </summary>
        /// <param name="other_donor"></param>
        /// <returns></returns>
        public Donor DonorForDonorName(Donor other_donor)
        {

            return DonorList.FirstOrDefault
                (d => d.name == other_donor.name);
        }

        /// <summary>
        /// Find a donor with the given code.
        /// </summary>
        /// <param name="_cd"></param>
        /// <returns>
        /// matching Donor object, or null if no match
        /// 
        /// TODO: Figure out if this needs a check for upper/lower case
        /// 
        /// </returns>
        public Donor DonorForDonorCode(string _cd)
        {
            return DonorList.FirstOrDefault
                (d => d.code == _cd);
        }

        /// <summary>
        /// Is there already an object in the list that matches
        /// the passed-in one?
        /// 
        /// Two GiftLabelInfo objects match if their years,
        /// request types, family ids and child names are the same.
        /// 
        /// </summary>
        /// <param name="gli"></param>
        /// <returns></returns>
        public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo gli)
        {
            return GliList.FirstOrDefault
                (g => (
                    g.year == gli.year &&
                    g.request_type == gli.request_type &&
                    g.family_id == gli.family_id &&
                    g.child_name == gli.child_name
                    )
                );
        }

        /// <summary>
        /// Is there already an object in the list that matches
        /// the passed-in one?
        /// 
        /// Two ServicesHouseholdEnrollment objects match if their
        /// years, service types, and heads of household are the same.
        /// </summary>
        /// <param name="henroll"></param>
        /// <returns></returns>
        public ServicesHouseholdEnrollment MatchingServicesHouseholdEnrollment(ServicesHouseholdEnrollment henroll)
        {
            return HoEnrList.FirstOrDefault
                (h =>(
                    h.year == henroll.year &&
                    h.service_type == henroll.service_type &&
                    h.head_of_household == henroll.head_of_household
                    )
                );
        }
        
        /// <summary>
        /// Check to see if matching object already exists. If so, replace it
        /// with the new one. If not, add the new one to the end of the list.
        /// </summary>
        /// <param name="newval"></param>
        public void AddOrUpdateBli(BagLabelInfo newval)
        {
            BagLabelInfo bli = MatchingBagLabelInfo(newval);
            if (bli != null)
            {
                int idx = BliList.IndexOf(bli);
                BliList[idx] = newval;
            }
            else
            {
                BliList.Add(newval);
            }
        }

        /// <summary>
        /// Check to see if matching object already exists. If so, 
        /// use the existing donor code. If not, add the new donor
        /// to the end of the list.
        /// 
        /// Return a Donor with the appropriate name and code.
        /// </summary>
        /// <param name="newval"></param>
        public Donor FindOrAddDonor(string newval)
        {
            Donor retDonor;
            retDonor = DonorForDonorName(newval);
            if (retDonor == null)
            {
                // Make a new Donor with the given name
                // and code generated from that name.
                //
                // NOTE: the Donor name and code property setters
                // may modify the given parameters. See Models.cs
                retDonor = new Donor();
                retDonor.name = newval;
                retDonor.code = newval;
                this.DonorList.Add(retDonor);
            }
            return retDonor;
        }

        /// <summary>
        /// Check to see if matching object already exists. If so, replace it
        /// with the new one. If not, add the new one to the end of the list.
        /// </summary>
        /// <param name="newval"></param>
        public void AddOrUpdateGli(GiftLabelInfo newval)
        {
            GiftLabelInfo gli = MatchingGiftLabelInfo(newval);
            if (gli != null)
            {
                int idx = GliList.IndexOf(gli);
                GliList[idx] = newval;
            }
            else
            {
                GliList.Add(newval);
            }
        }

        /// <summary>
        /// Check to see if matching object already exists. If so, replace it
        /// with the new one. If not, add the new one to the end of the list.
        /// </summary>
        /// <param name="newval"></param>
        public void AddOrUpdateHoEnr(ServicesHouseholdEnrollment newval)
        {
            ServicesHouseholdEnrollment henr = MatchingServicesHouseholdEnrollment(newval);
            if (henr != null)
            {
                int idx = HoEnrList.IndexOf(henr);
                HoEnrList[idx] = newval;
            }
            else
            {
                HoEnrList.Add(newval);
            }
        }

        /// <summary>
        /// Initial list of donors
        /// 
        /// When code in VestaImporter reads a record containing a donor
        /// organization name from a VESTA report, it assigns donor
        /// codes as follows:
        /// 
        /// 1. If the name in the record read by the importer exists in the
        /// array below, the corresponding code is used. Otherwise:
        /// 
        /// 2. A code is generated from the name using DAO.Donor.MakeDonorCode()
        /// </summary>
        private static string[][] initial_donors = new string[][]
        {
            new string[] {"EXCode1", "Example Donor Organization 1"},
            new string[] {"EXCode2", "Example Donor Organization 2"}
        };
    }

}

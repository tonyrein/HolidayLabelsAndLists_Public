using System.Collections.Generic;
using System.IO;
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
            this.Load();
            int default_donors_added = this.AddDefaultDonors();
            if (default_donors_added > 0) // list changed -- save changes
            {
                using (var db = this.GetDatabase())
                    this.SaveDonors(db);
            }
        }

        /// <summary>
        /// Read the list of initial (default) donors.
        /// Add any that are not already on our list.
        /// Return the number added.
        /// </summary>
        /// <returns></returns>
        private int AddDefaultDonors()
        {
            int retInt = 0;
            foreach (string[] sa in initial_donors)
            {
                string cd = sa[0]; string nm = sa[1];
                Donor d = new Donor(sa[0], sa[1]);
                if (MatchingDonor(d) == null)
                {
                    this.DonorList.Add(d);
                    retInt++;
                }
            }
            return retInt;
        }

        private LiteDatabase GetDatabase()
        {
            string dbpath = Path.Combine(FolderManager.OutputFolder, Properties.Resources.db_filename);
            return new LiteDatabase(dbpath);
        }
        /// <summary>
        /// Save data to persistent store.
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            using (LiteDatabase db = this.GetDatabase())
            {
                this.SaveDonors(db);
                this.SaveBagLabelInfo(db);
                this.SaveGiftabelInfo(db);
                this.SaveServicesHouseholdEnrollment(db);
                //LiteCollection<Donor_DAO> donor_coll = db.GetCollection<Donor_DAO>("donors");
                //foreach(Donor d in this.DonorList)
                //{
                //    donor_coll.Upsert(d.dao);
                //}

            }
        }

        /// <summary>
        /// Load data from persistent store
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            using (LiteDatabase db = this.GetDatabase())
            {
                this.LoadDonors(db);
                this.LoadBagLabelInfo(db);
                this.LoadGiftLabelInfo(db);
                this.LoadServicesHouseholdEnrollment(db);
                //LiteCollection<Donor_DAO> donor_coll = db.GetCollection<Donor_DAO>("donors");
                //foreach(Donor_DAO dao in donor_coll.Find(Query.All()))
                //{
                //    if (MatchingDonor(dao) == null) // This Donor is not already in list.
                //    {
                //        this.DonorList.Add(new Donor(dao));
                //    }
                //}
            }
        }

        /// <summary>
        /// If performance is an issue:
        /// Idea 1:
        ///     Query db first to find items in DonorList which are not
        ///     already in the collection. Then use InsertBulk()
        ///     to insert others.
        /// </summary>
        /// <param name="db"></param>
        public void SaveDonors(LiteDatabase db)
        {
            LiteCollection<Donor_DAO> coll = db.GetCollection<Donor_DAO>("donors");
            foreach (Donor d in this.DonorList)
            {
                coll.Upsert(d.dao);
            }
        }

        public void SaveBagLabelInfo(LiteDatabase db)
        {
            LiteCollection<BagLabelInfo_DAO> coll = db.GetCollection<BagLabelInfo_DAO>("bag_label_info");
            foreach (BagLabelInfo bli in this.BliList)
            {
                coll.Upsert(bli.dao);
            }

        }

        public void SaveGiftabelInfo(LiteDatabase db)
        {
            LiteCollection<GiftLabelInfo_DAO> coll = db.GetCollection<GiftLabelInfo_DAO>("gift_label_info");
            foreach (GiftLabelInfo gli in this.GliList)
            {
                coll.Upsert(gli.dao);
            }
        }


        public void SaveServicesHouseholdEnrollment(LiteDatabase db)
        {
            LiteCollection<ServicesHouseholdEnrollment_DAO> coll = db.GetCollection<ServicesHouseholdEnrollment_DAO>("services_enrollment");
            foreach (ServicesHouseholdEnrollment henroll in this.HoEnrList)
            {
                coll.Upsert(henroll.dao);
            }
        }

        public void LoadDonors(LiteDatabase db)
        {
            LiteCollection<Donor_DAO> coll = db.GetCollection<Donor_DAO>("donors");
            foreach (Donor_DAO dao in coll.Find(Query.All()))
            {
                if (MatchingDonor(dao) == null) // This Donor is not already in list.
                {
                    this.DonorList.Add(new Donor(dao));
                }
            }
        }

        public void LoadBagLabelInfo(LiteDatabase db)
        {
            LiteCollection<BagLabelInfo_DAO> coll = db.GetCollection<BagLabelInfo_DAO>("bag_label_info");
            foreach(BagLabelInfo_DAO dao in coll.Find(Query.All()))
            {
                if (MatchingBagLabelInfo(dao) == null)
                {
                    this.BliList.Add(new BagLabelInfo(dao));
                }
            }
        }

        public void LoadGiftLabelInfo(LiteDatabase db)
        {
            LiteCollection<GiftLabelInfo_DAO> coll = db.GetCollection<GiftLabelInfo_DAO>("gift_label_info");
            foreach (GiftLabelInfo_DAO dao in coll.Find(Query.All()))
            {
                if (MatchingGiftLabelInfo(dao) == null)
                {
                    this.GliList.Add(new GiftLabelInfo(dao));
                }
            }
        }

        public void LoadServicesHouseholdEnrollment(LiteDatabase db)
        {
            LiteCollection<ServicesHouseholdEnrollment_DAO> coll = db.GetCollection<ServicesHouseholdEnrollment_DAO>("services_enrollment");
            foreach (ServicesHouseholdEnrollment_DAO dao in coll.Find(Query.All()))
            {
                if (MatchingServicesHouseholdEnrollment(dao) == null)
                {
                    this.HoEnrList.Add(new ServicesHouseholdEnrollment(dao));
                }
            }
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
            return MatchingBagLabelInfo(bli.dao);
            //return BliList.FirstOrDefault
            //    (b => (
            //        b.year == bli.year &&
            //        b.request_type == bli.request_type &&
            //        b.family_id == bli.family_id
            //        )
            //    );
        }

        public BagLabelInfo MatchingBagLabelInfo(BagLabelInfo_DAO dao)
        {
            return BliList.FirstOrDefault
                           (b => (
                               b.year == dao.year &&
                               b.request_type == dao.request_type &&
                               b.family_id == dao.family_id
                               )
                           );
        }
        /// <summary>
        /// Is there a Donor in the list with the given name?
        /// 
        /// NOTE: There is no need to consider case here, since
        /// the setter for the Donor.name property "canonicalizes"
        /// the name.
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Donor DonorForDonorName(string name)
        {
            string s = Donor.MakeDonorName(name);
            return DonorList.FirstOrDefault
                (d => d.name == s);
                //(d => d.name.ToUpper() == s.ToUpper());
        }

      
        /// <summary>
        /// Is there a Donor in the list with the same
        /// name as the passed-in one?
        /// 
        /// Since we know that the other donor's name is
        /// already of proper format, we don't have to
        /// call MakeDonorName() as we do if we're just
        /// checking against a string.
        /// </summary>
        /// <param name="other_donor"></param>
        /// <returns></returns>
        public Donor DonorForDonorName(Donor other_donor)
        {
            return this.DonorList.FirstOrDefault(d => d.name == other_donor.name);
        }

        /// <summary>
        /// Since we know that a Donor_DAO's name is
        /// already of proper format, we don't have to
        /// call MakeDonorName() as we do if we're just
        /// checking against a string.
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        public Donor DonorForDonorName(Donor_DAO dao)
        {
            return this.DonorList.FirstOrDefault(d => d.name == dao.name);
        }

        /// <summary>
        /// Find a donor with the given code.
        /// 
        /// Donor codes are all upper-case, so there is no
        /// need to check for case here.
        /// </summary>
        /// <param name="_cd"></param>
        /// <returns>
        /// matching Donor object, or null if no match
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
        /// Two Donor objects match if their names are the same.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public Donor MatchingDonor(Donor d)
        {
            return MatchingDonor(d.dao);
        }

        public Donor MatchingDonor(Donor_DAO dao)
        {
            return DonorForDonorName(dao);
        }


        public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo gli)
        {
            return MatchingGiftLabelInfo(gli.dao);
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
        public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo_DAO dao)
        {
            return GliList.FirstOrDefault
                (g => (
                    g.year == dao.year &&
                    g.request_type == dao.request_type &&
                    g.family_id == dao.family_id &&
                    g.child_name == dao.child_name
                    )
                );
        }


        public ServicesHouseholdEnrollment MatchingServicesHouseholdEnrollment(ServicesHouseholdEnrollment henroll)
        {
            return MatchingServicesHouseholdEnrollment(henroll.dao);
        }

        /// <summary>
        /// Is there already an object in the list that matches
        /// the passed-in one?
        /// 
        /// Two ServicesHouseholdEnrollment objects match if their
        /// years, service types, and heads of household are the same.
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        public ServicesHouseholdEnrollment MatchingServicesHouseholdEnrollment(ServicesHouseholdEnrollment_DAO dao)
        {
            return HoEnrList.FirstOrDefault
                (h =>(
                    h.year == dao.year &&
                    h.service_type == dao.service_type &&
                    h.head_of_household == dao.head_of_household
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
        /// Check to see if matching object already exists. If so, replace it
        /// with the new one. If not, add the new one to the end of the list.
        /// </summary>
        /// <param name="newval"></param>
        public void AddOrUpdateDonor(Donor newval)
        {
            Donor d = MatchingDonor(newval);
            if (d != null)
            {
                int idx = DonorList.IndexOf(d);
                DonorList[idx] = newval;
            }
            else
            {
                DonorList.Add(newval);
            }
        }

        /// <summary>
        /// Check to see if matching object already exists. If not,
        /// create a new one.
        /// 
        /// A Donor object matches if its name == the value
        /// passed in.
        /// 
        /// Return either the already-existing Donor or the
        /// newly-created one.
        /// 
        /// </summary>
        /// <param name="newval"></param>
        public Donor FindOrCreateDonor(string newval)
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

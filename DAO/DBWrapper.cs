using System.Collections.Generic;
using System.IO;
using System.Linq;

using LiteDB;

namespace DAO
{
    public class DBWrapper
    {
        private LiteDatabase _db;
        private LiteCollection<Donor> _donorcoll;
        private LiteCollection<BagLabelInfo> _blicoll;
        private LiteCollection<GiftLabelInfo> _glicoll;
        private LiteCollection<ServicesHouseholdEnrollment> _henrcoll;
        private void SetUpCollections()
        {
            this._donorcoll = this._db.GetCollection<Donor>("donors");
            this._blicoll = this._db.GetCollection<BagLabelInfo>("blis");
            this._glicoll = this._db.GetCollection<GiftLabelInfo>("glis");
            this._henrcoll = this._db.GetCollection<ServicesHouseholdEnrollment>("enrollments");
            this._donorcoll.EnsureIndex(d => d.name);
            this._blicoll.EnsureIndex(b => b.family_id);
            this._glicoll.EnsureIndex(g => g.family_id);
            this._henrcoll.EnsureIndex(h => h.family_id);
        }
        public DBWrapper()
        {
            this._db = DbUtils.GetDatabase();
            this.SetUpCollections();
            this.AddDefaultDonors();
        }
        public List<BagLabelInfo> BliList { get; set; }
        public List<Donor> DonorList { get; set; }
        public List<GiftLabelInfo> GliList { get; set; }
        public List<ServicesHouseholdEnrollment> HoEnrList { get; set; }
        public bool IsEmpty
        {
            get
            {
                return (this._blicoll.Count() == 0) &&
                    (this._glicoll.Count() == 0) &&
                    (this._henrcoll.Count() == 0);
            }
        }

        // DONOR METHODS
        /////////////////
        public Donor FindDonorByName(string name)
        {
            return this._donorcoll.Find(d => d.name == name).FirstOrDefault();
        }

        public Donor FindDonorByCode(string code)
        {
            return this._donorcoll.Find(d => d.code  == code).FirstOrDefault();
        }

        public Donor MatchingDonor(Donor other)
        {
            return FindDonorByName(other.name);
        }

        public Donor AddOrUpdateDonor(Donor d)
        {
            Donor dbd = this.MatchingDonor(d);
            if (dbd == null)
            {
                // not found -- insert the one passed
                // to us. As a side effect, the Insert()
                // method updates the Id property of the
                // Donor.
                this._donorcoll.Insert(d);
            }
            else
            {
                // found -- set the id of the passed-in Donor to
                // that of the found Donor.
                d.Id = dbd.Id;
                // Now replace the Donor in the database with
                // the one passed in.
                this._donorcoll.Update(d);
            }
            return dbd;
        }

        /// <summary>
        /// Read the list of initial (default) donors.
        /// Add any that are not already on our list.
        /// </summary>
        /// <returns></returns>
        private void AddDefaultDonors()
        {
            foreach (Donor d in InitialDonors.InitialList ())
            {
                this.AddOrUpdateDonor(d);
            }
        }

        // BagLabelInfo methods:
        /////////////////////////////
        
        /// <summary>
        /// Is there already an object in the list that matches the
        /// passed-in one?
        /// 
        /// Two BagLabelInfo objects match if they have the same year, request type,
        /// and family id.
        /// </summary>
        public BagLabelInfo MatchingBagLabelInfo(BagLabelInfo other)
        {
            return this._blicoll.Find(b =>
                b.year == other.year &&
                b.request_type == other.request_type &&
                b.family_id == other.family_id
            ).FirstOrDefault();
        }

        /// <summary>
        /// Add passed-in object to database collection, if
        /// it's not already present. If it is already there,
        /// update the database element with the properties
        /// of the passed-inobject.
        /// </summary>
        /// <param name="bli"></param>
        /// <returns></returns>
        public BagLabelInfo  AddOrUpdateBLI(BagLabelInfo bli)
        {
            BagLabelInfo dbo = this.MatchingBagLabelInfo(bli);
            if (dbo == null)
            {
                // not found -- insert the one passed
                // to us. As a side effect, the Insert()
                // method updates the Id property of the
                // inserted object.
                this._blicoll.Insert(bli);
            }
            else
            {
                // found -- set the id of the passed-in object to
                // that of the found object.
                bli.Id = dbo.Id;
                // Now replace the Donor in the database with
                // the one passed in.
                this._blicoll.Update(bli);
            }
            return dbo;
        }

        // GiftLabelInfo methods
        /// <summary>
        /// ////////////////////

        public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo other)
        {
            return this._glicoll.Find(g =>
                g.year == other.year &&
                g.family_id == other.family_id &&
                g.child_name == other.child_name &&
                g.request_type == other.request_type
            ).FirstOrDefault();
        }

        /// <summary>
        /// Add passed-in object to database collection, if
        /// it's not already present. If it is already there,
        /// update the database element with the properties
        /// of the passed-inobject.
        /// </summary>
        /// <param name="bli"></param>
        /// <returns></returns>
        public GiftLabelInfo AddOrUpdateBLI(GiftLabelInfo gli)
        {
            GiftLabelInfo  dbo = this.MatchingGiftLabelInfo(gli);
            if (dbo == null)
            {
                // not found -- insert the one passed
                // to us. As a side effect, the Insert()
                // method updates the Id property of the
                // inserted object.
                this._glicoll.Insert(gli);
            }
            else
            {
                // found -- set the id of the passed-in object to
                // that of the found object.
                gli.Id = dbo.Id;
                // Now replace the Donor in the database with
                // the one passed in.
                this._glicoll.Update(gli);
            }
            return dbo;
        }



        /// <summary>
        ///// Is there already an object in the list that matches the
        ///// passed-in one?
        ///// 
        ///// Two BagLabelInfo objects match if they have the same year, request type,
        ///// and family id.
        ///// 
        ///// </summary>
        ///// <param name="bli"></param>
        ///// <returns></returns>
        //public BagLabelInfo MatchingBagLabelInfo(BagLabelInfo bli)
        //{
        //    return BliList.Where
        //                  (b => (
        //                      b.year == bli.year &&
        //                      b.request_type == bli.request_type &&
        //                      b.family_id == bli.family_id
        //                      )
        //                  ).FirstOrDefault();
        //}

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
        //public Donor DonorForDonorName(string name)
        //{
        //    string s = Donor.MakeDonorName(name);
        //    return DonorList.FirstOrDefault
        //        (d => d.name == s);
        //    //(d => d.name.ToUpper() == s.ToUpper());
        //}


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
        //public Donor DonorForDonorName(Donor other_donor)
        //{
        //    return this.DonorList.FirstOrDefault(d => d.name == other_donor.name);
        //}

        /// <summary>
        /// Since we know that a Donor's name is
        /// already of proper format, we don't have to
        /// call MakeDonorName() as we do if we're just
        /// checking against a string.
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        //public Donor DonorForDonorName(Donor dao)
        //{
        //    return this.DonorList.FirstOrDefault(d => d.name == dao.name);
        //}

        ///// <summary>
        ///// Find a donor with the given code.
        ///// 
        ///// Donor codes are all upper-case, so there is no
        ///// need to check for case here.
        ///// </summary>
        ///// <param name="_cd"></param>
        ///// <returns>
        ///// matching Donor object, or null if no match
        ///// </returns>
        //public Donor DonorForDonorCode(string _cd)
        //{
        //    return DonorList.FirstOrDefault
        //        (d => d.code == _cd);
        //}

        /// <summary>
        /// Is there already an object in the list that matches
        /// the passed-in one?
        /// 
        /// Two Donor objects match if their names are the same.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        //public Donor MatchingDonor(Donor otherdonor)
        //{
        //    return DonorList.Where(d => d.name == otherdonor.name).FirstOrDefault();
        //}





        //public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo gli)
        //{
        //    return GliList.Where(g =>
        //        (g.family_id == gli.family_id &&
        //        g.child_name == gli.child_name &&
        //        g.year == gli.year &&
        //        g.request_type == gli.request_type)
        //        ).FirstOrDefault();
        //}

        //public ServicesHouseholdEnrollment MatchingServicesHouseholdEnrollment(ServicesHouseholdEnrollment henroll)
        //{
        //    return HoEnrList.Where
        //        (h => (
        //        h.year == henroll.year &&
        //        h.service_type == henroll.service_type &&
        //        h.head_of_household == henroll.head_of_household
        //        )
        //    ).FirstOrDefault();
        //}

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
                // update record in list by using newvalue's dao
                // member, but retain previously-assigned id, if any:
                newval.dao.Id = bli.dao.Id;
                bli.dao = newval.dao;
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
s

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
                // update record in list by using newvalue's dao
                // member, but retain previously-assigned id, if any:
                newval.dao.Id = gli.dao.Id;
                gli.dao = newval.dao;
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
                // update record in list by using newvalue's dao
                // member, but retain previously-assigned id, if any:
                newval.dao.Id = henr.dao.Id;
                henr.dao = newval.dao;
            }
            else
            {
                HoEnrList.Add(newval);
            }
        }

    }

}

/// <summary>
/// Wrapper around data structures used for temporary
/// (in-RAM) storage and for saving to and loading from
/// persistent storage.
/// 
/// A DBWrapper object contains Lists of instances of the classes
/// defined in Models.cs:
///  - BagLabelInfo objects
///  - Donor objects
///  - GiftLabelInfo objects
///  - ServicesHouseholdEnrollment objects
/// </summary>
//    public class DBWrapper
//    {
//        public List<BagLabelInfo> BliList { get; set; }
//        public List<Donor> DonorList { get; set; }
//        public List<GiftLabelInfo> GliList { get; set; }
//        public List<ServicesHouseholdEnrollment> HoEnrList { get; set; }
//        public bool IsEmpty
//        {
//            get
//            {
//                return (this.BliList.Count() == 0) &&
//                    (this.GliList.Count() == 0) &&
//                    (this.HoEnrList.Count() == 0);
//            }
//        }

//        /// <summary>
//        /// Initialize this DBWrapper's data lists to empty lists.
//        /// Then add a default set of donors from the values supplied by
//        /// the client.
//        /// </summary>
//        public DBWrapper()
//        {
//            BliList = new List<BagLabelInfo>();
//            DonorList = new List<Donor>();
//            GliList = new List<GiftLabelInfo>();
//            HoEnrList = new List<ServicesHouseholdEnrollment>();
//        }

//        /// <summary>
//        /// Read the list of initial (default) donors.
//        /// Add any that are not already on our list.
//        /// Return the number added.
//        /// </summary>
//        /// <returns></returns>
//        private int AddDefaultDonors()
//        {
//            int retInt = 0;
//            foreach (Donor d in InitialDonors.InitialList())
//            {
//                if (MatchingDonor(d) == null) // not already present
//                {
//                    this.DonorList.Add(d);
//                    retInt++;
//                }
//            }
//            return retInt;
//        }

//        /// <summary>
//        /// Save data to persistent store.
//        /// </summary>
//        /// <returns></returns>
//        public void Save()
//        {
//            DbUtils.BackupDatabase();
//            using (LiteDatabase db = DbUtils.GetDatabase())
//            {
//                this.SaveDonors(db);
//                this.SaveBagLabelInfo(db);
//                this.SaveGiftabelInfo(db);
//                this.SaveServicesHouseholdEnrollment(db);
//            }
//        }

//        /// <summary>
//        /// Load data from persistent store
//        /// </summary>
//        /// <returns></returns>
//        public void Load()
//        {
//            using (LiteDatabase db = DbUtils.GetDatabase())
//            {
//                this.LoadDonors(db);
//                this.LoadBagLabelInfo(db);
//                this.LoadGiftLabelInfo(db);
//                this.LoadServicesHouseholdEnrollment(db);
//                int default_donors_added = this.AddDefaultDonors();
//                if (default_donors_added > 0) // list changed -- save changes
//                {
//                    this.SaveDonors(db);
//                }
//            }
//        }

//        /// <summary>
//        /// If performance is an issue:
//        /// Idea 1:
//        ///     Query db first to find items in DonorList which are not
//        ///     already in the collection. Then use InsertBulk()
//        ///     to insert others.
//        /// </summary>
//        /// <param name="db"></param>
//        public void SaveDonors(LiteDatabase db)
//        {
//            string collection_name = "donors";
//            LiteCollection<Donor> coll = db.GetCollection<Donor>(collection_name);
//            foreach (Donor d in this.DonorList)
//            {
//                coll.Upsert(d.dao);
//            }
//        }

//        public void SaveBagLabelInfo(LiteDatabase db)
//        {
//            string collection_name = "bag_label_info";
//            LiteCollection<BagLabelInfo> coll = db.GetCollection<BagLabelInfo>(collection_name);
//            foreach (BagLabelInfo bli in this.BliList)
//            {
//                coll.Upsert(bli.dao);
//            }

//        }

//        public void SaveGiftabelInfo(LiteDatabase db)
//        {
//            string collection_name = "gift_label_info";
//            LiteCollection<GiftLabelInfo> coll = db.GetCollection<GiftLabelInfo>(collection_name);
//            foreach (GiftLabelInfo gli in this.GliList)
//            {
//                coll.Upsert(gli.dao);
//            }
//        }


//        public void SaveServicesHouseholdEnrollment(LiteDatabase db)
//        {
//            string collection_name = "services_enrollment";
//            LiteCollection<ServicesHouseholdEnrollment> coll = db.GetCollection<ServicesHouseholdEnrollment>(collection_name);
//            foreach (ServicesHouseholdEnrollment henroll in this.HoEnrList)
//            {
//                coll.Upsert(henroll.dao);
//            }
//        }

//        public void LoadDonors(LiteDatabase db)
//        {
//            this.DonorList.Clear();
//            LiteCollection<Donor> coll = db.GetCollection<Donor>("donors");
//            foreach (Donor dao in coll.Find(Query.All()))
//            {
//                this.DonorList.Add(new Donor(dao));
//            }
//        }

//        public void LoadBagLabelInfo(LiteDatabase db)
//        {
//            this.BliList.Clear();
//            LiteCollection<BagLabelInfo> coll = db.GetCollection<BagLabelInfo>("bag_label_info");
//            foreach(BagLabelInfo dao in coll.Find(Query.All()))
//            {
//                this.BliList.Add(new BagLabelInfo(dao));
//            }
//        }

//        public void LoadGiftLabelInfo(LiteDatabase db)
//        {
//            this.GliList.Clear();
//            LiteCollection<GiftLabelInfo> coll = db.GetCollection<GiftLabelInfo>("gift_label_info");
//            foreach (GiftLabelInfo dao in coll.Find(Query.All()))
//            {
//                this.GliList.Add(new GiftLabelInfo(dao));
//            }
//        }

//        public void LoadServicesHouseholdEnrollment(LiteDatabase db)
//        {
//            this.HoEnrList.Clear();
//            LiteCollection<ServicesHouseholdEnrollment> coll = db.GetCollection<ServicesHouseholdEnrollment>("services_enrollment");
//            foreach (ServicesHouseholdEnrollment dao in coll.Find(Query.All()))
//            {
//                this.HoEnrList.Add(new ServicesHouseholdEnrollment(dao));
//            }
//        }

//        /// <summary>
//        /// Is there already an object in the list that matches the
//        /// passed-in one?
//        /// 
//        /// Two BagLabelInfo objects match if they have the same year, request type,
//        /// and family id.
//        /// 
//        /// </summary>
//        /// <param name="bli"></param>
//        /// <returns></returns>
//        public BagLabelInfo MatchingBagLabelInfo(BagLabelInfo bli)
//        {
//            return BliList.Where
//                          (b => (
//                              b.year == bli.year &&
//                              b.request_type == bli.request_type &&
//                              b.family_id == bli.family_id
//                              )
//                          ).FirstOrDefault();
//        }

//        /// <summary>
//        /// Is there a Donor in the list with the given name?
//        /// 
//        /// NOTE: There is no need to consider case here, since
//        /// the setter for the Donor.name property "canonicalizes"
//        /// the name.
//        /// 
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public Donor DonorForDonorName(string name)
//        {
//            string s = Donor.MakeDonorName(name);
//            return DonorList.FirstOrDefault
//                (d => d.name == s);
//                //(d => d.name.ToUpper() == s.ToUpper());
//        }


//        /// <summary>
//        /// Is there a Donor in the list with the same
//        /// name as the passed-in one?
//        /// 
//        /// Since we know that the other donor's name is
//        /// already of proper format, we don't have to
//        /// call MakeDonorName() as we do if we're just
//        /// checking against a string.
//        /// </summary>
//        /// <param name="other_donor"></param>
//        /// <returns></returns>
//        public Donor DonorForDonorName(Donor other_donor)
//        {
//            return this.DonorList.FirstOrDefault(d => d.name == other_donor.name);
//        }

//        /// <summary>
//        /// Since we know that a Donor's name is
//        /// already of proper format, we don't have to
//        /// call MakeDonorName() as we do if we're just
//        /// checking against a string.
//        /// </summary>
//        /// <param name="dao"></param>
//        /// <returns></returns>
//        public Donor DonorForDonorName(Donor dao)
//        {
//            return this.DonorList.FirstOrDefault(d => d.name == dao.name);
//        }

//        /// <summary>
//        /// Find a donor with the given code.
//        /// 
//        /// Donor codes are all upper-case, so there is no
//        /// need to check for case here.
//        /// </summary>
//        /// <param name="_cd"></param>
//        /// <returns>
//        /// matching Donor object, or null if no match
//        /// </returns>
//        public Donor DonorForDonorCode(string _cd)
//        {
//            return DonorList.FirstOrDefault
//                (d => d.code == _cd);
//        }

//        /// <summary>
//        /// Is there already an object in the list that matches
//        /// the passed-in one?
//        /// 
//        /// Two Donor objects match if their names are the same.
//        /// </summary>
//        /// <param name="d"></param>
//        /// <returns></returns>
//        public Donor MatchingDonor(Donor otherdonor)
//        {
//            return DonorList.Where(d => d.name == otherdonor.name).FirstOrDefault();
//        }

//        public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo gli)
//        {
//            return GliList.Where(g =>
//                (g.family_id == gli.family_id &&
//                g.child_name == gli.child_name &&
//                g.year == gli.year &&
//                g.request_type == gli.request_type)
//                ).FirstOrDefault();
//        }

//        public ServicesHouseholdEnrollment MatchingServicesHouseholdEnrollment(ServicesHouseholdEnrollment henroll)
//        {
//            return HoEnrList.Where
//                (h => (
//                h.year == henroll.year &&
//                h.service_type == henroll.service_type &&
//                h.head_of_household == henroll.head_of_household
//                )
//            ).FirstOrDefault();
//        }

//        /// <summary>
//        /// Check to see if matching object already exists. If so, replace it
//        /// with the new one. If not, add the new one to the end of the list.
//        /// </summary>
//        /// <param name="newval"></param>
//        public void AddOrUpdateBli(BagLabelInfo newval)
//        {
//            BagLabelInfo bli = MatchingBagLabelInfo(newval);
//            if (bli != null)
//            {
//                // update record in list by using newvalue's dao
//                // member, but retain previously-assigned id, if any:
//                newval.dao.Id = bli.dao.Id;
//                bli.dao = newval.dao;
//            }
//            else
//            {
//                BliList.Add(newval);
//            }
//        }

//        /// <summary>
//        /// Check to see if matching object already exists. If so, replace it
//        /// with the new one. If not, add the new one to the end of the list.
//        /// </summary>
//        /// <param name="newval"></param>
//        public void AddOrUpdateDonor(Donor newval)
//        {
//            Donor d = MatchingDonor(newval);
//            if (d != null)
//            {
//                // update record in list by using newvalue's dao
//                // member, but retain previously-assigned id, if any:
//                newval.dao.Id = d.dao.Id;
//                d.dao = newval.dao;
//            }
//            else
//            {
//                DonorList.Add(newval);
//            }
//        }

//        /// <summary>
//        /// Check to see if matching object already exists. If not,
//        /// create a new one.
//        /// 
//        /// A Donor object matches if its name == the value
//        /// passed in.
//        /// 
//        /// Return either the already-existing Donor or the
//        /// newly-created one.
//        /// 
//        /// </summary>
//        /// <param name="newval"></param>
//        public Donor FindOrCreateDonor(string newval)
//        {
//            Donor retDonor;
//            retDonor = DonorForDonorName(newval);
//            if (retDonor == null)
//            {
//                // Make a new Donor with the given name
//                // and code generated from that name.
//                //
//                // NOTE: the Donor name and code property setters
//                // may modify the given parameters. See Models.cs
//                retDonor = new Donor();
//                retDonor.name = newval;
//                retDonor.code = newval;
//                this.DonorList.Add(retDonor);
//            }
//            return retDonor;
//        }

//        /// <summary>
//        /// Check to see if matching object already exists. If so, replace it
//        /// with the new one. If not, add the new one to the end of the list.
//        /// </summary>
//        /// <param name="newval"></param>
//        public void AddOrUpdateGli(GiftLabelInfo newval)
//        {
//            GiftLabelInfo gli = MatchingGiftLabelInfo(newval);
//            if (gli != null)
//            {
//                // update record in list by using newvalue's dao
//                // member, but retain previously-assigned id, if any:
//                newval.dao.Id = gli.dao.Id;
//                gli.dao = newval.dao;
//            }
//            else
//            {
//                GliList.Add(newval);
//            }
//        }

//        /// <summary>
//        /// Check to see if matching object already exists. If so, replace it
//        /// with the new one. If not, add the new one to the end of the list.
//        /// </summary>
//        /// <param name="newval"></param>
//        public void AddOrUpdateHoEnr(ServicesHouseholdEnrollment newval)
//        {
//            ServicesHouseholdEnrollment henr = MatchingServicesHouseholdEnrollment(newval);
//            if (henr != null)
//            {
//                // update record in list by using newvalue's dao
//                // member, but retain previously-assigned id, if any:
//                newval.dao.Id = henr.dao.Id;
//                henr.dao = newval.dao;
//            }
//            else
//            {
//                HoEnrList.Add(newval);
//            }
//        }

//    }

//}

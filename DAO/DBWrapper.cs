using System.Collections.Generic;
using System.IO;
using System.Linq;

using LiteDB;

namespace DAO
{
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
    public class DBWrapper2
    {
        private LiteDatabase _db;
        private LiteCollection<Donor_DAO> _donorcoll;
        private LiteCollection<BagLabelInfo_DAO> _blicoll;
        private LiteCollection<GiftLabelInfo_DAO> _glicoll;
        private LiteCollection<ServicesHouseholdEnrollment_DAO> _henrcoll;

        public LiteCollection<Donor_DAO> Donors { get { return this._donorcoll; } }
        public  LiteCollection<BagLabelInfo_DAO> BLIs { get { return this._blicoll; }  }
        public LiteCollection<GiftLabelInfo_DAO> GLIs { get { return this._glicoll; } }
        public LiteCollection<ServicesHouseholdEnrollment_DAO> Enrollments { get { return this._henrcoll; } }

        private void SetUpCollections()
        {
            this._donorcoll = this._db.GetCollection<Donor_DAO>("donors");
            this._blicoll = this._db.GetCollection<BagLabelInfo_DAO >("blis");
            this._glicoll = this._db.GetCollection<GiftLabelInfo_DAO>("glis");
            this._henrcoll = this._db.GetCollection<ServicesHouseholdEnrollment_DAO >("enrollments");
            this._donorcoll.EnsureIndex(d => d.name);
            this._blicoll.EnsureIndex(b => b.family_id);
            this._glicoll.EnsureIndex(g => g.family_id);
            this._henrcoll.EnsureIndex(h => h.family_id);
        }
        public DBWrapper2()
        {
            this._db = DbUtils.GetDatabase();
            this.SetUpCollections();
        }

        public Donor MatchingDonor(Donor otherdonor)
        {
            Donor retDonor = null;
            Donor_DAO dao = this.Donors.Find(d => d.name == otherdonor.name).FirstOrDefault();
            if (dao != null)
            {
                retDonor = new Donor(dao);
            }
            return retDonor;
        }

        public void AddOrUpdateDonor(Donor d)
        {
            Donor dbd = this.MatchingDonor(d);
            if (dbd == null)
            {
                this.Donors.Insert(d);
            }
        }
    }


//    /// <summary>
//    /// Wrapper around data structures used for temporary
//    /// (in-RAM) storage and for saving to and loading from
//    /// persistent storage.
//    /// 
//    /// A DBWrapper object contains Lists of instances of the classes
//    /// defined in Models.cs:
//    ///  - BagLabelInfo objects
//    ///  - Donor objects
//    ///  - GiftLabelInfo objects
//    ///  - ServicesHouseholdEnrollment objects
//    /// </summary>
public class DBWrapper
{
    public List<BagLabelInfo> BliList { get; set; }
    public List<Donor> DonorList { get; set; }
    public List<GiftLabelInfo> GliList { get; set; }
    public List<ServicesHouseholdEnrollment> HoEnrList { get; set; }
    public bool IsEmpty
    {
        get
        {
            return (this.BliList.Count() == 0) &&
                (this.GliList.Count() == 0) &&
                (this.HoEnrList.Count() == 0);
        }
    }

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
        foreach (Donor d in InitialDonors.InitialList())
        {
            if (MatchingDonor(d) == null) // not already present
            {
                this.DonorList.Add(d);
                retInt++;
            }
        }
        return retInt;
    }

    /// <summary>
    /// Save data to persistent store.
    /// </summary>
    /// <returns></returns>
    public void Save()
    {
        DbUtils.BackupDatabase();
        using (LiteDatabase db = DbUtils.GetDatabase())
        {
            this.SaveDonors(db);
            this.SaveBagLabelInfo(db);
            this.SaveGiftabelInfo(db);
            this.SaveServicesHouseholdEnrollment(db);
        }
    }

    /// <summary>
    /// Load data from persistent store
    /// </summary>
    /// <returns></returns>
    public void Load()
    {
        using (LiteDatabase db = DbUtils.GetDatabase())
        {
            this.LoadDonors(db);
            this.LoadBagLabelInfo(db);
            this.LoadGiftLabelInfo(db);
            this.LoadServicesHouseholdEnrollment(db);
            int default_donors_added = this.AddDefaultDonors();
            if (default_donors_added > 0) // list changed -- save changes
            {
                this.SaveDonors(db);
            }
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
        string collection_name = "donors";
        LiteCollection<Donor_DAO> coll = db.GetCollection<Donor_DAO>(collection_name);
        foreach (Donor d in this.DonorList)
        {
            coll.Upsert(d.dao);
        }
    }

    public void SaveBagLabelInfo(LiteDatabase db)
    {
        string collection_name = "bag_label_info";
        LiteCollection<BagLabelInfo_DAO> coll = db.GetCollection<BagLabelInfo_DAO>(collection_name);
        foreach (BagLabelInfo bli in this.BliList)
        {
            coll.Upsert(bli.dao);
        }

    }

    public void SaveGiftabelInfo(LiteDatabase db)
    {
        string collection_name = "gift_label_info";
        LiteCollection<GiftLabelInfo_DAO> coll = db.GetCollection<GiftLabelInfo_DAO>(collection_name);
        foreach (GiftLabelInfo gli in this.GliList)
        {
            coll.Upsert(gli.dao);
        }
    }


    public void SaveServicesHouseholdEnrollment(LiteDatabase db)
    {
        string collection_name = "services_enrollment";
        LiteCollection<ServicesHouseholdEnrollment_DAO> coll = db.GetCollection<ServicesHouseholdEnrollment_DAO>(collection_name);
        foreach (ServicesHouseholdEnrollment henroll in this.HoEnrList)
        {
            coll.Upsert(henroll.dao);
        }
    }

    public void LoadDonors(LiteDatabase db)
    {
        this.DonorList.Clear();
        LiteCollection<Donor_DAO> coll = db.GetCollection<Donor_DAO>("donors");
        foreach (Donor_DAO dao in coll.Find(Query.All()))
        {
            this.DonorList.Add(new Donor(dao));
        }
    }

    public void LoadBagLabelInfo(LiteDatabase db)
    {
        this.BliList.Clear();
        LiteCollection<BagLabelInfo_DAO> coll = db.GetCollection<BagLabelInfo_DAO>("bag_label_info");
        foreach (BagLabelInfo_DAO dao in coll.Find(Query.All()))
        {
            this.BliList.Add(new BagLabelInfo(dao));
        }
    }

    public void LoadGiftLabelInfo(LiteDatabase db)
    {
        this.GliList.Clear();
        LiteCollection<GiftLabelInfo_DAO> coll = db.GetCollection<GiftLabelInfo_DAO>("gift_label_info");
        foreach (GiftLabelInfo_DAO dao in coll.Find(Query.All()))
        {
            this.GliList.Add(new GiftLabelInfo(dao));
        }
    }

    public void LoadServicesHouseholdEnrollment(LiteDatabase db)
    {
        this.HoEnrList.Clear();
        LiteCollection<ServicesHouseholdEnrollment_DAO> coll = db.GetCollection<ServicesHouseholdEnrollment_DAO>("services_enrollment");
        foreach (ServicesHouseholdEnrollment_DAO dao in coll.Find(Query.All()))
        {
            this.HoEnrList.Add(new ServicesHouseholdEnrollment(dao));
        }
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
        return BliList.Where
                      (b => (
                          b.year == bli.year &&
                          b.request_type == bli.request_type &&
                          b.family_id == bli.family_id
                          )
                      ).FirstOrDefault();
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
    public Donor MatchingDonor(Donor otherdonor)
    {
        return DonorList.Where(d => d.name == otherdonor.name).FirstOrDefault();
    }

    public GiftLabelInfo MatchingGiftLabelInfo(GiftLabelInfo gli)
    {
        return GliList.Where(g =>
            (g.family_id == gli.family_id &&
            g.child_name == gli.child_name &&
            g.year == gli.year &&
            g.request_type == gli.request_type)
            ).FirstOrDefault();
    }

    public ServicesHouseholdEnrollment MatchingServicesHouseholdEnrollment(ServicesHouseholdEnrollment henroll)
    {
        return HoEnrList.Where
            (h => (
            h.year == henroll.year &&
            h.service_type == henroll.service_type &&
            h.head_of_household == henroll.head_of_household
            )
        ).FirstOrDefault();
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
    public void AddOrUpdateDonor(Donor newval)
    {
        Donor d = MatchingDonor(newval);
        if (d != null)
        {
            // update record in list by using newvalue's dao
            // member, but retain previously-assigned id, if any:
            newval.dao.Id = d.dao.Id;
            d.dao = newval.dao;
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

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
            return this._donorcoll.Find(d => d.code == code).FirstOrDefault();
        }

        public Donor MatchingDonor(Donor other)
        {
            return FindDonorByName(other.name);
        }

        public void AddNewDonor(Donor d)
        {
            this._donorcoll.Insert(d);
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
            return d;
        }

        /// <summary>
        /// Read the list of initial (default) donors.
        /// Add any that are not already on our list.
        /// </summary>
        /// <returns></returns>
        private void AddDefaultDonors()
        {
            foreach (Donor d in InitialDonors.InitialList())
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
        public BagLabelInfo AddOrUpdateBLI(BagLabelInfo bli)
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
                // Now replace the object in the database with
                // the one passed in.
                this._blicoll.Update(bli);
            }
            return bli;
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
        public GiftLabelInfo AddOrUpdateGLI(GiftLabelInfo gli)
        {
            GiftLabelInfo dbo = this.MatchingGiftLabelInfo(gli);
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
                // Now replace the object in the database with
                // the one passed in.
                this._glicoll.Update(gli);
            }
            return gli;
        }

        // ServicesHouseholdEnrollment methods
        ///////////////////////////////////////
        public ServicesHouseholdEnrollment MatchingEnrollment(ServicesHouseholdEnrollment enr)
        {
            return this._henrcoll.Find(e =>
                e.year == enr.year &&
                e.service_type == enr.service_type &&
                e.head_of_household == enr.head_of_household
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
        public ServicesHouseholdEnrollment AddOrUpdateHoEnr(ServicesHouseholdEnrollment enr)
        {
            ServicesHouseholdEnrollment dbo = this.MatchingEnrollment(enr);
            if (dbo == null)
            {
                // not found -- insert the one passed
                // to us. As a side effect, the Insert()
                // method updates the Id property of the
                // inserted object.
                this._henrcoll.Insert(enr);
            }
            else
            {
                // found -- set the id of the passed-in object to
                // that of the found object.
                enr.Id = dbo.Id;
                // Now replace the object in the database with
                // the one passed in.
                this._henrcoll.Update(enr);
            }
            return enr;
        }


        // Public lists:
        ///////////////////
        public List<Donor> DonorList { get { return this._donorcoll.FindAll().ToList(); } }
        //public List<BagLabelInfo> BliList { get { return this._blicoll.FindAll().ToList(); } }
        public List<GiftLabelInfo> GliList { get { return this._glicoll.FindAll().ToList(); } }
        public List<ServicesHouseholdEnrollment> HoEnrList { get { return this._henrcoll.FindAll().ToList(); } }


        // specialized queries:
        //////////////////////


        public string[] RequestTypesInDb()
        {
            return (from g in this._glicoll.FindAll() select g.request_type).Concat
                    (from b in this._blicoll.FindAll() select b.request_type).Distinct().ToArray();
        }


        public int[] YearsInDb()
        {
            int[] intArray = (from g in this._glicoll.FindAll().ToArray()
                              select (int)g.year).Concat
                            (from b in this._blicoll.FindAll().ToArray()
                             select (int)b.year).Concat
                             (from s in this._henrcoll.FindAll().ToArray()
                              select (int)s.year).Distinct().ToArray();
            return intArray;
        }

        public List<BagLabelInfo > BLIsByYearDonorReqType(int year, Donor d, string req_type)
        {
            return this._blicoll.Find(b =>
                b.year == year &&
                b.request_type == req_type &&
                b.donor_name == d.name
            ).ToList();
        }
        
    }
}

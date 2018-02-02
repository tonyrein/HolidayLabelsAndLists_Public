using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace DAO
{
    public class EntitiesHandler : ValleyLabelsAndListsEntities
    {
        public EntitiesHandler()
            : base(DBUtils.FullConnectionString())
        {
            // Uncomment below line to show db activity on Output
            //Database.Log = sql => Debug.Write(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {

        }

        public dnr DonorForDonorCode(string donor_code)
        {
            dnr retDonor = null;
            if (!String.IsNullOrEmpty(donor_code))
            {
                retDonor = this.donors.Where(d => d.code.ToUpper() == donor_code.ToUpper()).FirstOrDefault();
            }
            return retDonor;
        }

        /// <summary>
        /// !!!! What happens if "Donor ID" column is empty?
        /// </summary>
        /// <param name="donor_code"></param>
        /// <returns></returns>
        public long DonorIdForDonorCode(string donor_code)
        {
            long retValue = -1L;
            dnr d = DonorForDonorCode(donor_code);
            if (d != null)
                retValue = d.id;
            return retValue;
        }

        /// <summary>
        ///  Given a donor name, return the corresponding donor id.
        ///  
        /// 1. Check whether there's a donor in the database with
        /// the given name.
        /// 
        /// 2. If there is no donor by that name, check again, this
        /// time by donor code. If there's no donor code supplied, 
        /// derive the donor code by transforming the name (removing punctuation,
        /// etc.) Then check to see if there's a donor with that
        /// code. .
        ///  
        /// 3. If we still don't have a donor, create one and add it to
        /// context. Make sure to save the new donor, in order to retrieve
        /// its database id.
        /// 
        /// 4. Return the database id.
        /// </summary>
        /// <param name="donor_name_text"></param>
        /// <returns></returns>
        public long DonorIdForName(string donor_name_text, string donor_code_text)
        {
            long retValue = -1L;
            if (!String.IsNullOrEmpty(donor_name_text))
            {
                var query = this.donors.Where<dnr>(d => d.name == donor_name_text);
                dnr dn = query.FirstOrDefault<dnr>();
                if (dn == null)
                {
                    // If donor_code_text is supplied, use that; otherwise, 
                    // generate a code from the name.
                    if (String.IsNullOrEmpty(donor_code_text))
                        donor_code_text = EntityUtils.DonorNameToCanonical(donor_name_text);
                    query = this.donors.Where<dnr>(d => d.code == donor_code_text);
                    dn = query.FirstOrDefault<dnr>();
                    if (dn == null) // Can't find it -- make a new one
                    {
                        dn = new dnr();
                        dn.code = donor_code_text;
                        dn.name = donor_name_text;
                        this.donors.Add(dn);
                        this.SaveChanges();
                    }
                }
                retValue = dn.id;
            }
            return retValue;
        }

        /// <summary>
        /// Copy database records to archive tables. If copy is successful,
        /// delete original records from the bag_label_info, gift_label_info,
        /// and services_household_enrollment tables. Do not delete records
        /// from donor table.
        /// </summary>
        public void DoArchive()
        {
            string[] sql_insert_commands = new string[]
            {
                @"INSERT INTO archive_donor (id, name, code) SELECT id, name, code FROM [donor]",

                @"INSERT INTO archive_bag_label_info (id, year, family_id, family_name, family_members, request_type, donor_id)"
                    + " SELECT id, year, family_id, family_name, family_members, request_type, donor_id FROM [bag_label_info]",


                // TODO: Fix column name 'request detail' in gift_label_info to remove space
                @"INSERT INTO archive_gift_label_info (id, year, family_id, family_name, child_name, child_gender, child_age,"
                    + " request_type, donor_id, request_detail)"
                    + " SELECT id, year, family_id, family_name, child_name, child_gender, child_age,"
                    + " request_type, donor_id, 'request detail' FROM [gift_label_info]",

                @"INSERT INTO archive_services_household_enrollment (id, service_type, year, month, day, head_of_household, phone, address,"
                    + " city, state_or_province, postal_code) SELECT id, service_type, year, month, day, head_of_household,"
                    + " phone, address, city, state_or_province, postal_code FROM [services_household_enrollment]"
            };
            string[] sql_delete_tables = new string[]
            {
                "bag_label_info", "gift_label_info", "services_household_enrollment"
            };

            using (var transaction = this.Database.BeginTransaction())
            {
                try
                {
                    foreach (string sql in sql_insert_commands)
                        this.Database.ExecuteSqlCommand(sql);
                    foreach (string tbl_name in sql_delete_tables)
                        this.Database.ExecuteSqlCommand(@"DELETE FROM '" + tbl_name + "'");
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                }
            }
        }

    }

    public static class EntityUtils
    {
        private static string non_alphanum = @"[^0-9a-zA-Z\s]";
        private static Regex rna = new Regex(non_alphanum);
        private static string multiple_spaces = @"[\s]+";
        private static Regex rms = new Regex(multiple_spaces);

        /// <summary>
        /// Transform a donor name to a canonical
        /// form (no punctuation, spaces replaced
        /// with underscores, title case)
        /// 
        /// ??? Why this instead of FileUtils.CleanString()?
        /// 
        /// </summary>
        /// <param name="donor_name"></param>
        /// <returns></returns>
        /// 
        public static string DonorNameToCanonical(string donor_name)
        {
            string retString = donor_name.Trim();
            // Capitalize first letter of each word:
            retString = Utils.TextUtils.ProperCase(retString);
            // Remove everything that's not a digit, letter, or whitespace:
            retString = rna.Replace(retString, "");
            // Replace space with underscore. Collapse multiple spaces
            // to single underscore
            retString = rms.Replace(retString, "_");
            return retString;
        }

        public static string DatabaseFileName()
        {
            string connName = "ValleyLabelsAndListsEntities";
            string connString = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
            string fn_marker = "data source=";
            int offset = connString.LastIndexOf(fn_marker);
            string fn_part = connString.Substring(offset + fn_marker.Length);
            offset = fn_part.LastIndexOf("\"");
            return fn_part.Substring(0, offset);
        }
    }

}

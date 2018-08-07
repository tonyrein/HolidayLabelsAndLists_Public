using System;
using System.Collections.Generic;

using DAO;

namespace VestaProcessor
{
    public enum LabelType
    {
        Bag,
        Gift
    }
    public enum RequestType
    {
        Clothing,
        Toys,
        Other
    }

    public static class ReportSectionFactory
    {
        /// <summary>
        /// Given a marker string, parse it to figure out
        /// what kind of label and what kind of request
        /// it's marking.
        /// </summary>
        /// <param name="marker_text"></param>
        /// <returns></returns>
        private static Tuple<LabelType, RequestType> parse_marker_text(string marker_text)
        {
            LabelType label_type =
                (LabelType)Enum.Parse(typeof(LabelType), marker_text.Split(' ')[1]);
            RequestType request_type =
                (RequestType)Enum.Parse(typeof(RequestType), marker_text.Split(' ')[4]);

            return new Tuple<LabelType, RequestType>(label_type, request_type);
        }
        private const string EMPTY_SECTION_MARKER = "- no data -";
        private const int EMPTY_SECTION_MARKER_COLUMN = 1;
        public static VestaReportSection create(
            ExcelSheet sheet,
            int top_row_idx,
            string marker_text,
            ReportTypes report_type,
            int year
            )
        {
            VestaReportSection retSect = null;
            ExcelRow r = sheet[top_row_idx + 2];
            if (!r.IsEmpty() &&
                (r[EMPTY_SECTION_MARKER_COLUMN]).Trim() != EMPTY_SECTION_MARKER)
            {
                switch (report_type)
                {
                    case ReportTypes.Participant:
                        retSect = new ParticipantReportSection(sheet, top_row_idx,
                            year);
                        break;
                    case ReportTypes.Labels:
                        Tuple<LabelType, RequestType> tu = parse_marker_text(marker_text);
                        LabelType lt = tu.Item1;
                        RequestType rt = tu.Item2;
                        switch (lt)
                        {
                            case LabelType.Bag:
                                retSect = new BagLabelReportSection(sheet, top_row_idx, rt, year);
                                break;
                            case LabelType.Gift:
                                retSect = new GiftLabelReportSection(sheet, top_row_idx, rt, year);
                                break;
                        }
                        break;
                }
            }
            return retSect;
        }
    }

    public abstract class VestaReportSection
    {
        ExcelSheet sheet;
        int top_row_idx;
        int header_row_idx;
        int first_data_row_idx;
        protected int year;
        protected Dictionary<string, int> field_indices = new Dictionary<string, int>();
        public VestaReportSection(ExcelSheet sheet, int top_row, int year)
        {
            this.sheet = sheet;
            this.top_row_idx = top_row;
            this.year = year;
            this.header_row_idx = this.top_row_idx + 1;
            this.first_data_row_idx = this.top_row_idx + 2;
            // read fields from header row:
            string[] field_names = (this.sheet[header_row_idx]).ToStringArray();
            for (int i = 0; i < field_names.Length; i++)
            {
                if (field_names[i] != "")
                    this.field_indices.Add(field_names[i], i);
            }
        }
        public void execute(DBWrapper context)
        {
            // start at first data row and keep going until
            // a non-data row is encountered.
            // For each row:
            //  Generate a record object from its contents
            //  If there's already a matching record,
            //      update that record's contents from
            //      the fields of the object we just
            //      generated.
            //  Otherwise, add the object we just generated
            //      to short-term storage.
            //
            int row_idx = this.first_data_row_idx;
            ExcelRow row = this.sheet[row_idx];
            // This data section may end with a row containing
            // "Counts households ..." in first column.
            while ((row != null) && (this.IsDataRow(row)))
            {
                object new_rec = this.ObjectFromVestaRow(context, row, year);
                this.add_or_update(new_rec, context);
                row_idx++;
                row = this.sheet[row_idx];
            }
        }

        protected abstract object ObjectFromVestaRow(DBWrapper context, ExcelRow row, int year);
        protected abstract void add_or_update(object rec, DBWrapper context);
        protected abstract bool IsDataRow(ExcelRow row);
    }


    public class ParticipantReportSection : VestaReportSection
    {
        public ParticipantReportSection(ExcelSheet sheet, int top_row_idx, int year)
            : base(sheet, top_row_idx, year)
        {
        }

        protected override object ObjectFromVestaRow(DBWrapper context, ExcelRow row, int year)
        {
            // Make an object to return
            ServicesHouseholdEnrollment retObj = new ServicesHouseholdEnrollment();
            // Get a date/time string and parse it:
            DateTime dt;
            string s = row[this.field_indices["Date Reserved/Enrolled"]];
            // s represents a double integer. Parse it into a number:
            double d;
            if (Double.TryParse(s, out d)) // string parsed to double OK
            {
                // Turn double into DateTime object and use its properties
                dt = DateTime.FromOADate(d);
            }
            else
            {
                dt = DateTime.Now; // can't get date from report -- just use current date
            }
            // Ignore year argument passed to us -- does not apply for services_household_enrollment
            // Set date fields from DateTime object:
            retObj.year = dt.Year;
            retObj.day = dt.Day;
            retObj.month = dt.Month;
            // The rest of the fields are simply strings:
            retObj.service_type = row[this.field_indices["Partner Network Service"]];
            retObj.head_of_household = row[this.field_indices["Head of Household"]];
            retObj.family_id = row[this.field_indices["client ID"]];
            retObj.phone = row[this.field_indices["Primary Phone"]];
            retObj.address = row[this.field_indices["Address"]];
            retObj.city = row[this.field_indices["City"]];
            retObj.state_or_province = row[this.field_indices["State"]];
            retObj.postal_code = row[this.field_indices["Zip"]];
            return retObj;
        }

   

        /// <summary>
        /// The end of a participant report data section is marked
        /// by a row with "Counts households ..." in the first column.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected override bool IsDataRow(ExcelRow row)
        {
            return ((!row.IsEmpty()) &&
                (!row[0].StartsWith("Counts households", ignoreCase: true,
                  culture: null))
                  );
        }

        protected override void add_or_update(object rec, DBWrapper context)
        {
            context.AddOrUpdateHoEnr((ServicesHouseholdEnrollment)rec);
        }
    }

    public abstract class LabelReportSection : VestaReportSection
    {
        protected RequestType request_type;
        protected LabelType label_type;
        public LabelReportSection(ExcelSheet sheet, int top_row, int year)
            : base(sheet, top_row, year)
        {
        }
        public LabelType labelType { get { return this.label_type; } }
    }

    public class BagLabelReportSection : LabelReportSection
    {
        public BagLabelReportSection(ExcelSheet sheet, int top_row_idx,
            RequestType rt, int year)
            : base(sheet, top_row_idx, year)
        {
            this.label_type = LabelType.Bag;
        }

        protected override object ObjectFromVestaRow(DBWrapper context, ExcelRow row, int year)
        {
            string donor_name;
            BagLabelInfo workObj = new BagLabelInfo();
            workObj.year = year;
            donor_name = row[this.field_indices["Donor Name"]];
            Donor d = context.FindOrCreateDonor(donor_name);
            workObj.donor_code = d.code;
            workObj.donor_name = d.name;
            workObj.family_id = row[this.field_indices["Family ID"]];
            workObj.family_name = row[this.field_indices["Family Name"]];
            workObj.family_members = row[this.field_indices["Family Members"]];
            // next line is probably redundant -- we got request type in constructor:
            workObj.request_type = row[this.field_indices["Request Type"]];
            return workObj;
        }


        protected override bool IsDataRow(ExcelRow row)
        {
            return (!row.IsEmpty());
        }

        protected override void add_or_update(object rec, DBWrapper context)
        {
            context.AddOrUpdateBli((BagLabelInfo)rec);
        }
    }


    public class GiftLabelReportSection : LabelReportSection
    {
        public GiftLabelReportSection(ExcelSheet sheet, int top_row_idx,
            RequestType rt, int year)
            : base(sheet, top_row_idx, year)
        {
            this.label_type = LabelType.Gift;
            this.request_type = rt;
        }

        protected override object ObjectFromVestaRow(DBWrapper context, ExcelRow row, int year)
        {
            object retObj = null;
            string donor_name;
            GiftLabelInfo workObj = new GiftLabelInfo();
            workObj.year = year;
            // If there's anything in the spreadsheet's "Donor ID"
            // column, use that to identify this row's donor.
            // Otherwise, use the "Donor Name" column.
            donor_name = row[this.field_indices["Donor Name"]];
            Donor d = context.FindOrCreateDonor(donor_name);
            workObj.donor_code = d.code;
            workObj.donor_name = d.name;
            int child_age;
            bool parsed_ok = int.TryParse(row[this.field_indices["Age"]],
                out child_age);
            if (!parsed_ok)
                child_age = -1;
            workObj.child_age = child_age;
            string scratch = (row[this.field_indices["Gender"]]).Substring(0, 1).ToUpper();
            if (scratch != "M" && scratch != "F")
                scratch = "NotSpecified";
            workObj.child_gender = scratch;
            workObj.child_name = row[this.field_indices["Family Member First Name"]];
            workObj.family_id = row[this.field_indices["Family ID"]];
            workObj.family_name = row[this.field_indices["Family Name"]];
            workObj.request_detail = row[this.field_indices["Request Detail"]];
            workObj.request_type = row[this.field_indices["Request Type"]];
            retObj = workObj;
            return retObj;
        }

       
        protected override bool IsDataRow(ExcelRow row)
        {
            return (!row.IsEmpty());
        }

        protected override void add_or_update(object rec, DBWrapper context)
        {
            context.AddOrUpdateGli((GiftLabelInfo)rec);
        }
    }

}

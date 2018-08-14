using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DAO;

using GlobRes = AppWideResources.Properties.Resources;

namespace VestaProcessor
{
    public class VestaImporter
    {
        protected ExcelSheet sheet;
        protected ReportTypes report_type;
        protected BackgroundWorker worker;
        protected List<VestaReportSection> sections = new List<VestaReportSection>();
        protected int year;
        protected bool valid_VESTA;
        private const int LABEL_YEAR_COLUMN = 2;
        private const int LABEL_YEAR_ROW = 4;
        private string filespec;
        private string sheet_name;
        private int data_section_count;

        public VestaImporter(BackgroundWorker wk, string filespec, string sheet_name)
        {
            this.worker = wk;
            this.filespec = filespec;
            this.sheet_name = sheet_name;
            this.Init();
            this.Survey();
        }

        private void Init()
        {
            worker.ReportProgress(0,
                string.Format(GlobRes.StartingEachReportMsg, this.filespec)
                );
            valid_VESTA = true;
            WorkbookWrapper bk = new WorkbookWrapper(this.filespec);
            if (bk == null)
            {
                worker.ReportProgress(0,
                    string.Format(GlobRes.InvalidWorksheetMsg, this.filespec)
                    );
                valid_VESTA = false;
                return;
            }

            this.sheet = bk.SheetByName(this.sheet_name);
            if (this.sheet == null)
            {
                worker.ReportProgress(0,
                    string.Format(GlobRes.SheetNotFoundInFileMsg, this.sheet_name, this.filespec)
                    );
                valid_VESTA = false;
                return;
            }
            this.report_type = VestaImporterUtils.ReportType(this.sheet);
            if (this.report_type == ReportTypes.Unknown)
            {
                worker.ReportProgress(0,
                    string.Format(GlobRes.UnknownReportTypeMsg, this.filespec)
                    );
                valid_VESTA = false;
                return;
            }
        }

        /// <summary>
        /// The data sections of VESTA reports begin with
        /// marker strings. These are different for each
        /// type of VESTA report.
        /// </summary>
        protected string[] SectionMarkers
        {
            get
            {
                string[] retArray = null;
                switch (this.report_type)
                {
                    case ReportTypes.Labels:
                        retArray = new string[]
                        {
                        "1. Gift Label - Toys",
                        "2. Gift Label - Clothing",
                        "3. Gift Label - Other",
                        "4. Bag Label - Toys",
                        "5. Bag Label - Clothing",
                        "6. Bag Label - Other"
                        };
                        break;
                    case ReportTypes.Participant:
                        retArray = new string[] { "2. List of Households Enrolled in EA Partner Network Services" };
                        break;
                    default: // shouldn't get here
                        throw new InvalidDataException("Unknown Report Type");
                }
                return retArray;
            }
        }


        /// <summary>
        /// Initialize data derived from the VESTA report's header.
        /// </summary>
        private void GetHeaderInfo()
        {
            if (this.report_type == ReportTypes.Labels) // not relevant for other report types
            {
                int label_year_row = this.sheet.RowIndexOf("Year", col_idx: LABEL_YEAR_COLUMN-1);
                if (label_year_row == -1)
                {
                    this.year = 2099;
                }
                else
                {
                    string y = this.sheet.ValueAt(label_year_row, LABEL_YEAR_COLUMN);
                    if (!int.TryParse(y, out this.year))
                        this.year = 2099;
                }
            }
            else
                this.year = -1;
        }


        /// <summary>
        /// Find all the data sections in this VESTA report and add them
        /// to our list.
        /// 
        /// VESTA report data sections are marked by particular text strings. Which
        /// text strings might be found in a particular report depends on the report
        /// type -- see SectionMarkers property of this class.
        /// 
        /// Return the number of data sections found.
        /// </summary>
        private int FindDataSections()
        {
            int retInt = 0;
            // Check for each marker string relevant to
            // this report type:
            foreach (string marker_text in this.SectionMarkers)
            {
                // find marker string:
                int top_row_idx = this.sheet.RowIndexOf(marker_text);
                if (top_row_idx != -1) // if not -1, we found it
                {
                    // if there's data, add a section to our list
                    VestaReportSection sect = ReportSectionFactory.create(
                        sheet,
                        top_row_idx,
                        marker_text,
                        this.report_type,
                        this.year);
                    if (sect != null)
                    {
                        this.sections.Add(sect);
                        retInt++;
                    }
                }
            }
            return retInt;
        }


        /// <summary>
        /// 1. Get information in this VESTA report's header.
        /// 2. Find all the data sections in this VESTA report and add them
        /// to our list.
        /// </summary>
        protected void Survey()
        {
            if (!worker.CancellationPending)
            {
                worker.ReportProgress(0, GlobRes.SearchingForDataSectionsMsg);
                this.GetHeaderInfo();
                this.data_section_count = this.FindDataSections();
                worker.ReportProgress(0,
                    string.Format(GlobRes.CountOfDataSectionsMsg, data_section_count)
                    );
            }
        }

        /// <summary>
        /// Read data from the VESTA report and write it to
        /// the temp storage.
        /// 
        /// Loop over each section of the report and call its
        /// execute() method.
        /// 
        /// Return 1 if all data sections processed OK; else return 0.
        /// </summary>
        /// <returns></returns>
        public int execute(DBWrapper context)
        {
            int i = 0;
            foreach (VestaReportSection section in this.sections)
            {
                if (!worker.CancellationPending)
                {
                    i++;
                    worker.ReportProgress(0,
                        string.Format(GlobRes.SectionXOfYMsg, i, data_section_count)
                        );
                    section.execute(context);
                }
            }
            if (i == data_section_count)
            {
                worker.ReportProgress(0, GlobRes.AllSectionsProcessedOKMsg);
                return 1;
            }
            else
            {
                worker.ReportProgress(0,
                    string.Format(GlobRes.SomeSectionsNotProcessedMsg, i, data_section_count)
                        );
                return 0;
            }
        }
    }
} // end of namespace
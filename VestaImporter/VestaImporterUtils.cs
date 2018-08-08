

using System.ComponentModel;
using System.IO;
using DAO;
using GlobRes = AppWideResources.Properties.Resources;

namespace VestaProcessor
{
    public enum ReportTypes
    {
        Labels,
        Participant,
        Unknown
    }

    public static class VestaImporterUtils
    {
        // row and column are ZERO-BASED!
        private const int REPORT_TYPE_MARKER_COLUMN = 0;
        private const int REPORT_TYPE_MARKER_ROW = 0;
        private const string LABELS_REPORT_MARKER_TEXT = "EA - Adopt-a-Family - Labels";
        private const string PARTICIPANT_REPORT_MARKER_TEXT = "EA - Partner Network Services Enrollment List";
 
        /// <summary>
        /// Given an ExcelSheet object, return the
        /// corresponding report type, if any.
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        public static ReportTypes ReportType(ExcelSheet sh)
        {
            ReportTypes retValue;
            if (sh == null)
            {
                retValue = ReportTypes.Unknown;
            }
            else
            {
                switch(sh.ValueAt(REPORT_TYPE_MARKER_ROW, REPORT_TYPE_MARKER_COLUMN))
                {
                    case LABELS_REPORT_MARKER_TEXT:
                        retValue = ReportTypes.Labels;
                        break;
                    case PARTICIPANT_REPORT_MARKER_TEXT:
                        retValue = ReportTypes.Participant;
                        break;
                    default:
                        retValue = ReportTypes.Unknown;
                        break;
                }
            }
            return retValue;
        }

        /// <summary>
        /// Import contents of each file selected
        /// by the user.
        /// </summary>
        /// <param name="wk"></param>
        /// <param name="context"></param>
        /// <param name="report_names"></param>
        /// <returns></returns>
        /// TODO: Move this method out of this class. It doesn't
        /// refer to anything in frmMain and does no user interaction. It should probably
        /// be in a helper or utility class -- perhaps VestaImporterUtils?
        /// 
        public static int ImportFromVesta(BackgroundWorker wk,
            DBWrapper context, string[] report_names)
        {
            int retInt = 0;
            foreach (string fn in report_names)
            {
                if (Path.GetExtension(fn) == ".xlsx")
                {
                    VestaImporter p = new VestaImporter(wk, fn, GlobRes.ResultsSheetDefaultName);
                    retInt += p.execute(context);
                }
            }
            // if we read anything in, save the changes to the database:
            if (retInt > 0)
                context.Save();
            return retInt;
        }

    }
}

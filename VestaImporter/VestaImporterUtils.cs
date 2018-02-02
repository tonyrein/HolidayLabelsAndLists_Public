

using DAO;

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
    }
}

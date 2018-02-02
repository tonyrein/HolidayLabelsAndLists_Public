using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public static class DAOConnectionStrings
    {
        public const string EXCEL_TEMPLATE = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Mode=Read;Extended Properties=&quot;Excel 12.0 Xml;HDR=YES&quot;";
        public const string SQLITE = @"data source=C:\Users\Public\Documents\ValleyLabelsAndLists\Data\Database\ValleyLabelsAndLists.sqlite";
    }
    
}

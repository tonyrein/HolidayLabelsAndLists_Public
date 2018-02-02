using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    class DAOAdapter
    {
        private DAOBackendTypes _backend_type;
        private bool _reads_add_initial_null;
        private bool _writes_add_initial_null;
        private string[] _field_names;
        private string _db_name;
        private string _table_name;
        private string _base_select_string;


        public DAOAdapter(DAOBackendTypes backend_type, string[] field_names,
            string db_name, string table_name, bool reads_add_initial_null=false,
            bool writes_add_initial_null=false)
        {
            this._backend_type = backend_type;
            this._reads_add_initial_null = reads_add_initial_null;
            this._writes_add_initial_null = writes_add_initial_null;
            this._field_names = field_names;
            this._db_name = db_name;
            this._table_name = table_name;
            this._base_select_string = this._BuildBaseSelectString();
        }
        private string _BuildBaseSelectString()
        {
            string s = "SELECT ";
            if (_reads_add_initial_null) { s += "NULL, "; }
            foreach(string f in this._field_names)
            {
                s += string.Format("[{0}], ", f);
            }
            s += string.Format("FROM {0}", this._table_name);
            return s;
        }

        public List<string[]> GetList(List<ArgumentInfo> args=null)
        {
            List<string[]> retList = new List<string[]>();


            return retList;
            
        }
    }
}

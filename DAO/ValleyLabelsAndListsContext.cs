using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    /// <summary>
    /// Partial class to supply alternate constructors
    /// </summary>
    public partial class ValleyLabelsAndListsEntities : DbContext
    {
        /// <summary>
        /// Create from a DBConnection
        /// </summary>
        /// <param name="conn"></param>
        public ValleyLabelsAndListsEntities(DbConnection conn)
            : base(conn, true)
        {
        }

        /// <summary>
        /// Create from a connection string
        /// </summary>
        /// <param name="conn_string"></param>
        public ValleyLabelsAndListsEntities(string conn_string)
            : base(conn_string)
        {

        }
    }
}

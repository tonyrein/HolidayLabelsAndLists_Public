//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAO
{
    using System;
    using System.Collections.Generic;
    
    public partial class gift_label_info
    {
        public long id { get; set; }
        public Nullable<long> year { get; set; }
        public string family_id { get; set; }
        public string family_name { get; set; }
        public string child_name { get; set; }
        public string child_gender { get; set; }
        public Nullable<long> child_age { get; set; }
        public Nullable<long> donor_id { get; set; }
        public string request_type { get; set; }
        public string request_detail { get; set; }
    
        public virtual dnr donor { get; set; }
    }
}

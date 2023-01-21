//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DCS_new.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class space
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public space()
        {
            this.freelancers = new HashSet<freelancer>();
            this.issues = new HashSet<issue>();
            this.messages = new HashSet<message>();
            this.tasks = new HashSet<task>();
        }
    
        public int s_id { get; set; }
        public string s_name { get; set; }

        [Required(ErrorMessage = "Required Feild")]
        public string s_status { get; set; }
        public Nullable<int> b_id { get; set; }
        public Nullable<int> head_e_id { get; set; }
    
        public virtual Business Business { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<freelancer> freelancers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<issue> issues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<message> messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<task> tasks { get; set; }
    }

    public partial class spaceModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public spaceModel()
        {
            this.freelancers = new HashSet<freelancer>();
            this.issues = new HashSet<issue>();
            this.messages = new HashSet<message>();
            this.tasks = new HashSet<task>();
        }

        public int s_id { get; set; }
        [Required(ErrorMessage = "Required Feild")]
        public string s_name { get; set; }

        [Required(ErrorMessage = "Required Feild")]
        public string s_status { get; set; }
        public Nullable<int> b_id { get; set; }

        [Required(ErrorMessage = "Required Field")]
        public Nullable<int> head_e_id { get; set; }

        public virtual Business Business { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<freelancer> freelancers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<issue> issues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<message> messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<task> tasks { get; set; }
    }
}

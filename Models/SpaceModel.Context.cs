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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SpacesEntities2 : DbContext
    {
        public SpacesEntities2()
            : base("name=SpacesEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Business> Businesses { get; set; }
        public virtual DbSet<freelancer> freelancers { get; set; }
        public virtual DbSet<issue> issues { get; set; }
        public virtual DbSet<message> messages { get; set; }
        public virtual DbSet<space> spaces { get; set; }
        public virtual DbSet<task> tasks { get; set; }
        public virtual DbSet<wage> wages { get; set; }
    }
}

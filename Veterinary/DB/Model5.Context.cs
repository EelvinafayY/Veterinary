﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Veterinary.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VeterinaryClinic2 : DbContext
    {
        public VeterinaryClinic2()
            : base("name=VeterinaryClinic2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Animals> Animals { get; set; }
        public virtual DbSet<Appointments> Appointments { get; set; }
        public virtual DbSet<Breed> Breed { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Diagnosis> Diagnosis { get; set; }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<MedicalRecords> MedicalRecords { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethod { get; set; }
        public virtual DbSet<Personal> Personal { get; set; }
        public virtual DbSet<Reason> Reason { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Services> Services { get; set; }
        public virtual DbSet<Specialization> Specialization { get; set; }
        public virtual DbSet<Species> Species { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Tests> Tests { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<TypeService> TypeService { get; set; }
    }
}

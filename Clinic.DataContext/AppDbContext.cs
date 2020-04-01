﻿using Clinic.Models.DomainClasses.Appointment;
using Clinic.Models.DomainClasses.NewsPage;
using Clinic.Models.DomainClasses.Others;
using Clinic.Models.DomainClasses.Users;
using Microsoft.EntityFrameworkCore;
using System;

namespace Clinic.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        { }

        public virtual DbSet<Visit> Visits { get; set; }
        public virtual DbSet<Drug> Drugs { get; set; }
        public virtual DbSet<InsuranceCard> InsuranceCards { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<PrescriptionDrug> PrescriptionDrugs { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WeekDay> WeekDays { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<SiteAdmin> SiteAdmins { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<ClinicManager> ClinicManagers { get; set; }
        public virtual DbSet<Pharmacy> Pharmacies { get; set; }
        public virtual DbSet<Administration> Administrations { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }

        public virtual DbSet<ManagerMessage> ManagerMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

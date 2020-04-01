using System;
using System.Collections.Generic;
using System.Text;
using Clinic.Models.DomainClasses.Others;
using Clinic.Models.DomainClasses.Users;
using Microsoft.EntityFrameworkCore;

namespace Clinic.DataContext.ModelBuilderExt
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SiteAdmin>()
                .HasData(new SiteAdmin()
                {
                    Id = 1,
                    Email = "admin@admin.com",
                    FullName = "مدیر سایت",
                    Username = "admin",
                    Password = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918"
                });
            modelBuilder.Entity<ClinicManager>()
                .HasData(new ClinicManager()
                {
                    Id = 2,
                    Email = "manager@manager.com",
                    FullName = "مدیر کلینیک",
                    Username = "manager",
                    Password = "6ee4a469cd4e91053847f5d3fcb61dbcc91e8f0ef10be7748da4c4a1ba382d17"
                });
            modelBuilder.Entity<Pharmacy>()
                .HasData(new Pharmacy()
                {
                    Id = 3,
                    Email = "pharmacy@pharmacy.com",
                    FullName = "مسئول داروخانه",
                    Username = "pharmacy",
                    Password = "20b04a4f018b89f369ce009213afc6369f59fe1c903c0770732ec7402163c358"
                });
            modelBuilder.Entity<Administration>()
                .HasData(new Administration()
                {
                    Id = 1,
                    Email = "masoud.xpress@gmail.com",
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Password = "MASOUD7559"
                });
        }
    }
}

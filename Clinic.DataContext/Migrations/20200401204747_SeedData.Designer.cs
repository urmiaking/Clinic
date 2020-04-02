﻿// <auto-generated />
using System;
using Clinic.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Clinic.DataContext.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200401204747_SeedData")]
    partial class SeedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Drug", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Cost")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Instruction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PharmacyId")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("PharmacyId");

                    b.ToTable("Drugs");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.InsuranceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<string>("InsuranceName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("InsuranceProviders");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Prescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VisitId")
                        .IsUnique();

                    b.ToTable("Prescriptions");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.PrescriptionDrug", b =>
                {
                    b.Property<int>("DrugId")
                        .HasColumnType("int");

                    b.Property<int>("PrescriptionId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<bool>("IsBought")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWantToBuy")
                        .HasColumnType("bit");

                    b.HasKey("DrugId", "PrescriptionId");

                    b.HasIndex("PrescriptionId");

                    b.ToTable("PrescriptionDrugs");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ComplainDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Response")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VisitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VisitId")
                        .IsUnique();

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Reservation", b =>
                {
                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReserveDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReserveStatus")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PatientId", "DoctorId", "ReserveDate");

                    b.HasIndex("DoctorId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Visit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CauseOfPatientReferral")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DoctorAssessment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DoctorNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("GivenScore")
                        .HasColumnType("real");

                    b.Property<int?>("InsuranceProviderId")
                        .HasColumnType("int");

                    b.Property<int>("ReservationDoctorId")
                        .HasColumnType("int");

                    b.Property<int>("ReservationPatientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReservationReserveDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("InsuranceProviderId");

                    b.HasIndex("ReservationPatientId", "ReservationDoctorId", "ReservationReserveDate")
                        .IsUnique();

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.WeekDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DoctorId")
                        .HasColumnType("int");

                    b.Property<string>("EightTen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FourteenSixteen")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenTwelve")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TwelveFourteen")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("WeekDays");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("NewsId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NewsId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ShortDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SiteAdminId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VisitCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SiteAdminId");

                    b.ToTable("News");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.NewsTag", b =>
                {
                    b.Property<int>("NewsId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("NewsId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("NewsTags");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.Reply", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CommentId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserId");

                    b.ToTable("Replies");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TagValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Others.Administration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Host")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Port")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Administrations");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "masoud.xpress@gmail.com",
                            Host = "smtp.gmail.com",
                            Password = "MASOUD7559",
                            Port = 587
                        });
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Others.ManagerMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Seen")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ManagerMessages");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("UserType").HasValue("User");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Users.ClinicManager", b =>
                {
                    b.HasBaseType("Clinic.Models.DomainClasses.Users.User");

                    b.HasDiscriminator().HasValue("ClinicManager");

                    b.HasData(
                        new
                        {
                            Id = 2,
                            Email = "manager@manager.com",
                            FullName = "مدیر کلینیک",
                            Password = "6ee4a469cd4e91053847f5d3fcb61dbcc91e8f0ef10be7748da4c4a1ba382d17",
                            Username = "manager"
                        });
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Users.Doctor", b =>
                {
                    b.HasBaseType("Clinic.Models.DomainClasses.Users.User");

                    b.Property<short>("Age")
                        .HasColumnType("smallint");

                    b.Property<string>("Articles")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("Biography")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("Books")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicalNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("NationalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(11)")
                        .HasMaxLength(11);

                    b.Property<string>("ProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.Property<string>("Specialty")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Doctor");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Users.Patient", b =>
                {
                    b.HasBaseType("Clinic.Models.DomainClasses.Users.User");

                    b.Property<string>("ActivationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(400)")
                        .HasMaxLength(400);

                    b.Property<short>("Age")
                        .HasColumnName("Patient_Age")
                        .HasColumnType("smallint");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnName("Patient_Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsValidated")
                        .HasColumnType("bit");

                    b.Property<string>("NationalCode")
                        .IsRequired()
                        .HasColumnName("Patient_NationalCode")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnName("Patient_PhoneNumber")
                        .HasColumnType("nvarchar(11)")
                        .HasMaxLength(11);

                    b.Property<string>("ProfilePic")
                        .HasColumnName("Patient_ProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Patient");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Users.Pharmacy", b =>
                {
                    b.HasBaseType("Clinic.Models.DomainClasses.Users.User");

                    b.HasDiscriminator().HasValue("Pharmacy");

                    b.HasData(
                        new
                        {
                            Id = 3,
                            Email = "pharmacy@pharmacy.com",
                            FullName = "مسئول داروخانه",
                            Password = "20b04a4f018b89f369ce009213afc6369f59fe1c903c0770732ec7402163c358",
                            Username = "pharmacy"
                        });
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Users.SiteAdmin", b =>
                {
                    b.HasBaseType("Clinic.Models.DomainClasses.Users.User");

                    b.HasDiscriminator().HasValue("SiteAdmin");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@admin.com",
                            FullName = "مدیر سایت",
                            Password = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Drug", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Users.Pharmacy", "Pharmacy")
                        .WithMany("Drugs")
                        .HasForeignKey("PharmacyId");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Prescription", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Appointment.Visit", "Visit")
                        .WithOne("Prescription")
                        .HasForeignKey("Clinic.Models.DomainClasses.Appointment.Prescription", "VisitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.PrescriptionDrug", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Appointment.Drug", "Drug")
                        .WithMany("PrescriptionDrugs")
                        .HasForeignKey("DrugId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Clinic.Models.DomainClasses.Appointment.Prescription", "Prescription")
                        .WithMany("PrescriptionDrugs")
                        .HasForeignKey("PrescriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Report", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Appointment.Visit", "Visit")
                        .WithOne("Report")
                        .HasForeignKey("Clinic.Models.DomainClasses.Appointment.Report", "VisitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Reservation", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Users.Doctor", "Doctor")
                        .WithMany("Reservations")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Clinic.Models.DomainClasses.Users.Patient", "Patient")
                        .WithMany("Reservations")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.Visit", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Appointment.InsuranceProvider", "InsuranceProvider")
                        .WithMany("Visits")
                        .HasForeignKey("InsuranceProviderId");

                    b.HasOne("Clinic.Models.DomainClasses.Appointment.Reservation", "Reservation")
                        .WithOne("Visit")
                        .HasForeignKey("Clinic.Models.DomainClasses.Appointment.Visit", "ReservationPatientId", "ReservationDoctorId", "ReservationReserveDate")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.Appointment.WeekDay", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Users.Doctor", "Doctor")
                        .WithMany("WeekDays")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.Comment", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.NewsPage.News", "News")
                        .WithMany("Comments")
                        .HasForeignKey("NewsId");

                    b.HasOne("Clinic.Models.DomainClasses.Users.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.News", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.Users.SiteAdmin", "SiteAdmin")
                        .WithMany("News")
                        .HasForeignKey("SiteAdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.NewsTag", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.NewsPage.News", "News")
                        .WithMany("NewsTags")
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Clinic.Models.DomainClasses.NewsPage.Tag", "Tag")
                        .WithMany("NewsTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Clinic.Models.DomainClasses.NewsPage.Reply", b =>
                {
                    b.HasOne("Clinic.Models.DomainClasses.NewsPage.Comment", "Comment")
                        .WithMany("Replies")
                        .HasForeignKey("CommentId");

                    b.HasOne("Clinic.Models.DomainClasses.Users.User", "User")
                        .WithMany("Replies")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
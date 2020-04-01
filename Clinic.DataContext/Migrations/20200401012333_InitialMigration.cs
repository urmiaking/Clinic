using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Clinic.DataContext.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    Host = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceProviders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsuranceName = table.Column<string>(nullable: true),
                    Discount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagerMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Seen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Username = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: false),
                    UserType = table.Column<string>(nullable: false),
                    ResetPasswordCode = table.Column<string>(nullable: true),
                    MedicalNumber = table.Column<string>(maxLength: 10, nullable: true),
                    NationalCode = table.Column<string>(maxLength: 10, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 11, nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    Age = table.Column<short>(nullable: true),
                    Score = table.Column<float>(nullable: true),
                    Specialty = table.Column<string>(nullable: true),
                    ProfilePic = table.Column<string>(nullable: true),
                    Biography = table.Column<string>(maxLength: 1000, nullable: true),
                    Articles = table.Column<string>(maxLength: 1000, nullable: true),
                    Books = table.Column<string>(maxLength: 1000, nullable: true),
                    Patient_NationalCode = table.Column<string>(maxLength: 10, nullable: true),
                    Patient_PhoneNumber = table.Column<string>(maxLength: 11, nullable: true),
                    Patient_Gender = table.Column<string>(nullable: true),
                    Patient_Age = table.Column<short>(nullable: true),
                    Address = table.Column<string>(maxLength: 400, nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Patient_ProfilePic = table.Column<string>(nullable: true),
                    ActivationCode = table.Column<string>(nullable: true),
                    IsValidated = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drugs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    Instruction = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false),
                    Cost = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    PharmacyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drugs_Users_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    ImageName = table.Column<string>(nullable: true),
                    VisitCount = table.Column<int>(nullable: false),
                    SiteAdminId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Users_SiteAdminId",
                        column: x => x.SiteAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    PatientId = table.Column<int>(nullable: false),
                    DoctorId = table.Column<int>(nullable: false),
                    ReserveDate = table.Column<DateTime>(nullable: false),
                    ReserveStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => new { x.PatientId, x.DoctorId, x.ReserveDate });
                    table.ForeignKey(
                        name: "FK_Reservations_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WeekDays",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayName = table.Column<string>(nullable: true),
                    EightTen = table.Column<string>(nullable: true),
                    TenTwelve = table.Column<string>(nullable: true),
                    TwelveFourteen = table.Column<string>(nullable: true),
                    FourteenSixteen = table.Column<string>(nullable: true),
                    DoctorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekDays_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    NewsId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsTags",
                columns: table => new
                {
                    NewsId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTags", x => new { x.NewsId, x.TagId });
                    table.ForeignKey(
                        name: "FK_NewsTags_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CauseOfPatientReferral = table.Column<string>(nullable: true),
                    DoctorAssessment = table.Column<string>(nullable: true),
                    DoctorNote = table.Column<string>(nullable: true),
                    GivenScore = table.Column<float>(nullable: true),
                    InsuranceProviderId = table.Column<int>(nullable: true),
                    ReservationDoctorId = table.Column<int>(nullable: false),
                    ReservationPatientId = table.Column<int>(nullable: false),
                    ReservationReserveDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_InsuranceProviders_InsuranceProviderId",
                        column: x => x.InsuranceProviderId,
                        principalTable: "InsuranceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visits_Reservations_ReservationPatientId_ReservationDoctorId_ReservationReserveDate",
                        columns: x => new { x.ReservationPatientId, x.ReservationDoctorId, x.ReservationReserveDate },
                        principalTable: "Reservations",
                        principalColumns: new[] { "PatientId", "DoctorId", "ReserveDate" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    CommentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replies_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    PaymentMethod = table.Column<string>(nullable: true),
                    VisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ComplainDate = table.Column<DateTime>(nullable: false),
                    Response = table.Column<string>(nullable: true),
                    VisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionDrugs",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(nullable: false),
                    DrugId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    IsBought = table.Column<bool>(nullable: false),
                    IsWantToBuy = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionDrugs", x => new { x.DrugId, x.PrescriptionId });
                    table.ForeignKey(
                        name: "FK_PrescriptionDrugs_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescriptionDrugs_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_NewsId",
                table: "Comments",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Drugs_PharmacyId",
                table: "Drugs",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_News_SiteAdminId",
                table: "News",
                column: "SiteAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsTags_TagId",
                table: "NewsTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionDrugs_PrescriptionId",
                table: "PrescriptionDrugs",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_VisitId",
                table: "Prescriptions",
                column: "VisitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_CommentId",
                table: "Replies",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_UserId",
                table: "Replies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_VisitId",
                table: "Reports",
                column: "VisitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DoctorId",
                table: "Reservations",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_InsuranceProviderId",
                table: "Visits",
                column: "InsuranceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ReservationPatientId_ReservationDoctorId_ReservationReserveDate",
                table: "Visits",
                columns: new[] { "ReservationPatientId", "ReservationDoctorId", "ReservationReserveDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeekDays_DoctorId",
                table: "WeekDays",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrations");

            migrationBuilder.DropTable(
                name: "ManagerMessages");

            migrationBuilder.DropTable(
                name: "NewsTags");

            migrationBuilder.DropTable(
                name: "PrescriptionDrugs");

            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "WeekDays");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Drugs");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "InsuranceProviders");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

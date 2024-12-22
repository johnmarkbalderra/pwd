using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWD_DSWD_SPC.Migrations
{
    public partial class _1stmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisabilityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    suffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CivilStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherMiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherMiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuardianMiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfDisability = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CauseOfDisability = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNoAndStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barangay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Municipality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandlineNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EducationalAttainment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusOfEmployment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryOfEmployment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfEmployment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationAffiliated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficeTelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SSSNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSISNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagIBIGNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PSNNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhilHealthNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccomplishByLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccomplishByFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccomplishByMiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisapprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminCredential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminCredential", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QrCodes",
                columns: table => new
                {
                    QrCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstablishmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfQRCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QrCodeBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCodes", x => x.QrCodeId);
                });

            migrationBuilder.CreateTable(
                name: "CommodityTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemainingDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstablishmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_CommodityTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicineTransactionLedgers",
                columns: table => new
                {
                    LedgerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineTransactionLedgers", x => x.LedgerId);
                    table.ForeignKey(
                        name: "FK_MedicineTransactionLedgers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Establishment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Acknowledged = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Reports_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirement1 = table.Column<bool>(type: "bit", nullable: false),
                    Requirement2 = table.Column<bool>(type: "bit", nullable: false),
                    Requirement3 = table.Column<bool>(type: "bit", nullable: false),
                    Requirement4 = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Status_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredential_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommodityItems",
                columns: table => new
                {
                    CommodityItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityItems", x => x.CommodityItemId);
                    table.ForeignKey(
                        name: "FK_CommodityItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommodityItems_CommodityTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "CommodityTransactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicineTransactions",
                columns: table => new
                {
                    MedTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrescribedQuantity = table.Column<int>(type: "int", nullable: false),
                    PurchasedQuantity = table.Column<int>(type: "int", nullable: false),
                    RemainingBalance = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePurchased = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttendingPhysician = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PTRNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstablishmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LedgerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineTransactions", x => x.MedTransactionId);
                    table.ForeignKey(
                        name: "FK_MedicineTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineTransactions_MedicineTransactionLedgers_LedgerId",
                        column: x => x.LedgerId,
                        principalTable: "MedicineTransactionLedgers",
                        principalColumn: "LedgerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommodityItems_AccountId",
                table: "CommodityItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityItems_TransactionId",
                table: "CommodityItems",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTransactions_AccountId",
                table: "CommodityTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineTransactionLedgers_AccountId",
                table: "MedicineTransactionLedgers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineTransactions_AccountId",
                table: "MedicineTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineTransactions_LedgerId",
                table: "MedicineTransactions",
                column: "LedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AccountId",
                table: "Reports",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_AccountId",
                table: "Status",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCredential_AccountId",
                table: "UserCredential",
                column: "AccountId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminCredential");

            migrationBuilder.DropTable(
                name: "CommodityItems");

            migrationBuilder.DropTable(
                name: "MedicineTransactions");

            migrationBuilder.DropTable(
                name: "QrCodes");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "UserCredential");

            migrationBuilder.DropTable(
                name: "CommodityTransactions");

            migrationBuilder.DropTable(
                name: "MedicineTransactionLedgers");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}

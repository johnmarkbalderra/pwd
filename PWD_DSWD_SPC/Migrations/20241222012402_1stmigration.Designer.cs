﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PWD_DSWD_SPC.Data;

#nullable disable

namespace PWD_DSWD_SPC.Migrations
{
    [DbContext(typeof(RegisterDbContext))]
    [Migration("20241222012402_1stmigration")]
    partial class _1stmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AD")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccomplishByFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccomplishByLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccomplishByMiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApplicantType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Barangay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryOfEmployment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CauseOfDisability")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CivilStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactPerson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisabilityNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DisapprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EducationalAttainment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FatherFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FatherLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FatherMiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GSISNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GuardianFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GuardianLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GuardianMiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HouseNoAndStreet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LandlineNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MobileNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MotherFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MotherLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MotherMiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Municipality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Occupation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OfficeAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OfficeTelNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationAffiliated")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OtherOccupation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PSNNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PagIBIGNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhilHealthNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReferenceNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SSSNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusOfEmployment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeOfDisability")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeOfEmployment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ValidUntil")
                        .HasColumnType("datetime2");

                    b.Property<string>("suffix")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.AdminCredentials", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("AdminCredential");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.ApprovalStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("Requirement1")
                        .HasColumnType("bit");

                    b.Property<bool>("Requirement2")
                        .HasColumnType("bit");

                    b.Property<bool>("Requirement3")
                        .HasColumnType("bit");

                    b.Property<bool>("Requirement4")
                        .HasColumnType("bit");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("Status");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.CommodityItem", b =>
                {
                    b.Property<Guid>("CommodityItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DiscountedPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CommodityItemId");

                    b.HasIndex("AccountId");

                    b.HasIndex("TransactionId");

                    b.ToTable("CommodityItems");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.CommodityTransaction", b =>
                {
                    b.Property<Guid>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BranchName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EstablishmentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("RemainingDiscount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Signature")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TransactionId");

                    b.HasIndex("AccountId");

                    b.ToTable("CommodityTransactions");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Medicine+MedicineTransaction", b =>
                {
                    b.Property<Guid>("MedTransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AttendingPhysician")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DatePurchased")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DiscountedPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("EstablishmentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("LedgerId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MedicineName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PTRNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PrescribedQuantity")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PurchasedQuantity")
                        .HasColumnType("int");

                    b.Property<int>("RemainingBalance")
                        .HasColumnType("int");

                    b.Property<string>("Signature")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("MedTransactionId");

                    b.HasIndex("AccountId");

                    b.HasIndex("LedgerId");

                    b.ToTable("MedicineTransactions");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Medicine+MedicineTransactionLedger", b =>
                {
                    b.Property<Guid>("LedgerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LedgerId");

                    b.HasIndex("AccountId");

                    b.ToTable("MedicineTransactionLedgers");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.QrCode", b =>
                {
                    b.Property<Guid>("QrCodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EstablishmentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QrCodeBase64")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeOfQRCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QrCodeId");

                    b.ToTable("QrCodes");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Report", b =>
                {
                    b.Property<int>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReportId"), 1L, 1);

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Acknowledged")
                        .HasColumnType("bit");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Establishment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProblemDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReportId");

                    b.HasIndex("AccountId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.UserCredentials", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("UserCredential");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.ApprovalStatus", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", "Accounts")
                        .WithOne("Status")
                        .HasForeignKey("PWD_DSWD_SPC.Models.Registered.ApprovalStatus", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.CommodityItem", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", null)
                        .WithMany("CommodityItems")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PWD_DSWD_SPC.Models.Registered.CommodityTransaction", "CommodityTransaction")
                        .WithMany("Items")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CommodityTransaction");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.CommodityTransaction", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", "Account")
                        .WithMany("CommodityTransactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Medicine+MedicineTransaction", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", "Account")
                        .WithMany("MedicineTransactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Medicine+MedicineTransactionLedger", "MedicineTransactionLedger")
                        .WithMany("Transactions")
                        .HasForeignKey("LedgerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("MedicineTransactionLedger");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Medicine+MedicineTransactionLedger", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", "Account")
                        .WithMany("MedicineTransactionLedgers")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Report", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", "Accounts")
                        .WithMany("Reports")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.UserCredentials", b =>
                {
                    b.HasOne("PWD_DSWD_SPC.Models.Registered.Account", "Accounts")
                        .WithOne("UserCredential")
                        .HasForeignKey("PWD_DSWD_SPC.Models.Registered.UserCredentials", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Account", b =>
                {
                    b.Navigation("CommodityItems");

                    b.Navigation("CommodityTransactions");

                    b.Navigation("MedicineTransactionLedgers");

                    b.Navigation("MedicineTransactions");

                    b.Navigation("Reports");

                    b.Navigation("Status")
                        .IsRequired();

                    b.Navigation("UserCredential")
                        .IsRequired();
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.CommodityTransaction", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("PWD_DSWD_SPC.Models.Registered.Medicine+MedicineTransactionLedger", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
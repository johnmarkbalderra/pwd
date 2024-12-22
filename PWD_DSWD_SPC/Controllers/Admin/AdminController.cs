using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWD_DSWD_SPC.Data;
using PWD_DSWD_SPC.Models.Registered; // Include your QrCode model
using QRCoder;
using SkiaSharp;
using System.Security.Principal;
using System.Text.Json;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace PWD_DSWD_SPC.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly RegisterDbContext _registerDbContext;
        private readonly ILogger<AdminController> _logger;
        private readonly IEmailService _emailService;

        // Base URLs for QR Code generation
        private readonly string _baseCommoditiesUrl = "https://pwdhealthservices.azurewebsites.net/User/Commodities"; // Update with your actual URL
        private readonly string _baseMedicineUrl = "https://pwdhealthservices.azurewebsites.net/User/Medicine"; // Update with your actual URL

        // Constructor with dependency injection
        public AdminController(RegisterDbContext registerDbContext, ILogger<AdminController> logger, IEmailService emailService)
        {
            _registerDbContext = registerDbContext ?? throw new ArgumentNullException(nameof(registerDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService;
        }


        // Admin Dashboard View
        public IActionResult Admin()
        {
            // Count the number of approved applicants
            int totalApprovedApplicants = _registerDbContext.Accounts
                .Where(a => a.Status.Status == "Approved")
                .Count();

            // Count the number of pending applicants
            int totalPendingApplicants = _registerDbContext.Accounts
                .Where(a => a.Status.Status == "Pending") // Adjust if needed
                .Count();

            // Count the number of archived applicants (Disapproved, Deceased, Change of Residency, Expired.)
            int totalArchivedApplicants = _registerDbContext.Accounts
                .Where(a => a.Status.Status == "Disapproved" ||
                             a.Status.Status == "Deceased" ||
                             a.Status.Status == "Expired" ||
                             a.Status.Status == "Change of Residency")
                .Count();

            // Pass the count to the view using ViewBag
            ViewBag.TotalApprovedApplicants = totalApprovedApplicants;
            ViewBag.TotalPendingApplicants = totalPendingApplicants;
            ViewBag.TotalArchivedApplicants = totalArchivedApplicants;


            // Calculate Total Accredited Establishments (distinct QrCodeId count)
            int totalAccreditedEstablishments = _registerDbContext.QrCodes
                .Select(q => q.QrCodeId)
                .Distinct()
                .Count();

            ViewBag.TotalAccreditedEstablishments = totalAccreditedEstablishments;


            // Fetch total visits based on date of transaction for Medicine and Commodity
            // Get the date 30 days ago from today
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow.Date; // The current date (end of the range)

            var totalVisitsQuery = _registerDbContext.CommodityTransactions
                .Where(c => c.CreatedDate.Date >= startDate && c.CreatedDate.Date <= endDate) // Filter by today's date for example
                .GroupBy(c => new { c.EstablishmentName, c.BranchName })
                .Select(g => new
                {
                    EstablishmentName = g.Key.EstablishmentName,
                    BranchName = g.Key.BranchName,
                    TotalVisits = g.Count() // Count the number of transactions (visits)
                })
                .ToList();

            // Similarly, if you need to include Medicine Transactions, you can fetch and count those
            // Query for Medicine Transaction Ledgers (count visits based on Ledger and date of MedicineTransaction)
            var totalMedicineVisits = _registerDbContext.MedicineTransactionLedgers
                .Where(ledger => ledger.Transactions
                    .Any(medTransaction => medTransaction.DatePurchased.Date >= startDate && medTransaction.DatePurchased.Date <= endDate)) // Filter based on date
                .GroupBy(ledger => new {
                    EstablishmentName = ledger.Transactions.First().EstablishmentName, // Get EstablishmentName from the first transaction
                    BranchName = ledger.Transactions.First().Branch // Get Branch from the first transaction
                })
                .Select(g => new
                {
                    EstablishmentName = g.Key.EstablishmentName,
                    BranchName = g.Key.BranchName,
                    TotalVisits = g.Count() // Count distinct ledger entries (representing a visit)
                })
                .ToList();

            // Combine the results from Medicine and Commodity transactions
            var combinedVisits = totalVisitsQuery
                .Union(totalMedicineVisits)
                .GroupBy(v => new { v.EstablishmentName, v.BranchName })
                .Select(g => new
                {
                    EstablishmentName = g.Key.EstablishmentName,
                    BranchName = g.Key.BranchName,
                    TotalVisits = g.Sum(v => v.TotalVisits) // Sum total visits from both categories
                })
                .ToList();

            // Pass the total visits data to the view
            ViewBag.TotalVisits = combinedVisits;



            // PWD Per Barangay function - include only approved accounts
            var pwdCountsPerBarangay = _registerDbContext.Accounts
                .Where(a => a.Status.IsApproved) // Only consider approved accounts
                .GroupBy(a => a.Barangay)
                .Select(g => new
                {
                    Barangay = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // List of all barangays
            var allBarangays = new List<string> {
             "I-A (Sambat)", "I-B (City+Riverside)", "I-C (Bagong Bayan)",
             "II-A (Triangulo)", "II-B (Guadalupe)", "II-C (Unson)",
             "II-D (Bulante)", "II-E (San Anton)", "II-F (Villa Rey)",
             "III-A (Hermanos Belen)", "III-B", "III-C (Labak/De Roma)",
             "III-D (Villongco)", "III-E", "III-F (Balagtas)",
             "IV-A", "IV-B", "IV-C",
             "V-A", "V-B", "V-C", "V-D",
             "VI-A (Mavenida)", "VI-B", "VI-C (Bagong Pook)", "VI-D (Lparkers)",
             "VI-E (YMCA)", "VII-A (P.Alcantara)", "VII-B",
             "VII-C", "VII-D", "VII-E",
             "Atisan", "Bautista", "Concepcion (Bunot)", "Del Remedio (Wawa)",
             "Dolores", "San Antonio 1 (Balanga)", "San Antonio 2 (Sapa)",
             "San Bartolome (Matang-ag)", "San Buenaventura (Palakpakin)",
             "San Crispin (Lumbangan)", "San Cristobal", "San Diego (Tiim)",
             "San Francisco (Calihan)", "San Gabriel (Butucan)", "San Gregorio",
             "San Ignacio", "San Isidro (Balagbag)", "San Joaquin", "San Jose (Malamig)",
             "San Juan", "San Lorenzo (Saluyan)", "San Lucas 1 (Malinaw)",
             "San Lucas 2", "San Marcos (Tikew)", "San Mateo", "San Miguel",
             "San Nicolas", "San Pedro", "San Rafael (Magampon)", "San Roque (Buluburan)",
             "San Vicente", "Santa Ana", "Santa Catalina (Sandig)",
             "Santa Cruz (Putol)", "Santa Elena", "Santa Filomena (Banlagin)",
             "Santa Isabel", "Santa Maria", "Santa Maria Magdalena (Boe)",
             "Santa Monica", "Santa Veronica (Bae)", "Santiago I (Bulaho)",
             "Santiago II", "Santisimo Rosario", "Santo Angel (Ilog)",
             "Santo Cristo", "Santo Niño (Arsum)", "Soledad (Macopa)"
         };

            // Create a dictionary to store counts
            var barangayCounts = allBarangays.ToDictionary(b => b, b => 0);

            // Update counts from the database
            foreach (var item in pwdCountsPerBarangay)
            {
                barangayCounts[item.Barangay] = item.Count;
            }

            // Sort the barangays by count in descending order
            var sortedBarangays = barangayCounts
                .OrderByDescending(b => b.Value)
                .Select(b => new
                {
                    Barangay = b.Key,
                    Count = b.Value
                })
                .ToList();

            ViewBag.PwdCountsPerBarangay = sortedBarangays;


            // Fetch total reports and their details
            var reports = _registerDbContext.Reports
                .Select(r => new
                {
                    FullName = r.Accounts.FirstName + " " + r.Accounts.LastName,
                    r.Accounts.TypeOfDisability,
                    Status = r.Acknowledged ? "Acknowledged" : "Pending"
                })
                .ToList();

            ViewBag.TotalReports = reports.Count;
            ViewBag.Reports = reports;


            return View();
        }

        [HttpGet]
        public IActionResult GenerateApprovedApplicantsExcel()
        {
            var approvedApplicants = _registerDbContext.Accounts
                .Where(a => a.Status.IsApproved)
                .Select(a => new
                {
                    a.ApplicantType,
                    a.DisabilityNumber,
                    a.CreatedAt,
                    a.LastName,
                    a.FirstName,
                    a.MiddleName,
                    a.suffix,
                    a.DateOfBirth,
                    a.Gender,
                    a.CivilStatus,
                    a.FatherLastName,
                    a.FatherFirstName,
                    a.FatherMiddleName,
                    a.MotherLastName,
                    a.MotherFirstName,
                    a.MotherMiddleName,
                    a.GuardianLastName,
                    a.GuardianFirstName,
                    a.GuardianMiddleName,
                    a.TypeOfDisability,
                    a.CauseOfDisability,
                    a.HouseNoAndStreet,
                    a.Barangay,
                    a.Municipality,
                    a.Province,
                    a.Region,
                    a.LandlineNo,
                    a.MobileNo,
                    a.EmailAddress,
                    a.EducationalAttainment,
                    a.StatusOfEmployment,
                    a.CategoryOfEmployment,
                    a.TypeOfEmployment,
                    a.Occupation,
                    a.OtherOccupation,
                    a.OrganizationAffiliated,
                    a.ContactPerson,
                    a.OfficeAddress,
                    a.OfficeTelNo,
                    a.SSSNo,
                    a.GSISNo,
                    a.PagIBIGNo,
                    a.PSNNo,
                    a.PhilHealthNo,
                    a.AccomplishByLastName,
                    a.AccomplishByFirstName,
                    a.AccomplishByMiddleName
                })
                .ToList();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Approved Applicants");
            var currentRow = 1;

            // Set headers for the Excel file
            worksheet.Cell(currentRow, 1).Value = "Applicant Type";
            worksheet.Cell(currentRow, 2).Value = "Disability Number";
            worksheet.Cell(currentRow, 3).Value = "Created At";
            worksheet.Cell(currentRow, 4).Value = "Last Name";
            worksheet.Cell(currentRow, 5).Value = "First Name";
            worksheet.Cell(currentRow, 6).Value = "Middle Name";
            worksheet.Cell(currentRow, 7).Value = "Suffix";
            worksheet.Cell(currentRow, 8).Value = "Date of Birth";
            worksheet.Cell(currentRow, 9).Value = "Gender";
            worksheet.Cell(currentRow, 10).Value = "Civil Status";
            worksheet.Cell(currentRow, 11).Value = "Father Last Name";
            worksheet.Cell(currentRow, 12).Value = "Father First Name";
            worksheet.Cell(currentRow, 13).Value = "Father Middle Name";
            worksheet.Cell(currentRow, 14).Value = "Mother Last Name";
            worksheet.Cell(currentRow, 15).Value = "Mother First Name";
            worksheet.Cell(currentRow, 16).Value = "Mother Middle Name";
            worksheet.Cell(currentRow, 17).Value = "Guardian Last Name";
            worksheet.Cell(currentRow, 18).Value = "Guardian First Name";
            worksheet.Cell(currentRow, 19).Value = "Guardian Middle Name";
            worksheet.Cell(currentRow, 20).Value = "Type of Disability";
            worksheet.Cell(currentRow, 21).Value = "Cause of Disability";
            worksheet.Cell(currentRow, 22).Value = "House No and Street";
            worksheet.Cell(currentRow, 23).Value = "Barangay";
            worksheet.Cell(currentRow, 24).Value = "Municipality";
            worksheet.Cell(currentRow, 25).Value = "Province";
            worksheet.Cell(currentRow, 26).Value = "Region";
            worksheet.Cell(currentRow, 27).Value = "Landline No";
            worksheet.Cell(currentRow, 28).Value = "Mobile No";
            worksheet.Cell(currentRow, 29).Value = "Email Address";
            worksheet.Cell(currentRow, 30).Value = "Educational Attainment";
            worksheet.Cell(currentRow, 31).Value = "Status of Employment";
            worksheet.Cell(currentRow, 32).Value = "Category of Employment";
            worksheet.Cell(currentRow, 33).Value = "Type of Employment";
            worksheet.Cell(currentRow, 34).Value = "Occupation";
            worksheet.Cell(currentRow, 35).Value = "Other Occupation";
            worksheet.Cell(currentRow, 36).Value = "Organization Affiliated";
            worksheet.Cell(currentRow, 37).Value = "Contact Person";
            worksheet.Cell(currentRow, 38).Value = "Office Address";
            worksheet.Cell(currentRow, 39).Value = "Office Tel No";
            worksheet.Cell(currentRow, 40).Value = "SSS No";
            worksheet.Cell(currentRow, 41).Value = "GSIS No";
            worksheet.Cell(currentRow, 42).Value = "Pag-IBIG No";
            worksheet.Cell(currentRow, 43).Value = "PSN No";
            worksheet.Cell(currentRow, 44).Value = "PhilHealth No";
            worksheet.Cell(currentRow, 45).Value = "Accomplished By Last Name";
            worksheet.Cell(currentRow, 46).Value = "Accomplished By First Name";
            worksheet.Cell(currentRow, 47).Value = "Accomplished By Middle Name";

            // Populate data rows
            foreach (var applicant in approvedApplicants)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = applicant.ApplicantType;
                worksheet.Cell(currentRow, 2).Value = applicant.DisabilityNumber;
                worksheet.Cell(currentRow, 3).Value = applicant.CreatedAt;
                worksheet.Cell(currentRow, 4).Value = applicant.LastName;
                worksheet.Cell(currentRow, 5).Value = applicant.FirstName;
                worksheet.Cell(currentRow, 6).Value = applicant.MiddleName;
                worksheet.Cell(currentRow, 7).Value = applicant.suffix;
                worksheet.Cell(currentRow, 8).Value = applicant.DateOfBirth;
                worksheet.Cell(currentRow, 9).Value = applicant.Gender;
                worksheet.Cell(currentRow, 10).Value = applicant.CivilStatus;
                worksheet.Cell(currentRow, 11).Value = applicant.FatherLastName;
                worksheet.Cell(currentRow, 12).Value = applicant.FatherFirstName;
                worksheet.Cell(currentRow, 13).Value = applicant.FatherMiddleName;
                worksheet.Cell(currentRow, 14).Value = applicant.MotherLastName;
                worksheet.Cell(currentRow, 15).Value = applicant.MotherFirstName;
                worksheet.Cell(currentRow, 16).Value = applicant.MotherMiddleName;
                worksheet.Cell(currentRow, 17).Value = applicant.GuardianLastName;
                worksheet.Cell(currentRow, 18).Value = applicant.GuardianFirstName;
                worksheet.Cell(currentRow, 19).Value = applicant.GuardianMiddleName;
                worksheet.Cell(currentRow, 20).Value = applicant.TypeOfDisability;
                worksheet.Cell(currentRow, 21).Value = applicant.CauseOfDisability;
                worksheet.Cell(currentRow, 22).Value = applicant.HouseNoAndStreet;
                worksheet.Cell(currentRow, 23).Value = applicant.Barangay;
                worksheet.Cell(currentRow, 24).Value = applicant.Municipality;
                worksheet.Cell(currentRow, 25).Value = applicant.Province;
                worksheet.Cell(currentRow, 26).Value = applicant.Region;
                worksheet.Cell(currentRow, 27).Value = applicant.LandlineNo;
                worksheet.Cell(currentRow, 28).Value = applicant.MobileNo;
                worksheet.Cell(currentRow, 29).Value = applicant.EmailAddress;
                worksheet.Cell(currentRow, 30).Value = applicant.EducationalAttainment;
                worksheet.Cell(currentRow, 31).Value = applicant.StatusOfEmployment;
                worksheet.Cell(currentRow, 32).Value = applicant.CategoryOfEmployment;
                worksheet.Cell(currentRow, 33).Value = applicant.TypeOfEmployment;
                worksheet.Cell(currentRow, 34).Value = applicant.Occupation;
                worksheet.Cell(currentRow, 35).Value = applicant.OtherOccupation;
                worksheet.Cell(currentRow, 36).Value = applicant.OrganizationAffiliated;
                worksheet.Cell(currentRow, 37).Value = applicant.ContactPerson;
                worksheet.Cell(currentRow, 38).Value = applicant.OfficeAddress;
                worksheet.Cell(currentRow, 39).Value = applicant.OfficeTelNo;
                worksheet.Cell(currentRow, 40).Value = applicant.SSSNo;
                worksheet.Cell(currentRow, 41).Value = applicant.GSISNo;
                worksheet.Cell(currentRow, 42).Value = applicant.PagIBIGNo;
                worksheet.Cell(currentRow, 43).Value = applicant.PSNNo;
                worksheet.Cell(currentRow, 44).Value = applicant.PhilHealthNo;
                worksheet.Cell(currentRow, 45).Value = applicant.AccomplishByLastName;
                worksheet.Cell(currentRow, 46).Value = applicant.AccomplishByFirstName;
                worksheet.Cell(currentRow, 47).Value = applicant.AccomplishByMiddleName;
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream); // Save workbook to stream
            stream.Position = 0; // Reset position after saving

            var fileName = $"Approved_Applicants_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            const string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream.ToArray(), mimeType, fileName); // Return the file without using 'using' for the stream
        }




        public async Task<IActionResult> ListofAllAccounts()
        {
            var accounts = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .Include(a => a.UserCredential)
                .Where(a => a.Status.IsApproved &&
                            a.Status.Status != "Deceased" &&
                            a.Status.Status != "Change of Residency" &&
                            a.Status.Status != "Disapproved")
                .ToListAsync();

            var expiredAccounts = accounts.Where(a => a.IsExpired).ToList();

            foreach (var expiredAccount in expiredAccounts)
            {
                expiredAccount.Status.Status = "Expired"; // Use "Expired"
                _logger.LogInformation($"Expired account: {expiredAccount.LastName}, {expiredAccount.FirstName} (ID: {expiredAccount.Id})");

                // Send expiration email notification
                try
                {
                    await _emailService.SendExpirationEmailAsync(expiredAccount.EmailAddress);
                    _logger.LogInformation($"Expiration email sent to {expiredAccount.EmailAddress}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send expiration email to {expiredAccount.EmailAddress}.");
                }
            }

            if (expiredAccounts.Any())
            {
                await _registerDbContext.SaveChangesAsync();
            }

            var activeAccounts = accounts.Where(a => !a.IsExpired).ToList();

            _logger.LogInformation($"Fetched {activeAccounts.Count} active approved accounts.");
            return View(activeAccounts);
        }





        [HttpPost]
        public async Task<JsonResult> UpdateDetails(Guid id, [FromBody] Account updatedAccount)

        {

            var existingAccount = await _registerDbContext.Accounts.FindAsync(id);
            if (existingAccount == null)
            {
                return Json(new { success = false, message = "Account not found." });
            }

            // Update properties
            existingAccount.DisabilityNumber = updatedAccount.DisabilityNumber;
            existingAccount.FatherLastName = updatedAccount.FatherLastName;
            existingAccount.FatherFirstName = updatedAccount.FatherFirstName;
            existingAccount.MotherLastName = updatedAccount.MotherLastName;
            existingAccount.MotherFirstName = updatedAccount.MotherFirstName;
            existingAccount.GuardianLastName = updatedAccount.GuardianLastName;
            existingAccount.GuardianFirstName = updatedAccount.GuardianFirstName;
            existingAccount.TypeOfDisability = updatedAccount.TypeOfDisability;
            existingAccount.CauseOfDisability = updatedAccount.CauseOfDisability;
            existingAccount.HouseNoAndStreet = updatedAccount.HouseNoAndStreet;
            existingAccount.Barangay = updatedAccount.Barangay;
            existingAccount.Municipality = updatedAccount.Municipality;
            existingAccount.Region = updatedAccount.Region;
            existingAccount.LandlineNo = updatedAccount.LandlineNo;
            existingAccount.MobileNo = updatedAccount.MobileNo;
            existingAccount.EmailAddress = updatedAccount.EmailAddress;
            existingAccount.EducationalAttainment = updatedAccount.EducationalAttainment;
            existingAccount.StatusOfEmployment = updatedAccount.StatusOfEmployment;
            existingAccount.CategoryOfEmployment = updatedAccount.CategoryOfEmployment;
            existingAccount.TypeOfEmployment = updatedAccount.TypeOfEmployment;
            existingAccount.Occupation = updatedAccount.Occupation;
            existingAccount.OrganizationAffiliated = updatedAccount.OrganizationAffiliated;
            existingAccount.ContactPerson = updatedAccount.ContactPerson;
            existingAccount.OfficeAddress = updatedAccount.OfficeAddress;
            existingAccount.OfficeTelNo = updatedAccount.OfficeTelNo;
            existingAccount.SSSNo = updatedAccount.SSSNo;
            existingAccount.GSISNo = updatedAccount.GSISNo;
            existingAccount.PagIBIGNo = updatedAccount.PagIBIGNo;
            existingAccount.PSNNo = updatedAccount.PSNNo;
            existingAccount.PhilHealthNo = updatedAccount.PhilHealthNo;
            existingAccount.AccomplishByLastName = updatedAccount.AccomplishByLastName;
            existingAccount.AccomplishByFirstName = updatedAccount.AccomplishByFirstName;
            existingAccount.AccomplishByMiddleName = updatedAccount.AccomplishByMiddleName;

            try
            {
                await _registerDbContext.SaveChangesAsync();
                return Json(new { success = true, message = "Account details updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account details.");
                return Json(new { success = false, message = "An error occurred while updating the account." });
            }
        }


        // Archive Button
        [HttpPost]
        public async Task<IActionResult> Archive(Guid id, [FromBody] string reason)
        {
            var account = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account != null && !string.IsNullOrEmpty(reason))
            {
                // Update the Status based on the selected reason
                if (reason == "Deceased" || reason == "Change of Residency")
                {
                    account.Status.Status = reason; // Use existing status property
                }

                await _registerDbContext.SaveChangesAsync();

                // Redirect to the ArchivedAccounts action after successful archive
                return RedirectToAction("ArchivedAccounts", "Admin");
            }

            // Handle failure case, redirect to an error or the same page with a message
            return RedirectToAction("ListofAllAccounts", "Admin", new { error = "Unable to archive the account." });
        }
        //Archive Table
        public async Task<IActionResult> ArchivedAccounts()
        {
            var accounts = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .Include(a => a.UserCredential) // Optional: Include UserCredentials if needed
                .Where(a => a.Status.Status == "Disapproved" ||
                            a.Status.Status == "Deceased" ||
                            a.Status.Status == "Change of Residency" ||
                            a.Status.Status == "Expired") // Use "Expired"
                .ToListAsync();

            return View(accounts);
        }


        [HttpPost]
        public async Task<IActionResult> Restore(Guid id)
        {
            var account = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account != null)
            {
                if (account.Status.Status == "Disapproved" || account.Status.Status == "Deceased" || account.Status.Status == "Change of Residency")
                {
                    // Restore the account by setting the Status back to Approved
                    account.Status.Status = "Approved";
                    account.Status.IsApproved = true;

                    await _registerDbContext.SaveChangesAsync();

                    TempData["RestoreSuccess"] = "Account has been successfully restored.";
                    return RedirectToAction("ListofAllAccounts"); // Redirect to the active accounts list
                }

                if (account.Status.Status == "Expired")
                {
                    // Restore expired account with a 5-year validity extension
                    DateTime validUntil = DateTime.Now.AddYears(5);
                    account.ValidUntil = validUntil;
                    account.Status.Status = "Approved";
                    account.Status.IsApproved = true;

                    // Send renewal email notification
                    try
                    {
                        await _emailService.SendRenewalEmailAsync(account.EmailAddress);
                        _logger.LogInformation($"Renewal email sent to {account.EmailAddress}.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send renewal email to {account.EmailAddress}.");
                    }

                    await _registerDbContext.SaveChangesAsync();

                    TempData["RestoreSuccess"] = "Expired account has been successfully restored and renewed for 5 more years.";
                    return RedirectToAction("ListofAllAccounts"); // Redirect to the active accounts list
                }
            }

            // Handle failure case with an error message
            TempData["RestoreError"] = "Unable to restore the account. Please try again.";
            return RedirectToAction("ArchivedAccounts");
        }


        public ActionResult QR()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QR(QrCode model)
        {
            if (model == null || string.IsNullOrEmpty(model.EstablishmentName))
            {
                ModelState.AddModelError("EstablishmentName", "Establishment Name cannot be empty.");
                return View(model);
            }

            string qrContent;

            // Determine the QR content based on the type of QR code
            if (model.TypeOfQRCode == "Commodities")
            {
                qrContent = $"{_baseCommoditiesUrl}?establishment={model.EstablishmentName}&branch={model.Branch}";
            }
            else if (model.TypeOfQRCode == "Medicine")
            {
                qrContent = $"{_baseMedicineUrl}?establishment={model.EstablishmentName}&branch={model.Branch}";
            }
            else if (model.TypeOfQRCode == "Both")
            {
                var combinedContent = new
                {
                    CommoditiesUrl = $"{_baseCommoditiesUrl}?establishment={model.EstablishmentName}&branch={model.Branch}",
                    MedicineUrl = $"{_baseMedicineUrl}?establishment={model.EstablishmentName}&branch={model.Branch}",
                    Establishment = model.EstablishmentName,
                    Branch = model.Branch
                };

                qrContent = JsonSerializer.Serialize(combinedContent);
            }
            else
            {
                ModelState.AddModelError("", "Invalid QR Code Type.");
                return View(model);
            }

            // Generate QR Code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(7);
                    string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
                    string qrCodeUri = $"data:image/png;base64,{qrCodeBase64}";

                    // Save to database
                    var qrCodeEntity = new QrCode
                    {
                        EstablishmentName = model.EstablishmentName,
                        Branch = model.Branch,
                        TypeOfQRCode = model.TypeOfQRCode,
                        QrCodeBase64 = qrCodeBase64,
                        RegistrationUrl = qrContent
                    };

                    await _registerDbContext.QrCodes.AddAsync(qrCodeEntity);
                    await _registerDbContext.SaveChangesAsync();

                    ViewBag.QrCodeUri = qrCodeUri;
                }
            }

            return View(model);
        }

        public ActionResult Commodities()
        {
            // Fetch all users with their AccountId, Name, and PWD No.
            var users = _registerDbContext.Accounts
                .Select(u => new
                {
                    AccountId = u.Id, // AccountId as key
                    Name = $"{u.FirstName} {u.LastName}",
                    PwdNo = u.DisabilityNumber
                })
                .ToList();

            return View(users); // Pass user data to the view
        }

        [HttpGet]
        public IActionResult GetTransactionsForUser(Guid userId)
        {
            var transactions = _registerDbContext.CommodityTransactions
                .Where(t => t.AccountId == userId)
                .Select(t => new
                {
                    t.TransactionId,
                    CreatedDate = t.CreatedDate,
                    t.EstablishmentName,
                    TotalPrice = t.Items.Sum(i => i.TotalPrice),
                    DiscountedPrice = t.Items.Sum(i => i.DiscountedPrice),
                    t.RemainingDiscount
                }).ToList();

            return Json(transactions);
        }

        [HttpGet]
        public IActionResult GetItemsForTransaction(Guid transactionId)
        {
            var items = _registerDbContext.CommodityItems
                .Where(i => i.TransactionId == transactionId)
                .Select(i => new
                {
                    i.Description,
                    i.Quantity,
                    i.Price,
                    i.TotalPrice,
                    i.DiscountedPrice
                }).ToList();

            return Json(items);
        }



        public ActionResult Medicines()
        {
            var users = _registerDbContext.Accounts
                .Select(u => new
                {
                    AccountId = u.Id,
                    Name = $"{u.FirstName} {u.LastName}",
                    PwdNo = u.DisabilityNumber
                })
                .ToList();

            ViewBag.Users = users; // Store users in ViewBag
            return View();
        }

        [HttpGet]
        public JsonResult GetMedicineTransactions(Guid accountId)
        {
            var transactions = _registerDbContext.MedicineTransactions
                .Where(t => t.AccountId == accountId)
                .Select(t => new
                {
                    t.MedTransactionId,
                    t.DatePurchased,
                    EstablishmentName = t.EstablishmentName ?? "N/A", // Default to "N/A"
                    MedicineName = t.MedicineName ?? "N/A", // Default to "N/A"
                    AttendingPhysician = t.AttendingPhysician ?? "N/A", // Default to "N/A"
                    PTRNo = t.PTRNo ?? "N/A", // Default to "N/A"
                    t.PrescribedQuantity,
                    t.PurchasedQuantity,
                    t.RemainingBalance,
                    TotalPrice = t.TotalPrice != null ? t.TotalPrice : 0, // Default to 0
                    DiscountedPrice = t.DiscountedPrice != null ? t.DiscountedPrice : 0, // Default to 0
                    t.Branch // Make sure the Branch field is included
                })
                .ToList();

            return Json(transactions); // Ensure it's returning an array
        }





        public IActionResult Report(string searchTerm, string statusFilter)
        {
            var reportsQuery = _registerDbContext.Reports
                .Select(r => new
                {
                    r.ReportId,
                    r.AccountId,
                    r.ProblemDescription,
                    r.Establishment,
                    r.Branch,
                    r.CreatedDate,
                    FullName = r.Accounts.FirstName + " " + r.Accounts.LastName,
                    r.Accounts.TypeOfDisability,
                    r.Accounts.MobileNo,
                    r.Accounts.Barangay,
                    r.Accounts.EmailAddress,
                    r.Acknowledged
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                reportsQuery = reportsQuery.Where(r => r.FullName.ToLower().Contains(searchTerm.ToLower()) || r.ReportId.ToString().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Acknowledged")
                {
                    reportsQuery = reportsQuery.Where(r => r.Acknowledged);
                }
                else if (statusFilter == "Pending")
                {
                    reportsQuery = reportsQuery.Where(r => !r.Acknowledged);
                }
            }

            var reports = reportsQuery.ToList();

            return View(reports);
        }

        [HttpPost]
        public async Task<IActionResult> AcknowledgeReport(int id)
        {
            // Find the report by ID
            var report = _registerDbContext.Reports
                .Include(r => r.Accounts)
                .FirstOrDefault(r => r.ReportId == id);

            if (report == null)
            {
                return Json(new { success = false, message = "Report not found." });
            }

            // Update the acknowledgment status
            report.Acknowledged = true;

            // Save changes to the database
            _registerDbContext.SaveChanges();

            // Send acknowledgment email
            string subject = "Your Report Has Been Acknowledged";
            string message = $@"
            Hello {report.Accounts.FirstName},

            Your report regarding the establishment '{report.Establishment}' and branch '{report.Branch}' has been acknowledged. We have reviewed the details and will proceed with the necessary actions.

            Regarding the issue with the establishment, we have noted the concern and will address it as soon as possible. 

            Please submit any additional requirements between 8:00 AM and 5:00 PM at the DSWD office. If you have any questions or need further assistance, feel free to reach out.

            Thank you for your cooperation!

            Best regards,
            DSWD San Pablo City";

            await _emailService.SendEmailAsync(report.Accounts.EmailAddress, subject, message);

            return Json(new { success = true, message = "Report acknowledged and email sent." });
        }

        public ActionResult AddAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAccount(AdminCredentials adminCredentials)
        {
            // Validate input
            if (string.IsNullOrEmpty(adminCredentials.Username) || string.IsNullOrEmpty(adminCredentials.Password))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View();
            }

            // Username validation
            if (!IsValidUsername(adminCredentials.Username))
            {
                ViewBag.ErrorMessage = "Username must be a valid email or a non-email alphanumeric string.";
                return View();
            }

            // Password validation
            if (!IsValidPassword(adminCredentials.Password))
            {
                ViewBag.ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.";
                return View();
            }

            // Check if username already exists
            if (_registerDbContext.AdminCredential.Any(u => u.Username == adminCredentials.Username))
            {
                ViewBag.ErrorMessage = "Username already exists.";
                return View();
            }

            // Hash the password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminCredentials.Password);

            // Create and save the new admin account
            var newAdminAccount = new AdminCredentials
            {
                Username = adminCredentials.Username,
                Password = hashedPassword // Store the hashed password
            };

            _registerDbContext.AdminCredential.Add(newAdminAccount);
            _registerDbContext.SaveChanges();

            ViewBag.SuccessMessage = "Admin account successfully created.";
            return View();
        }

        // Username validation method
        private bool IsValidUsername(string username)
        {
            // Check if username is a valid email or a non-email alphanumeric string
            var emailRegex = new Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
            var alphanumericRegex = new Regex("^[a-zA-Z0-9]*$");

            return emailRegex.IsMatch(username) || alphanumericRegex.IsMatch(username);
        }

        // Password validation method
        private bool IsValidPassword(string password)
        {
            // Check for at least one uppercase letter, one number, and one special character
            var passwordRegex = new Regex("^(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\"{}|<>]).{8,}$");

            return passwordRegex.IsMatch(password);
        }

        // Dispose method to clean up resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _registerDbContext.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}

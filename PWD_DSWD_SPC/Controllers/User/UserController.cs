using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWD_DSWD_SPC.Data;
using PWD_DSWD_SPC.Models.Registered;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // To handle JSON objects
using System;
using System.Collections.Generic;
using BCrypt.Net; // Include this for BCrypt.Net.BCrypt
using Microsoft.Extensions.Logging; // For logging
using PWD_DSWD_SPC.Models;
using SkiaSharp;
using System.Text.Json;

using System.IO;
using DocumentFormat.OpenXml.InkML;
using static PWD_DSWD_SPC.Models.Registered.Medicine;


namespace PWD_DSWD_SPC.Controllers.User
{

    public class UserController : Controller
    {
        private readonly RegisterDbContext _registerDbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(RegisterDbContext registerDbContext, ILogger<UserController> logger)
        {
            _registerDbContext = registerDbContext;
            _logger = logger;
        }

        public IActionResult UserDash()
        {
            var userAccountId = GetUserAccountId(); // Get the logged-in user's AccountId
        
            if (userAccountId == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }
        
            // Convert current UTC time to Philippine Standard Time (UTC +8)
            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var philippineDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);
        
            // Check if today is Monday in Philippine time
            bool isMonday = philippineDateTime.DayOfWeek == DayOfWeek.Monday;
        
            // Get the start of the current week (Monday at 00:00:00) in Philippine time
            DateTime startOfWeek = philippineDateTime.Date.AddDays(-(int)philippineDateTime.DayOfWeek + (int)DayOfWeek.Monday);
        
            // Fetch the most recent commodity transaction for the user account
            var lastTransaction = _registerDbContext.CommodityTransactions
                .Where(t => t.AccountId == userAccountId)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefault();
            // Default balance
            decimal remainingBalance = 2500;
        
            if (lastTransaction != null)
            {
                // Check if the last transaction belongs to the current week
                if (lastTransaction.CreatedDate >= startOfWeek)
                {
                    remainingBalance = lastTransaction.RemainingDiscount;
                }
            }
        
            ViewBag.RemainingBalance = remainingBalance; // Pass the calculated balance to the view
        
            // Fetch distinct accredited establishments
            var accreditedEstablishments = _registerDbContext.QrCodes
                .GroupBy(q => new { q.EstablishmentName, q.Branch })
                .Select(g => g.First()) // Get the first entry of each group
                .ToList();
        
            return View(accreditedEstablishments);
        }


        public IActionResult UserProfile()
        {
            var userName = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userAccount = _registerDbContext.Accounts
                                                    .Include(a => a.Status)
                                                    .Include(a => a.UserCredential)
                                                    .FirstOrDefault(a => a.UserCredential.Username == userName);

                if (userAccount == null)
                {
                    return NotFound("User not found");
                }

                ViewBag.FullName = $"{userAccount.FirstName} {userAccount.LastName}";
                ViewBag.UserAccountId = userAccount.Id; // Set the UserAccountId
                ViewBag.PWDNo = userAccount.DisabilityNumber;
                ViewBag.Validity = userAccount.ValidUntil;
                ViewBag.DisabilityType = userAccount.TypeOfDisability;
                ViewBag.ContactNo = userAccount.MobileNo;
                ViewBag.Address = userAccount.Barangay;
                ViewBag.DateOfBirth = userAccount.DateOfBirth.ToString("MM/dd/yyyy");
                ViewBag.Sex = userAccount.Gender;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for {Username}", userName);
                return StatusCode(500, "An unexpected error occurred while fetching user profile.");
            }
        }

        public IActionResult AccountSetting()
        {
            return View();
        }


        // Change Password function
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userName = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userCredential = _registerDbContext.UserCredential.FirstOrDefault(uc => uc.Username == userName);
                if (userCredential == null)
                {
                    ViewBag.PasswordChangeError = "User not found.";
                    return View("AccountSetting");
                }

                if (!VerifyPassword(currentPassword, userCredential.Password))
                {
                    ViewBag.PasswordChangeError = "The current password is incorrect.";
                    return View("AccountSetting");
                }

                if (newPassword != confirmPassword)
                {
                    ViewBag.PasswordChangeError = "New password and confirm password do not match.";
                    return View("AccountSetting");
                }

                userCredential.Password = HashPassword(newPassword);
                _registerDbContext.SaveChanges();

                ViewBag.PasswordChangeSuccess = "Password changed successfully.";
                return View("AccountSetting");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for {Username}", userName);
                ViewBag.PasswordChangeError = "An unexpected error occurred. Please try again.";
                return View("AccountSetting");
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
        }

        public IActionResult AccreditedEstab()
        {
            // Fetch data from the database and remove duplicates based on EstablishmentName and Branch
            var qrCodes = _registerDbContext.QrCodes
                                  .GroupBy(q => new { q.EstablishmentName, q.Branch })
                                  .Select(g => g.First()) // Select the first record in each group
                                  .ToList();

            return View(qrCodes);
        }


        public IActionResult QrScan()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ProcessQrCode(string qrCodeValue)
        {
            if (!string.IsNullOrEmpty(qrCodeValue))
            {
                try
                {
                    var qrContent = JsonConvert.DeserializeObject<dynamic>(qrCodeValue);

                    if (qrContent != null)
                    {
                        var commoditiesUrl = qrContent.CommoditiesUrl?.ToString();
                        var medicineUrl = qrContent.MedicineUrl?.ToString();
                        var establishment = qrContent.EstablishmentName?.ToString();
                        var branch = qrContent.Branch?.ToString();

                        if (!string.IsNullOrEmpty(commoditiesUrl) && !string.IsNullOrEmpty(medicineUrl))
                        {
                            // Store the values in TempData for cross-action persistence
                            TempData["CommoditiesUrl"] = commoditiesUrl;
                            TempData["MedicineUrl"] = medicineUrl;
                            TempData["Establishment"] = establishment;
                            TempData["Branch"] = branch;

                            // Based on the QR code content, redirect to the appropriate action
                            if (!string.IsNullOrEmpty(commoditiesUrl))
                            {
                                return RedirectToAction("Commodities", new { establishment = establishment, branch = branch });
                            }
                            else if (!string.IsNullOrEmpty(medicineUrl))
                            {
                                return RedirectToAction("Medicine", new { establishment = establishment, branch = branch });
                            }
                        }
                        else
                        {
                            ViewBag.ScannedResult = "Invalid QR code content.";
                        }
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    ViewBag.ScannedResult = qrCodeValue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing QR code: {QrCodeValue}", qrCodeValue);
                    ViewBag.ScannedResult = "An unexpected error occurred while processing the QR code.";
                }
            }
            else
            {
                ViewBag.Error = "QR code scanning failed or no value received.";
            }

            return View("ProcessQrCode");
        }



        public IActionResult Medicine(string establishment, string branch)
        {
            //Retrieve the user's account ID
            var accountId = GetUserAccountId();
            ViewBag.AccountId = accountId;

            // Use TempData or query parameters to pass the data
            ViewBag.Establishment = TempData["Establishment"] ?? establishment;
            ViewBag.Branch = TempData["Branch"] ?? branch;

            return View();
        }


        [Route("User/SubmitTransaction")]
        [HttpPost]
        public async Task<IActionResult> SubmitTransaction([FromBody] List<Medicine.MedicineTransaction> transactions)
        {
            if (transactions == null || !transactions.Any())
            {
                return BadRequest(new { message = "No transactions were provided." });
            }
        
            try
            {
                foreach (var transaction in transactions)
                {
                    var account = await _registerDbContext.Accounts.FindAsync(transaction.AccountId);
                    if (account == null)
                    {
                        return BadRequest(new { message = "Session Expired, please log in again. Thank you!" });
                    }
        
                    // Validate input values
                    transaction.Price = Math.Max(transaction.Price, 0);
                    transaction.TotalPrice = Math.Max(transaction.TotalPrice, 0);
                    transaction.DiscountedPrice = Math.Max(transaction.DiscountedPrice, 0);
        
                    // Fetch or create a ledger
                    var ledger = await _registerDbContext.MedicineTransactionLedgers
                        .FirstOrDefaultAsync(l => l.AccountId == transaction.AccountId);
        
                    if (ledger == null)
                    {
                        ledger = new MedicineTransactionLedger
                        {
                            LedgerId = Guid.NewGuid(),
                            AccountId = transaction.AccountId,
                        };
                        _registerDbContext.MedicineTransactionLedgers.Add(ledger);
                        await _registerDbContext.SaveChangesAsync();
                    }
        
                    var remainingBalance = transaction.PrescribedQuantity - transaction.PurchasedQuantity;
                    if (remainingBalance < 0)
                    {
                        return BadRequest(new { message = "Purchased quantity cannot exceed prescribed quantity." });
                    }
        
                    // Check for existing transactions
                    var existingTransaction = await _registerDbContext.MedicineTransactions
                        .FirstOrDefaultAsync(t => t.MedTransactionId == transaction.MedTransactionId);
        
                    if (existingTransaction != null)
                    {
                        Console.WriteLine("Existing transaction found.");
        
                        // Update fields if changes are detected
                        if (existingTransaction.PurchasedQuantity != transaction.PurchasedQuantity ||
                            existingTransaction.RemainingBalance != remainingBalance)
                        {
                            existingTransaction.PurchasedQuantity = transaction.PurchasedQuantity;
                            existingTransaction.RemainingBalance = remainingBalance;
                            existingTransaction.Price = transaction.Price;
                            existingTransaction.TotalPrice = transaction.TotalPrice;
                            existingTransaction.DiscountedPrice = transaction.DiscountedPrice;
                            existingTransaction.EstablishmentName = string.IsNullOrEmpty(transaction.EstablishmentName) ? "Unknown" : transaction.EstablishmentName;
                            existingTransaction.Branch = string.IsNullOrEmpty(transaction.Branch) ? "Unknown" : transaction.Branch;
                            existingTransaction.DatePurchased = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Asia/Manila");
                        }
                        else
                        {
                            Console.WriteLine("Existing transaction found but not altered.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No existing transaction found; creating new.");
                        transaction.MedTransactionId = Guid.NewGuid();
                        transaction.RemainingBalance = remainingBalance;
                        transaction.LedgerId = ledger.LedgerId;
        
                        transaction.DatePurchased = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Asia/Manila");
                        transaction.EstablishmentName = string.IsNullOrEmpty(transaction.EstablishmentName) ? "Unknown" : transaction.EstablishmentName;
                        transaction.Branch = string.IsNullOrEmpty(transaction.Branch) ? "Unknown" : transaction.Branch;
                        _registerDbContext.MedicineTransactions.Add(transaction);
                    }
                }
        
                await _registerDbContext.SaveChangesAsync();
                return Ok(new { message = "Transactions submitted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the transactions.", error = ex.Message });
            }
        }



        [HttpGet]
        [Route("User/GetUnfinishedTransactions")]
        public async Task<IActionResult> GetUnfinishedTransactions(Guid accountId)
        {
            try
            {
                var transactions = await _registerDbContext.MedicineTransactions
                    .Where(t => t.AccountId == accountId && t.RemainingBalance > 0)
                    .ToListAsync();

                if (!transactions.Any())
                    return NotFound(new { message = "No unfinished transactions found." });

                return Ok(transactions.Select(t => new
                {
                    t.MedTransactionId,
                    t.MedicineName,
                    t.PrescribedQuantity,
                    t.PurchasedQuantity,
                    t.RemainingBalance,
                    t.Price,
                    t.DatePurchased,
                    t.PTRNo, // Include PTR No.
                    t.AttendingPhysician // Include Attending Physician
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch transactions.", error = ex.Message });
            }
        }


        [HttpGet]
        [Route("User/GetTransactionDetails")]
        public async Task<IActionResult> GetTransactionDetails(Guid transactionId)
        {
            try
            {
                var transaction = await _registerDbContext.MedicineTransactions
                    .Where(t => t.MedTransactionId == transactionId)
                    .FirstOrDefaultAsync();

                if (transaction == null)
                    return NotFound(new { message = "Transaction not found." });

                return Ok(new
                {
                    transaction.PTRNo,
                    transaction.AttendingPhysician,
                    transaction.Signature
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch transaction details.", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("User/UpdateTransactionDetails")]
        public async Task<IActionResult> UpdateTransactionDetails([FromBody] MedicineTransaction updatedTransaction)
        {
            try
            {
                if (updatedTransaction == null)
                    return BadRequest(new { message = "Invalid transaction data." });

                // Fetch the existing transaction
                var transaction = await _registerDbContext.MedicineTransactions
                    .Where(t => t.MedTransactionId == updatedTransaction.MedTransactionId)
                    .FirstOrDefaultAsync();

                if (transaction == null)
                    return NotFound(new { message = "Transaction not found." });

                // Update only the fields that are provided
                if (!string.IsNullOrEmpty(updatedTransaction.PTRNo))
                    transaction.PTRNo = updatedTransaction.PTRNo;

                if (!string.IsNullOrEmpty(updatedTransaction.AttendingPhysician))
                    transaction.AttendingPhysician = updatedTransaction.AttendingPhysician;

                if (!string.IsNullOrEmpty(updatedTransaction.Signature))
                    transaction.Signature = updatedTransaction.Signature;

                // Save the changes
                await _registerDbContext.SaveChangesAsync();

                return Ok(new { message = "Transaction updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update transaction.", error = ex.Message });
            }
        }



        private Guid GetUserAccountId()
        {
            var userName = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userName))
            {
                return Guid.Empty; // or throw an exception, or return null as per your need
            }

            var userAccount = _registerDbContext.Accounts
                .Include(a => a.UserCredential)
                .FirstOrDefault(a => a.UserCredential.Username == userName);

            return userAccount?.Id ?? Guid.Empty; // Assuming AccountId is of type Guid
        }
        public IActionResult Commodities(string establishment, string branch)
        {
            // Retrieve the user's account ID
            var accountId = GetUserAccountId();
            ViewBag.AccountId = accountId;

            // Use TempData or query parameters to pass the data
            ViewBag.Establishment = TempData["Establishment"] ?? establishment; // Prefer TempData if available
            ViewBag.Branch = TempData["Branch"] ?? branch;

            return View();
        }


        [HttpGet]
        [Route("User/GetRemainingBalance")]
        public IActionResult GetRemainingBalance(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return Json(new { success = false, message = "Invalid AccountId." });
            }
        
            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var philippineDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);
        
            // Check if today is Monday in Philippine time
            bool isMonday = philippineDateTime.DayOfWeek == DayOfWeek.Monday;
        
            // Get the start of the current week (Monday at 00:00:00) in Philippine time
            DateTime startOfWeek = philippineDateTime.Date.AddDays(-(int)philippineDateTime.DayOfWeek + (int)DayOfWeek.Monday);
        
            // Fetch the most recent commodity transaction for the account
            var lastTransaction = _registerDbContext.CommodityTransactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefault();
        
            // Introduce a weekly balance variable
            decimal weeklyBalance = 2500;
        
            if (lastTransaction != null)
            {
                // Check if the last transaction belongs to the current week
                if (lastTransaction.CreatedDate >= startOfWeek)
                {
                    weeklyBalance = lastTransaction.RemainingDiscount;
                }
                else if (isMonday)
                {
                    // On Monday, reset the balance
                    weeklyBalance = 2500;
                }
            }
        
            return Json(new { success = true, remainingBalance = weeklyBalance });
        }



        [HttpPost]
        [Route("User/SubmitCommodities")]
        public IActionResult SubmitCommodities([FromBody] CommodityTransaction transaction)
        {
            if (transaction == null || transaction.Items == null || !transaction.Items.Any())
            {
                return BadRequest("Invalid transaction data.");
            }
        
            transaction.AccountId = GetUserAccountId();
        
            if (transaction.AccountId == Guid.Empty)
            {
                return BadRequest("Session Timeout. Please log in again.");
            }
        
            var account = _registerDbContext.Accounts.FirstOrDefault(a => a.Id == transaction.AccountId);
            if (account == null)
            {
                return BadRequest("Invalid AccountId.");
            }
        
            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var philippineTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);
        
            bool isMonday = philippineTime.DayOfWeek == DayOfWeek.Monday;
            DateTime startOfWeek = philippineTime.Date.AddDays(-(int)philippineTime.DayOfWeek + (int)DayOfWeek.Monday);
        
            // Initialize the weekly balance
            decimal weeklyBalance = 2500;
        
            var lastTransaction = _registerDbContext.CommodityTransactions
                .Where(t => t.AccountId == transaction.AccountId)
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefault();
        
            if (lastTransaction != null)
            {
                if (lastTransaction.CreatedDate >= startOfWeek)
                {
                    weeklyBalance = lastTransaction.RemainingDiscount;
                }
                else if (isMonday)
                {
                    weeklyBalance = 2500;
                }
            }
        
            // Calculate the total amount for the new transaction
            var totalAmount = transaction.Items.Sum(i => i.Quantity * i.Price * 0.95m);
            transaction.RemainingDiscount = weeklyBalance - totalAmount;
        
            if (transaction.RemainingDiscount < 0)
            {
                transaction.RemainingDiscount = 0;
            }
        
            transaction.TransactionId = Guid.NewGuid();
            transaction.CreatedDate = philippineTime;
            transaction.ModifiedDate = philippineTime;
        
            foreach (var item in transaction.Items)
            {
                item.AccountId = transaction.AccountId;
                item.TotalPrice = item.Quantity * item.Price;
                item.DiscountedPrice = item.TotalPrice * 0.95m;
                item.CreatedDate = philippineTime;
                item.ModifiedDate = philippineTime;
            }
        
            _registerDbContext.CommodityTransactions.Add(transaction);
            _registerDbContext.SaveChanges();
        
            return Ok(new
            {
                success = true,
                message = "Transaction completed!",
                remainingDiscount = transaction.RemainingDiscount
            });
        }


         public IActionResult History()
 {
     var userAccountId = GetUserAccountId(); // Get the logged-in user's AccountId

     if (userAccountId == Guid.Empty)
     {
         return RedirectToAction("Login", "Account");
     }

     // Fetch commodity transactions
     var transactions = _registerDbContext.CommodityTransactions
         .Where(t => t.AccountId == userAccountId)
         .Include(t => t.Items)
         .OrderByDescending(t => t.CreatedDate)
         .Select(t => new
         {
             TransactionId = t.TransactionId.ToString(),
             CreatedDate = t.CreatedDate.ToString("yyyy-MM-dd"), // Format as string for display
             t.EstablishmentName,
             Branch = t.BranchName, // Add Branch as an empty string for consistency
             PurchaseType = "Commodity",
             Items = t.Items.Select(i => new
             {
                 Description = i.Description,
                 Quantity = i.Quantity,
                 Price = i.Price,
                 TotalPrice = i.TotalPrice,
                 DiscountedPrice = i.DiscountedPrice
             }).ToList()
         })
         .ToList();

     // Fetch medicine transactions
     var medicineTransactions = _registerDbContext.MedicineTransactions
         .Where(m => m.AccountId == userAccountId)
         .OrderByDescending(m => m.DatePurchased)
         .Select(m => new
         {
             TransactionId = m.MedTransactionId.ToString(),
             CreatedDate = m.DatePurchased.ToString("yyyy-MM-dd"),
             m.EstablishmentName,
             Branch = m.Branch,
             PurchaseType = "Medicine",
             Items = new[]
             {
         new
         {
             Description = m.MedicineName,
             Quantity = m.PurchasedQuantity,
             m.Price,
             m.TotalPrice,
             m.DiscountedPrice
         }
             }.ToList() // Convert array to List for consistency
         })
         .ToList();

     // Combine both transaction types
     var allTransactions = transactions.Concat(medicineTransactions).OrderByDescending(t => t.CreatedDate).ToList();

     ViewBag.Transactions = Newtonsoft.Json.JsonConvert.SerializeObject(allTransactions);

     return View();
 }



        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Report(string problemDescription, string establishment, string branch)
        {
            // Get the logged-in user's AccountId
            var userAccountId = GetUserAccountId();
            if (userAccountId == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if ProblemDescription is empty
            if (string.IsNullOrWhiteSpace(problemDescription))
            {
                // Display a SweetAlert error if ProblemDescription is missing
                TempData["Error"] = "Problem Description is required.";
                return RedirectToAction("Report");
            }


            // Create a new Report object and populate it with form data
            var report = new Report
            {
                ReportId = 0, // Assuming it’s auto-incremented
                AccountId = userAccountId,
                ProblemDescription = problemDescription,
                Establishment = establishment,
                Branch = branch,
                CreatedDate = DateTime.UtcNow
            };

            try
            {
                _registerDbContext.Reports.Add(report);
                _registerDbContext.SaveChanges();

                TempData["Success"] = "Report submitted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving report to the database.");
                TempData["Error"] = "An unexpected error occurred while submitting the report.";
            }

            return RedirectToAction("Report");
        }








    }
}

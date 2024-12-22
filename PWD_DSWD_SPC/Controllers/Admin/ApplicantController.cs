using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWD_DSWD_SPC.Data;
using PWD_DSWD_SPC.Models;
using PWD_DSWD_SPC.Models.Registered;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace PWD_DSWD_SPC.Controllers.Admin
{
    public class ApplicantController : Controller
    {
        private readonly RegisterDbContext _registerDbContext;
        private readonly IFluentEmail _fluentEmail;
        private readonly ILogger<ApplicantController> _logger;

        // Constructor with dependency injection
        public ApplicantController(RegisterDbContext registerDbContext, IFluentEmail fluentEmail, ILogger<ApplicantController> logger)
        {
            _registerDbContext = registerDbContext ?? throw new ArgumentNullException(nameof(registerDbContext));
            _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Applicants Table View
        public async Task<IActionResult> Applicants()
        {
            var pendingApplications = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .Where(a => a.Status.Status == "Pending" || a.Status.Status == "Waiting") // Fetch pending and ready-for-approval applicants
                .ToListAsync();

            _logger.LogInformation($"Fetched {pendingApplications.Count} pending or ready-for-approval applicants");

            return View(pendingApplications);
        }

        // Update Status Button (Requirements)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, bool? requirement1, bool? requirement2, bool? requirement3, bool? requirement4)
        {
            var application = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            // Update the requirements
            application.Status.Requirement1 = requirement1 ?? false;
            application.Status.Requirement2 = requirement2 ?? false;
            application.Status.Requirement3 = requirement3 ?? false;
            application.Status.Requirement4 = requirement4 ?? false;

            // Logic for updating status
            if (application.Status.Requirement1 && application.Status.Requirement2 && application.Status.Requirement3 && application.Status.Requirement4)
            {
                application.Status.Status = "Waiting"; // Indicate it's ready for admin to approve
                application.Status.IsApproved = false;
            }
            else if (!application.Status.Requirement1 && !application.Status.Requirement2 && !application.Status.Requirement3 && !application.Status.Requirement4)
            {
                application.Status.Status = "Disapproved";
                application.Status.IsApproved = false;
            }
            else
            {
                application.Status.Status = "Pending";
                application.Status.IsApproved = false;
            }

            await _registerDbContext.SaveChangesAsync();

            return RedirectToAction("Applicants");
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ApproveAccount(Guid accountId, int validity, string pwdNumber)
        {
            var account = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return NotFound();
            }

            // Ensure that the requirements are met before approval
            if (account.Status.Requirement1 && account.Status.Requirement2 && account.Status.Requirement3 && account.Status.Requirement4)
            {
                // Check if the disability number (PWD number) is already in use by another account
                var existingAccountWithPwdNumber = await _registerDbContext.Accounts
                    .FirstOrDefaultAsync(a => a.DisabilityNumber == pwdNumber && a.Id != accountId);

                if (existingAccountWithPwdNumber != null)
                {
                    // PWD number is already in use by another account, return an error message
                    TempData["ErrorMessage"] = "The PWD number is already in use by another applicant.";
                    return RedirectToAction("Applicants", new { id = accountId }); // Redirect back to the details view or appropriate view
                }

                // Set static 5 years validity for all approved accounts
                DateTime validUntil = DateTime.Now.AddYears(5);
                account.ValidUntil = validUntil;

                if (account.ApplicantType == "New Applicant" || account.ApplicantType == "Renewal")
                {
                    if (string.IsNullOrEmpty(pwdNumber))
                    {
                        return BadRequest("PWD number is required for new applicants.");
                    }

                    account.DisabilityNumber = pwdNumber;
                    var username = GenerateUsername();
                    var password = GeneratePassword();

                    // Hash the password before saving
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    // Check if the UserCredentials already exists
                    var existingCredential = await _registerDbContext.UserCredential
                        .FirstOrDefaultAsync(uc => uc.AccountId == accountId);

                    if (existingCredential != null)
                    {
                        // Update existing credentials
                        existingCredential.Username = username;
                        existingCredential.Password = hashedPassword;  // Update the hashed password
                        existingCredential.Role = "Client";
                        _registerDbContext.UserCredential.Update(existingCredential);
                    }
                    else
                    {
                        // Create a new UserCredential entity
                        var userCredential = new UserCredentials
                        {
                            AccountId = accountId,
                            Username = username,
                            Password = hashedPassword, // Store the hashed password
                            Role = "Client"
                        };
                        await _registerDbContext.UserCredential.AddAsync(userCredential);
                    }

                    // Extract full name
                    string fullName = $"{account.FirstName} {account.LastName}";

                    // Send credentials email
                    await SendCredentialsEmailAsync(account.EmailAddress, fullName, username, password, DateTime.Now, account.ReferenceNumber, account.DisabilityNumber);
                }

                // Final approval
                account.Status.Status = "Approved";
                account.Status.IsApproved = true;

                await _registerDbContext.SaveChangesAsync();

                return RedirectToAction("ListofAllAccounts", "Admin");
            }
            else
            {
                return BadRequest("Requirements are not met for approval.");
            }
        }


        // Disapprove Button
        [HttpPost]
         public async Task<IActionResult> DisapproveAccount(Guid accountId)
            {
                var account = await _registerDbContext.Accounts
                    .Include(a => a.Status)
                    .FirstOrDefaultAsync(a => a.Id == accountId);

                if (account != null)
                {
                    account.Status.Status = "Disapproved";
                    account.Status.IsApproved = false;
                    account.DisapprovalDate = DateTime.Now; // Set disapproval date
                    await _registerDbContext.SaveChangesAsync();

                    // Send disapproval email
                    var applicantName = $"{account.FirstName} {account.LastName}";
                    var disapprovalDate = account.DisapprovalDate?.ToString("MMMM dd, yyyy");
                    var emailBody = $@"
                    Dear {applicantName},

                    We regret to inform you that your registration has been disapproved as of {disapprovalDate}. 
                    Unfortunately, you did not meet the requirements for the registration.

                    If you have any queries or need further assistance, please do not hesitate to visit the office of the PWD 
                    in the Municipality of San Pablo City. Our office hours are from 8:00 AM to 5:00 PM, Monday to Friday.

                    Thank you for your understanding.

                    Sincerely,
                    DSWD San Pablo City";

                    var sender = await _fluentEmail
                        .To(account.EmailAddress) // Use the Email property from the Account model
                        .Subject("Registration Disapproval Notice")
                        .Body(emailBody, isHtml: false) // Set isHtml to true if you want to include HTML formatting
                        .SendAsync();

                    if (!sender.Successful)
                    {
                        // Handle email sending failure (optional)
                        TempData["Error"] = "Email could not be sent. Please try again.";
                    }
                }

                return RedirectToAction("ArchivedAccounts", "Admin");
            }

            // Auto Generation of Username
            private string GenerateUsername()
            {
                // Generate a random username
                var random = new Random();
                int number = random.Next(10000000, 100000000);
                return $"PWD-{number}";
        }

        // Auto Generation of Password
        private string GeneratePassword()
        {
            // Generate a random password
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Sending of Email upon approval
        private async Task SendCredentialsEmailAsync(string email, string fullName, string username, string password, DateTime approvalDate, string referenceNumber, string disabilityNumber)
            {
                // Send email with the generated credentials
                await _fluentEmail
                    .To(email)
                    .Subject("Your PWD Registration Application Has Been Approved!")
                    .Body($@"
                            <p>Dear {fullName},</p>

                                <p>We are pleased to inform you that your application for the <strong>Persons with Disabilities (PWD) Registration</strong> has been approved as of <strong>{approvalDate:MMMM dd, yyyy}</strong>.</p>

                                <p>Your login credentials for accessing your account are provided below:</p>
                                <p>
                                    <strong>Username:</strong> {username}<br/>
                                    <strong>Password:</strong> {password}
                                </p>

                                <p>For your reference, here is your unique information for tracking your application status:</p>
                                <p>
                                    <strong>Reference Number:</strong> {referenceNumber}<br/>
                                    <strong>Disability Number:</strong> {disabilityNumber}
                                </p>

                                <p><strong>Important:</strong> Please ensure that you change your password upon your first login for security purposes.</p>

                                <p>If you have any questions or require further assistance, please do not hesitate to contact our office at DSWD, Municipality of San Pablo City.</p>

                                <p>Thank you for your application. We look forward to assisting you further.</p>

                                <p>Best Regards,<br/>
                                DSWD San Pablo City</p>
                            ", isHtml: true)
                                    .SendAsync();


            }

                // GET: Applicants/Details/5
                public async Task<IActionResult> Details(Guid id)
                {
                    var application = await _registerDbContext.Accounts
                        .Include(a => a.Status)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (application == null)
                    {
                        return NotFound();
                    }

                    return View(application);
                }
    }
}

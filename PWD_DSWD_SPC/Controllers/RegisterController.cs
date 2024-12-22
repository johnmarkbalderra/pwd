using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentEmail.Core;
using FluentEmail.Razor;
using PWD_DSWD_SPC.Barangays;
using PWD_DSWD_SPC.Data;
using PWD_DSWD_SPC.Models;
using PWD_DSWD_SPC.Models.Registered;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace PWD_DSWD_SPC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly RegisterDbContext _registerDbContext;
        private readonly IFluentEmail _fluentEmail;

        // Constructor with dependency injection
        public RegisterController(RegisterDbContext registerDbContext, IFluentEmail fluentEmail)
        {
            _registerDbContext = registerDbContext ?? throw new ArgumentNullException(nameof(registerDbContext));
            _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
        }

        public IActionResult Add()
        {
            var barangaysData = Brgy.GetAll();

            var model = new AccountViewModel();
            model.BarangayList = new List<SelectListItem>();

            foreach (var account in barangaysData)
            {
                model.BarangayList.Add(new SelectListItem { Text = account.Barangay, Value = account.Barangay });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AccountViewModel addAccount)
        {
            if (addAccount == null)
            {
                return BadRequest("Account details cannot be null");
            }

            // Check if the disability number is already in use
            if (await IsDisabilityNumberInUseAsync(addAccount.DisabilityNumber))
            {
                ModelState.AddModelError("DisabilityNumber", "This disability number is already in use.");
                return View(addAccount); // Return the view with the model to display the error
            }

            // Check if the email is already in use (either approved or disapproved)
            if (await IsEmailInUseAsync(addAccount.EmailAddress))
            {
                ModelState.AddModelError("EmailAddress", "This email is already in use by an active or recently disapproved account.");
                return View(addAccount); // Return the view with the model to display errors
            }

            // Generate Reference Number
            string referenceNumber = GenerateReferenceNumber();


            // Ensure the Occupation field is set from the hidden input
            string occupation = addAccount.Occupation == "Other" ? Request.Form["Occupation"] : addAccount.Occupation;

            


            var account = new Account()
            {
                Id = Guid.NewGuid(),
                ApplicantType = addAccount.ApplicantType,
                DisabilityNumber = addAccount.DisabilityNumber,
                CreatedAt = addAccount.CreatedAt,
                LastName = addAccount.LastName,
                FirstName = addAccount.FirstName,
                MiddleName = addAccount.MiddleName,
                suffix = addAccount.suffix,
                DateOfBirth = addAccount.DateOfBirth,
                Gender = addAccount.Gender,
                CivilStatus = addAccount.CivilStatus,
                FatherLastName = addAccount.FatherLastName,
                FatherFirstName = addAccount.FatherFirstName,
                FatherMiddleName = addAccount.FatherMiddleName,
                MotherLastName = addAccount.MotherLastName,
                MotherFirstName = addAccount.MotherFirstName,
                MotherMiddleName = addAccount.MotherMiddleName,
                GuardianLastName = addAccount.GuardianLastName,
                GuardianFirstName = addAccount.GuardianFirstName,
                GuardianMiddleName = addAccount.GuardianMiddleName,
                TypeOfDisability = addAccount.TypeOfDisability,
                CauseOfDisability = addAccount.CauseOfDisability,
                HouseNoAndStreet = addAccount.HouseNoAndStreet,
                AD = addAccount.SelectedBrgy,
                Barangay = addAccount.SelectedBrgy,
                Municipality = addAccount.Municipality,
                Province = addAccount.Province,
                Region = addAccount.Region,
                LandlineNo = addAccount.LandlineNo,
                MobileNo = addAccount.MobileNo,
                EmailAddress = addAccount.EmailAddress,
                EducationalAttainment = addAccount.EducationalAttainment,
                StatusOfEmployment = addAccount.StatusOfEmployment,
                CategoryOfEmployment = addAccount.CategoryOfEmployment,
                TypeOfEmployment = addAccount.TypeOfEmployment,
                Occupation = occupation,
                OrganizationAffiliated = addAccount.OrganizationAffiliated,
                ContactPerson = addAccount.ContactPerson,
                OfficeAddress = addAccount.OfficeAddress,
                OfficeTelNo = addAccount.OfficeTelNo,
                SSSNo = addAccount.SSSNo,
                GSISNo = addAccount.GSISNo,
                PagIBIGNo = addAccount.PagIBIGNo,
                PSNNo = addAccount.PSNNo,
                PhilHealthNo = addAccount.PhilHealthNo,
                AccomplishByLastName = addAccount.AccomplishByLastName,
                AccomplishByFirstName = addAccount.AccomplishByFirstName,
                AccomplishByMiddleName = addAccount.AccomplishByMiddleName,
                ReferenceNumber = referenceNumber
            };

            // Add default Approval and Credentials
            account.Status = new ApprovalStatus();
            account.UserCredential = new UserCredentials();

            await _registerDbContext.Accounts.AddAsync(account);
            await _registerDbContext.SaveChangesAsync();


            // Send email after successfully saving to the database
            await _fluentEmail
                .To(addAccount.EmailAddress)
                .Subject("Your Registration Reference Number")
                .Body($@"
                <p>Dear {addAccount.FirstName} {addAccount.LastName},</p>

                <p>We are pleased to inform you that your application for the PWD (Persons with Disabilities) Registration has been successfully received as of <strong>{addAccount.CreatedAt:MMMM dd, yyyy}</strong>. Below is your unique reference number for tracking your application status:</p>

                <p><strong>Reference Number:</strong> {referenceNumber}</p>

                <p>Please ensure to submit the following required documents in person:</p>
                <ul>
                    <li><strong>Three (3) 1x1 ID Photos:</strong> Recent 1x1 ID pictures.</li>
                    <li><strong>One (1) Valid Government-issued ID:</strong> A government-issued ID (e.g., passport, driver’s license, or national ID card).</li>
                    <li><strong>Barangay Certificate of Residency:</strong> Proof of residency within your barangay.</li>
                    <li><strong>Proof of Disability:</strong> Documentation that verifies your status as a person with a disability (PWD).</li>
                </ul>

                <p>These documents are essential to proceed with your application. Please note that this is just the initial stage of registration. A legislative validation process must be completed before final approval by the administrator.</p>

                <p><strong>Submission Details:</strong></p>
                <p>The above requirements must be submitted at the office of DSWD, Municipality of San Pablo City, from 8:00 AM to 5:00 PM.</p>

                <p>Thank you for your application. Should you have any questions, feel free to reach out to our office. We look forward to assisting you.</p>

                <p>Best Regards,<br/>
                DSWD San Pablo City</p>
            ", isHtml: true)
                        .SendAsync();



            return RedirectToAction("Landing", "Landing");
        }

        // Helper method to check if an PWD no. is already in use
        private async Task<bool> IsDisabilityNumberInUseAsync(string disabilityNumber)
        {
            return await _registerDbContext.Accounts
                .AnyAsync(a => a.DisabilityNumber == disabilityNumber);
        }



        // Helper method to check if an email is already in use
        private async Task<bool> IsEmailInUseAsync(string email)
        {
            var account = await _registerDbContext.Accounts
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.EmailAddress == email);

            if (account != null)
            {
                if (account.Status.Status == "Approved" && account.ValidUntil > DateTime.Now)
                {
                    return true; // Email is in use by an approved account
                }

                // Check if disapproved and if 3 days have passed
                if (account.Status.Status == "Disapproved" && account.DisapprovalDate.HasValue)
                {
                    var daysSinceDisapproval = (DateTime.Now - account.DisapprovalDate.Value).TotalDays;
                    if (daysSinceDisapproval < 3)
                    {
                        return true; // Email is in use by a disapproved account
                    }
                }
            }

            return false; // Email is not in use
        }
        private string GenerateReferenceNumber()
        {
            // Example logic to generate a reference number
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
        }
    }
}

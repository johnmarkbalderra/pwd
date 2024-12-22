using FluentEmail.Core;
using PWD_DSWD_SPC.Models.Registered;
using System.Threading.Tasks;

namespace PWD_DSWD_SPC
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendExpirationEmailAsync(string emailAddress); // New method for sending expiration emails
        Task SendRenewalEmailAsync(string emailAddress); // New method
        Task SendForgotPasswordEmailAsync(string emailAddress, string newpassword); // New method for Forgot Password
    }

    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(message)
                .SendAsync();
        }

        public async Task SendExpirationEmailAsync(string emailAddress)
        {
            string subject = "Your Account has Expired";
            string body = @"
            <p>Dear Applicant,</p>

            <p>Please ensure to submit the following required documents in person:</p>

            <ul>
                <li>Three (3) 1x1 ID Photos: Recent 1x1 ID pictures.</li>
                <li>One (1) Valid Government-issued ID: A government-issued ID (e.g., passport, driver’s license, or national ID card).</li>
                <li>Barangay Certificate of Residency: Proof of residency within your barangay.</li>
                <li>Proof of Disability: Documentation that verifies your status as a person with a disability (PWD).</li>
            </ul>

            <p>If you have any questions or require further assistance, please do not hesitate to contact our office at DSWD, Municipality of San Pablo City.</p>

            <p>Thank you.</p>

            <p>Sincerely,<br/>
            DSWD San Pablo City</p>";

            await _fluentEmail
                .To(emailAddress)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();

        }

        public async Task SendRenewalEmailAsync(string emailAddress)
        {
            string subject = "Account Successfully Renewed";
            string body = @"
    <p>Dear Applicant,</p>

    <p>We are pleased to inform you that your account has been successfully renewed for another 5 years.</p>

    <p>If you have any questions or require further assistance, please do not hesitate to contact our office at DSWD, Municipality of San Pablo City.</p>

    <p>Thank you.</p>

    <p>Sincerely,<br/>
    DSWD San Pablo City</p>";

            await _fluentEmail
                .To(emailAddress)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();
        }
        public async Task SendForgotPasswordEmailAsync(string emailAddress, string newPassword)
        {
            string subject = "Password Reset Request";
            string body = $@"
            <p>Dear User,</p>

            <p>Your password has been successfully reset. Please find your new password below:</p>

            <p><strong>{newPassword}</strong></p>

            <p>We recommend you log in and update your password to one of your choosing as soon as possible for added security.</p>

            <p>If you have any questions or need further assistance, please contact our support team.</p>

            <p>Thank you,</p>
            <p>DSWD San Pablo City</p>";

            await _fluentEmail
                .To(emailAddress)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();
        }

    }
}

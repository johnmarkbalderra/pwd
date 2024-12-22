using Microsoft.AspNetCore.Mvc.Rendering;

namespace PWD_DSWD_SPC.Models
{
    public class AccountViewModel
    {
        public string ApplicantType { get; set; }
        public string? DisabilityNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string? suffix { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string CivilStatus { get; set; }
        public string FatherLastName { get; set; }
        public string FatherFirstName { get; set; }
        public string FatherMiddleName { get; set; }
        public string MotherLastName { get; set; }
        public string MotherFirstName { get; set; }
        public string MotherMiddleName { get; set; }
        public string GuardianLastName { get; set; }
        public string GuardianFirstName { get; set; }
        public string GuardianMiddleName { get; set; }
        public string TypeOfDisability { get; set; }
        public string CauseOfDisability { get; set; }
        public string HouseNoAndStreet { get; set; }
        public string SelectedBrgy { get; set; }
        public List<SelectListItem> BarangayList { get; set; }
        public string Municipality { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string? LandlineNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string EducationalAttainment { get; set; }
        public string? StatusOfEmployment { get; set; }
        public string? CategoryOfEmployment { get; set; }
        public string? TypeOfEmployment { get; set; }
        public string? Occupation { get; set; }
        public string? OtherOccupation { get; set; }
        public string? OrganizationAffiliated { get; set; }
        public string? ContactPerson { get; set; }
        public string? OfficeAddress { get; set; }
        public string? OfficeTelNo { get; set; }
        public string? SSSNo { get; set; }
        public string? GSISNo { get; set; }
        public string? PagIBIGNo { get; set; }
        public string? PSNNo { get; set; }
        public string? PhilHealthNo { get; set; }
        public string AccomplishByLastName { get; set; }
        public string AccomplishByFirstName { get; set; }
        public string AccomplishByMiddleName { get; set; }
        public string ReferenceNumber { get; set; }
      
    }
}

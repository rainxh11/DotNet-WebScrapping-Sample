using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgresMESRS.Middleware.API.Models
{
    public class StudentDetail : UniversityStudent
    {
        public byte[] Picture { get; set; }
        public string FirstNameArabic { get; set; }
        public string FamilyNameArabic { get; set; }
        public string BirthPlace { get; set; }
        public IndividualDetails individualDetails { get; set; } = new IndividualDetails();
        public RegistrationDetails RegistrationDetails { get; set; } = new RegistrationDetails();
        public StudentParents  Parents { get; set; } = new StudentParents();
        public AccessDetails AccessDetails { get; set; } = new AccessDetails();
        public List<StudentAddress> Addresses { get; set; } = new List<StudentAddress>();
        public List<StudentEmail> EmailAddresses { get; set; } = new List<StudentEmail>();
        public List<StudentPhone> PhoneNumbers { get; set; } = new List<StudentPhone>();
        public List<Grade> Grades { get; set; } = new List<Grade>();
        public List<Inscription> Inscriptions { get; set; } = new List<Inscription>();
        public List<AnnualReview> AnnualReviews { get; set; } = new List<AnnualReview>();
        public List<StudentTransfer> Transfers { get; set; } = new List<StudentTransfer>();
        public List<SpecialNeed> SpecialNeeds { get; set; } = new List<SpecialNeed>();
        public List<SportsSituation> SportsSituations { get; set; } = new List<SportsSituation>();
    }
    public class RegistrationDetails
    {
        public string CurrentUniversity { get; set; }
        public string PreviousUniversity { get; set; }
        public string PreviousUniversityArabic { get; set; }
        public string RegisterDate { get; set; }
        public string CurrentAcademicYear { get; set; }
        public string RegistrationNumber { get; set; }
        public string CurrentLevel { get; set; }
        public string CurrentBranch { get; set; }
        public string CurrentFormation { get; set; }
        public string CurrentCycle { get; set; }
        public string CurrentSpeciality { get; set; }
        public string CurrentDomain { get; set; }

    }
    public class IndividualDetails
    {
        public string Civility { get; set; }
        public string Presumed { get; set; }
        public string Nationality { get; set; }
        public string ArmyService { get; set; }
        public string FamilySituation { get; set; }
        public string BloodType { get; set; }
        public string Quality { get; set; }
    }
    public class StudentParents
    {
        public string FatherFamilyName { get; set; }
        public string FatherFamilyNameArabic { get; set; }
        public string FatherFirstName { get; set; }
        public string FatherFirstNameArabic { get; set; }
        public string MotherFamilyName { get; set; }
        public string MotherFamilyNameArabic { get; set; }
        public string MotherFirstName { get; set; }
        public string MotherFirstNameArabic { get; set; }
    }
    public class StudentAddress
    {
        public string AddressType { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
    public class StudentPhone
    {
        public string PhoneType { get; set; }
        public string AddressType { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class StudentEmail
    {
        public string EmailType { get; set; }
        public string AddressType { get; set; }
        public string Email { get; set; }
    }
    public class AccessDetails
    {
        public string TitleNumber { get; set; }
        public string AccessTitleType { get; set; }
        public decimal Score { get; set; }
        public string FirstForeignLanguage { get; set; }
        public string SecondForeignLanguage { get; set; }
        public string Mention { get; set; }
        public string ObtainingInstitution { get; set; }
        public string ObtainingYear { get; set; }
        public string Speciality { get; set; }
        public string Equivalence { get; set; }

    }
    public class Grade
    {
        public string ClassSubject { get; set; }
        public string ClassSubjectArabic { get; set; }
        public decimal Score { get; set; }
        public decimal BaseScore { get; set; }
    }
    public class Inscription
    {
        public string AcademicYear { get; set; }
        public string University { get; set; }
        public string RegistrationNumber { get; set; }
        public string Level { get; set; }
        public string Cycle { get; set; }
        public string Formation { get; set; }
    }
    public class AnnualReview
    {
        public string AcademicYear { get; set; }
        public string University { get; set; }
        public string Level { get; set; }
        public string Cycle { get; set; }
        public decimal Score { get; set; }
        public decimal Credit { get; set; }
        public string Mention { get; set; }
        public string Decision { get; set; }
    }
    public class StudentTransfer
    {
        public string AcademicYear { get; set; }
        public string Date { get; set; }
        public string TransferType { get; set; }
        public string Reason { get; set; }
        public string OriginDomain { get; set; }
        public string OriginBranch { get; set; }
        public string RequestedDomain { get; set; }
        public string RequestedBranch { get; set; }
        public string OriginInstitution { get; set; }
        public string RequestedInstitution { get; set; }
        public string FinalDecision { get; set; }
    }
    public class SpecialNeed
    {
        public string NeedType { get; set; }
        public string NeedTypeArabic { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Observation { get; set; }
    }
    public class SportsSituation
    {
        public string Number { get; set; }
        public string Discipline { get; set; }
        public string DisciplineArabic { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Observation { get; set; }
    }
}

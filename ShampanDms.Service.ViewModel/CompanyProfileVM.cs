using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShampanDms.Service.ViewModel
{

    public class CompanyProfileVM
    {
        public int Id { get; set; }

        [DisplayName("Code")]
        public string? Code { get; set; }

        [Required]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Company Bangla Name")]
        public string? CompanyBanglaName { get; set; }

        [DisplayName("Company Legal Name")]
        public string? CompanyLegalName { get; set; }

        [DisplayName("Address Line 1")]
        public string? Address1 { get; set; }

        [DisplayName("Address Line 2")]
        public string? Address2 { get; set; }

        [DisplayName("Address Line 3")]
        public string? Address3 { get; set; }

        [DisplayName("City")]
        public string? City { get; set; }

        [DisplayName("Zip Code")]
        public string? ZipCode { get; set; }

        [DisplayName("Telephone No.")]
        public string? TelephoneNo { get; set; }

        [DisplayName("Fax No.")]
        public string? FaxNo { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Contact Person")]
        public string? ContactPerson { get; set; }

        [DisplayName("Contact Person's Designation")]
        public string? ContactPersonDesignation { get; set; }

        [DisplayName("Contact Person's Telephone")]
        public string? ContactPersonTelephone { get; set; }

        [DisplayName("Contact Person's Email")]
        public string? ContactPersonEmail { get; set; }

        [DisplayName("TIN No.")]
        public string? TINNo { get; set; }

        [DisplayName("VAT Registration No.")]
        public string? VatRegistrationNo { get; set; }

        [DisplayName("Comments")]
        public string? Comments { get; set; }

        [Required]
        [DisplayName("Fiscal Year Start")]
        public string FYearStart { get; set; }

        [Required]
        [DisplayName("Fiscal Year End")]
        public string FYearEnd { get; set; }

        [DisplayName("Business Nature")]
        public string? BusinessNature { get; set; }

        [DisplayName("Accounting Nature")]
        public string? AccountingNature { get; set; }

        [DisplayName("Company Type ID")]
        public int? CompanyTypeId { get; set; }

        [DisplayName("Section")]
        public string? Section { get; set; }

        [DisplayName("BIN")]
        public string? BIN { get; set; }

        [DisplayName("Is VDS With Holder?")]
        public bool IsVDSWithHolder { get; set; }

        [DisplayName("Application Version")]
        public string? AppVersion { get; set; }

        [DisplayName("License")]
        public string? License { get; set; }


        [Display(Name = "Archived")]
        public bool IsArchive { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        public string? CreatedFrom { get; set; }

        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }
        public string? LastUpdateFrom { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }
        public string? Operation { get; set; }
        public string? Status { get; set; }
        [DisplayName("Fiscal Year Start")]
        public string FYearStartSalePerson { get; set; }

        [DisplayName("Fiscal Year End")]
        public string FYearEndSalePerson { get; set; }

    }

}

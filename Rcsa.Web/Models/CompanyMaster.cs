using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Rcsa.Web.Models
{
  [Table("CompanyMaster")]
  public class CompanyMaster
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CompanyId { get; set; }
    [Required(ErrorMessage = "Please enter company name")]
    [StringLength(300)]
    public string ComapnyName { get; set; }
    public string CompanyDescription { get; set; }
    public string CompanyNo { get; set; }

    [MaxLength(2000)]
    [StringLength(2000)]
    public string CompanyAddress { get; set; }

    [MaxLength(100)]
    [StringLength(100)]
    public string ContactName { get; set; }

    [MaxLength(50)]
    [StringLength(50)]
    [RegularExpression(@"^\(\d{3}\) ?\d{3}( |-)?\d{4}|^\d{3}( |-)?\d{3}( |-)?\d{4}", ErrorMessage = "Enter valid phone number. Phone no. should in format like (658)154-1122 or 6581541122 or 658-154-1122")]
    public string Phone { get; set; }

    public int? InsertedBy { get; set; }
    public DateTime? Insertedon { get; set; }
    public string Insertedmachineinfo { get; set; }
    public int? Updatedby { get; set; }
    public DateTime? Updatedon { get; set; }
    public string Updatemachineinfo { get; set; }
  }
}
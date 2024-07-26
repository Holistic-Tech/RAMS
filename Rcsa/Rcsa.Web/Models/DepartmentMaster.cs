using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rcsa.Web.Models
{
    [Table("DepartmentMaster")]
    public class DepartmentMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Please enter department name")]
        [StringLength(300)]
        public string DepartmentName { get; set; }

        [ForeignKey("CompaniesMasters")]
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual CompanyMaster CompaniesMasters { get; set; }

        public int? InsertedBy { get; set; }
        public DateTime? Insertedon { get; set; }
        public string Insertedmachineinfo { get; set; }
        public int? Updatedby { get; set; }
        public DateTime? Updatedon { get; set; }
        public string Updatemachineinfo { get; set; }
    }
}
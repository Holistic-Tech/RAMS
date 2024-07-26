//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Rcsa.Web.Models
//{
//  [Table("ConsultantsUser")]
//  public class ConsultantsUser
//  {
//    [Key]
//    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//    public int ConsultantUserId { get; set; }
    
//    [ForeignKey("UsersProfile")]
//    public int UserId { get; set; }
//    [ForeignKey("UserId")]
//    public virtual UserProfile UsersProfile { get; set; }

//    public int? UpdatedBy { get; set; }
//    public DateTime? UpdatedOn { get; set; }
//  }
//}

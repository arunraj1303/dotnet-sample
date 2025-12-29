using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApi.Models
{
    [Table("companies")]
    public class Company
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("company_name")]
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [Column("website")]
        [StringLength(500)]
        public string? Website { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("founded_year")]
        public int? FoundedYear { get; set; }

        [Column("headquarters")]
        [StringLength(255)]
        public string? Headquarters { get; set; }
    }
}

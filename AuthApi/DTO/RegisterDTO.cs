using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTO
{
    public class RegisterDTO
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }=String.Empty;
        [Required]
        [MaxLength(20)]
        public string password { get; set; }=String.Empty;
    }
}

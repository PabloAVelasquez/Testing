using System.ComponentModel.DataAnnotations;

namespace BnPBank.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool IsAdmin { get; set; }

        public byte[] ProfilePicture { get; set; } // Only this for the picture

        // Add properties for the hashed password
        public string HashedPassword { get; set; }
    }
}
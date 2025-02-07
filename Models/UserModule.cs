using System.ComponentModel.DataAnnotations;

namespace Institute_Management.Models
{
    public class UserModule
    {
        public class User
        {
            [Key]
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string ContactDetails { get; set; }
        }
    }
}

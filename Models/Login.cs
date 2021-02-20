using System.ComponentModel.DataAnnotations;

namespace Bank_Accounts
{
    public class Login
    {
        // Other fields
        [Required]
        public string Email {get; set;}
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

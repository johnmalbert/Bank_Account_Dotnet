using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank_Accounts.Models
{
    public class User
    {
        [Key]
        public int UserId{get;set;}
        [Required(ErrorMessage="First Name is Required.")]
        [MinLength(2)]
        public string FirstName{get;set;}

        [Required(ErrorMessage="Last Name is Required.")]
        [MinLength(2)]
        public string LastName{get;set;}

        [Required(ErrorMessage="Email Address is Required.")]
        [EmailAddress]
        public string Email{get;set;}
        
        [Required]
        [MinLength(8,ErrorMessage="Password must be at least eight characters.")]
        [DataType(DataType.Password)]
        public string Password{get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        public List<Transaction> Transactions {get;set;}

    }
}
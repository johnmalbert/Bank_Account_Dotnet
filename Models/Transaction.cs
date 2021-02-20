using System;
using System.ComponentModel.DataAnnotations;

namespace Bank_Accounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}
        [Required]
        public decimal Amount{get;set;}        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        public User Creator {get;set;}

    }
}
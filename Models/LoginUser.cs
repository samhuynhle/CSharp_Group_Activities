using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace qwerty.Models
{
    // Login User for users trying to log in, gathering email and password inputted
    // Use this to check database for current users and their passwords
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [DataType(DataType.Password)]
        [Required]
        public string Password {get;set;}
    }
}
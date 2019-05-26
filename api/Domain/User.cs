using Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class User : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }

        //audit
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

    }

    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress(ErrorMessage = ErrorConstants.InvalidEmail)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}

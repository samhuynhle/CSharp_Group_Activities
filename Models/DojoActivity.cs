using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace qwerty.Models
{
    public class DojoActivity
    {
        [Key]
        public int DojoActivityId {get;set;}
        [MinLength(8, ErrorMessage="Name must be 8 characters or longer")]
        [Required (ErrorMessage="DojoActivity required")]
        public string DojoActivityName {get;set;}
        [Required]
        [DataType(DataType.Date)]
        [DateValidation]
        public DateTime ActivityDate {get;set;}
        [Required]
        [DataType(DataType.Time)]
        public DateTime ActivityTime {get;set;}
        [Required]
        [TimeValidation]
        public int DurationInt {get;set;}
        [Required]
        public string DurationMeasure {get;set;}
        [Required]
        public string ActivityDescription {get;set;}
        [Required]
        public int CoordinatorId {get;set;}
        public User Coordinator {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Association> JoinedUsers {get;set;}
    }
    public class DateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime DoB = (DateTime)value; 
            DateTime today = DateTime.Now;
            if (today > DoB)
            {
                return new ValidationResult("You can't go back in time");    
            }

            return ValidationResult.Success;
        }
    }
    public class TimeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int time = (int)value;
            if(time < 0)
            {
                return new ValidationResult("You can't have negative time");
            }

            return ValidationResult.Success;
        }
    }
}
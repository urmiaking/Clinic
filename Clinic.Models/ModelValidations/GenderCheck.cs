using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.ModelValidations
{
    public class GenderCheck : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool isValid = !(value is string inputValue && !(inputValue.Equals("مرد") || inputValue.Equals("زن")));

            return isValid;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.ModelValidations
{
    public class ValidateAge : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool isValid = !(value is short inputValue && !(inputValue > 18 && inputValue < 150));

            return isValid;
        }
    }
}

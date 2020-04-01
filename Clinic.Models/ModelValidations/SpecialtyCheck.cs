using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.ModelValidations
{
    public class SpecialtyCheck : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool isValid = !(value is string inputValue &&
                             !(inputValue.Equals("متخصص اطفال") ||
                               inputValue.Equals("متخصص زنان") ||
                               inputValue.Equals("پزشک عمومی") ||
                               inputValue.Equals("چشم پزشک") ||
                               inputValue.Equals("دندان پزشک")));

            return isValid;
        }
    }
}

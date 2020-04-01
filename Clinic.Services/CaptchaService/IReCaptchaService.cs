using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Services.CaptchaService
{
    public interface IReCaptchaService
    {
        Task<bool> ValidateRecaptchaAsync(string gRecaptchaResponse);
    }
}

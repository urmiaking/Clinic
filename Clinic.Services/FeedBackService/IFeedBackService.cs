using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Services.FeedBackService
{
    public interface IFeedBackService
    {
        Task<bool> SendFeedBackAsync(string name, string email, string message);
    }
}

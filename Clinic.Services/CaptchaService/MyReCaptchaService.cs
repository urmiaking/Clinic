using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Clinic.Models.DomainClasses.Others;
using Newtonsoft.Json;

namespace Clinic.Services.CaptchaService
{
    public class MyReCaptchaService : IReCaptchaService
    {
        public async Task<bool> ValidateRecaptchaAsync(string gRecaptchaResponse)
        {
            string urlToPost = "https://www.google.com/recaptcha/api/siteverify";
            string secretKey = "6LcU6EIUAAAAAOI_sIfOOgkQZ2Amqo8jXbakTFnV";

            var postData = "secret=" + secretKey + "&response=" + gRecaptchaResponse;

            // send post data
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToPost);
            request.Method = "POST";
            request.ContentLength = postData.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            await using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                await streamWriter.WriteAsync(postData);
            }

            // receive the response now
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using var reader = new StreamReader(response.GetResponseStream() ?? throw new Exception());
                result = await reader.ReadToEndAsync();
            }

            // validate the response from Google reCaptcha
            var captchaResponse = JsonConvert.DeserializeObject<reCaptchaResponse>(result);

            return captchaResponse.Success;
        }
    }
}

using Clinic.DataContext;
using Clinic.Services.LoginService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clinic.Models.DomainClasses.Users;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Utilities.ModelBinders
{
    public class DoctorModelBinder : IModelBinder
    {
        private readonly AppDbContext _db;
        private readonly ILoginService _loginService;

        public DoctorModelBinder(AppDbContext db, ILoginService loginService)
        {
            _db = db;
            _loginService = loginService;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var request = bindingContext.HttpContext.Request;

            var dict = request.Form
                .ToDictionary(a => a.Key, a => a.Value);

            int index = Convert.ToInt32(dict["index"]);
            int id = Convert.ToInt32(dict["Doctors[" + index + "].Id"]);
            string fullName = dict["Doctors[" + index + "].FullName"];
            string email = dict["Doctors[" + index + "].Email"];
            string username = dict["Doctors[" + index + "].Username"];
            string password = dict["Doctors[" + index + "].Password"];
            string medicalNumber = dict["Doctors[" + index + "].MedicalNumber"];
            string nationalCode = dict["Doctors[" + index + "].NationalCode"];
            string phoneNumber = dict["Doctors[" + index + "].PhoneNumber"];
            string gender = dict["Doctors[" + index + "].Gender"];
            short age = Convert.ToInt16(dict["Doctors[" + index + "].Age"]);
            string specialty = dict["Doctors[" + index + "].Specialty"];
            string biography = dict["Doctors[" + index + "].Biography"];
            string books = dict["Doctors[" + index + "].Books"];
            string articles = dict["Doctors[" + index + "].Articles"];
            int score = Convert.ToInt32(dict["Doctors[" + index + "].Score"]);
            var picture = request.Form.Files["docImage"];


            password = password == null || string.IsNullOrWhiteSpace(password) ? _db.Doctors.Find(id).Password : password.Length >= 6 ? _loginService.GetHash(password) : null;

            var oldImagePath = _db.Doctors.Find(id).ProfilePic;

            var imageUrl = picture == null || picture.Length == 0
                ? oldImagePath : Guid.NewGuid() + Path.GetExtension(picture.FileName);

            var doctor = _db.Doctors.Find(id);

            doctor.Id = id;
            doctor.FullName = fullName;
            doctor.Email = email;
            doctor.Username = username;
            doctor.Password = password;
            doctor.MedicalNumber = medicalNumber;
            doctor.NationalCode = nationalCode;
            doctor.PhoneNumber = phoneNumber;
            doctor.Gender = gender;
            doctor.Age = age;
            doctor.Specialty = specialty;
            doctor.Score = score;
            doctor.Biography = biography;
            doctor.Books = books;
            doctor.Articles = articles;
            doctor.ProfilePic = imageUrl;

            var result = new DoctorProfilePicViewModel()
            {
                Doctor = doctor,
                Picture = picture,
                OldImagePath = oldImagePath
            };

            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Appointment;
using Clinic.Models.DomainClasses.Users;
using Microsoft.Extensions.Logging;

namespace Clinic.Services.InitService
{
    public class MyInitializerService: IInitializerService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<MyInitializerService> _logger;

        public MyInitializerService(AppDbContext db, ILogger<MyInitializerService> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<bool> InitializeDoctorWeekdaysAsync(Doctor doctor)
        {
            try
            {
                WeekDay saturday = new WeekDay()
                {
                    DayName = "شنبه",
                    EightTen = "خالی",
                    TenTwelve = "خالی",
                    TwelveFourteen = "خالی",
                    FourteenSixteen = "خالی",
                    Doctor = doctor
                };
                await _db.WeekDays.AddAsync(saturday);

                WeekDay sunday = new WeekDay()
                {
                    DayName = "یکشنبه",
                    EightTen = "خالی",
                    TenTwelve = "خالی",
                    TwelveFourteen = "خالی",
                    FourteenSixteen = "خالی",
                    Doctor = doctor
                };
                await _db.WeekDays.AddAsync(sunday);

                WeekDay monday = new WeekDay()
                {
                    DayName = "دوشنبه",
                    EightTen = "خالی",
                    TenTwelve = "خالی",
                    TwelveFourteen = "خالی",
                    FourteenSixteen = "خالی",
                    Doctor = doctor
                };
                await _db.WeekDays.AddAsync(monday);

                WeekDay tuesday = new WeekDay()
                {
                    DayName = "سه شنبه",
                    EightTen = "خالی",
                    TenTwelve = "خالی",
                    TwelveFourteen = "خالی",
                    FourteenSixteen = "خالی",
                    Doctor = doctor
                };
                await _db.WeekDays.AddAsync(tuesday);

                WeekDay wednesday = new WeekDay()
                {
                    DayName = "چهارشنبه",
                    EightTen = "خالی",
                    TenTwelve = "خالی",
                    TwelveFourteen = "خالی",
                    FourteenSixteen = "خالی",
                    Doctor = doctor
                };
                await _db.WeekDays.AddAsync(wednesday);

                WeekDay thursday = new WeekDay()
                {
                    DayName = "پنج شنبه",
                    EightTen = "خالی",
                    TenTwelve = "خالی",
                    TwelveFourteen = "خالی",
                    FourteenSixteen = "خالی",
                    Doctor = doctor
                };
                await _db.WeekDays.AddAsync(thursday);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Exception occured when adding doctor's weekdays message = {e.Message}");
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Others;
using Clinic.Models.DomainClasses.Users;
using Clinic.Services.MailService;
using Clinic.Utilities.EqualityComparers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Clinic.WebApplication.Hubs
{
    [Authorize(Roles = "Patient,Doctor")]
    public class ChatHub : Hub
    {
        private static readonly ConnectionMapping<string> Connections =
            new ConnectionMapping<string>();
        private readonly AppDbContext _db;
        private readonly IMailService _mailService;

        public ChatHub(AppDbContext db, IMailService mailService)
        {
            _db = db;
            _mailService = mailService;
        }

        public async Task SendChatMessage(string who, string message)
        {
            if (!Connections.GetConnections(who).Any())
            {
                await Clients.Caller.SendAsync("disconnected");
            }

            string name = Context.User.Identity.Name;

            foreach (var connectionId in Connections.GetConnections(who))
            {
                await Clients.Client(connectionId).SendAsync("receiveMessage", message);
            }
        }

        public async Task SendChatRequestToDoctor(string doctorName, string reserveDateTime, string patientName)
        {
            var doctor = _db.Doctors.FirstOrDefault(a => a.Username.Equals(doctorName));

            if (doctor != null)
            {
                if (!Connections.GetConnections(doctorName).Any())
                {
                    await Clients.Caller.SendAsync("doctorOffline");
                    if (!await _db.ChatRequests
                        .AnyAsync(a => 
                            a.PatientUsername.Equals(patientName) && a.Doctor.Id.Equals(doctor.Id)))
                    {
                        await _mailService.SendEmailAsync(doctor?.Email,
                            "بیماری درخواست گفتگو با شما را در سایت کلینیک بهار دارد لطفا به سایت مراجعه کنید",
                            "درخواست گفتگوی بیمار | کلینیک فوق تخصصی بهار");

                        var chatRequest = new ChatRequest()
                        {
                            Doctor = doctor,
                            PatientUsername = patientName
                        };

                        await _db.ChatRequests.AddAsync(chatRequest);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    DateTime.TryParse(reserveDateTime, out var reserveDateTimeResult);

                    var name = Context.User.Identity.Name;
                    var patientId = _db.Patients.FirstOrDefault(a => a.Username.Equals(name))?.Id;

                    foreach (var connectionId in Connections.GetConnections(doctorName))
                    {
                        await Clients.Client(connectionId)
                            .SendAsync("patientRequest", doctor.Id, patientId, reserveDateTimeResult);
                    }
                }
            }
        }

        public async Task SendConnectedNotificationToPatient(string patientName)
        {
            foreach (var connectionId in Connections.GetConnections(patientName))
            {
                await Clients.Client(connectionId).SendAsync("doctorConnected");
            }
        }

        public async Task ExitPatient(string doctorName)
        {
            foreach (var connectionId in Connections.GetConnections(doctorName))
            {
                await Clients.Client(connectionId).SendAsync("patientExited");
            }

            await OnDisconnectedAsync(new Exception("Patient Exited"));
        }

        public async Task ExitDoctor(string patientName)
        {
            foreach (var connectionId in Connections.GetConnections(patientName))
            {
                await Clients.Client(connectionId).SendAsync("doctorExited");
            }

            await OnDisconnectedAsync(new Exception("Doctor Exited"));
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Doctor"))
            {
                var chatRequests = await _db.ChatRequests
                    .Where(a =>
                        a.Doctor.Username.Equals(Context.User.Identity.Name))
                    .ToListAsync();
                if (chatRequests.Any())
                {
                    chatRequests = chatRequests.Distinct(new DistincChatRequestComparer()).ToList();
                    foreach (var chatRequest in chatRequests)
                    {
                        var patient = await _db.Patients
                            .FirstOrDefaultAsync(a =>
                                a.Username.Equals(chatRequest.PatientUsername));

                        var doctorFullName = _db.Doctors
                            .FirstOrDefault(a => a.Username.Equals(Context.User.Identity.Name))?.FullName;

                        await _mailService.SendEmailAsync(patient?.Email,
                            $"کاربر گرامی! هم اکنون دکتر {doctorFullName} آنلاین می باشند! می توانید به سایت مراجعه کرده و مجددا درخواست گفتگو دهید!",
                            "درخواست گفتگوی بیمار | کلینیک فوق تخصصی بهار");
                    }
                    _db.ChatRequests.RemoveRange(chatRequests);
                    await _db.SaveChangesAsync();
                }
            }
            string name = Context.User.Identity.Name;
            Connections.Add(name, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string name = Context.User.Identity.Name;
            Connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        //public Task OnReconnected()
        //{
        //    string name = Context.User.Identity.Name;

        //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(name, Context.ConnectionId);
        //    }
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clinic.DataContext;
using Clinic.Models.DomainClasses.Users;
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
        public readonly List<string> GroupsList;
        public readonly List<string> ConnectionIds;

        public ChatHub(AppDbContext db)
        {
            _db = db;
            GroupsList = new List<string>();
            ConnectionIds = new List<string>();
        }


        public async Task SendChatMessage(string who, string message)
        {
            string name = Context.User.Identity.Name;

            foreach (var connectionId in Connections.GetConnections(who))
            {
                await Clients.Client(connectionId).SendAsync("receiveMessage", message);
            }
        }

        public async Task SendChatRequestToDoctor(string doctorName)
        {
            string name = Context.User.Identity.Name;
            var patientId = _db.Patients.FirstOrDefault(a => a.Username.Equals(name))?.Id;
            var doctorId = _db.Doctors.FirstOrDefault(a => a.Username.Equals(doctorName))?.Id;
            foreach (var connectionId in Connections.GetConnections(doctorName))
            {
                await Clients.Client(connectionId).SendAsync("patientRequest", doctorId, patientId);
            }
        }

        public async Task SendConnectedNotificationToPatient(string patientName)
        {
            foreach (var connectionId in Connections.GetConnections(patientName))
            {
                await Clients.Client(connectionId).SendAsync("doctorConnected");
            }
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            Connections.Add(name, Context.ConnectionId);
            return base.OnConnectedAsync();
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
        public async Task BeginChat(string doctorConnectionId, string patientConnectionId)
        {
            var groupName = patientConnectionId + "mio" + doctorConnectionId;
            await Groups.AddToGroupAsync(doctorConnectionId, groupName);
            await Groups.AddToGroupAsync(patientConnectionId, groupName);
            GroupsList.Add(groupName);
            await Clients.Group(groupName).SendAsync("receiveMessage", "چت آغاز شد");
        }
    }
}

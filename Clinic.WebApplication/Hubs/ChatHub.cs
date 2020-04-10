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
        private readonly AppDbContext _db;
        public readonly List<User> Users;

        public ChatHub(AppDbContext db)
        {
            _db = db;
            Users = new List<User>();
        }
    }
}

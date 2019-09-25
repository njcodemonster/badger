using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Hubs
{
    public class ClaimtHub : Hub
    {
        [Authorize]
        public async Task SendClaim(object claim)
        {
            await Clients.All.SendAsync("PushClaim", claim);
        }
    }
}

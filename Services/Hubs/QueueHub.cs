using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    public class QueueHub : Hub
    {
        public async Task SendQueueStatusUpdate(int liveQueueId, string newStatus)
        {
            await Clients.All.SendAsync("ReceiveQueueStatusUpdate", liveQueueId, newStatus);
        }
    }
}
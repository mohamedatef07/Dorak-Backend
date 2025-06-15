using Microsoft.AspNetCore.SignalR;

namespace Hubs
{
    public class QueueHub : Hub
    {
        public async Task SendQueueStatusUpdate(int liveQueueId, string newStatus)
        {
            await Clients.All.SendAsync("ReceiveQueueStatusUpdate", liveQueueId, newStatus);
        }


        public async Task JoinShiftGroup(int shiftId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"shift_{shiftId}");
        }

        public async Task LeaveShiftGroup(int shiftId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"shift_{shiftId}");
        }
    }
}
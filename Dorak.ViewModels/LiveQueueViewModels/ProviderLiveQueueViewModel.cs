using Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Dorak.ViewModels
{ public class ProviderLiveQueueViewModel
    { public int LiveQueueId { get; set; } 
      public string ClientFullName { get; set; }
      public ClientType ClientType { get; set; }
      public TimeOnly EstimatedTime { get; set; } 
      public TimeOnly? ArrivalTime { get; set; }
      public QueueAppointmentStatus Status { get; set; }
      public string PhoneNumber { get; set; }
      public int? CurrentQueuePosition { get; set; } 
      public IEnumerable AvailableStatuses { get; set; }
      public string ProviderName { get; set; }


    } 
}
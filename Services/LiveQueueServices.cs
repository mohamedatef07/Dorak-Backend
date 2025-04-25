using Dorak.DataTransferObject;
using Dorak.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LiveQueueServices
    {
        private readonly LiveQueueRepository liveQueueRepository;

        public LiveQueueServices(LiveQueueRepository _liveQueueRepository)
        {
            liveQueueRepository = _liveQueueRepository;
        }

        //public List<GetQueueEntriesDTO> GetQueueEntries(Provider provider)
        //{

        //}
    }
}

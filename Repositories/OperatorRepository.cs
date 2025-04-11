using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.Identity.Client;

namespace Repositories
{
    public class OperatorRepository : BaseRepository<Operator>
    {
        CommitData commitData;
        private readonly ApiResponse<Operator> apiResponse;

        public OperatorRepository(DorakContext _dbContext, CommitData _commitData) : base(_dbContext)
        {
            commitData = _commitData;
        }

    }
}

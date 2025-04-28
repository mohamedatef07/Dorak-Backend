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
        private readonly CommitData commitData;

        public OperatorRepository(DorakContext _dbContext, CommitData _commitData) : base(_dbContext)
        {
            commitData = _commitData;
        }
        public Operator GetOperatorById(string operatorId)
        {
            return GetById(op => op.OperatorId == operatorId);
        }

    }
}

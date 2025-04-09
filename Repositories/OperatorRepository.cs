using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.Identity.Client;

namespace Repositories
{
    public class OperatorRepository : BaseRepository<Operator>
    {
        CommitData commitData;
        public OperatorRepository(DorakContext _dbContext, CommitData _commitData) : base(_dbContext)
        {
            commitData = _commitData;
        }

        public void SoftDelete(string operatorId)
        {
            var SelectedOperator = GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator != null)
            {
                SelectedOperator.IsDeleted = true;
                Edit(SelectedOperator);
                commitData.SaveChanges();
            }
        }

        public void RestoreOperator(string operatorId)
        {
            var SelectedOperator = GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator != null && SelectedOperator.IsDeleted == true) 
            {
                SelectedOperator.IsDeleted = false;
                Edit(SelectedOperator);
                commitData.SaveChanges();
            }
        }

        public bool IsExist(string operatorId) 
        {
            var SelectedOperator = GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator == null)
                return false;

            return true;
        }


    }
}

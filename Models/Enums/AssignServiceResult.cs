using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.Models.Enums
{
    public enum AssignServiceResult
    {
        Success,
        InvalidInput,
        ProviderNotFound,
        ServiceNotFound,
        CenterNotFound,
        AssignmentAlreadyExists,
        UnknownError
    }
}

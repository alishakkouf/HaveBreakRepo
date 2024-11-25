using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Shared.Exceptions
{
    /// <summary>
    /// Exception for business rules conflicts. 
    /// </summary>
    [Serializable]
    public class BusinessException(string message) : Exception(message)
    {
    }
}

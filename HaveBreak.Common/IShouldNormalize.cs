using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Common
{
    public interface IShouldNormalize
    {
        /// <summary>
        /// Normalize dto fields before calling the action method.
        /// </summary>
        void Normalize();
    }
}

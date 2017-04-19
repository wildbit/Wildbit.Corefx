using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Wildbit.Corefx
{
    public static class ExceptionDispatchInfoThrower
    {
        public static void Throw(Exception ex)
        {
            //TODO: this is not exactly the same as the implementation discussed here:
            // https://msdn.microsoft.com/en-us/library/system.runtime.exceptionservices.exceptiondispatchinfo.throw(v=vs.110).aspx
            // Perhaps figured out how to fix that.
            throw ex;
        }
    }
}

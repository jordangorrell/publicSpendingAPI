using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() {}

        public NotFoundException(string message) : base(message)
        {}
    }
}

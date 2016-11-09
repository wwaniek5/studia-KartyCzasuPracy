using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkDays.Domain.Exceptions
{
        [Serializable]
        public class NoWorkerException : Exception
        {
            public NoWorkerException()
            {
            }

            public NoWorkerException(string message) : base(message)
            {
            }

            public NoWorkerException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected NoWorkerException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    
}

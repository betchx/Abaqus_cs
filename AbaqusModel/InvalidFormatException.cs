using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abaqus
{
    class InvalidFormatException : Exception
    {

        public InvalidFormatException(string msg)
            : base(msg)
        {
        }

        public InvalidFormatException()
        {
        }

        public InvalidFormatException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        public InvalidFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


    }
}

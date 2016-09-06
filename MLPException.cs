using System;
using System.Runtime.Serialization;

namespace MultiLayerPerceptron
{
    [Serializable]
    internal class MLPException : Exception
    {
        public MLPException()
        {
        }

        public MLPException(string message) : base(message)
        {
        }

        public MLPException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MLPException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
    }
}
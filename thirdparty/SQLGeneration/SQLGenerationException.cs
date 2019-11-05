using System;
using System.Runtime.Serialization;

namespace SQLGeneration
{
    /// <summary>
    /// Represents an exception that is thrown when an error occurs within SQLGeneration.
    /// </summary>
    [DataContract]
    public class SQLGenerationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of a SQLGenerationException.
        /// </summary>
        public SQLGenerationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of a SQLGenerationException.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public SQLGenerationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SQLGenerationException.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">The exception that caused the exception.</param>
        public SQLGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

       
    }
}

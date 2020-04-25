using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Domain.Communication
{
    public class GenericResponse<T> : BaseResponse where T : class
    {
        public T Result { get; private set; }
        private GenericResponse(bool success, string message, T result) : base(success, message)
        {
            Result = result;
        }

        /// <summary>
        /// Success
        /// </summary>
        /// <param name="Result"></param>
        public GenericResponse(T result) : this(true, string.Empty, result)
        {

        }

        /// <summary>
        /// Error message
        /// </summary>
        /// <param name="message"></param>
        public GenericResponse(string message) : this(false, message, null)
        {

        }
    }
}
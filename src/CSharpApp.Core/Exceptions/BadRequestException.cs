using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Exceptions {
    public class BadRequestException : Exception {
        public string FullErrorResponse { get; }

        public BadRequestException(string message, string fullErrorResponse) : base(message) {
            FullErrorResponse = fullErrorResponse;
        }
    }
}

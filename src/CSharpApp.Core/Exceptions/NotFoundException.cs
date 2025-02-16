using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Exceptions {
    public class NotFoundException : Exception {
        public string FullErrorResponse { get; }

        public NotFoundException(string message, string fullErrorResponse)
            : base(message) {
            FullErrorResponse = fullErrorResponse;
        }
    }

}

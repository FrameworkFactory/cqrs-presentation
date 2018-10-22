
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FWF.ComponentModel
{
    public class ValidationResponse
    {
        public ValidationResponse()
        {
            this.Errors = new List<ValidationError>();
        }

        public string Message
        {
            get;
            set;
        }

        public IEnumerable<ValidationError> Errors
        {
            get;
            set;
        }
        
    }
}



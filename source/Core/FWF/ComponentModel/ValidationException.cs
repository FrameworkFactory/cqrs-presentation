using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace FWF.ComponentModel
{
    [Serializable]
    public class ValidationException : Exception
    {

        private readonly IEnumerable<ValidationError> _validationErrors;

        public ValidationException(IEnumerable<ValidationError> validationErrors, string message)
            : base(message)
        {
            _validationErrors = validationErrors;
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
        }

        public IEnumerable<ValidationError> ValidationErrors
        {
            get { return _validationErrors; }
        }

    }
}



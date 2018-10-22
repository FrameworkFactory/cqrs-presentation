using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using FWF.ComponentModel;

namespace FWF
{
    public static class ValidationExtensions
    {

        [DebuggerStepThrough]
        public static string RenderValidationErrors(this IEnumerable<ValidationError> validationResult)
        {
            var stringBuilder = new StringBuilder();

            foreach (var validationError in validationResult)
            {
                stringBuilder.AppendLine(
                    string.Concat(
                        validationError.PropertyName,
                        " = ",
                        validationError.ErrorMessage
                        )
                        );
            }

            return stringBuilder.ToString();
        }

    }
}




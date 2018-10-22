using System;
using System.Collections.Generic;

namespace FWF.ComponentModel 
{
    public interface IValidate
    {
        IEnumerable<ValidationError> Validate();
    }
}




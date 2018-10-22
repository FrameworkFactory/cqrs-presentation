using FWF.Json;
using System;

namespace FWF.CQRS
{
    public interface IEvent : IValidJson
    {

        Guid? EventId { get; set; }

        DateTime? EventTimestamp { get; set; }

        OperationMask Operation
        {
            get;
        }
        
    }
}



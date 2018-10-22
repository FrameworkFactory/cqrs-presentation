
using FWF.Json;

namespace FWF.CQRS
{
    public interface ICommandResponse : IJsonConvertable
    {
        int? ErrorCode { get; set; }

        string ErrorMessage { get; set; }
    }
}


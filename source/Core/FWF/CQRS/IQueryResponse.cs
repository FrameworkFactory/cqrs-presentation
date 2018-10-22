using FWF.Json;

namespace FWF.CQRS
{
    public interface IQueryResponse : IJsonConvertable
    {

        int? ErrorCode { get; set; }

        string ErrorMessage { get; set; }

    }
}



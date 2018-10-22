using System;

namespace FWF.Json
{
    public interface IJsonReader : IDisposable
    {

        JsonToken TokenType
        {
            get;
        }

        int Depth
        {
            get;
        }

        object Value
        {
            get;
        }


        bool Read();

        T Read<T>(T defaultValue);



        bool CloseInput
        {
            get; set;
        }
    }
}

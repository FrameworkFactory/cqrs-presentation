using System;

namespace FWF
{
    [Flags]
    public enum OperationMask : byte
    {
        None = 0x0,

        Select = 0x01,
        Insert = 0x02,
        Update = 0x04,
        Delete = 0x08,
        Import = 0x10,
        Export = 0x20,

        // Alias
        Read = Select,
        Write = Update,
        Add = Insert,
        Remove = Delete,
        
        All = Select | Insert | Update | Delete | Import | Export,
    }
}




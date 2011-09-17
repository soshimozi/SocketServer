using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.SharedObjects
{
    public enum ServerObjectDataType : int
    {
        Integer,
        Double,
        Float,
        String,
        Boolean,
        Byte,
        Character,
        Long,
        Short,
        BzObject,
        IntegerArray,
        DoubleArray,
        FloatArray,
        StringArray,
        BooleanArray,
        ByteArray,
        CharacterArray,
        LongArray,
        ShortArray,
        BzObjectArray
    }
}

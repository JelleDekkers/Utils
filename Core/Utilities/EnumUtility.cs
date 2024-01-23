using System;
using System.Collections.Generic;

namespace Utils.Core.Extensions
{
    public static class EnumUtility
    {
        public static IEnumerable<TEnum> GetEnumValues<TEnum>() => (TEnum[])Enum.GetValues(typeof(TEnum));
    }
}
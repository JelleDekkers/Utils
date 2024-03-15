using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.EnumDict
{
    /// <summary>
    /// A serializable dictionary that uses Enum as key, useable in the editor
    /// </summary>
    /// <typeparam name="TEnum">The enum</typeparam>
    /// <typeparam name="TData">Any serializable type</typeparam>
    [Serializable]
    public class EnumDict<TEnum, TData> : IEnumerable<EnumDict<TEnum, TData>.Entry> where TEnum : Enum
    {
#if UNITY_EDITOR
        [SerializeField] private bool isOpen;
#endif
        [SerializeField] private string enumTypeName;
        [SerializeField] private Entry[] entries;

        public EnumDict()
        {
            enumTypeName = typeof(TEnum).AssemblyQualifiedName;
            entries = EnumUtility.GetEnumValues<TEnum>()
                        .Select(it => new Entry(it, default))
                        .ToArray();
        }

        public TData Get(TEnum key) => entries.First(it => it.Enum.Equals(key)).Value;

        public TData this[TEnum key] => Get(key);

        public IEnumerator<Entry> GetEnumerator()
        {
            return ((IEnumerable<Entry>)entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        public struct Entry
        {
            [SerializeField] private TEnum @enum;
            [SerializeField] private TData value;

            public TEnum Enum => @enum;
            public TData Value => value;

            public Entry(TEnum @enum, TData value)
            {
                this.@enum = @enum;
                this.value = value;
            }
        }
    }
}

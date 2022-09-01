using System;
using System.Collections.Generic;
using System.Linq;

namespace Delobytes.Extensions.Configuration;

internal static class DictionaryExtensions
{
    ///<summary>Сравнивает два словаря используя сравнение их элементов.</summary>
    ///<param name="dictionary">Исходный словарь.</param>
    ///<param name="otherDictionary">Словарь для сравнения.</param>
    ///<returns>Одинаковы ли значения в обоих словарях.</returns>
    public static bool ContentEquals<TK, TV>(this IDictionary<TK, TV> dictionary, Dictionary<TK, TV> otherDictionary) where TK : notnull
    {
        return (otherDictionary ?? new Dictionary<TK, TV>())
            .OrderBy(kvp => kvp.Key)
            .SequenceEqual((dictionary ?? new Dictionary<TK, TV>())
                .OrderBy(kvp => kvp.Key));
    }
}

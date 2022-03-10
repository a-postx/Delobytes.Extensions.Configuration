using System;
using System.Collections.Generic;
using System.Linq;

namespace Delobytes.Extensions.Configuration;

internal static class DictionaryExtensions
{
    ///<summary>Получает значение ключа из словаря без выбрасывания исключения KeyNotFoundException.</summary>
    ///<param name="dict">Исходный словарь.</param>
    ///<param name="key">Ключ, значение которого нужно получить.</param>
    ///<param name="defaultValue">Значение по-умолчанию, которое нужно вернуть, если ключ не найден.</param>
    ///<returns>Значение ключа.</returns>
    public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default)
    {
        return dict.TryGetValue(key, out TV value) ? value : defaultValue;
    }

    ///<summary>Сравнивает два словаря используя сравнение их элементов.</summary>
    ///<param name="dictionary">Исходный словарь.</param>
    ///<param name="otherDictionary">Словарь для сравнения.</param>
    ///<returns>Одинаковы ли значения в обоих словарях.</returns>
    public static bool ContentEquals<TK, TV>(this IDictionary<TK, TV> dictionary, Dictionary<TK, TV> otherDictionary)
    {
        return (otherDictionary ?? new Dictionary<TK, TV>())
            .OrderBy(kvp => kvp.Key)
            .SequenceEqual((dictionary ?? new Dictionary<TK, TV>())
                .OrderBy(kvp => kvp.Key));
    }
}

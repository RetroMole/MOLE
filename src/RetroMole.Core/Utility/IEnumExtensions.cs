using System;
using System.Collections;

namespace RetroMole.Core.Utility;

public static class IEnumerableExtensions
{
    public static IEnumerable<TSource> ApplyIf<TSource>(
        this IEnumerable<TSource> enumerable,
        bool condition,
        Func<TSource, TSource> apply
        ) => condition
            ? enumerable.Apply(apply)
            : enumerable;
    public static IEnumerable<TSource> Apply<TSource>(
        this IEnumerable<TSource> enumerable,
        Func<TSource, TSource> apply
    ) => enumerable.Select(e => apply(e));
}

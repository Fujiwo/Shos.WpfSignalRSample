﻿namespace WpfSignalR.Models;

public static class EnumerableExtension
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
            action(item);
    }
}

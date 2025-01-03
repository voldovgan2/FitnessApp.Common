﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace FitnessApp.Common.Paged;

[ExcludeFromCodeCoverage]
public static class PagedExtension
{
    public static PagedDataModel<T> ToPaged<T>(this T[] data, GetPagedDataModel model)
    {
        var totalCount = data.Length;
        if (!string.IsNullOrWhiteSpace(model.SortBy))
        {
            var propertyInfo = typeof(T).GetProperty(model.SortBy);
            if (propertyInfo != null)
            {
                data = [
                    ..model.Asc ?
                        data.OrderBy(x => GetProperty(x, propertyInfo))
                        : data.OrderByDescending(x => GetProperty(x, propertyInfo))
                ];
            }
        }

        var items = data.Skip(model.Page * model.PageSize).Take(model.PageSize);
        return new PagedDataModel<T>
        {
            Page = model.Page,
            TotalCount = totalCount,
            Items = [.. items]
        };
    }

    private static object GetProperty<T>(T instance, PropertyInfo propertyInfo)
    {
        return propertyInfo.GetValue(instance, null);
    }
}

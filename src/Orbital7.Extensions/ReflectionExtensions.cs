﻿using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace System;

public static class ReflectionExtensions
{
    public static MemberInfo GetPropertyInformation(this Expression propertyExpression)
    {
        MemberExpression memberExpr = propertyExpression as MemberExpression;
        if (memberExpr == null)
        {
            UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
            if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
            {
                memberExpr = unaryExpr.Operand as MemberExpression;
            }
        }

        if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
        {
            return memberExpr.Member;
        }

        return null;
    }

    public static List<Type> GetTypes<T>(this Assembly assembly)
    {
        return assembly.GetTypes(typeof(T));
    }

    public static List<Type> GetTypes(this Assembly assembly, Type baseType)
    {
        var types = new List<Type>();
        string targetInterface = baseType.ToString();

        foreach (var assemblyType in assembly.GetTypes())
        {
            if ((assemblyType.IsPublic) && (!assemblyType.IsAbstract))
            {
                if (assemblyType.IsSubclassOf(baseType) || assemblyType.Equals(baseType) || (assemblyType.GetInterface(targetInterface) != null))
                    types.Add(assemblyType);
            }
        }

        return types;
    }

    public static T CreateInstance<T>(this Type type)
    {
        return (T)Activator.CreateInstance(type);
    }

    public static T CreateInstance<T>(this Type type, object[] parameters)
    {
        return (T)Activator.CreateInstance(type, parameters);
    }

    public static List<T> CreateInstances<T>(this List<Type> types)
    {
        var instances = new List<T>();

        foreach (Type typeItem in types)
        {
            try
            {
                T instance = typeItem.CreateInstance<T>();
                instances.Add(instance);
            }
            catch { }
        }

        return instances;
    }

    public static List<T> CreateInstances<T>(this Assembly assembly)
    {
        return assembly.GetTypes<T>().CreateInstances<T>();
    }

    public static List<T> CreateInstances<T>(this Assembly assembly, Type baseType)
    {
        return assembly.GetTypes(baseType).CreateInstances<T>();
    }

    public static List<Type> GetExternalTypes(this Type baseType)
    {
        return baseType.GetExternalTypes(ReflectionHelper.GetExecutingAssemblyFolderPath());
    }

    public static List<Type> GetExternalTypes(this Type baseType, string assembliesFolderPath)
    {
        List<Type> types = new List<Type>();

        foreach (string filePath in Directory.GetFiles(assembliesFolderPath, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(filePath);
                types.AddRange(assembly.GetTypes(baseType));
            }
            catch { }
        }

        return types;
    }

    public static string GetDisplayValue<TValue>(
        this MemberInfo memberInfo,
        TValue value,
        TimeConverter timeConverter,
        DisplayValueOptions options = null)
    {
        return CalculateDisplayValue(
            value,
            memberInfo.Name,
            memberInfo,
            timeConverter,
            options);
    }

    // TODO: Move to Orbital7.Extensions.
    public static string GetDisplayValue<TValue>(
        this TValue value,
        TimeConverter timeConverter,
        string propertyName = null,
        DisplayValueOptions options = null)
    {
        return CalculateDisplayValue(
            value,
            propertyName,
            null,
            timeConverter,
            options);
    }

    private static string CalculateDisplayValue<TValue>(
        TValue value,
        string propertyName,
        MemberInfo memberInfo,
        TimeConverter timeConverter,
        DisplayValueOptions displayValueOptions)
    {
        if (value != null)
        {
            var type = value.GetType();
            var options = displayValueOptions ?? new DisplayValueOptions();
            var dataTypeAttribute = memberInfo?.GetAttribute<DataTypeAttribute>(false);

            if (type.IsBaseOrNullableEnumType())
            {
                return (value as Enum).ToDisplayString();
            }
        #if NET8_0_OR_GREATER
            else if (type == typeof(DateOnly) || type == typeof(DateOnly?))
            {
                var date = (DateOnly)(object)value;
                if (options.DateTimeFormat.HasText())
                {
                    return new DateTime(date, new TimeOnly()).ToString(
                        options.DateTimeFormat);
                }
                else
                {
                    return new DateTime(date, new TimeOnly()).ToShortDateString();
                }
            }
            else if (type == typeof(TimeOnly) || type == typeof(TimeOnly?))
            {
                var time = (TimeOnly)(object)value;
                var dateTime = ToDateTime(
                        new DateTime(new DateOnly(), time),
                    propertyName,
                    timeConverter,
                    options);

                return dateTime.ToShortTimeString();
            }
        #endif
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var dateTime = ToDateTime(
                    (DateTime)(object)value,
                    propertyName,
                    timeConverter,
                    options);

                if (options.DateTimeFormat.HasText())
                {
                    return dateTime.ToString(options.DateTimeFormat);
                }
                else
                {
                    return dateTime.ToDefaultDateTimeString();
                }
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return ((bool)(object)value).ToYesNo();
            }
            else if (options.UseCurrencyForDecimals && (
                type == typeof(decimal) || type == typeof(decimal?)))
            {
                var currency = (decimal?)(object)value;
                if (currency.HasValue)
                {
                    return currency.Value.ToCurrencyString(options);
                }
            }
            else if (dataTypeAttribute?.DataType == DataType.Password)
            {
                return "0000000000000000".Mask();
            }
        }

        return value?.ToString();
    }

    private static DateTime ToDateTime(
        DateTime dateTime,
        string propertyName,
        TimeConverter timeConverter,
        DisplayValueOptions displayValueOptions)
    {
        DateTime value;

        // TODO: Should we just always do this and assuem the date/time is in UTC?
        if (propertyName?.EndsWith("Utc") ?? false)
        {
            DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        if (timeConverter != null)
        {
            if (displayValueOptions.TimeZoneId.HasText())
            {
                value = timeConverter.ToDateTime(dateTime, displayValueOptions.TimeZoneId);
            }
            else
            {
                value = timeConverter.ToLocalDateTime(dateTime);
            }
        }
        else
        {
            if (displayValueOptions.TimeZoneId.HasText())
            {
                value = dateTime.UtcToTimeZone(
                    TimeZoneInfo.FindSystemTimeZoneById(displayValueOptions.TimeZoneId));
            }
            else
            {
                value = dateTime.ToLocalTime();
            }
        }

        return value;
    }
}

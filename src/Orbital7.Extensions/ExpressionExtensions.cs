﻿using System.ComponentModel.DataAnnotations;

namespace System.Linq.Expressions;

public static class ExpressionExtensions
{
    public static TAttribute GetAttribute<TProperty, TAttribute>(
       this Expression<Func<TProperty>> expression)
       where TAttribute : Attribute
    {
        return expression.Body
            .GetPropertyInformation()
            .GetAttribute<TAttribute>(isRequired: false);
    }

    public static bool HasAttribute<TProperty, TAttribute>(
        this Expression<Func<TProperty>> expression)
        where TAttribute : Attribute
    {
        return expression.HasAttribute(typeof(TAttribute));
    }

    public static bool HasAttribute<TProperty>(
        this Expression<Func<TProperty>> expression, 
        Type attributeType)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member
                .GetCustomAttributes(attributeType, inherit: false)
                .Length > 0;
        }

        return false;
    }

    public static string GetDisplayName<TProperty>(
        this Expression<Func<TProperty>> expression)
    {
        var memberInfo = expression.Body.GetPropertyInformation();
        return memberInfo.GetDisplayName();
    }

    public static DisplayAttribute GetDisplayAttribute<TProperty>(
        this Expression<Func<TProperty>> expression)
    {
        var memberInfo = expression.Body.GetPropertyInformation();
        return memberInfo.GetDisplayAttribute();
    }
}

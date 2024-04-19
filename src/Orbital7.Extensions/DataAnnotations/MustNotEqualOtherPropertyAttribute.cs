﻿using System.Globalization;

namespace System.ComponentModel.DataAnnotations;

public class MustNotEqualOtherPropertyAttribute : ValidationAttribute
{
    public string OtherProperty { get; private set; }

    public MustNotEqualOtherPropertyAttribute(
        string otherProperty)
    {
        OtherProperty = otherProperty;
    }

    protected override ValidationResult IsValid(
        object value, 
        ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetRuntimeProperty(OtherProperty);
        if (property == null)
        {
            return new ValidationResult(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} is unknown property",
                    OtherProperty
                )
            );
        }
        var otherValue = property.GetValue(validationContext.ObjectInstance, null);
        if (object.Equals(value, otherValue))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
        }
        return null;
    }
}

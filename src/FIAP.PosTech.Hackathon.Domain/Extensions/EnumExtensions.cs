using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FIAP.PosTech.Hackathon.Domain.Extensions;

[ExcludeFromCodeCoverage]
public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        return enumValue
            .GetType()
            .GetMember(enumValue.ToString())[0]
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? string.Empty;
    }

    public static T? GetAttribute<T>(this Enum objValue) where T : Attribute
    {
        var type = objValue.GetType();
        var field = type.GetField(objValue.ToString());

        return field?.GetCustomAttribute<T>();
    }

    public static string GetName(this Enum enumValue)
    {
        var attr = GetAttribute<DisplayNameAttribute>(enumValue);

        return attr == null ? enumValue.ToString() : attr.DisplayName;
    }

    public static string GetDisplay(this Enum value, string propertyName = "Name")
    {
        var field = value.GetType().GetField(value.ToString());

        if (field == null)
            return string.Empty;

        var attributes = field.GetCustomAttributes(false);

        if (attributes.Length == 0)
            return string.Empty;

        var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();

        if (displayAttribute == null)
            return string.Empty;

        var propertyValue = (string?)displayAttribute.GetType().GetProperty(propertyName)?.GetValue(displayAttribute, null) ?? string.Empty;

        return propertyValue ?? value.ToString();
    }

    public static string? GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var member = type.GetMember(enumValue.ToString()).FirstOrDefault();

        if (member != null)
        {
            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute != null)
                return displayAttribute.GetName();
        }

        return enumValue.ToString();
    }

    public static string GetDescriptions(this Enum value)
    {
        var descriptionAttribute = value.GetAttribute<DescriptionAttribute>();
        return descriptionAttribute != null ? descriptionAttribute.Description : value.ToString();
    }
}
using System.ComponentModel;

namespace Mistruna.Core.Helpers;

public static class EnumDescriptionHelper
{
    public static async Task<string[]> GetEnumDescriptionsAsync(IEnumerable<Enum> enumValues)
        => await Task.WhenAll(enumValues.Select(GetEnumDescriptionAsync).ToArray());

    private static async Task<string> GetEnumDescriptionAsync(Enum value)
        => await Task.Run(() =>
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null
                ? value.ToString()
                : attribute.Description;
        });
}

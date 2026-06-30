using System.ComponentModel;

namespace Mistruna.Core.Contracts.Helpers;

/// <summary>
/// Provides helper methods for retrieving descriptive text from enum values
/// decorated with <see cref="System.ComponentModel.DescriptionAttribute"/>.
/// </summary>
public static class EnumDescriptionHelper
{
    /// <summary>
    /// Asynchronously retrieves the description strings for a collection of enum values.
    /// </summary>
    /// <param name="enumValues">The enum values to process.</param>
    /// <returns>An array of description strings (falls back to <see cref="Enum.ToString()"/> when no attribute is present).</returns>
    public static async Task<string[]> GetEnumDescriptionsAsync(IEnumerable<Enum> enumValues)
        => await Task.WhenAll(enumValues.Select(GetEnumDescriptionAsync).ToArray());

    private static async Task<string> GetEnumDescriptionAsync(Enum value)
    {
        return await Task.Run(() =>
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null)
            {
                return value.ToString();
            }

            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute is null
                ? value.ToString()
                : attribute.Description;
        });
    }
}

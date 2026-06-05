using ExampleUmbraco.Extension.UmbracoMapping.UmbracoConverter;
using Our.Umbraco.Vorto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ExampleUmbraco.Extension.UmbracoMapping
{
    public static class PublishContentMapper
    {
        // For List Mapping
        public static List<T> ContentMapToList<T>(this IEnumerable<IPublishedContent> contents) where T : new()
        {
            if (contents == null || !contents.Any()) return new List<T>();

            // Use LINQ to loop and map for each
            return contents.Select(content => content.ContentMapTo<T>()).ToList();
        }

        public static T ContentMapTo<T>(this IPublishedContent content) where T : new()
        {
            if (content == null) return default;

            // Create object
            T targetObject = new T();
            Type targetType = typeof(T);

            // Get all properties of a class
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                // If property is read-only, then skip
                if (!prop.CanWrite) continue;

                // Get Name of current C# property
                string alias = prop.Name;

                // Convert to camelCase due to C# auto use PascalCase
                string lowerAlias = char.ToLowerInvariant(alias[0]) + alias.Substring(1);

                // Check IPublishedContent have property match the c# alias name
                if (!content.HasProperty(alias) && !content.HasProperty(lowerAlias))
                    continue;

                string finalAlias = content.HasProperty(alias) ? alias : lowerAlias;

                // * Vorto is a plugin used to wrap Umbraco properties for Multilingual feature *
                var hasVorto = content.HasVortoValue(finalAlias);
                try
                {
                    var propertyType = content.ContentType.GetPropertyType(finalAlias);
                    string editorAlias = propertyType != null ? propertyType.PropertyEditorAlias : string.Empty;

                    // --------------------[ HANDLE IMAGE ]--------------------
                    if (editorAlias.Contains("MediaPicker") || editorAlias.Contains("ImageCropper"))
                    {
                        // Must call <IPublishedContent> to get Umbraco object instead an object Id
                        var mediaItem = content.GetPropertyValue<IPublishedContent>(finalAlias);
                        if (mediaItem != null && prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(targetObject, mediaItem.Url);
                        }

                        continue;
                    }

                    // --------------------[ CUSTOM CONVERTER ]--------------------
                    var converterAttr = prop.GetCustomAttribute<UmbracoConverterAttribute>();
                    if (converterAttr != null)
                    {
                        // UseRawDataValue = true: bypass Umbraco/Vorto, read raw DataValue
                        object rawInput = converterAttr.UseRawDataValue ?
                            content.GetProperty(finalAlias)?.DataValue  // Converter auto handle unwrap Vorto
                            : hasVorto ?
                                content.GetVortoValue(finalAlias)
                                : content.GetPropertyValue(finalAlias);    // Fallback to base umbraco if value is simple Vorto

                        // If have data, parse into C# object
                        if (rawInput != null)
                        {
                            var converter = Activator.CreateInstance(converterAttr.ConverterType)
                                            as IUmbracoPropertyConverter;
                            prop.SetValue(targetObject, converter?.ConvertRaw(rawInput));
                        }
                        continue;
                    }

                    // --------------------[ DEFAULT: VORTO → NORMAL ]--------------------
                    // Check and get Vorto Value if exist / else Get normal Umbraco Value
                    object finalContentValue = hasVorto ? content.GetVortoValue(finalAlias) : content.GetPropertyValue(finalAlias);
                    // Asign content value into model if != null
                    if (finalContentValue != null)
                    {
                        if (prop.PropertyType.IsAssignableFrom(finalContentValue.GetType()))
                        {
                            prop.SetValue(targetObject, finalContentValue);
                        }
                        else
                        {
                            var convertedValue = Convert.ChangeType(finalContentValue, prop.PropertyType);
                            prop.SetValue(targetObject, convertedValue);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return targetObject;
        }
    }
}
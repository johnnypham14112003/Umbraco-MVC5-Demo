using Our.Umbraco.Vorto.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Configuration;
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
            string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

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
                    TypeConverter customConverter = null;

                    // Check if the property assigned [TypeConverter(...)] explicitly
                    bool hasCustomAttribute = TypeDescriptor.GetProperties(targetObject)[prop.Name]
                                                .Attributes.OfType<TypeConverterAttribute>().Any();

                    if (hasCustomAttribute)
                        customConverter = TypeDescriptor.GetProperties(targetObject)[prop.Name].Converter;

                    // --------------------[ HANDLE IMAGE ]--------------------
                    var propertyType = content.ContentType.GetPropertyType(finalAlias);
                    string editorAlias = propertyType != null ? propertyType.PropertyEditorAlias : string.Empty;

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

                    // --------------------[ RELATED LINK (UMBRACO UID ERROR) ]--------------------
                    // * This section for handle including null internal link (umbraco uid parse error)
                    if (customConverter != null)
                    {
                        var umbProperty = content.GetProperty(finalAlias);

                        if (umbProperty != null && umbProperty.DataValue is string rawString && customConverter.CanConvertFrom(typeof(string)))
                        {
                            var convertedValue = customConverter.ConvertFrom(rawString);
                            prop.SetValue(targetObject, convertedValue);
                            continue;
                        }
                    }

                    // --------------------[ DEFAULT: VORTO → NORMAL(DÙNG UMBRACO PARSED DATA) ]--------------------
                    // Assign content value into model if != null
                    object finalContentValue = hasVorto ? content.GetVortoValue(finalAlias) : content.GetPropertyValue(finalAlias);
                    if (finalContentValue != null)
                    {
                        if (prop.PropertyType.IsAssignableFrom(finalContentValue.GetType()))
                        {
                            prop.SetValue(targetObject, finalContentValue);
                        }
                        // Custom Converter not null and can handle Assigned object Umbraco
                        else if (customConverter != null && customConverter.CanConvertFrom(finalContentValue.GetType()))
                        {
                            var convertedValue = customConverter.ConvertFrom(finalContentValue);
                            prop.SetValue(targetObject, convertedValue);
                        }
                        // Fallback: Default convert of C# (int, bool,...)
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
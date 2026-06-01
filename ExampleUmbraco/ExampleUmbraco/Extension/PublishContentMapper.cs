using Our.Umbraco.Vorto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace ExampleUmbraco.Extension
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

            // Create object by Activator 
            T targetObject = new T();
            Type targetType = typeof(T);

            // Get all properties of a class
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                // If property is read-only, then skip
                if (!prop.CanWrite) continue;

                // Use c# attribute name as alias name of Umbraco (camelCase || match name)
                string alias = prop.Name;

                // Convert to camelCase due to c# auto use PascalCase
                string lowerAlias = char.ToLowerInvariant(alias[0]) + alias.Substring(1);

                // Check IPublishedContent have property match the c# alias name
                if (!content.HasProperty(alias) && !content.HasProperty(lowerAlias))
                    continue;

                string finalAlias = content.HasProperty(alias) ? alias : lowerAlias;

                // * Vorto is a plugin used to wrap Umbraco properties for Multilingual feature *
                try
                {
                    //var umbProperty = content.GetProperty(finalAlias);
                    //string editorAlias = umbProperty != null ? umbProperty.PropertyType.PropertyEditorAlias : string.Empty;

                    /* => This umbProperty is an interface <IPublishedProperty>, this interface not declare Property Type
                     *
                     * So skip this and call get directly through the structure ContentType like below
                     */

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

                    // Check and get Vorto Value if exist / get normal Umbraco Value
                    object finalValue = content.GetPropertyValue(finalAlias); ;
                    if (content.HasVortoValue(finalAlias))
                    { finalValue = content.GetVortoValue(finalAlias); }

                    // Asign value into model
                    if (finalValue != null)
                    {
                        if (prop.PropertyType.IsAssignableFrom(finalValue.GetType()))
                        {
                            prop.SetValue(targetObject, finalValue);
                        }
                        else
                        {
                            var convertedValue = Convert.ChangeType(finalValue, prop.PropertyType);
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
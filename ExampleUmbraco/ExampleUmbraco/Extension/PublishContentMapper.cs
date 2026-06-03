using ExampleUmbraco.Extension.UmbracoConverter;
using Newtonsoft.Json.Linq;
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

            // Create object
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

                    // --------------------[ BYPASS UMBRACO IMPLICIT CONVERTER ]--------------------
                    var converterAttr = prop.GetCustomAttribute<UmbracoConverterAttribute>();
                    if (converterAttr != null &&
                       (converterAttr.ConverterType == typeof(RelatedLinkListConverter) || converterAttr.ConverterType == typeof(RelatedLinkConverter)))
                    {
                        string rawJsonToConvert = null;
                        var propData = content.GetProperty(finalAlias);

                        if (propData != null && propData.DataValue != null)
                        {
                            string rawString = propData.DataValue.ToString();

                            // Check if the raw data is JSON of Vorto
                            if (rawString.Contains("\"values\":") && rawString.Contains("\"dtdGuid\":"))
                            {
                                try
                                {
                                    var vortoJson = JObject.Parse(rawString);
                                    var valuesObj = vortoJson["values"] as JObject;

                                    // Count: how many lingual
                                    if (valuesObj != null && valuesObj.Count > 0)
                                    {
                                        // Get current culture
                                        var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

                                        // Get JSON of current culture, if null -> fallback get first language found
                                        var cultureToken = valuesObj[currentCulture] ?? valuesObj.Properties().First().Value;
                                        rawJsonToConvert = cultureToken?.ToString();
                                    }
                                }
                                catch { /* skip parse, fallback to null */ }
                            }
                            else
                            {   // Use normal JOSN of umbraco
                                rawJsonToConvert = rawString;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(rawJsonToConvert))
                        {
                            var converter = Activator.CreateInstance(converterAttr.ConverterType) as IUmbracoPropertyConverter;
                            var converted = converter.ConvertRaw(rawJsonToConvert);
                            prop.SetValue(targetObject, converted);
                        }
                        continue;
                    }

                    // Check and get Vorto Value if exist / else Get normal Umbraco Value
                    object finalValue = null;
                    if (content.HasVortoValue(finalAlias))
                    {
                        finalValue = content.GetVortoValue(finalAlias);
                    }
                    else
                    {
                        finalValue = content.GetPropertyValue(finalAlias);
                    }

                    // Asign content value into model if != null
                    if (finalValue != null)
                    {
                        // --------------------[ HANDLE RELATED LINK ]--------------------
                        converterAttr = prop.GetCustomAttribute<UmbracoConverterAttribute>();
                        if (converterAttr != null)
                        {
                            // Create instance converter by Activator
                            var converter = Activator.CreateInstance(converterAttr.ConverterType) as IUmbracoPropertyConverter;
                            if (converter != null)
                            {
                                var converted = converter.ConvertRaw(finalValue);
                                prop.SetValue(targetObject, converted);
                                continue;
                            }
                        }

                        // --------------------[ HANDLE OTHER ]--------------------
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
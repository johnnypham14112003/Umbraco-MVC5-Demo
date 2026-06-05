using System;
using System.Collections.Generic;

namespace ExampleUmbraco.Extension.UmbracoMapping.UmbracoConverter
{
    /// <summary><![CDATA[
    /// + Attach this attribute to property in model to asign specific converter.
    /// + Converter must implement IUmbracoPropertyConverter<T>.
    /// + Example:
    ///     [UmbracoConverter(typeof(RelatedLinkListConverter), UseRawDataValue = true)]
    ///     public List<RelatedLink> ListNavigators { get; set; }
    ///     
    ///     [UmbracoConverter(typeof(SomeOtherConverter))]
    ///     public MyType SomeOtherProp { get; set; }
    /// ]]></summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        /// <summary>
        /// True: extension will read raw DataValue (bypass Umbraco/Vorto implicit converter).<para/>
        /// Use when Umbraco converter can't parse JSON Vorto data type (eg: RelatedLinks + Vorto).
        /// </summary>
        public bool UseRawDataValue { get; set; } = false;

        public UmbracoConverterAttribute(Type converterType)
        {
            if (!typeof(IUmbracoPropertyConverter).IsAssignableFrom(converterType))
                throw new ArgumentException(
                    $"{converterType.Name} must implement IUmbracoPropertyConverter<T>",
                    nameof(converterType));

            ConverterType = converterType;
        }
    }
}
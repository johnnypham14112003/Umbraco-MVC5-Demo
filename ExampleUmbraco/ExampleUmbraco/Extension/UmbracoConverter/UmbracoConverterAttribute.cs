using System;

namespace ExampleUmbraco.Extension.UmbracoConverter
{
    /// <summary>
    /// Attach this attribute to property in model to asign specific converter.<para/>
    /// Converter must implement IUmbracoPropertyConverter&lt;T&gt;.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

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
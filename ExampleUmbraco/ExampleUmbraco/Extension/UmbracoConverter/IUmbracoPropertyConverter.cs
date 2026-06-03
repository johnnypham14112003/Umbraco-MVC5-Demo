namespace ExampleUmbraco.Extension.UmbracoConverter
{
    /// <summary>
    /// Implement this interface to custom convert any Umbraco raw value.
    /// </summary>
    public interface IUmbracoPropertyConverter<out T>
    {
        T Convert(object rawValue);
    }

    /// <summary>
    /// For extension class use reflection without <T>
    /// </summary>
    public interface IUmbracoPropertyConverter
    {
        object ConvertRaw(object rawValue);
    }
}

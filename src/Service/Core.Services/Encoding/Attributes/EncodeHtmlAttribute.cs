using System;

namespace Core.Services.Encoding.Attributes
{
    /// <summary>
    /// indica che questa proprietà deve essere sottoposta all' HtmlEncode
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncodeHtmlAttribute : Attribute { }
}

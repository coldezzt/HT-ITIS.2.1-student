using System.Reflection;

namespace Hw7.FormComponents;

public sealed class HtmlInput : HtmlField
{
    public HtmlInput(PropertyInfo propInfo) : base(propInfo) 
    { }
    
    protected override string GetHtmlField()
    {
        var inputType = Type == typeof(string) ? "text" : "number";
        return $"<input class=\"form-control\" type=\"{inputType}\"  name=\"{Name}\" id=\"{Name}\"/>";
    }
}
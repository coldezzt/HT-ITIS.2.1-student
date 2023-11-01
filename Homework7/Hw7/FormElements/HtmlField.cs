using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Hw7.FormComponents;

public abstract class HtmlField
{
    public string Name { get; }
    public Type Type { get; }
    public string? Label { get; }
    
    protected HtmlField(PropertyInfo propInfo)
    {
        Name = propInfo.Name;
        Type = propInfo.PropertyType;
        
        var displayAttr = (DisplayAttribute?) propInfo
            .GetCustomAttributes()
            .FirstOrDefault(x => x is DisplayAttribute);
        Label = displayAttr?.Name;
    }
    
    public string GetHtmlContent()
    {
        var content = "";
        content += GetHtmlLabel();
        content += GetHtmlField();
        return content;
    }
    
    private string GetHtmlLabel()
    {
        var l = Label is not null && Label != string.Empty
            ? Label
            : string.Concat(Name.Select(x => char.IsUpper(x) ? $" {x}" : x.ToString()))
              .Trim();
        return $"<label for=\"{Name}\">{l}</label>";
    }

    protected abstract string GetHtmlField();
}
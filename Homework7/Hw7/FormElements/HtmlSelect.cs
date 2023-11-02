using System.Reflection;
using System.Text;

namespace Hw7.FormComponents;

public sealed class HtmlSelect : HtmlField
{
    public HtmlSelect(PropertyInfo propInfo) : base(propInfo) 
    { }

    protected override string GetHtmlField()
    {
        var outputString = $"<select class=\"form-control\" name=\"{Name}\" required>";
        foreach (var value in Enum.GetValues(Type))
            outputString += $"<option value=\"{value}\">{value}</option>";

        outputString += "</select>";
        return outputString;
    }
}
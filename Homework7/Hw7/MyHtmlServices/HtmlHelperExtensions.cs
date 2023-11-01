using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Hw7.FormComponents;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hw7.MyHtmlServices;

public static class HtmlHelperExtensions
{
    public static IHtmlContent MyEditorForModel(this IHtmlHelper helper)
    {
        var model = helper.ViewData.Model;
        var modelType = helper.ViewData.ModelMetadata.ModelType;

        var result = "";
        foreach (var propInfo in modelType.GetProperties())
        {
            result += "<div class=\"form-group\">";
            HtmlField field =  propInfo.PropertyType.IsEnum 
                ? new HtmlSelect(propInfo)
                : new HtmlInput(propInfo);
            result += field.GetHtmlContent();

            var validationResult = CheckProperty(propInfo, model);

            result +=  validationResult is not null 
                ? $"<span class=\"text-danger\">{validationResult}</small>"
                : "";
            
            result += "</div>";
        }

        return new HtmlString(result);
    }

    private static string? CheckProperty(PropertyInfo propInfo, object? model)
    {
        return model is null 
            ? null 
            : propInfo
                .GetCustomAttributes()
                .OfType<ValidationAttribute>()
                .FirstOrDefault(x => 
                    !x.IsValid(propInfo.GetValue(model)))
                ?.ErrorMessage;
    }
}
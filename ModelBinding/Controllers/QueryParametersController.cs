using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModelBinding.Controllers;

[ApiController]
[Route("filter")]
public class QueryParametersController : ControllerBase
{
    [HttpGet]
    public IList<JObject> Get([ModelBinder(BinderType = typeof(QueryParameterModelBinder))] QueryParameters queryParameters, IList<JObject> list)
    {
        List<FilterParameters> filters = queryParameters.FilterParametersList.ToList();

        foreach (var filter in filters)
        {
            switch (filter!.Type)
            {
                case "startswith":
                    list = list.Where(jo => jo.Property(filter.Column).Value.ToString().StartsWith(filter.Value)).ToList();
                    break;
                case "endswith":
                    list = list.Where(jo => jo.Property(filter.Column).Value.ToString().EndsWith(filter.Value)).ToList();
                    break;
                case "contains":
                    list = list.Where(jo => jo.Property(filter.Column).Value.ToString().Contains(filter.Value)).ToList();
                    break;
                case "equals":
                    list = list.Where(jo => jo.Property(filter.Column).Value.ToString().Equals(filter.Value)).ToList();
                    break;

                default:
                    break;
            }
        }

        if (queryParameters.OrderBy is not null)
        {
            if (queryParameters.IsDescending == true)
            {
                list = list.OrderByDescending(jo => jo.Property(queryParameters.OrderBy).Value.ToString()).ToList();
            }
            else
            {
                list = list.OrderBy(jo => jo.Property(queryParameters.OrderBy).Value.ToString()).ToList();
            }
        }

        return list;
    }
    // TODO expressions, delegates, functions, lambdas
}

public class QueryParameterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        ValueProviderResult filtersValueProviderResult = bindingContext.ValueProvider.GetValue("filters");
        ValueProviderResult orderByValueProviderResult = bindingContext.ValueProvider.GetValue("orderBy");
        ValueProviderResult descendingValueProviderResult = bindingContext.ValueProvider.GetValue("descending");
        ValueProviderResult isConjunctionValueProviderResult = bindingContext.ValueProvider.GetValue("isConjunction");

        if (filtersValueProviderResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        string? filtersValueAsString = filtersValueProviderResult.FirstValue;

        if (filtersValueAsString is null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        List<FilterParameters>? filters = JsonConvert.DeserializeObject<List<FilterParameters>>(filtersValueAsString);

        QueryParameters queryParameters = new()
        {
            OrderBy = orderByValueProviderResult.FirstValue,
            IsDescending = bool.Parse(descendingValueProviderResult.FirstValue!),
            IsConjunction = bool.Parse(isConjunctionValueProviderResult.FirstValue!),
            FilterParametersList = filters!
        };

        if (queryParameters is null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(queryParameters);

        return Task.CompletedTask;
    }
}
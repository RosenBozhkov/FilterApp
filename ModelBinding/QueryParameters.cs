namespace ModelBinding;

public class QueryParameters
{
    public string? OrderBy { get; set; }

    public bool IsDescending { get; set; } = false;

    public bool IsConjunction { get; set; } = false;

    public List<FilterParameters> FilterParametersList { get; set; }
}
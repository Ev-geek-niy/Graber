using System.Globalization;
using System.Text.RegularExpressions;

namespace Graber.Infrastructure.Hls;

internal sealed class HlsAttributes
{
    private static readonly Regex AttributeRegex = new("""(?<name>[A-Z0-9-]+)=(?:"(?<quotedValue>[^"]*)"|(?<value>[^,]*))""");
    private static readonly Regex TagRegex = new("^(?<tag>#[A-Z0-9-]+)(?=:|$)", RegexOptions.CultureInvariant);

    private readonly Dictionary<string, string> _attributes;
    
    public string Tag { get; }
    
    private HlsAttributes(string tag, Dictionary<string, string> attributes)
    {
        Tag = tag;
        _attributes = attributes;
    }
    
    public static HlsAttributes Parse(string line)
    {
        line = line.Trim();

        var tagMatch = TagRegex.Match(line);
        if (!tagMatch.Success)
            throw new FormatException("Line does not contain a valid HLS tag.");

        var tag = tagMatch.Groups["tag"].Value;
        var attributes = new Dictionary<string, string>();
        var attributeMatches = AttributeRegex.Matches(line);
        foreach (Match match in attributeMatches)
        {
            if (match.Groups["quotedValue"].Success)
                attributes.TryAdd(match.Groups["name"].Value, match.Groups["quotedValue"].Value);
            if (match.Groups["value"].Success)
                attributes.TryAdd(match.Groups["name"].Value, match.Groups["value"].Value);
        }

        return new HlsAttributes(tag, attributes);
    }
    
    public string RequiredString(string key)
    {
        if (!_attributes.TryGetValue(key, out var str))
            throw new FormatException($"{Tag} attribute {key} is required.");
        
        if (string.IsNullOrWhiteSpace(str))
            throw new FormatException($"{Tag} attribute {key} must not be null or whitespace.");

        return str;
    }
    
    public string? OptionalString(string key) => _attributes.GetValueOrDefault(key);
    
    private int RequiredInt(string key)
    {
        if (!_attributes.TryGetValue(key, out var value))
            throw new FormatException($"{Tag} attribute {key} is required.");

        if (!int.TryParse(value, CultureInfo.InvariantCulture, out var result))
            throw new FormatException($"Value of {key} attribute must be an integer.");
        
        return result;
    }

    public int RequiredPositiveInt(string key)
    {
        var result = RequiredInt(key);

        return result > 0
            ? result
            : throw new FormatException($"{Tag} attribute {key} must be greater than 0.");
    }

    private int? OptionalInt(string key)
    {
        if (!_attributes.TryGetValue(key, out var value))
            return null;
        
        if (!int.TryParse(value, CultureInfo.InvariantCulture, out var result))
            throw new FormatException($"{Tag} attribute {key} must be an integer.");
        
        return result;
    }

    public int? OptionalPositiveInt(string key)
    {
        var result = OptionalInt(key);
        return result switch
        {
            null => null,
            > 0 => result,
            <= 0 => throw new FormatException($"{Tag} attribute {key} must be greater than 0.")
        };
    }
   
    public Uri RequiredUri(string key)
    {
        if (!_attributes.TryGetValue(key, out var uriStr))
            throw new FormatException($"{Tag} attribute {key} is required.");
     
        if (string.IsNullOrWhiteSpace(uriStr))
            throw new FormatException($"{Tag} attribute {key} must not be empty.");
        
        if (!Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out var uri))
            throw new FormatException($"{Tag} attribute {key} must be a valid URI.");
        
        return uri;
    }
    
    public Resolution? OptionalResolution(string key)
    {
        if (!_attributes.TryGetValue(key, out var resolutionString))
            return null;
        
        var values = resolutionString.Split("x");
        
        if (values.Length != 2)
            throw new FormatException($"{Tag} attribute {key} must contain 2 values");
        if (!int.TryParse(values[0], CultureInfo.InvariantCulture, out var width) || 
            !int.TryParse(values[1], CultureInfo.InvariantCulture, out var height))
            throw new FormatException($"{Tag} attribute {key} values {resolutionString} must be integers.");
        
        if (width <= 0 || height <= 0)
            throw new FormatException($"{Tag} attribute {key} values {resolutionString} must be greater than zero.");
        
        return new Resolution {Width = width, Height = height};
    }

    public bool OptionalYesNo(string key)
    {
        if (!_attributes.TryGetValue(key, out var booleanValue))
            return false;

        return booleanValue switch
        {
            "YES" => true,
            "NO" => false,
            _ => throw new FormatException($"{Tag} attribute {key} must be 'YES' or 'NO'.")
        };
    }
}

using TextGame.Data;

namespace TextGame.Api.Transformers
{
    public sealed class KebabCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value?.ToString()?.FromPascalToKebabCase();
        }
    }
}


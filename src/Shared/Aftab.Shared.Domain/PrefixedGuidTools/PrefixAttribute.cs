namespace Aftab.Shared.Domain.PrefixedGuidTools;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PrefixAttribute(string prefix) : Attribute
{
    public string Prefix { get; } = prefix;
}

namespace Sundew.Injection.Generator.TypeSystem;

[DiscriminatedUnions.DiscriminatedUnion]
public enum ErrorType
{
    InfiniteRecursions,
    NoPropertyGetMethodFound,
    ParameterTypeResolutionFailed,
    InvalidFactoryMethodBinding,
}
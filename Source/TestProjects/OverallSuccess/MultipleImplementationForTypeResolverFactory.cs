namespace OverallSuccess;

using OverallSuccess.TypeResolver;

public partial class MultipleImplementationForTypeResolverFactory : IMultipleImplementationForTypeResolverFactory
{
}

public partial interface IMultipleImplementationForTypeResolverFactory
{
    IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverA();

    IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverB();
}
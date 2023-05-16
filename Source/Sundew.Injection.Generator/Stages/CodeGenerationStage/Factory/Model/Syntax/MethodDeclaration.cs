namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System.Collections.Immutable;
using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal record MethodDeclaration(DeclaredAccessibility Accessibility,
    bool IsVirtual,
    bool IsAsync,
    string Name,
    ValueList<ParameterDeclaration> Parameters,
    ValueList<AttributeDeclaration> Attributes,
    DefiniteType? ReturnType = null)
{
    public MethodDeclaration(
        DeclaredAccessibility accessibility,
        bool isVirtual,
        string name,
        ValueList<ParameterDeclaration> parameters,
        DefiniteType? returnType = null)
    : this(accessibility, isVirtual, false, name, parameters, ImmutableArray<AttributeDeclaration>.Empty, returnType)
    {
    }
}
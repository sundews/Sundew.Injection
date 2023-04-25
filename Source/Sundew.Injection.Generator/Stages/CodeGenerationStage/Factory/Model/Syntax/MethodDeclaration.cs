namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal record MethodDeclaration(DeclaredAccessibility Accessibility, bool IsVirtual, string Name, ValueList<ParameterDeclaration> Parameters, DefiniteType? ReturnType = null);
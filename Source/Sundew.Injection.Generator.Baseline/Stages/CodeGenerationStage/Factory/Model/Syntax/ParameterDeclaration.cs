namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using Sundew.Injection.Generator.TypeSystem;

public readonly record struct ParameterDeclaration(DefiniteType Type, string Name, string? DefaultValue = null) : IDeclaration;

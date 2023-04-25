namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FieldDeclaration(DefiniteType Type, string Name, CreationExpression? CreationExpression) : IDeclaration;
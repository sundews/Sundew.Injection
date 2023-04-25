namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using Sundew.Injection.Generator.TypeSystem;

internal interface IDeclaration
{
    DefiniteType Type { get; }

    string Name { get; }
}
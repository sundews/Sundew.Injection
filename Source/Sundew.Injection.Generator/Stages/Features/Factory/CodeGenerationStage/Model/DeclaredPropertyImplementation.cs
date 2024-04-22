namespace Sundew.Injection.Generator.Stages.Features.Factory.CodeGenerationStage.Model;

using System.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

internal readonly record struct DeclaredPropertyImplementation(PropertyDeclaration Declaration, ImmutableList<Statement> GetPropertyImplementation);
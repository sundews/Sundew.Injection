namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System.Collections.Generic;
using Sundew.Injection.Generator.TypeSystem;

internal record InterfaceDeclaration(DefiniteType Type, IReadOnlyList<DefiniteType> InterfaceTypes, IReadOnlyList<MethodDeclaration> Members);
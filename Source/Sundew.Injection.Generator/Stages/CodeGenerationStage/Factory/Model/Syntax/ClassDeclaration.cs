namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System.Collections.Generic;
using Sundew.Injection.Generator.TypeSystem;

internal record ClassDeclaration(DefiniteType Type, bool IsSealed, IReadOnlyList<Member> Members, IReadOnlyList<DefiniteType> Interfaces);
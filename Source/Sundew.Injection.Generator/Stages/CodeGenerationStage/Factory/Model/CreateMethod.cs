// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateMethod.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct CreateMethod(string Name, ValueList<ParameterDeclaration> Parameters, DefiniteType ReturnType);
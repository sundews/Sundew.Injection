// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethod.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FactoryMethod(string Name, ValueList<ParameterDeclaration> Parameters, Type ReturnType);
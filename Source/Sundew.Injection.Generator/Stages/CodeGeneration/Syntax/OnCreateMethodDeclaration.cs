﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnCreateMethodDeclaration.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGeneration.Syntax;

using System.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal record OnCreateMethodDeclaration(string Name, ImmutableList<ParameterDeclaration> Parameters, Type ReturnType);
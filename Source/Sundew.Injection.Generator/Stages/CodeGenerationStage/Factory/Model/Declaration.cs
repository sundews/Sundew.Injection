﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Declaration.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model;

using Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;
using Sundew.Injection.Generator.TypeSystem;

public readonly record struct Declaration(DefiniteType Type, string Name) : IDeclaration;
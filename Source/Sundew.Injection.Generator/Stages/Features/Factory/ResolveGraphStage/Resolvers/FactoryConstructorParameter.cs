﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryConstructorParameter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage.Resolvers;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct FactoryConstructorParameter(Type Type, string Name, TypeMetadata TypeMetadata);
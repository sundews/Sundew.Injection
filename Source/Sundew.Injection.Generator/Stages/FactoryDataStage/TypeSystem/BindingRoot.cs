// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingRoot.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.TypeSystem;

using Sundew.Injection.Generator.TypeSystem;

internal readonly record struct BindingRoot(Binding Binding, Accessibility Accessibility, DefiniteType ReturnType);

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMayOverrideNewNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Base.Collections.Immutable;
using Sundew.Injection.Generator.TypeSystem;

internal interface IMayOverrideNewNode
{
    DefiniteType TargetReferenceType { get; }

    CreationSource CreationSource { get; }

    ValueArray<DefiniteParameter>? OverridableNewParametersOption { get; }
}
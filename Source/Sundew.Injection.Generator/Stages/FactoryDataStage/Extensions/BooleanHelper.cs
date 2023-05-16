// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Extensions;

public static class BooleanHelper
{
    public static void SetIfTrue(ref bool target, bool value)
    {
        target = value ? value : target;
    }
}
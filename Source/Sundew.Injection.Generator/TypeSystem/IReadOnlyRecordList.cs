// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyRecordList.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Collections.Generic;

internal interface IReadOnlyRecordList<TItem> : IReadOnlyList<TItem>, IEquatable<IReadOnlyRecordList<TItem>>
{
}
namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Collections.Generic;

public interface IReadOnlyRecordList<TItem> : IReadOnlyList<TItem>, IEquatable<IReadOnlyRecordList<TItem>>
{
}
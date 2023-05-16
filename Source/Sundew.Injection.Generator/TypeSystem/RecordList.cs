namespace Sundew.Injection.Generator.TypeSystem;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sundew.Base.Equality;

public sealed class RecordList<TItem> : IList<TItem>, IReadOnlyRecordList<TItem>, IEquatable<RecordList<TItem>>
{
    private readonly List<TItem> inner;

    public RecordList()
    {
        this.inner = new List<TItem>();
    }

    public int Count => this.inner.Count;

    public bool IsReadOnly => false;

    public TItem this[int index]
    {
        get => this.inner[index];
        set => this.inner[index] = value;
    }

    public static bool operator ==(RecordList<TItem>? left, RecordList<TItem>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RecordList<TItem>? left, RecordList<TItem>? right)
    {
        return !Equals(left, right);
    }

    public bool Equals(RecordList<TItem>? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.inner.SequenceEqual(other.inner);
    }

    public bool Equals(IReadOnlyRecordList<TItem>? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.inner.SequenceEqual(other);
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        return Equality.Equals<IReadOnlyRecordList<TItem>>(this, obj);
    }

    public override int GetHashCode()
    {
        return StructuralComparisons.StructuralEqualityComparer.GetHashCode(this.inner);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this.inner).GetEnumerator();
    }

    public void Add(TItem item)
    {
        this.inner.Add(item);
    }

    public void Clear()
    {
        this.inner.Clear();
    }

    public bool Contains(TItem item)
    {
        return this.inner.Contains(item);
    }

    public void CopyTo(TItem[] array, int arrayIndex)
    {
        this.inner.CopyTo(array, arrayIndex);
    }

    public bool Remove(TItem item)
    {
        return this.inner.Remove(item);
    }

    public int IndexOf(TItem item)
    {
        return this.inner.IndexOf(item);
    }

    public void Insert(int index, TItem item)
    {
        this.inner.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        this.inner.RemoveAt(index);
    }
}
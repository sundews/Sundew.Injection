namespace OverallSuccess.Generics;

using OverallSuccessDependency;

public class Generic<T> : IGeneric<T>, IIdentifiable
{
    public Generic(T value)
    {
        this.Value = value;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public T Value { get; }
}
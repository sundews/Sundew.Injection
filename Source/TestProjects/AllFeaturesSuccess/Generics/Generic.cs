namespace AllFeaturesSuccess.Generics;

using AllFeaturesSuccessDependency;

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
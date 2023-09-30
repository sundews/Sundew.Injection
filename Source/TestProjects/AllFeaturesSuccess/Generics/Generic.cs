namespace AllFeaturesSuccess.Generics;

public class Generic<T> : IGeneric<T>
{
    public Generic(T value)
    {
        this.Value = value;
    }

    public T Value { get; }
}
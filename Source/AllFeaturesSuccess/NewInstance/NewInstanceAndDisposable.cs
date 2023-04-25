namespace AllFeaturesSuccess.NewInstance;

using System;

public class NewInstanceAndDisposable : IPrint, IDisposable
{
    public void Dispose()
    {
    }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
    }
}
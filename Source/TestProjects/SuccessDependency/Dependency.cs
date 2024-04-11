namespace SuccessDependency;

using System.Threading.Tasks;
using Initialization.Interfaces;

public class Dependency : IAsyncInitializable
{
    public ValueTask InitializeAsync()
    {
        return default;
    }
}
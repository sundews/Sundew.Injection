
namespace NetStandardLibrarySuccess
{
    using System.Threading.Tasks;
    using Sundew.Injection;

    public class FactoryDeclaration : IInjectionDeclaration
    {
        public void Configure(IInjectionBuilder injectionBuilder)
        {
            injectionBuilder.CreateFactory<T>();
        }
    }

    public class T
    {
    }
}

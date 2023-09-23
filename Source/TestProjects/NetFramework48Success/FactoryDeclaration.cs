
namespace NetFramework48Success
{
    using System.Collections;
    using System.Collections.Generic;
    using Sundew.Injection;

    public class FactoryDeclaration : IInjectionDeclaration
    {
        public void Configure(IInjectionBuilder injectionBuilder)
        {
            injectionBuilder.Bind<IAbstract, Concrete1>();
            injectionBuilder.Bind<IAbstract, Concrete2>();
            injectionBuilder.CreateFactory<T>();
        }
    }

    public class T
    {
        private readonly IEnumerable<IAbstract> abstracts;
        public T(IEnumerable<IAbstract> abstracts)
        {
            this.abstracts = abstracts;
        }
    }

    public interface IAbstract
    {
    }

    public class Concrete1: IAbstract { }
    public class Concrete2 : IAbstract { }
}

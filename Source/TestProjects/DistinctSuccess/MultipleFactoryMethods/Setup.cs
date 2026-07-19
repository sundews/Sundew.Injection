namespace DistinctSuccess.MultipleFactoryMethods;

using Sundew.Injection;

public class Setup : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<IMultiple, MultipleA>();
        injectionBuilder.Bind<IMultiple, MultipleB>();

        injectionBuilder.ImplementFactory<RootFactory, IMultipleFactory>();
    }
}

public class MultipleA : IMultiple
{
}

public class MultipleB : IMultiple
{
}

public interface IMultiple
{
}

public partial interface IMultipleFactory
{
    IMultiple CreateMultipleA();

    IMultiple CreateMultipleB();
}

public partial class RootFactory
{
}
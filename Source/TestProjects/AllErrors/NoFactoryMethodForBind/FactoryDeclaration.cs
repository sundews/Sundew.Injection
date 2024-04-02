namespace AllErrors.NoFactoryMethodForBind;

using Sundew.Injection;

public class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        injectionBuilder.Bind<NoAccessibleConstructor>();
    }
}
public class NoAccessibleConstructor
{
    private NoAccessibleConstructor()
    {
    }

    public static readonly NoAccessibleConstructor Instance = new NoAccessibleConstructor();
}
namespace AllFeaturesSuccess
{
    using AllFeaturesSuccess.OptionalInterface;
    using AllFeaturesSuccess.RequiredInterface;

    public class Program
    {
        public static void Main()
        {
#if PERFORMANCE_TEST

#else
            var injectedSeparately1 = new InjectedSeparately();
            var injectedSeparately2 = new InjectedSeparately();
            var viewModelModuleFactory = new ResolveRootFactory(
                new RequiredParameters(
                    new MultipleModuleRequiredParameter1(),
                    new MultipleModuleRequiredParameter2(),
                    new SingleModuleRequiredParameterConstructorMethod(),
                    new SingleModuleRequiredParameterCreateMethod()),
                injectedSeparately1,
                new InjectedByType(),
                injectedSeparately2,
                new OptionalParameters(),
                "Name");
            var viewModelModule = viewModelModuleFactory.CreateResolveRoot(new int[] { 1, 2, 3, 4}, () => new RequiredService());
            viewModelModule.InterfaceSingleInstancePerFactory.Start();
            viewModelModule.Intercepted.ToString();
            viewModelModule.PrintMe(0);
#endif
        }
    }

    public class InjectedSeparately : IInjectedSeparately
    {
    }

    public class InjectedByType: IInjectedByType
    {
    }

    public class RequiredService : IRequiredService
    {
    }

    public class RequiredParameters : IRequiredParameters
    {
        public RequiredParameters(
            IMultipleModuleRequiredParameter firstSpecificallyNamedModuleParameter,
            IMultipleModuleRequiredParameter secondSpecificallyNamedModuleParameter,
            ISingleModuleRequiredParameterConstructorMethod singleModuleRequiredConstructorMethodParameter,
            ISingleModuleRequiredParameterCreateMethod singleModuleRequiredCreateMethodParameter)
        {
            this.FirstSpecificallyNamedModuleParameter = firstSpecificallyNamedModuleParameter;
            this.SecondSpecificallyNamedModuleParameter = secondSpecificallyNamedModuleParameter;
            this.SingleModuleRequiredConstructorMethodParameter = singleModuleRequiredConstructorMethodParameter;
            this.SingleModuleRequiredCreateMethodParameter = singleModuleRequiredCreateMethodParameter;
        }

        public IMultipleModuleRequiredParameter FirstSpecificallyNamedModuleParameter { get; }
        public IMultipleModuleRequiredParameter SecondSpecificallyNamedModuleParameter { get; }
        public ISingleModuleRequiredParameterConstructorMethod SingleModuleRequiredConstructorMethodParameter { get; }
        public ISingleModuleRequiredParameterCreateMethod SingleModuleRequiredCreateMethodParameter { get; }
    }

    public class MultipleModuleRequiredParameter1 : IMultipleModuleRequiredParameter
    {
    }
    public class MultipleModuleRequiredParameter2 : IMultipleModuleRequiredParameter
    {
    }

    public class SingleModuleRequiredParameterConstructorMethod : ISingleModuleRequiredParameterConstructorMethod
    {
    }

    public class SingleModuleRequiredParameterCreateMethod : ISingleModuleRequiredParameterCreateMethod
    {
    }
}
namespace AllFeaturesSuccess
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using AllFeaturesSuccess.ChildFactory;
    using AllFeaturesSuccess.ConstructorSelection;
    using AllFeaturesSuccess.InterfaceImplementationBindings;
    using AllFeaturesSuccess.InterfaceSegregationBindings;
    using AllFeaturesSuccess.MultipleImplementations;
    using AllFeaturesSuccess.NewInstance;
    using AllFeaturesSuccess.Operations;
    using AllFeaturesSuccess.OptionalInterface;
    using AllFeaturesSuccess.OverridableNew;
    using AllFeaturesSuccess.RequiredInterface;
    using AllFeaturesSuccess.SingleInstancePerFactory;
    using AllFeaturesSuccess.SingleInstancePerRequest;
    using AllFeaturesSuccessDependency;
    using Sundew.Injection;
    using Sundew.Injection.Interception;

    internal class FactoryDeclaration : IInjectionDeclaration
    {
        public void Configure(IInjectionBuilder injectionBuilder)
        {
            injectionBuilder.RequiredParameterInjection = Inject.ByTypeAndName;
            injectionBuilder.AddParameter<IInjectedSeparately>(Inject.Separately);
            injectionBuilder.AddParameter<IInjectedByType>();
            
            injectionBuilder.AddParameterProperties<IRequiredParameters>();
            injectionBuilder.AddParameterProperties<OptionalParameters>();

            injectionBuilder.BindGeneric<IEnumerable<object>, ImmutableList<object>>(Scope.Auto, () => CreateList<object>(default!));

            injectionBuilder.Bind<IInitializationParameters, IDisposalParameters, ILifecycleParameters, LifecycleParameters>(isInjectable: true);
            injectionBuilder.Bind<IInjectableByInterface, InjectableByInterface>(isInjectable: true);
            injectionBuilder.Bind<IMultipleImplementation, MultipleImplementationA>();
            injectionBuilder.Bind<IMultipleImplementation, MultipleImplementationB>();
            injectionBuilder.Bind<IInterfaceSegregationOverridableNewA, IInterfaceSegregationOverridableNewB, IInterfaceSegregationOverridableNew, InterfaceSegregationOverridableNewImplementation>(Scope.SingleInstancePerFactory, isNewOverridable: true);
            injectionBuilder.Bind<IInterfaceSingleInstancePerFactory, InterfaceSingleInstancePerFactory>(Scope.SingleInstancePerFactory);
            injectionBuilder.Bind<ImplementationSingleInstancePerFactory>(Scope.SingleInstancePerFactory);
            injectionBuilder.Bind<IInjectableSingleInstancePerRequest, InjectableSingleInstancePerRequest>(Scope.SingleInstancePerRequest, isInjectable: true);
            injectionBuilder.Bind<ISelectFactoryMethod, SelectFactoryMethod>(Scope.Auto, () => CreateFeatureService1(default!, default!, default!, default!));
            injectionBuilder.Bind<ISelectConstructor, SelectConstructor>(Scope.Auto, () => new SelectConstructor(default!, default!, default!));
            injectionBuilder.Bind<IInterfaceSegregationA, IInterfaceSegregationB, IInterfaceSegregation, InterfaceSegregationImplementation>(Scope.SingleInstancePerRequest, isInjectable: true);
            injectionBuilder.Bind<NewInstanceAndDisposable>(isInjectable: true);
            injectionBuilder.Bind<IIntercepted, Intercepted>();
            injectionBuilder.Bind<OverrideableNewImplementation>(isNewOverridable: true);

            injectionBuilder.AddInterceptor<IInterceptor>();
            injectionBuilder.Intercept<Intercepted>(Methods.Exclude, x => x.Title, x => x.Description, x => x.Link);
            injectionBuilder.Intercept<InjectableByInterface>();

            injectionBuilder.CreateFactory<ConstructedChild>();

            injectionBuilder.BindFactory<DependencyFactory>();
            injectionBuilder.BindFactory<ManualDependencyFactory>(x => x.Create());

            injectionBuilder.CreateFactory(
                factories => factories
                        .Add<IOperation, OperationA>()
                        .Add<IOperation, OperationB>(),
                "GeneratedOperationFactory",
                generateInterface: true);

            injectionBuilder.CreateFactory(
                factories => factories.Add<IResolveRoot, ResolveRoot>(), generateInterface: true);
        }

        internal static ImmutableList<T> CreateList<T>(T[] defaultItems)
        {
            return ImmutableList.Create(defaultItems);
        }

        internal static SelectFactoryMethod CreateFeatureService1(IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory, IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IRequiredService requiredService, IInterfaceSegregationB interfaceSegregationB)
        {
            return new(interfaceSingleInstancePerFactory, injectableSingleInstancePerRequest, requiredService, interfaceSegregationB);
        }
    }
}

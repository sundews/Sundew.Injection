namespace AllFeaturesSuccess;
using AllFeaturesSuccess.ChildFactory;
using AllFeaturesSuccess.ConstructorSelection;
using AllFeaturesSuccess.Generics;
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
using AllFeaturesSuccess.TypeResolver;
using AllFeaturesSuccessDependency;
using Sundew.Injection;
using Sundew.Injection.Interception;

internal class FactoryDeclaration : IInjectionDeclaration
{
    public void Configure(IInjectionBuilder injectionBuilder)
    {
        // The default method for matching declared parameters
        injectionBuilder.RequiredParameterInjection = Inject.ByParameterName;

        // Each consumer will have its own parameter generated
        injectionBuilder.AddParameter<IInjectedSeparately>(Inject.Separately);
        // All consumers of this type will receive the same parameter
        injectionBuilder.AddParameter<IInjectedByType>();

        // Declares that all properties are eligible as parameters
        injectionBuilder.AddParameterProperties<IRequiredParameters>();

        injectionBuilder.AddParameter<RequiredParameter>();

        // Same as above, but these parameters are optional
        injectionBuilder.AddParameterProperties<OptionalParameters>();

        // Generic binding for any type of IEnumerable<> to an ImmutableList<>
        injectionBuilder.BindGeneric<IGeneric<object>, Generic<object>>(Scope.Auto, () => CreateGeneric<object>(default!));

        // Declares parameters for controlling initialization and disposal of the generated factory
        injectionBuilder.Bind<IInitializationParameters, IDisposalParameters, ILifecycleParameters, LifecycleParameters>(isInjectable: true);

        // Interface to implementation binding, where the instance may be provided by a parameter (see OptionalParameters)
        injectionBuilder.Bind<IInjectableByInterface, InjectableByInterface>(isInjectable: true);

        // Multiple implementation of the same interface binding, inject IEnumerable<IMultipleImplementationForArray> to consume
        injectionBuilder.Bind<IMultipleImplementationForArray, MultipleImplementationForArrayA>();
        injectionBuilder.Bind<IMultipleImplementationForArray, MultipleImplementationForArrayB>();

        // Multiple implementation of the same interface binding, inject IEnumerable<IMultipleImplementationForEnumerable> to consume
        injectionBuilder.Bind<IMultipleImplementationForEnumerable, MultipleImplementationForEnumerableA>();
        injectionBuilder.Bind<IMultipleImplementationForEnumerable, MultipleImplementationForEnumerableB>();

        // Segregated interface binding as a singleton, that allows new to be overriden in a derived factory
        injectionBuilder.Bind<IInterfaceSegregationOverridableNewA, IInterfaceSegregationOverridableNewB, IInterfaceSegregationOverridableNew, InterfaceSegregationOverridableNewImplementation>(Scope.SingleInstancePerFactory, isNewOverridable: true);

        // Singleton bindings
        injectionBuilder.Bind<IInterfaceSingleInstancePerFactory, InterfaceSingleInstancePerFactory>(Scope.SingleInstancePerFactory);
        injectionBuilder.Bind<ImplementationSingleInstancePerFactory>(Scope.SingleInstancePerFactory);

        // Single instance per request binding
        injectionBuilder.Bind<IInjectableSingleInstancePerRequest, InjectableSingleInstancePerRequest>(Scope.SingleInstancePerRequest, isInjectable: true);

        // Factory selector binding
        injectionBuilder.Bind<ISelectFactoryMethod, SelectFactoryMethod>(Scope.Auto, () => CreateFeatureService1(default!, default!, default!, default!));

        // Constructor selector binding
        injectionBuilder.Bind<ISelectConstructor, SelectConstructor>(Scope.Auto, () => new SelectConstructor(default!, default!, default!));

        // Segregated interface binding, that where the instance may be provided as a parameter
        // In this case the IInterfaceSegregation interface must implement all previous interfaces, but consumers can inject which ever they see fit
        injectionBuilder.Bind<IInterfaceSegregationA, IInterfaceSegregationB, IInterfaceSegregation, InterfaceSegregationImplementation>(Scope.SingleInstancePerRequest, isInjectable: true);

        injectionBuilder.Bind<NewInstanceAndDisposable>(isInjectable: true);
        injectionBuilder.Bind<OverrideableNewImplementation>(isNewOverridable: true);

        // Add an interceptor (Not implemented yet)
        injectionBuilder.AddInterceptor<IInterceptor>();
        // Intercept all methods except the list ones
        injectionBuilder.Intercept<Intercepted>(Methods.Exclude, x => x.Title, x => x.Description, x => x.Link);
        // Intercept all methods
        injectionBuilder.Intercept<InjectableByInterface>();
        injectionBuilder.Bind<IIntercepted, Intercepted>();

        // Creates a factory for ConstructedChild
        injectionBuilder.CreateFactory<ConstructedChildFactory, IConstructedChildFactory>(
            x => x.Add<ConstructedChild>());

        injectionBuilder.Bind<IMultipleImplementationForTypeResolver, MultipleImplementationForTypeResolverA>();
        injectionBuilder.Bind<IMultipleImplementationForTypeResolver, MultipleImplementationForTypeResolverB>();

        injectionBuilder.Bind<DependencyShared>(Scope.SingleInstancePerFactory);

        injectionBuilder.CreateFactory<MultipleImplementationForTypeResolverFactory, IMultipleImplementationForTypeResolverFactory>(
            x => x.Add<IMultipleImplementationForTypeResolver>());

        // Binding to a generated factory in another assembly
        injectionBuilder.BindFactory<DependencyFactory>();

        // Binding to the Create method of a manual factory
        injectionBuilder.BindFactory<ManualDependencyFactory>(x => x.Create());

        // Creates a factory with a create method for each of the added operations
        injectionBuilder.CreateFactory<GeneratedOperationFactory, IGeneratedOperationFactory>(
            factories => factories
                    .Add<IOperation, OperationA>()
                    .Add<IOperation, OperationB>());

        // Creates a factory for ResolveRoot and generates an interface for it as well
        injectionBuilder.CreateFactory<ResolveRootFactory, IResolveRootFactory>(
            factories => factories
                .Add<IResolveRoot, ResolveRoot>()
                .Add<IInterfaceSingleInstancePerFactory>()
                .Add<IMultipleImplementationForTypeResolver, MultipleImplementationForTypeResolverC>());

        injectionBuilder.CreateResolver<Container>(x => x
                .Add<MultipleImplementationForTypeResolverFactory>()
                .Add<DependencyFactory>()
                .Add<ManualDependencyFactory>()
                .Add<GeneratedOperationFactory>()
                .Add<ResolveRootFactory>());
    }

    internal static Generic<T> CreateGeneric<T>(T defaultItem)
    {
        return new Generic<T>(defaultItem);
    }

    internal static SelectFactoryMethod CreateFeatureService1(IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory, IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IRequiredService requiredService, IInterfaceSegregationB interfaceSegregationB)
    {
        return new(interfaceSingleInstancePerFactory, injectableSingleInstancePerRequest, requiredService, interfaceSegregationB);
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverallSuccessServiceProviderFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.IntegrationTests.Overall;

using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using OverallSuccess;
using OverallSuccess.Operations;
using OverallSuccess.OptionalInterface;
using OverallSuccess.RequiredInterface;
using OverallSuccessDependency;

[NotInParallel(nameof(FactoryLifetime))]
public class OverallSuccessServiceProviderFixture
{
    [Test]
    public void GetRequiredService_When_ServiceIsRegistered_Then_ServiceShouldBeReturned()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var result = testee.GetRequiredService<Dependency>();

        result.Should().NotBeNull();
    }

    [Test]
    public void GetService_When_ServiceIsRegistered_Then_ServiceShouldBeReturned()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var result = testee.GetService(typeof(ManualMultipleSingletonDependency));

        result.Should().BeOfType<ManualMultipleSingletonDependency>();
    }

    [Test]
    public void TryGetService_When_ServiceIsNotRegistered_Then_ResultShouldBeFalse()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var result = testee.TryGetService<string>(out var service);

        result.Should().BeFalse();
        service.Should().BeNull();
        testee.GetService(typeof(string)).Should().BeNull();
    }

    [Test]
    public void GetRequiredService_When_ServiceIsNotRegistered_Then_ServiceNotFoundExceptionShouldBeThrown()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        Action act = () => testee.GetRequiredService<string>();

        act.Should().Throw<ServiceNotFoundException>().Which.ServiceType.Should().Be(typeof(string));
    }

    [Test]
    public void GetRequiredService_When_ArgumentsMatchFactoryMethod_Then_ArgumentsShouldBePassed()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var result = testee.GetRequiredService<IOperation>(2, 3);

        result.Execute().Should().Be(5);
    }

    [Test]
    public void TryGetService_When_ArgumentsDoNotMatchFactoryMethod_Then_ResultShouldBeFalse()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var mismatchingTypesResult = testee.TryGetService<IOperation>(out _, "2", 3);
        var mismatchingCountResult = testee.TryGetService<IOperation>(out _, 2);
        var noArgumentsResult = testee.TryGetService<IOperation>(out _);

        mismatchingTypesResult.Should().BeFalse();
        mismatchingCountResult.Should().BeFalse();
        noArgumentsResult.Should().BeFalse();
    }

    [Test]
    public void GetRequiredService_When_ArgumentTypesAreAssignableToParameterTypes_Then_ServiceShouldBeReturned()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);
        var integers = new[] { 1, 2, 3, 4 };
        Func<RequiredService> requiredServiceFactory = () => new RequiredService();

        var result = testee.GetRequiredService<IResolveRoot>(integers, 5, requiredServiceFactory, new RequiredParameter("Test"));

        result.Should().NotBeNull();
    }

    [Test]
    public void GetRequiredService_When_NullIsPassedForOptionalParameters_Then_ServiceShouldBeReturned()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);
        Func<IRequiredService> requiredServiceFactory = () => new RequiredService();

        var result = testee.GetRequiredService<IResolveRoot>(new[] { 1, 2, 3, 4 }, 5, requiredServiceFactory, new RequiredParameter("Test"), null, null);

        result.Should().NotBeNull();
    }

    [Test]
    public void TryGetService_When_NullIsPassedForValueTypeParameter_Then_ResultShouldBeFalse()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var result = testee.TryGetService<IOperation>(out _, null, 3);

        result.Should().BeFalse();
    }

    [Test]
    public void TryDispose_When_ServiceWasResolvedFromProvider_Then_ServiceShouldBeDisposedOnce()
    {
        FactoryLifetime.Reset();
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);
        object resolveRoot = ResolveRoot(testee);
        var logLengthBeforeDispose = FactoryLifetime.Log.Length;

        var firstResult = testee.TryDispose(resolveRoot);
        var logLengthAfterFirstDispose = FactoryLifetime.Log.Length;
        var secondResult = testee.TryDispose(resolveRoot);

        firstResult.Should().BeTrue();
        secondResult.Should().BeTrue();
        FactoryLifetime.Log.ToString(logLengthBeforeDispose, logLengthAfterFirstDispose - logLengthBeforeDispose).Should().Contain("Disposed: ");
        FactoryLifetime.Log.Length.Should().Be(logLengthAfterFirstDispose);
    }

    [Test]
    public async Task TryDisposeAsync_When_ServiceWasResolvedFromProvider_Then_ServiceShouldBeDisposedOnce()
    {
        FactoryLifetime.Reset();
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);
        var resolveRoot = ResolveRoot(testee);
        var logLengthBeforeDispose = FactoryLifetime.Log.Length;

        var firstResult = await testee.TryDisposeAsync(resolveRoot);
        var logLengthAfterFirstDispose = FactoryLifetime.Log.Length;
        var secondResult = await testee.TryDisposeAsync(resolveRoot);

        firstResult.Should().BeTrue();
        secondResult.Should().BeTrue();
        FactoryLifetime.Log.ToString(logLengthBeforeDispose, logLengthAfterFirstDispose - logLengthBeforeDispose).Should().Contain("Disposed: ");
        FactoryLifetime.Log.Length.Should().Be(logLengthAfterFirstDispose);
    }

    [Test]
    public void TryDispose_When_ManualFactoryHasDisposeMethod_Then_FactoryDisposeShouldBeCalled()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var manualMultipleDependencyFactory = new ManualMultipleDependencyFactory();
        var testee = CreateServiceProvider(resolveRootFactory, manualMultipleDependencyFactory);
        var manualMultipleDependency = testee.GetRequiredService<ManualMultipleDependency>();

        var result = testee.TryDispose(manualMultipleDependency);

        result.Should().BeTrue();
        manualMultipleDependencyFactory.DisposedInstances.Should().Equal(manualMultipleDependency);
    }

    [Test]
    public async Task TryDisposeAsync_When_ManualFactoryHasOnlySynchronousDisposeMethod_Then_FactoryDisposeShouldBeCalled()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var manualMultipleDependencyFactory = new ManualMultipleDependencyFactory();
        var testee = CreateServiceProvider(resolveRootFactory, manualMultipleDependencyFactory);
        var manualMultipleDependency = testee.GetRequiredService<ManualMultipleDependency>();

        var result = await testee.TryDisposeAsync(manualMultipleDependency);

        result.Should().BeTrue();
        manualMultipleDependencyFactory.DisposedInstances.Should().Equal(manualMultipleDependency);
    }

    [Test]
    public void TryDispose_When_ManualFactoryHasOnlyAsynchronousDisposeMethod_Then_FactoryDisposeAsyncShouldBeCalled()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var manualMultipleDependencyFactory = new ManualMultipleDependencyFactory();
        var testee = CreateServiceProvider(resolveRootFactory, manualMultipleDependencyFactory);
        object manualMultipleSingletonDependency = testee.GetRequiredService<ManualMultipleSingletonDependency>();

        var result = testee.TryDispose(manualMultipleSingletonDependency);

        result.Should().BeTrue();
        manualMultipleDependencyFactory.DisposedInstances.Should().Equal(manualMultipleSingletonDependency);
    }

    [Test]
    public async Task TryDisposeAsync_When_ManualFactoryHasOnlyAsynchronousDisposeMethod_Then_FactoryDisposeAsyncShouldBeCalled()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var manualMultipleDependencyFactory = new ManualMultipleDependencyFactory();
        var testee = CreateServiceProvider(resolveRootFactory, manualMultipleDependencyFactory);
        object manualMultipleSingletonDependency = testee.GetRequiredService<ManualMultipleSingletonDependency>();

        var result = await testee.TryDisposeAsync(manualMultipleSingletonDependency);

        result.Should().BeTrue();
        manualMultipleDependencyFactory.DisposedInstances.Should().Equal(manualMultipleSingletonDependency);
    }

    [Test]
    public void TryDispose_When_ServiceTypeHasNoDisposingFactory_Then_ResultShouldBeFalse()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);
        var dependency = testee.GetRequiredService<Dependency>();

        var result = testee.TryDispose(dependency);

        result.Should().BeFalse();
    }

    [Test]
    public void TryDispose_When_InstanceWasNotCreatedByProvider_Then_ResultShouldBeFalse()
    {
        using var resolveRootFactory = CreateResolveRootFactory();
        var testee = CreateServiceProvider(resolveRootFactory);

        var result = testee.TryDispose(new object());

        result.Should().BeFalse();
    }

    private static IResolveRoot ResolveRoot(IInjectionServiceProvider serviceProvider)
    {
        Func<IRequiredService> requiredServiceFactory = () => new RequiredService();
        return serviceProvider.GetRequiredService<IResolveRoot>(new[] { 1, 2, 3, 4 }, 5, requiredServiceFactory, new RequiredParameter("Test"));
    }

    private static IInjectionServiceProvider CreateServiceProvider(ResolveRootFactory resolveRootFactory, ManualMultipleDependencyFactory? manualMultipleDependencyFactory = null)
    {
        return new ServiceProvider(
            new MultipleImplementationForTypeResolverFactory(),
            new DependencyFactory(),
            manualMultipleDependencyFactory ?? new ManualMultipleDependencyFactory(),
            new GeneratedOperationFactory(),
            resolveRootFactory);
    }

    private static ResolveRootFactory CreateResolveRootFactory()
    {
        return new ResolveRootFactory(
            new RequiredParameters(
                new MultipleModuleRequiredParameter1(),
                new MultipleModuleRequiredParameter2(),
                new SingleModuleRequiredParameterConstructorMethod(),
                new SingleModuleRequiredParameterCreateMethod()),
            new InjectedSeparately(),
            new InjectedByType(),
            new InjectedSeparately(),
            new OptionalParameters(),
            "Name");
    }
}

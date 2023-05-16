# Sundew.Injection

The overarching goal of Sundew.Injection is to increase performance related to object creation, through the means of [Pure DI](https://blog.ploeh.dk/2014/06/10/pure-di/), while keeping most of the benefits of using a Dependency Injection Container (DIC).

Software applications types based on time running.

| Application types            | Time running                                                                                                      |
| ---------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| Classical server application | Long run time, where most of the application is created on startup. Will often create an object graph per request |
| Desktop application          | These vary from having a medium to long run time, but typically create object graph per request                   |
| Mobile application           | Short run time as the OS will suspend apps not in focus, requiring it to be initialized again on activation       |
| WebJob (Cloud enabled)       | Short run time as the cloud system will suspend the app.                                                          |
| Tests run                    | Very short run time with frequent start-ups when running tests                                                    |

Generally, short-lived applications can benefit the most from Pure DI during start-up, but server applications can fx improve the number of request the can process by using Pure DI.

### Motivation
* Recognize that DI containers are a too generic solution to application object graph creation
  * 90+% of all types created in an application have a static relationship to the application (e.g. Only one implementation per interface)
  * A DI container will allow an application to resolve nearly any type, but effectively any application explicitly resolves a finite number of types
     * The resolved types might change over time, which is why DI containers are convenient.
       *  As long as changing which types are resolved and building an object graph is easy, a DI container is not needed
* Performance
  * Whether DI container performance is a issue can be debated, reflected/dynamically compiled code is slower than statically compiled code. 
  * DI container adds high costs to high-level tests, alternatives such as reusing containers introduce complexity and risk of not running tests properly isolated
* Support platforms that do not support dynamic code compilation: iOS

## Features

| Features                                                        | Usage                                                                                                                                  | Comparison to DIC                                                            |
| --------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| **Declaration**                                                 | Declare how types, map and what factories to create                                                                                    | Configuration of the container                                               |
| - Declaring factories to be created                             | Used to define which root types can be created                                                                                         | Dependending on DIC this is similar to explicit calls to Resolve&lt;T&gt;()  |
| - Generate factory interface                                    | Used to optionally declare whether an interface will be generated for the fatory                                                       | Not relevant                                                                 |
| - Multiple Create functions per factory                         | Sharing objects between created types or create an 'operations factory'                                                                | Some DICs supports implementing (runtime) a factory interface                |
| - Instantiation of unregistered types                           | When a dependency is an 'instantiable' type there is no need to register it.                                                           | Supported by most DICs                                                       |
| - Constructor selection                                         | Used to manually select the constructor to use                                                                                         | Supported by most DICs                                                       |
| - Mapping interface to implementation                           | Used to specify what concrete type should be instantiated for a specific interface                                                     | Supported by most DICs                                                       |
| - Multiple implementations                                      | Used to resolve all implementations of an interface                                                                                    | Supported by most DICs                                                       |
| - Generic types                                                 | Used to register generic types, so that any bound generic of that type can be resolved                                                 | Supported by most DICs                                                       |
| **Parameters (Modular development, required/public interface)** | Used to pass in required parameters into the factory                                                                                   | Similar to resolve overrides in other DICs                                   |
| - Required parameters (required interface)                      | Required parameters are explicitly part of the factory interface                                                                       | Same as above, although interface is not explicit                            |
| - Required parameter from custom class                          | Specify a custom class that contains parameters, to control public interface                                                           | Same as above                                                                |
| - Optional parameters                                           | Explicitly specifies a parameter, but in case of null the implementation is resovled                                                   | Depends on DICs, typically emulated though an empty multiple implementations |
| **Lifetime scopes**                                             | Used to declare the dependency graph e.g. how object communicate                                                                       | Supported by all DICs                                                        |
| - Single instance per factory                                   | The same instance will be used throughout the factory lifetime                                                                         | Equivalent to singleton                                                      |
| - Single instance per request                                   | A new instance will be created per call to the 'Create' method and thus be shared                                                      | Equivalent to single instance per request/resolve                            |
| - New instance                                                  | A new instance is created every time it is requested                                                                                   | Equivalent to transient                                                      |
| **Override 'new' in derived factory class**                     | Useful when wanting to replace an implementation with a different one.<br/>e.g. a mock without making explicitly part of the interface | Typically registrations can be overwritten                                   |
| **Thread safety**                                               | Create methods can be called on multiple threads                                                                                       | Supported by most DICs                                                       |
| **Initialization**                                              | Implement IInitializable or IAsyncInitializable in a type to perform initialization not suited for the ctor.                           | Support for similar functionality in some DICs                               |
| **Disposal**                                                    | Disposal by disposing factory or explicit Dispose(TCreated) method                                                                     | Depends on DIC, some support only disposing singletons                       |
| **Factories can depend on other generated factories**           | Currently, factories can be injected, but does not support calling the 'Create' method (planned)                                       | Supported by some DICs through child containers                              |
| **Zero reflection**                                             | Improved performance<br/>Enable .NET Native/NativeAOT etc.                                                                             | Not supported by DICs                                                        |

## Not implemented yet:
* Calling child factory 'Create' methods
* Examples
* Test error cases
* Generating documentation
* Custom lifetime scope, to support implementing something like single instance per thread or per session
* Interception
* Test correctness of generated code

## Not supported DIC features
* No dynamic assembly loading such as a plug-in system -> Use an existing DI container/AssemblyLoadContext for the high-level plug-in loading and Sundew.Injection for the plug-ins themselves.
* No support for describing conditional object creation -> Considered business logic, implement a factory manually

## Factory or Dependency Injection Container?

An issue with the factory pattern is that it is only conceptually reusable. Hence DICs provide a productivity improvement as a reusable pattern for instantiating object graphs.
Although the output of Sundew.Injection resembles the Factory pattern more closely, the declaration of these are very aligned with usage of a DICs. Therefore the name Sundew.Injection was chosen.

With this, an instantiated Factory is conceptually similar to a configured DIC instance.

## Definition of IDisposable/IAsyncDisposable ownership

An IDisposable/IAsyncDisposable object is considered owned by a factory in the following scenarios
1. It was instantiated by the factory
2. It was created by a Func<> passed into the factory and creation was triggered by the factory.

## Open questions
* How to integrate with application frameworks?
  * Some frameworks like ASP.NET dictates the use of an DICs.

### Challenges/Risk
* Build performance
  * If solution cannot be implemented in a performant (enough) way with IIncrementalGenerator
    * Support fallback non-code-generated (common DI container) solution for non release builds?
* Versioning across multiple projects, since the library contains some required types internal and public types.
  * Internal types are added to the project, by public types like IInterceptor can only be included as a PackageReferences, so that other projects have a chance to implement an interceptor.
  * Only expose generated factories? How can this work with a non-code-generated fallback? (It probably can't, as the generator has to run to determine the public interface (Only a problem if build performance is a problem))
* Hard to integrate with application framework the dictate a the use of a dynamic DIC.
  * Generate code to integrate with DICs
  * Explore how these could be changed to rely less on reflection

### Integration with application frameworks
| Framework            | Comments                                                                                                                                                                                                                                                |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ASP.NET/Blazor/Razor | Currently, there is no good answer for ASP.NET, but it does allow replacing it middleware at various interception points.<br/>Here Sundew.Injection could be used, although some reflection would still be involved to determine which factory to call. |
| ReactiveUI           | TODO.                                                                                                                                                                                                                                                   |
| LightMVVM            | TODO.                                                                                                                                                                                                                                                   |
| Maui                 | TODO.                                                                                                                                                                                                                                                   |
| Uno                  | TODO.                                                                                                                                                                                                                                                   |
| Console              | New it and use it.                                                                                                                                                                                                                                      |

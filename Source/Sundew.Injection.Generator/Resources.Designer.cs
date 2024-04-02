﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sundew.Injection.Generator {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sundew.Injection.Generator.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when ImplementFactory is called with invalid type parameters.
        /// </summary>
        internal static string InvalidFactoryTypeDescription {
            get {
                return ResourceManager.GetString("InvalidFactoryTypeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;{0}&apos; must be a regular class.
        /// </summary>
        internal static string InvalidFactoryTypeMessageFormat {
            get {
                return ResourceManager.GetString("InvalidFactoryTypeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Factory types must be a regular class.
        /// </summary>
        internal static string InvalidFactoryTypeTitle {
            get {
                return ResourceManager.GetString("InvalidFactoryTypeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string MultipleParametersNotSupportedForBindFactoryErrorDescription {
            get {
                return ResourceManager.GetString("MultipleParametersNotSupportedForBindFactoryErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}.
        /// </summary>
        internal static string MultipleParametersNotSupportedForBindFactoryErrorMessageFormat {
            get {
                return ResourceManager.GetString("MultipleParametersNotSupportedForBindFactoryErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ICreate.
        /// </summary>
        internal static string MultipleParametersNotSupportedForBindFactoryErrorTitle {
            get {
                return ResourceManager.GetString("MultipleParametersNotSupportedForBindFactoryErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when there is no way to construct a type, either by missing a binding, the type not having a constructor or being abstract/interface.
        /// </summary>
        internal static string NoBindingFoundForNonConstructableTypeErrorDescription {
            get {
                return ResourceManager.GetString("NoBindingFoundForNonConstructableTypeErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Binding not found for type: &apos;{0}&apos; and type is not constructable.
        /// </summary>
        internal static string NoBindingFoundForNonConstructableTypeErrorMessageFormat {
            get {
                return ResourceManager.GetString("NoBindingFoundForNonConstructableTypeErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Binding not found.
        /// </summary>
        internal static string NoBindingFoundForNonConstructableTypeErrorTitle {
            get {
                return ResourceManager.GetString("NoBindingFoundForNonConstructableTypeErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when a binding does not specify a factory method or constructor and no constructor could be resovled automatically.
        /// </summary>
        internal static string NoFactoryMethodFoundForTypeErrorDescription {
            get {
                return ResourceManager.GetString("NoFactoryMethodFoundForTypeErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not Resolve factory method for type: &apos;{0}&apos;.
        /// </summary>
        internal static string NoFactoryMethodFoundForTypeErrorMessageFormat {
            get {
                return ResourceManager.GetString("NoFactoryMethodFoundForTypeErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No factory method was found to instanciate type.
        /// </summary>
        internal static string NoFactoryMethodFoundForTypeErrorTitle {
            get {
                return ResourceManager.GetString("NoFactoryMethodFoundForTypeErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Types without a viable constructor are not supported.
        /// </summary>
        internal static string NoViableConstructorFoundErrorDescription {
            get {
                return ResourceManager.GetString("NoViableConstructorFoundErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type: &apos;{0}&apos; did not contain a viable constructor.
        /// </summary>
        internal static string NoViableConstructorFoundErrorMessageFormat {
            get {
                return ResourceManager.GetString("NoViableConstructorFoundErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No viable constructor could be found.
        /// </summary>
        internal static string NoViableConstructorFoundErrorTitle {
            get {
                return ResourceManager.GetString("NoViableConstructorFoundErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only generic types are supported.
        /// </summary>
        internal static string OnlyGenericTypeSupportedErrorDescription {
            get {
                return ResourceManager.GetString("OnlyGenericTypeSupportedErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type: &apos;{0}&apos; is not a generic type.
        /// </summary>
        internal static string OnlyGenericTypeSupportedErrorMessageFormat {
            get {
                return ResourceManager.GetString("OnlyGenericTypeSupportedErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TImplementationType must be generic..
        /// </summary>
        internal static string OnlyGenericTypeSupportedErrorTitle {
            get {
                return ResourceManager.GetString("OnlyGenericTypeSupportedErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some types are required for the code generator, when not found in the compilation, this error is reported.
        /// </summary>
        internal static string RequiredTypeNotFoundErrorDescription {
            get {
                return ResourceManager.GetString("RequiredTypeNotFoundErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}.
        /// </summary>
        internal static string RequiredTypeNotFoundErrorMessageFormat {
            get {
                return ResourceManager.GetString("RequiredTypeNotFoundErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A required type was not found in the compilation.
        /// </summary>
        internal static string RequiredTypeNotFoundErrorTitle {
            get {
                return ResourceManager.GetString("RequiredTypeNotFoundErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when failing to resolves a generic method and its parameters.
        /// </summary>
        internal static string ResolveGenericMethodDescription {
            get {
                return ResourceManager.GetString("ResolveGenericMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generic method could not be resolved.
        /// </summary>
        internal static string ResolveGenericMethodErrorTitle {
            get {
                return ResourceManager.GetString("ResolveGenericMethodErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not resolve the following parameters for type: &apos;{0}&apos; - parameters: &apos;{1}&apos;.
        /// </summary>
        internal static string ResolveGenericMethodMessageFormat {
            get {
                return ResourceManager.GetString("ResolveGenericMethodMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when failing to resolves a method and its parameters.
        /// </summary>
        internal static string ResolveMethodDescription {
            get {
                return ResourceManager.GetString("ResolveMethodDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method could not be resolved.
        /// </summary>
        internal static string ResolveMethodErrorTitle {
            get {
                return ResourceManager.GetString("ResolveMethodErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not resolve the following parameters for type: &apos;{0}&apos; - parameters: &apos;{1}&apos;.
        /// </summary>
        internal static string ResolveMethodMessageFormat {
            get {
                return ResourceManager.GetString("ResolveMethodMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when a required parameter cannot be resolved.
        /// </summary>
        internal static string ResolveRequiredParameterErrorDescription {
            get {
                return ResourceManager.GetString("ResolveRequiredParameterErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not resolve type: &apos;{0}&apos; as a parameter for: &apos;{1}&apos; from the available parameter sources: &apos;{2}&apos;.
        /// </summary>
        internal static string ResolveRequiredParameterErrorMessageFormat {
            get {
                return ResourceManager.GetString("ResolveRequiredParameterErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not resolve required parameter.
        /// </summary>
        internal static string ResolveRequiredParameterErrorTitle {
            get {
                return ResourceManager.GetString("ResolveRequiredParameterErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when type could not be resolved.
        /// </summary>
        internal static string ResolveTypeErrorDescription {
            get {
                return ResourceManager.GetString("ResolveTypeErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to Resolve type: &apos;{0}&apos;.
        /// </summary>
        internal static string ResolveTypeErrorMessageFormat {
            get {
                return ResourceManager.GetString("ResolveTypeErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not Resolve type.
        /// </summary>
        internal static string ResolveTypeErrorTitle {
            get {
                return ResourceManager.GetString("ResolveTypeErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when a type is registered with an invalid lifetime scope .
        /// </summary>
        internal static string ScopeErrorDescription {
            get {
                return ResourceManager.GetString("ScopeErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type: &apos;{0}&apos; with scope: &apos;{1}&apos; is invalid due to the parent: {2} with scope: {3}.
        /// </summary>
        internal static string ScopeErrorMessageFormat {
            get {
                return ResourceManager.GetString("ScopeErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid lifetime scope.
        /// </summary>
        internal static string ScopeErrorTitle {
            get {
                return ResourceManager.GetString("ScopeErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type is either an abstract class or a string, which cannot be instantiated.
        /// </summary>
        internal static string TypeNotInstantiableErrorDescription {
            get {
                return ResourceManager.GetString("TypeNotInstantiableErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type: &apos;{0}&apos; cannot be instantiated.
        /// </summary>
        internal static string TypeNotInstantiableErrorMessageFormat {
            get {
                return ResourceManager.GetString("TypeNotInstantiableErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TImplementationType must be instantiable.
        /// </summary>
        internal static string TypeNotInstantiableErrorTitle {
            get {
                return ResourceManager.GetString("TypeNotInstantiableErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Used when an unknown error occurs.
        /// </summary>
        internal static string UnknownErrorDescription {
            get {
                return ResourceManager.GetString("UnknownErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown error: &apos;{0}&apos;.
        /// </summary>
        internal static string UnknownErrorMessageFormat {
            get {
                return ResourceManager.GetString("UnknownErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown error occured in Sundew.Injection.Generator.
        /// </summary>
        internal static string UnknownErrorTitle {
            get {
                return ResourceManager.GetString("UnknownErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Occurs when an factory instance method is not supported.
        /// </summary>
        internal static string UnsupportedInstanceMethodErrorDescription {
            get {
                return ResourceManager.GetString("UnsupportedInstanceMethodErrorDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The instance method: &apos;{0}&apos; is not supported on the type: &apos;{1}&apos; for injecting into: &apos;{2}&apos;.
        /// </summary>
        internal static string UnsupportedInstanceMethodErrorMessageFormat {
            get {
                return ResourceManager.GetString("UnsupportedInstanceMethodErrorMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported instance method..
        /// </summary>
        internal static string UnsupportedInstanceMethodErrorTitle {
            get {
                return ResourceManager.GetString("UnsupportedInstanceMethodErrorTitle", resourceCulture);
            }
        }
    }
}

namespace AllFeaturesSuccess.SingleInstancePerRequest
{
    using System;
    using System.Collections.Generic;
    using AllFeaturesSuccess.Generics;
    using AllFeaturesSuccess.RequiredInterface;

    public class InjectableSingleInstancePerRequest : IInjectableSingleInstancePerRequest, IDisposable
    {
        private readonly ISingleModuleRequiredParameterCreateMethod singleModuleRequiredParameterCreateMethod;
        private readonly IEnumerable<int> integers;
        private readonly IGeneric<int> generic;

        public InjectableSingleInstancePerRequest(ISingleModuleRequiredParameterCreateMethod singleModuleRequiredParameterCreateMethod, IEnumerable<int> integers, IGeneric<int> generic)
        {
            this.singleModuleRequiredParameterCreateMethod = singleModuleRequiredParameterCreateMethod;
            this.integers = integers;
            this.generic = generic;
        }

        public void PrintMe(int indent)
        {
            Console.WriteLine(new string(' ', indent) + this.GetType().Name);
            Console.WriteLine(new string(' ', indent) + this.singleModuleRequiredParameterCreateMethod.GetType().Name);
            Console.WriteLine(new string(' ', indent) + this.integers.GetType().Name);
            Console.WriteLine(new string(' ', indent) + this.generic.GetType().Name);

        }

        public void Dispose()
        {
        }
    }
}
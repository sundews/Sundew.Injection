namespace AllFeaturesSuccess.SingleInstancePerRequest
{
    using System;
    using System.Collections.Generic;
    using AllFeaturesSuccess.RequiredInterface;

    public class InjectableSingleInstancePerRequest : IInjectableSingleInstancePerRequest, IDisposable
    {
        private readonly ISingleModuleRequiredParameterCreateMethod singleModuleRequiredParameterCreateMethod;

        public InjectableSingleInstancePerRequest(ISingleModuleRequiredParameterCreateMethod singleModuleRequiredParameterCreateMethod, IEnumerable<int> integers)
        {
            this.singleModuleRequiredParameterCreateMethod = singleModuleRequiredParameterCreateMethod;
        }

        public void PrintMe(int indent)
        {
            Console.WriteLine(new string(' ', indent) + this.GetType().Name);
            Console.WriteLine(new string(' ', indent) + this.singleModuleRequiredParameterCreateMethod.GetType().Name);
        }

        public void Dispose()
        {
        }
    }
}
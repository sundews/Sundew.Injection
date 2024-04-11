namespace Success.RequiredInterface;

public interface IRequiredParameters
{
    IMultipleModuleRequiredParameter FirstSpecificallyNamedModuleParameter { get; }

    IMultipleModuleRequiredParameter SecondSpecificallyNamedModuleParameter { get; }

    ISingleModuleRequiredParameterConstructorMethod SingleModuleRequiredConstructorMethodParameter { get; }

    ISingleModuleRequiredParameterCreateMethod SingleModuleRequiredCreateMethodParameter { get; }
}
namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal sealed partial record NullCoalescingOperatorExpression(Expression Lhs, Expression Rhs) : Expression;
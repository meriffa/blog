using CommandLine;
using System.Linq.Expressions;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Expression Tree controller
/// </summary>
[Verb("Concepts-ExpressionTree", HelpText = "Expression Tree operation.")]
public class ExpressionTreeController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        int number = 5;
        var function1 = GetImplicitExpressionTreeDelegate();
        var function2 = GetExplicitExpressionTreeDelegate();
        displayService.WriteInformation($"F1({number}) = {function1(number)}, F2({number}) = {function2(number)}");
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return implicit Expression Tree delegate
    /// </summary>
    /// <returns></returns>
    private static Func<int, int> GetImplicitExpressionTreeDelegate()
    {
        Expression<Func<int, int>> lambda = number => number * number;
        return lambda.Compile();
    }

    /// <summary>
    /// Return explicit Expression Tree delegate
    /// </summary>
    /// <returns></returns>
    private static Func<int, int> GetExplicitExpressionTreeDelegate()
    {
        var parameter = Expression.Parameter(typeof(int), "number");
        var lambda = Expression.Lambda<Func<int, int>>(Expression.Multiply(Expression.Constant(2), parameter), new List<ParameterExpression>() { parameter });
        return lambda.Compile();
    }
    #endregion

}
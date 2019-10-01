using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MvcTesting.Html
{
    public class ExpressionHelper
    {
        private static MethodInfo _getExpressionText;

        public static string GetExpressionText(LambdaExpression property)
        {
            if (_getExpressionText == null)
            {
                var type = typeof(ModelExpressionProvider).Assembly
                    .GetTypes()
                    .Single(t => t.Name == "ExpressionHelper");

                _getExpressionText = type.GetMethod("GetExpressionText");
            }

            var text = _getExpressionText.Invoke(null, new object[] { property, null });
            return (string)text;
        }
    }
}

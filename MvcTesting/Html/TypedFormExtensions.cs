using System;
using System.Linq.Expressions;

namespace MvcTesting.Html
{
    public static class TypedFormExtensions
    {
        public static string GetText<T>(this TypedForm<T> form, Expression<Func<T, string>> property)
        {
            var formName = FormName(property);
            return form.GetSingle(formName).Value;
        }

        public static TypedForm<T> SetText<T>(this TypedForm<T> form, string formName, string value)
        {
            form.GetSingle(formName).SetValue(value);
            return form;
        }

        public static TypedForm<T> SetText<T>(this TypedForm<T> form, Expression<Func<T, string>> property, string value)
        {
            var formName = FormName(property);
            return form.SetText(formName, value);
        }

        public static TypedForm<T> SetFile<T>(this TypedForm<T> form, string formName, string fileName, byte[] content)
        {
            var fileUpload = form.GetFile(formName);
            fileUpload.SetContent(fileName, content);
            return form;
        }

        public static TypedForm<T> SetFile<T>(this TypedForm<T> form, Expression<Func<T, byte[]>> property, string fileName, byte[] content)
        {
            var formName = FormName(property);
            return form.SetFile(formName, fileName, content);
        }

        public static string FormName(LambdaExpression property)
        {
            return ExpressionHelper.GetExpressionText(property);
        }
    }
}

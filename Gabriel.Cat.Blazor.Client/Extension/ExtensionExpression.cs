using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class ExtensionExpression
    {
        public static string GetPropertyName<TValue>(Expression<Func<TValue>> exp)
        {
            MemberExpression expression = (exp.Body as MemberExpression);
            if (expression == null)
                expression = (exp.Body as UnaryExpression).Operand as MemberExpression;
            return expression.Member.Name;
        }
    }
}

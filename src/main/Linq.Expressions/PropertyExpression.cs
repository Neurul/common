using System;
using System.Linq.Expressions;

namespace neurUL.Common.Linq.Expressions
{
    public class PropertyExpression<T, TValue>
    {
        public PropertyExpression(Expression<Func<T, TValue>> property)
        {
            Getter = ExpressionUtils.CreateGetter(property);
            Setter = ExpressionUtils.CreateSetter(property);
        }

        public Func<T, TValue> Getter { get; set; }
        public Action<T, TValue> Setter { get; set; }
    }
}

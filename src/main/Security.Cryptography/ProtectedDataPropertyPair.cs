using neurUL.Common.Linq.Expressions;
using System;
using System.Linq.Expressions;

namespace neurUL.Common.Security.Cryptography
{
    public class ProtectedDataPropertyPair<T, TData>
    {
        public ProtectedDataPropertyPair(
            Expression<Func<T, TData>> dataProperty,
            Expression<Func<T, bool>> isDataProtectedProperty
        )
        {
            DataProperty = new PropertyExpression<T, TData>(dataProperty);
            IsDataProtectedProperty = new PropertyExpression<T, bool>(isDataProtectedProperty);
        }

        public PropertyExpression<T, TData> DataProperty { get; set; }

        public PropertyExpression<T, bool> IsDataProtectedProperty { get; set; }
    }
}

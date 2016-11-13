using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RequestIdValidator
{
    public interface IIdentityValidator
    {
        ValidationResponse VerifyIds<T>(object queryStringId, T body, Expression<Func<T, object>> identityProperty);
    }

    public class IdentityValidator : IIdentityValidator
    {
        public static ValidationResponse VerifyIds<T>(object queryStringId, T body, Expression<Func<T, object>> identityProperty)
        {
            if (body == null)
            {
                return new ValidationResponse(ValidationResult.ErrorMissingBody);
            }

            var memberSelectorExpression = GetMemberExpression(identityProperty?.Body);
            if (memberSelectorExpression == null)
            {
                return new ValidationResponse(ValidationResult.ErrorMissingOrInvalidLamda);
            }

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property != null)
            {
                object bodyId = property.GetValue(body);
                object defaultValue = GetDefaultValue(bodyId.GetType());

                if (bodyId.Equals(defaultValue))
                {
                    property.SetValue(body, queryStringId, null);
                }
                else if (!queryStringId.Equals(bodyId))
                {
                    return new ValidationResponse(ValidationResult.ErrorUnequalIds);
                }
            }

            return new ValidationResponse(ValidationResult.Valid);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            MemberExpression body = expression as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = expression as UnaryExpression;
                if (ubody != null)
                {
                    body = ubody.Operand as MemberExpression;
                }
            }

            return body;
        }

        private static object GetDefaultValue(Type t)
        {
            if (t.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(t) == null)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        ValidationResponse IIdentityValidator.VerifyIds<T>(object queryStringId, T body, Expression<Func<T, object>> identityProperty)
        {
            return VerifyIds(queryStringId, body, identityProperty);
        }
    }
}
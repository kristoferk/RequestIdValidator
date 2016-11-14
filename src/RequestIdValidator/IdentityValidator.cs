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
            if (identityProperty == null || memberSelectorExpression == null)
            {
                return new ValidationResponse(ValidationResult.ErrorMissingOrInvalidLamda);
            }

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                return new ValidationResponse(ValidationResult.ErrorMissingOrInvalidLamda);
            }

            var navigationPropertyName = GetParentString(identityProperty.Body);

            object bodyId;
            if (navigationPropertyName.Split('.').Length - 1 <= 0)
            {
                bodyId = property.GetValue(body);
                object defaultValue = GetDefaultValue(bodyId.GetType());

                if (bodyId.Equals(defaultValue))
                {
                    property.SetValue(body, queryStringId, null);
                }
                else if (!queryStringId.Equals(bodyId))
                {
                    return new ValidationResponse(ValidationResult.ErrorNotEqualIds);
                }
            }
            else
            {
                var compiled = identityProperty.Compile();
                bodyId = compiled.Invoke(body);
                object defaultValue = GetDefaultValue(bodyId.GetType());

                if (bodyId.Equals(defaultValue))
                {
                    SetProperty(navigationPropertyName, body, queryStringId);
                }
                else if (!queryStringId.Equals(bodyId))
                {
                    return new ValidationResponse(ValidationResult.ErrorNotEqualIds);
                }
            }

            return new ValidationResponse(ValidationResult.Valid);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            MemberExpression body = expression as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = expression as UnaryExpression;
                if (ubody != null)
                {
                    return GetMemberExpression(ubody.Operand);
                }
            }

            return body;
        }

        private static void SetProperty(string compoundProperty, object target, object value)
        {
            string[] bits = compoundProperty.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetTypeInfo().GetDeclaredProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
            }
            PropertyInfo propertyToSet = target.GetType().GetTypeInfo().GetDeclaredProperty(bits[bits.Length-1]);
            propertyToSet.SetValue(target, value, null);
        }

        private static object GetDefaultValue(Type t)
        {
            if (t.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(t) == null)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        private static string GetParentString(MemberExpression expression, string str)
        {
            if (expression == null)
            {
                return string.Empty;
            }

            var propertyExpression = expression.Expression as MemberExpression;
            if (propertyExpression != null)
            {
                str = propertyExpression.Member.Name + "." + str;
                return GetParentString(propertyExpression, str);
            }

            return str;
        }

        private static string GetParentString(Expression expression)
        {
            MemberExpression body = expression as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = expression as UnaryExpression;
                if (ubody != null)
                {
                    body = ubody.Operand as MemberExpression;
                }
            }

            return GetParentString(body, body?.Member.Name);
        }

        ValidationResponse IIdentityValidator.VerifyIds<T>(object queryStringId, T body, Expression<Func<T, object>> identityProperty)
        {
            return VerifyIds(queryStringId, body, identityProperty);
        }
    }
}
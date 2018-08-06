using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Talent.ClientCommonLib
{
    public static class PropertySupport
    {
        #region static methods

        #region ExtractPropertyName 根据表达式获取属性名称
        /// <summary>
        /// 根据表达式获取属性名称
        /// </summary>
        /// <typeparam name="T">对象类型.</typeparam>
        /// <param name="propertyExpression">属性表达式(e.g. p => p.PropertyName)</param>
        /// <returns>属性名称.</returns>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = ExtractProperty<T>(propertyExpression);

            return memberExpression.Name;
        }
        /// <summary>
        /// 根据表达式获取MemberInfo
        /// </summary>
        /// <example>
        /// <code>
        ///  string prpName =PropertySupport.ExtractPropertyName(() => this.Id);
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static PropertyInfo ExtractProperty<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("表达式错误！非正确的属性表达式！");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("表达式错误！非正确的属性表达式！");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("表达式错误！非正确的属性表达式！");
            }
            return memberExpression.Member as PropertyInfo;
        }
        #endregion

        #endregion

    }

    public class ErrorMessage
    {
        public static readonly string ExpressionError = "";
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Talent.ClientCommonLib
{
    /// <summary>
    /// Model层类库，实现了接口 INotifyPropertyChanged, INotifyDataErrorInfo
    /// </summary>
    public class BaseNotifyObject : INotifyPropertyChanged//, INotifyDataErrorInfo
    {
        #region INotifyPropertyChanged 成员

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            this.RaisePropertyChanged(propertyName);
        }

        #endregion

        #region INotifyDataErrorInfo 成员

        /// <summary>
        /// 存放错误信息，一个Property可能对应多个错误信息
        /// </summary>
        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 获取错误信息列表
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>错误信息列表</returns>
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;
            else if (errors.ContainsKey(propertyName))
                return errors[propertyName];
            else
                return null;
        }

        /// <summary>
        /// 是否有错误信息
        /// </summary>
        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        /// <summary>
        /// 设置属性的错误信息
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyErrors">错误信息列表</param>
        protected void SetErrors(string propertyName, List<string> propertyErrors)
        {
            errors.Remove(propertyName);
            errors.Add(propertyName, propertyErrors);
            //if (ErrorsChanged != null)
            //    ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 清空属性的错误信息
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void ClearErrors(string propertyName)
        {
            errors.Remove(propertyName);
            //if (ErrorsChanged != null)
            //    ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion
    }
}

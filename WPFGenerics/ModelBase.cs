using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WPFGenerics
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = new Dictionary<string, PropertyChangedEventArgs>();
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyChange<T>(Expression<Func<T>> propertySelector) {
            var myName = GetMemberName<T>(propertySelector);
            if (!string.IsNullOrEmpty(myName))
                NotifyChange(myName);
        }

        protected virtual void NotifyChange(string propertyName)
        {
            if (_argsCache != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                NotifyChange(_argsCache[propertyName]);
            }
        }

        private void NotifyChange(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public string GetMemberName<T>(Expression<Func<T>> propertySelector)
        {
            var expression = (MemberExpression)propertySelector.Body;
            return expression.Member.Name;
        }
    }
}

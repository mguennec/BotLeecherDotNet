using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{
    /// <summary>
    /// http://mui.codeplex.com/discussions/454400
    /// http://wpflocalizeextension.codeplex.com/discussions/435767
    /// https://github.com/SeriousM/WPFLocalizationExtension/blob/master/Tests/XamlLocalizationTest/Window1.xaml.cs#L50
    /// http://stackoverflow.com/questions/491429/how-to-get-the-propertyinfo-of-a-specific-property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class PropertyHelper<T>
    {
        public static PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

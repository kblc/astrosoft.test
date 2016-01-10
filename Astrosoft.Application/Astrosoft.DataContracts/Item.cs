using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Astrosoft.DataContracts
{
    public class Item : IItem
    {
        private long index = 0;
        private string message = string.Empty;
        private string system = string.Empty;
        private DateTime time = DateTime.MinValue;
        private char type = char.MinValue;

        public long Index { get { return index; } set { index = value; RaisePropertyChanged(); } }
        public string Message { get { return message; } set { message = value; RaisePropertyChanged(); } }
        public string System { get { return system; } set { system = value; RaisePropertyChanged(); } }
        public DateTime Time { get { return time; } set { time = value; RaisePropertyChanged(); } }
        public char Type { get { return type; } set { type = value; RaisePropertyChanged(); } }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpr = propertyExpression.Body as MemberExpression;
            if (memberExpr == null)
                throw new ArgumentException("propertyExpression should represent access to a member");
            string memberName = memberExpr.Member.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        protected virtual void RaisePropertyChanged([ParenthesizePropertyName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

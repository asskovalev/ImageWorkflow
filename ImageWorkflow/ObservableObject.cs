using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ImageWorkflow
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
		{
			var propertyName = GetPropertyName(action);
			RaisePropertyChanged(propertyName);
		}

		private static string GetPropertyName<T>(Expression<Func<T>> action)
		{
			var expression = (MemberExpression)action.Body;
			var propertyName = expression.Member.Name;
			return propertyName;
		}

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

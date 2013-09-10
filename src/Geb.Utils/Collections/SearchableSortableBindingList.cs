using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Geb.Utils.Collections
{
	public class PropertyComparer<T> : System.Collections.Generic.IComparer<T>
	{
		private PropertyDescriptor _property;
		private ListSortDirection _direction;

		public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
		{
			_property = property;
			_direction = direction;
		}

		#region IComparer<T>

		public int Compare(T xWord, T yWord)
		{
			object xValue = GetPropertyValue(xWord, _property.Name);
			object yValue = GetPropertyValue(yWord, _property.Name);

			if (_direction == ListSortDirection.Ascending)
			{
				return CompareAscending(xValue, yValue);
			}
			else
			{
				return CompareDescending(xValue, yValue);
			}
		}

		public bool Equals(T xWord, T yWord)
		{
			return xWord.Equals(yWord);
		}

		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}

		#endregion

		private int CompareAscending(object xValue, object yValue)
		{
			if (xValue == null && yValue == null) return 0;
			if (xValue == null) return -1;

			int result;

			if (xValue is IComparable)
			{
				result = ((IComparable)xValue).CompareTo(yValue);
			}
			else if (xValue.Equals(yValue))
			{
				result = 0;
			}
			else result = xValue.ToString().CompareTo(yValue.ToString());

			return result;
		}

		private int CompareDescending(object xValue, object yValue)
		{
			return CompareAscending(xValue, yValue) * -1;
		}

		private object GetPropertyValue(T value, string property)
		{
			// Get property
			PropertyInfo propertyInfo = value.GetType().GetProperty(property);

			// Return value
			return propertyInfo.GetValue(value, null);
		}
	}
	public class SearchableSortableBindingList<T> : BindingList<T>
	{
		private bool _isSorted;
		private PropertyDescriptor _sortProperty;
		private ListSortDirection _sortDirection;

		protected override bool SupportsSortingCore
		{
			get { return true; }
		}

		// Missing from Part 2
		protected override ListSortDirection SortDirectionCore
		{
			get { return _sortDirection; }
		}

		// Missing from Part 2
		protected override PropertyDescriptor SortPropertyCore
		{
			get { return _sortProperty; }
		}

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{

			List<T> items = this.Items as List<T>;

			if (items != null)
			{
				PropertyComparer<T> pc = new PropertyComparer<T>(property, direction);
				items.Sort(pc);
				_isSorted = true;
			}
			else
			{
				_isSorted = false;
			}

			_sortProperty = property;
			_sortDirection = direction;

			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override bool IsSortedCore
		{
			get { return _isSorted; }
		}

		protected override void RemoveSortCore()
		{
			_isSorted = false;
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override bool SupportsSearchingCore
		{
			get { return true; }
		}

		protected override int FindCore(PropertyDescriptor property, object key)
		{

			// Specify search columns
			if (property == null) return -1;

			// Get list to search
			List<T> items = this.Items as List<T>;

			// Traverse list for value
			foreach (T item in items)
			{

				// Test column search value
				string value = (string)property.GetValue(item);

				// If value is the search value, return the 
				// index of the data item
				if ((string)key == value) return IndexOf(item);
			}
			return -1;
		}
	}
}

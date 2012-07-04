using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Utils
{
	public class CategoryItem
	{
		public String Name { get; set; }
		public String Category { get; set; }
		public String Introduce { get; set; }
		public Object Value { get; set; }
		public CategoryItem()
		{
			Name = Category = Introduce = String.Empty;
		}
	}
}

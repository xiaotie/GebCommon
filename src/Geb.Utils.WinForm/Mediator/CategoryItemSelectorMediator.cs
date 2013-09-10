using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Geb.Utils.WinForm
{
	public class CategoryItemSelectorMediator
	{
		protected List<CategoryItem> Items { get; set; }
		protected ListBox BindedListBox { get; set; }
		protected ComboBox BindedComboBox { get; set; }
		protected TextBox BindedTextBox { get; set; }

		public CategoryItemSelectorMediator()
		{
			Items = new List<CategoryItem>();
		}

		public event EventHandler<EventArgs> ListBoxItemDoubleClick;

		protected virtual void OnListBoxItemDoubleClick(EventArgs e)
		{
			EventHandler<EventArgs> handler = ListBoxItemDoubleClick;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public void Bind(IEnumerable<CategoryItem> items, ComboBox comboBox, ListBox listBox, TextBox txtBox)
		{
			if (items == null) throw new ArgumentNullException("items");
			if (comboBox == null) throw new ArgumentNullException("comboBox");
			if (listBox == null) throw new ArgumentNullException("listBox");
			if (txtBox == null) throw new ArgumentNullException("txtBox");

			this.Items.Clear();
			this.Items.AddRange(items);
			this.BindedComboBox = comboBox;
			this.BindedListBox = listBox;
			this.BindedTextBox = txtBox;
			this.BindedComboBox.SelectedIndexChanged += new EventHandler(BindedComboBox_SelectedIndexChanged);
			this.BindedListBox.SelectedIndexChanged += new EventHandler(BindedListBox_SelectedIndexChanged);
			this.BindedListBox.MouseDoubleClick += new MouseEventHandler(BindedListBox_MouseDoubleClick);
			this.BindedComboBox.DataSource = this.GetCategorys();
			BindListBox("全部");
		}

		private void BindedListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int itemRegionHeight = this.BindedListBox.Items.Count * this.BindedListBox.ItemHeight;
			if (e.Y <= itemRegionHeight)
			{
				OnListBoxItemDoubleClick(EventArgs.Empty);
			}
		}

		private void BindListBox(String category)
		{
			this.BindedListBox.DataSource = this.Find(category);
			this.BindedListBox.DisplayMember = "Name";
		}

		private List<String> GetCategorys()
		{
			List<String> categorys = new List<string>();
			categorys.Add("全部");
			foreach (var item in Items)
			{
				Boolean find = false;
				foreach (var c in categorys)
				{
					if (c == item.Category)
					{
						find = true;
						break;
					}
				}
				if (find == false) categorys.Add(item.Category);
			}
			return categorys;
		}

		private List<CategoryItem> Find(String categoryName)
		{
			if (categoryName == "全部") return this.Items;
			else
			{
				List<CategoryItem> finds = new List<CategoryItem>();
				foreach (var item in this.Items)
				{
					if (item.Category == categoryName) finds.Add(item);
				}
				return finds;
			}
		}

		private void BindedComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.BindListBox(this.BindedComboBox.Text);
		}

		private void BindedListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Object obj = this.BindedListBox.SelectedItem;
			if (obj == null) this.BindedTextBox.Text = String.Empty;
			else
			{
				CategoryItem item = obj as CategoryItem;
				this.BindedTextBox.Text = item.Introduce;
			}
		}

		public List<CategoryItem> SelectItems
		{
			get
			{
				List<CategoryItem> list = new List<CategoryItem>(this.BindedListBox.SelectedItems.Count);
				foreach (var item in this.BindedListBox.SelectedItems)
				{
					CategoryItem i = item as CategoryItem;
					if (i != null) list.Add(i);
				}
				return list;
			}
		}

		public List<Object> SelectValues
		{
			get
			{
				List<Object> list = new List<Object>(this.BindedListBox.SelectedItems.Count);
				foreach (var item in this.BindedListBox.SelectedItems)
				{
					CategoryItem i = item as CategoryItem;
					if (i != null) list.Add(i.Value);
				}
				return list;
			}
		}

		public List<T> GetSelectValues<T>()
		{
			List<T> list = new List<T>(this.BindedListBox.SelectedItems.Count);
			foreach (var item in this.BindedListBox.SelectedItems)
			{
				CategoryItem i = item as CategoryItem;
				if (i != null && i.Value != null) list.Add((T)(i.Value));
			}
			return list;
		}
	}
}

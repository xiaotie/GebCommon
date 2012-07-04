using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Geb.Utils.Collections;

namespace Geb.Utils.WinForm
{
	public class PagerMediator<T> where T : class
	{
		public Int32 CountPerPage = 200;

		protected BindingNavigator Navigator { get; set; }
		protected DataGridView GridView { get; set; }
		protected BindingSource BindingSource { get; set; }
		protected BindingList<SearchableSortableBindingList<T>> DataSourcePagered { get; set; }

		private Object SyncRoot = new object();

		public Int32 TotalCount
		{
			get
			{
				Int32 cnt = 0;
				if (DataSourcePagered != null)
				{
					lock (SyncRoot)
					{
						foreach (var item in DataSourcePagered)
						{
							cnt += item.Count;
						}
					}
				}
				return cnt;
			}
		}

		public PagerMediator()
		{
			DataSourcePagered = new BindingList<SearchableSortableBindingList<T>>();
			BindingSource = new BindingSource();
			BindingSource.DataSource = DataSourcePagered;
			this.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
		}

		public event EventHandler<EventArgs> CountChanged;

		protected virtual void OnCountChanged(EventArgs e)
		{
			EventHandler<EventArgs> handler = CountChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public void Bind(BindingNavigator nav, DataGridView dgv)
		{
			if (nav == null) throw new ArgumentNullException("nav");
			if (dgv == null) throw new ArgumentNullException("dgv");

			this.Navigator = nav;
			this.GridView = dgv;

			this.Navigator.BindingSource = this.BindingSource;
		}

		private void BindingSource_CurrentChanged(object sender, EventArgs e)
		{
			SearchableSortableBindingList<T> clist = this.BindingSource.Current as SearchableSortableBindingList<T>;
			if (clist != null)
			{
				BindDataGridView(clist);
			}
		}

		public void InitData(IEnumerable<T> data)
		{
			InitData(data, false);
		}

		public void InitData(IEnumerable<T> data, Boolean gotoPageBefore)
		{
			this.Navigator.SuspendLayout();
			Int32 page = this.BindingSource.Position;
			lock (SyncRoot)
			{
				DataSourcePagered.Clear();
				SearchableSortableBindingList<T> list = new SearchableSortableBindingList<T>();
				foreach (var t in data)
				{
					list.Add(t);
					if (list.Count == CountPerPage)
					{
						DataSourcePagered.Add(list);
						list = new SearchableSortableBindingList<T>();
					}
				}

				if (list.Count > 0) DataSourcePagered.Add(list);
			}

			this.Navigator.ResumeLayout();

			if (0 < DataSourcePagered.Count)
			{
				if (gotoPageBefore == true)
				{
					if (page < 0) page = 0;
					else if (page >= DataSourcePagered.Count) page = DataSourcePagered.Count - 1;

					this.BindingSource.Position = page;
				}
				else
				{
					page = 0;
				}
				BindDataGridView(DataSourcePagered[page]);
			}
			else
			{
				BindDataGridView(null);
			}

			OnCountChanged(EventArgs.Empty);
		}

		public void Add(T t)
		{
			AddRange(new T[1] { t });
		}

		public void AddRange(IEnumerable<T> tlist)
		{
			Boolean refreshDgv =
					(this.DataSourcePagered.Count > 0)
					&& (this.BindingSource.Position == this.DataSourcePagered.Count - 1)
					&& (this.DataSourcePagered[this.DataSourcePagered.Count - 1].Count < CountPerPage);

			lock (SyncRoot)
			{
				foreach (var item in tlist)
				{
					SearchableSortableBindingList<T> list = null;
					if (DataSourcePagered.Count > 0) list = DataSourcePagered[DataSourcePagered.Count - 1];
					if (list == null || list.Count == CountPerPage)
					{
						list = new SearchableSortableBindingList<T>();
						DataSourcePagered.Add(list);
					}
					list.Add(item);
				}
			}

			if (refreshDgv)
			{
				BindDataGridView(DataSourcePagered[this.BindingSource.Position]);
			}

			OnCountChanged(EventArgs.Empty);
		}

		private void BindDataGridView(SearchableSortableBindingList<T> clist)
		{
			if (this.GridView != null)
				this.GridView.DataSource = clist;
		}

		public List<T> GetSelected()
		{
			List<T> find = new List<T>();
			SearchableSortableBindingList<T> ds = this.GridView.DataSource as SearchableSortableBindingList<T>;
			if (ds != null)
			{
				foreach (DataGridViewRow row in this.GridView.SelectedRows)
				{
					T t = ds[row.Index];
					find.Add(t);
				}
			}
			return find;
		}
	}
}

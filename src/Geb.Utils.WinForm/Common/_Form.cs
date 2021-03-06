﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Geb.Utils.WinForm
{
    public delegate void Action();
    public delegate void Action<T>(T t);
    public delegate void Action<T0,T1>(T0 t0,T1 t1);

    public class ControlFuncContext
	{
		public Control Control { get; private set; }
		public Delegate Delegate { get; private set; }

		public ControlFuncContext(Control ctl, Delegate d)
		{
			this.Control = ctl;
			this.Delegate = d;
		}

		public void Invoke0()
		{
            if (Control.IsHandleCreated == true && Control.IsDisposed == false)
			{
                try
                {
                    Delegate.DynamicInvoke();
                }
                catch(ObjectDisposedException ex)
                {
                }
			}
		}

		public void Invoke1<T>(T obj)
		{
			if (Control.IsHandleCreated == true && Control.IsDisposed == false)
			{
                try
                {
                    Delegate.DynamicInvoke(obj);
                }
                catch (ObjectDisposedException ex)
                {
                }
			}
		}

		public void Invoke2<T0,T1>(T0 obj0, T1 obj1)
		{
            if (Control.IsHandleCreated == true && Control.IsDisposed == false)
			{
                try
                {
                    Delegate.DynamicInvoke(obj0, obj1);
                }
                catch (ObjectDisposedException ex)
                {
                }
			}
		}
	}

    public static class PictureBoxClassHelper
    {
        public static void ShowImage(this PictureBox box, System.Drawing.Image image, Boolean disposeOld = true)
        {
            System.Drawing.Image old = box.Image;
            box.Image = image;
            if (disposeOld == true && old != null) old.Dispose();
        }
    }

	public static class FormClassHelper
	{

		#region Control Extend Methods

		public static void EnableControls(this Control ctl, Boolean status)
		{
			ctl.Enabled = status;
            foreach (Control c in ctl.Controls)
            {
                if (c is Label) continue;
                c.Enabled = status;
            }
		}

		public static void InvokeAction(this Control ctl, Action action)
		{
			if (ctl.IsHandleCreated == true)
			{
                try
                {
                    ControlFuncContext fc = new ControlFuncContext(ctl, action);
                    ctl.Invoke(new Action(fc.Invoke0));
                }
                catch (ObjectDisposedException)
                {
                }
			}
		}

        public static void InvokeAction<T>(this Control ctl, Action<T> action, T obj)
		{
			if (ctl.IsHandleCreated == true)
			{
                try
                {
                    ControlFuncContext fc = new ControlFuncContext(ctl, action);
                    ctl.Invoke(new Action<T>(fc.Invoke1<T>), obj);
                }
                catch (ObjectDisposedException)
                {
                }
			}
		}

        public static void InvokeAction<T0, T1>(this Control ctl, Action<T0, T1> action, T0 obj0, T1 obj1)
		{
			if (ctl.IsHandleCreated == true)
			{
                try
                {
                    ControlFuncContext fc = new ControlFuncContext(ctl, action);
                    ctl.Invoke(new Action<T0, T1>(fc.Invoke2<T0, T1>), obj0, obj1);
                }
                catch (ObjectDisposedException)
                {
                }
			}
		}

		public static void ShowErrorReport(this Control ctl, Exception ex, Boolean showStack)
		{
			if (ex == null) return;
			FrmErrorReport f = new FrmErrorReport();
			f.Title = ex.Message;
			if (showStack)
				f.Content = ex.StackTrace;
			f.ShowDialog();
		}

		public static Boolean ShowEnsureDelMsgBox(this Control ctl)
		{
			return MessageBox.Show("确认删除?","警告", MessageBoxButtons.YesNo) == DialogResult.Yes;
		}

		public static Boolean ShowWarningEmptyListMsgBoxAndEnsureDelMsgBox<T>(this Control ctl, ICollection<T> dels)
		{
			if (dels == null || dels.Count == 0)
			{
                MessageBox.Show("没有选中项.");
				return false;
			}

			return ctl.ShowEnsureDelMsgBox();
		}

		#endregion

		public static List<T> GetSelectedItems<T>(this DataGridView dgv)
		{
			List<T> selects = new List<T>();
			foreach (DataGridViewRow row in dgv.SelectedRows)
			{
				T t = (T)row.DataBoundItem;
				selects.Add(t);
			}
			return selects;
		}

		/// <summary>
		/// 修改DataGridView的属性：
		///		SelectionMode : DataGridViewSelectionMode.FullRowSelect
		///		AllowUserToAddRows: false
		///		AllowUserToDeleteRows : false
		///		AllowUserToOrderColumns : false
		///		AllowUserToResizeRows : false
		///		AutoSizeColumnsMode : DataGridViewAutoSizeColumnsMode.Fill
		///		BackgroundColor : System.Drawing.Color.White
		///		RowHeadersVisible : false
		/// </summary>
		/// <param name="dgv"></param>
		public static void UseDefaultMode00(this DataGridView dgv)
		{
			dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgv.AllowUserToAddRows = false;
			dgv.AllowUserToDeleteRows = false;
			dgv.AllowUserToOrderColumns = false;
			dgv.AllowUserToResizeRows = false;
			dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dgv.BackgroundColor = System.Drawing.Color.White;
			dgv.RowHeadersVisible = false;
		}

        /// <summary>
        /// 设置TextBox为：
        ///     显示垂直滚动条;
        ///     内容长度为Int32.MaxValue;
        ///     Ctrl+A 选中全部内容.
        /// </summary>
        /// <param name="tb">TextBox实例</param>
        public static void Enhance(this TextBox tb)
        {
            tb.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            tb.ScrollBars = ScrollBars.Vertical;
            tb.MaxLength = Int32.MaxValue;
        }

        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.A)
            {
                TextBox tb = sender as TextBox;
                tb.SelectAll();
            }
        }

        public static void OpenFile(this Form element, Action<String> callbackOnFilePath, String filter = "所有文件|*.*")
        {
            String filePath= null;
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = filter;
            dlg.FileOk += (object sender, CancelEventArgs e) =>
            {
                filePath = dlg.FileName;
            };

            dlg.ShowDialog();

            if (filePath != null)
            {
                if (callbackOnFilePath != null)
                    callbackOnFilePath(filePath);
            }
        }

        public static void OpenImageFile(this Form element, Action<String> callbackOnFilePath, String filter = "图像文件|*.bmp;*.jpg;*.gif;*.png")
        {
            OpenFile(element, callbackOnFilePath, filter);
        }

        public static void OpenVideoFile(this Form element, Action<String> callbackOnFilePath, String filter = "视频文件|*.avi;*.mp4;*.flv;*.f4v")
        {
            OpenFile(element, callbackOnFilePath, filter);
        }

        public static void OpenDir(this Form element, Action<String> callbackOnDirPath)
        {
            String dirPath;
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (callbackOnDirPath != null)
                {
                    dirPath = dlg.SelectedPath;
                    callbackOnDirPath(dirPath);
                }
            }
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerContext cxt = e.Argument as WorkerContext;
            try
            {
                cxt.OnStart();
            }
            catch (Exception ex)
            {
                if (cxt.OnException != null)
                {
                    cxt.OnException(ex);
                }
            }
            finally
            {
                if (cxt.OnComplete != null)
                {
                    cxt.OnComplete();
                }
            }
        }

        private class WorkerContext
        {
            public Action OnStart;
            public Action OnComplete;
            public Action<Exception> OnException;
        }

        public static void Start(this BackgroundWorker worker, Action onStart, Action onComplete = null, Action<Exception> onException = null)
        {
            worker.DoWork -= Worker_DoWork;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync(new WorkerContext
            {
                OnComplete = onComplete,
                OnStart = onStart,
                OnException = onException
            });
        }
	}
}

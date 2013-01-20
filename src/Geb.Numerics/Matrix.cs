using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Numerics
{
    public class Matrix : IDisposable
    {
        /// <summary>
        /// 创建单位矩阵，
        /// </summary>
        /// <param name="rank">矩阵的秩</param>
        /// <returns>单位矩阵</returns>
        public static Matrix CreateIdentity(int rank)
        {
            Matrix m = new Matrix(rank, rank);
            for (int i = 0; i < rank; i++)
            {
                m[i, i] = 1;
            }
            return m;
        }

        private unsafe double* Data;

        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly int _length;

        /// <summary>
        /// 矩阵的行数
        /// </summary>
        public int RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// 矩阵的列数
        /// </summary>
        public int ColumnCount
        {
            get { return _columnCount; }
        }

        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// 代表当前矩阵数据的二维数组。
        /// .Net 的 IDE 均不支持直接查看.Net程序中的指针内容，DataSnapshot 提供了调试时查看
        /// 矩阵数据的唯一途径。请谨慎使用本方法。
        /// </summary>
        public double[,] DataSnapshot
        {
            get
            {
                double[,] array = new double[RowCount, ColumnCount];
                for (int r = 0; r < RowCount; r++)
                {
                    for (int c = 0; c < ColumnCount; c++)
                    {
                        array[r, c] = this[r, c];
                    }
                }
                return array;
            }
        }

        /// <summary>
        /// 读写矩阵的元素。本处读写不进行边界检查。
        /// </summary>
        /// <param name="row">元素所处行</param>
        /// <param name="column">元素所处值</param>
        /// <returns>矩阵元素值</returns>
        public unsafe double this[int row, int column]
        {
            get { return Data[row * _columnCount + column]; }
            set { Data[row * _columnCount + column] = value; }
        }

        /// <summary>
        /// 读写矩阵的元素。本处读写不进行边界检查。主要用于将矩阵用作向量（一维矩阵）时的操作。
        /// </summary>
        /// <param name="index">将矩阵当作向量，元素在向量中所处的位置</param>
        /// <returns>矩阵元素值</returns>
        public unsafe double this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public unsafe Matrix(double[] data, int rows, int columns)
        {
            if (rows < 1) throw new ArgumentException("rows must > 0");
            if (columns < 1) throw new ArgumentException("columns must > 0");
            if (data.Length != rows * columns) throw new ArgumentException("data != rows * columns");

            _rowCount = rows;
            _columnCount = columns;
            _length = _rowCount * _columnCount;
            Data = (double*)Marshal.AllocHGlobal(_rowCount * _columnCount * sizeof(double));
            for (int r = 0; r < _rowCount; r++)
            {
                int cStart = r * columns;
                for (int c = 0; c < _columnCount; c++)
                {
                    this[r, c] = data[cStart + c];
                }
            }
        }

        public unsafe Matrix(int rows, int columns = 1, Boolean fillWidthZero = true)
        {
            if (rows < 1) throw new ArgumentException("rows must > 0");
            if (columns < 1) throw new ArgumentException("columns must > 0");
            _rowCount = rows;
            _columnCount = columns;
            _length = _rowCount * _columnCount;
            Data = (double*)Marshal.AllocHGlobal(_rowCount * _columnCount * sizeof(double));
            if (fillWidthZero == true)
            {
                Fill(0);
            }
        }

        /// <summary>
        /// 向矩阵中填充元素。
        /// </summary>
        /// <param name="val"></param>
        public unsafe void Fill(double val)
        {
            double* pStart = Data;
            double* pEnd = pStart + _rowCount * _columnCount;
            while (pStart != pEnd)
            {
                *pStart = val;
                pStart++;
            }
        }

        /// <summary>
        /// 求矩阵的逆矩阵或者广义逆矩阵。
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            Matrix m = null;
            Matrix I = null;
            if (_rowCount >= _columnCount)
            {
                I = CreateIdentity(_rowCount);
                m = Solve(I);
                I.Dispose();
                return m;
            }

            I = CreateIdentity(_columnCount);
            Matrix t = Transpose();
            Matrix tInversed = t.Solve(I);
            m = tInversed.Transpose();
            I.Dispose();
            t.Dispose();
            tInversed.Dispose();
            return m;
        }

        public Matrix Transpose()
        {
            Matrix m = new Matrix(this.ColumnCount, this.RowCount);
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    m[c, r] = this[r,c];
                }
            }
            return m;
        }

        /// <summary>
        /// 解方程 A*X = B。A 为本矩阵。
        /// </summary>
        /// <param name="B">右侧矩阵</param>
        /// <returns></returns>
        public Matrix Solve(Matrix B)
        {
            if (_rowCount == _columnCount)
            {
                LUDecomposition lu = new LUDecomposition(this);
                Matrix m = lu.Solve(B);
                lu.Dispose();
                return m;
            }

            throw new InvalidOperationException("Only square supported.");
        }

        public unsafe Matrix Clone()
        {
            Matrix m = new Matrix(this.RowCount, this.ColumnCount, false);
            double* pSrc = this.Data;
            double* pDst = m.Data;
            double* pSrcEnd = pSrc + this.RowCount * this.ColumnCount;
            while (pSrc != pSrcEnd)
            {
                *pDst = *pSrc;
                pSrc++;
                pDst++;
            }
            return m;
        }


        #region 操作符重载

        public unsafe static Matrix operator +( Matrix m1, Matrix m2)
        {
            if (m1.Length != m2.Length) throw new ArgumentException("m1 and m2 must be same size");

            Matrix m = m2.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            double* p1 = m1.Data;
            while (p != pEnd)
            {
                *p = *p + *p1;
                p++; p1++;
            }
            return m;
        }

        public unsafe static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.Length != m2.Length) throw new ArgumentException("m1 and m2 must be same size");

            Matrix m = m2.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            double* p1 = m1.Data;
            while (p != pEnd)
            {
                *p = *p1 - *p;
                p++; p1++;
            }
            return m;
        }

        public unsafe static Matrix operator -(Matrix m1)
        {
            Matrix m = m1.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            while (p != pEnd)
            {
                *p = -*p;
                p++; 
            }
            return m;
        }

        public unsafe static Matrix operator *(Matrix m1, double val)
        {
            Matrix m = m1.Clone();
            double* p = m.Data;
            double* pEnd = p + m.Length;
            while (p != pEnd)
            {
                *p *= val;
                p++;
            }
            return m;
        }

        public unsafe static Matrix operator *(double val, Matrix m1)
        {
            return m1 * val;
        }

        public unsafe static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.ColumnCount != m2.RowCount) throw new ArgumentException("m1 and m2 cann't multiply");

            Matrix m = new Matrix(m1.RowCount, m2.ColumnCount, false);
            for (int j = 0; j < m2.ColumnCount; j++)
            {
                for (int i = 0; i < m1.RowCount; i++)
                {
                    double s = 0;
                    for (int k = 0; k < m1.ColumnCount; k++)
                    {
                        s += m1[i, k] * m2[k, j];
                    }

                    m[i,j] = s;
                }
            }
            return m;
        }

        #endregion 

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected unsafe virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                disposed = true;
                Marshal.FreeHGlobal((IntPtr)Data);
            }
        }

        private bool disposed;

        ~Matrix()
        {
            Dispose(false);
        } 

        #endregion
    }
}

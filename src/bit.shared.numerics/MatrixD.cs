using System;

namespace bit.shared.numerics
{
    public class MatrixD
    {
        private VectorD[] _rows;
                      
        public int NRows { get { return _rows.Length; } }
        public int NCols { get { return _rows[0].Dimension; } }
        
        public MatrixD(int nrows, int ncols)
        {
            _rows = new VectorD[nrows];
             for(int i=0;i<_rows.Length;++i) {
                _rows[i] = new VectorD(ncols);
            }
        }
        
        public MatrixD(MatrixD matrix)
        {
            _rows = new VectorD[matrix.NRows];
            for(int i=0;i<_rows.Length;++i) {
                _rows[i] = new VectorD(matrix._rows[i]);
            }
        }
        
        public MatrixD(VectorD[] rows)
        {
            _rows = rows;
        }
        
        public MatrixD(double[][] rows)
        {
            _rows = new VectorD[rows.Length];
            for(int i=0;i<_rows.Length;++i) {
                _rows[i] = new VectorD(rows[i]);
            }
        }
        
        public static MatrixD With(params VectorD[] rows)
        {
            return new MatrixD(rows);
        }
        
        public VectorD this[int i]
        {
            get 
            {
                return _rows[i];
            }
            set
            {
                _rows[i] = value;
            }
        }
        
        public double[][] ToArray()
        {
            var array = new double[this.NRows][];
            for(int i=0;i<array.Length;++i) {
                array[i] = _rows[i].ToArray();
            }
            return array;            
        }
        
        public override string ToString ()
        {
              return "["+ string.Join(",",Map.Transform(_rows,((i,x)=>x.ToString()))) +"]";
        }
        
        public VectorD Row(int i) 
        {
            return _rows[i];
        }
        
        public VectorD Col(int j) 
        {
            var colv = new double[this.NRows];
            for(int i=0;i<colv.Length;++i) {
                colv[i] = _rows[i][j];
            }
            return new VectorD(colv);
        }
        
        public MatrixD SwapRows(int i1,int i2)
        {
            if(i1!=i2) {
                var c = _rows[i1];
                _rows[i1] = _rows[i2];
                _rows[i2] = c;
            }
            return this;
        }
                                 
        public VectorD ProductWithColumnVector(MatrixD a, VectorD b)
        {
            if(a.NCols != b.Dimension) {
                throw new InvalidOperationException("Matrix Row/Column Mistmatch");                
            }
            
            var r = new VectorD(a.NCols);
            for(int i=0;i<r.Dimension;++i){
                r[i] = a.Row(i) * b;
            }
            return r;
        }
        
        public MatrixD DoGaussianElimination()
        {
            for(int k=0;k<this.NRows;++k)
            {
                var i_max = Reduce.ArgMax(k,this.NRows,(i)=>Math.Abs(this[i][k]));               
                this.SwapRows(k,i_max);                
                var a = this[k][k];
                if (a == 0.0) {
                   return null; // matrix is singular 
                }
                this[k] *= 1.0/a;
                for(int i=k+1;i<this.NRows;++i) {
                    this[i] -= this[k]*this[i][k];
                }      
            }   
            return this;
        }
        
        public MatrixD DoGaussJordan()
        {
            if(this.DoGaussianElimination()!=null)
            {
                for(int k=this.NRows-1;k>0;--k)
                {
                    for(int i=k-1;i>=0;--i) {
                        this[i] -= this[k]*this[i][k];
                    } 
                }
                return this;
            }
            return null;
        }
        
        
        public static MatrixD operator +(MatrixD a, MatrixD b)
        {
            var r = new MatrixD(a);
             for(int i=0;i<r._rows.Length;++i) {
                r._rows[i] += b._rows[i];
            }
            return r;
        }

        public static MatrixD operator -(MatrixD a, MatrixD b)
        {
            var r = new MatrixD(a);
             for(int i=0;i<r._rows.Length;++i) {
                r._rows[i] -= b._rows[i];
            }
            return r;
        }

        public static MatrixD operator -(MatrixD b)
        {
            var r = new MatrixD(b);
             for(int i=0;i<r._rows.Length;++i) {
                r._rows[i] = -r._rows[i];
            }
            return r;
        }

        public static MatrixD operator *(MatrixD a, double s)
        {
           var r = new MatrixD(a);
             for(int i=0;i<r._rows.Length;++i) {
                r._rows[i] *= s;
            }
            return r;
        }

        public static MatrixD operator *(double s, MatrixD a)
        {
            var r = new MatrixD(a);
             for(int i=0;i<r._rows.Length;++i) {
                r._rows[i] *= s;
            }
            return r;
        }
        
        public static MatrixD operator *(MatrixD a, MatrixD b)
        {
            if(a.NCols != b.NRows) {
                throw new InvalidOperationException("Matrix Row/Column Mistmatch");                
            }
            
            var r = new MatrixD(a.NRows,b.NCols);
            for(int i=0;i<r.NRows;++i){
                for(int j=0;j<r.NCols;++j) {
                    r._rows[i][j] = a.Row(i) * b.Col(j);
                }
            }
            return r;
        }     
    }    
}


// Складання матриці з числом зі строковим розподілом по стовпцях.
Console.WriteLine("Hello, World!");

internal class Matrix<T>{
    private T[,] _matrixField;
    public Matrix(uint RowCount = 0, uint ColumnCount = 0){
        _matrixField = new T[RowCount, ColumnCount];
    }

    public Matrix(Matrix<T> CopyMatrix){
        this._matrixField = CopyMatrix.ToArray();
    }

   public T this[uint i, uint j]
   {
      get { return _matrixField[i, j]; }
      set { _matrixField[i, j] = value; }
   }

   public T[,] ToArray(){
        return _matrixField;
   }
}
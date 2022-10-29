// Складання матриці з числом зі строковим розподілом по стовпцях.
using AdvancedMath;

Matrix<int> matrixSample = new Matrix<int>(5,5);
MatrixElementApplier<int,int> adder = new MatrixElementApplier<int, int>();
adder.ApplyElementToElements(matrixSample, 5, delegate(int a, int b){ return a + b;}).Print();

namespace AdvancedMath{
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

        public void Print(){
                for(uint i = 0; i < this.ToArray().GetLength(0); ++i){
                    for(uint j = 0; j < this.ToArray().GetLength(1); ++j){
                        Console.Write(_matrixField[i, j].ToString() + " ");
                    }
                    Console.Write("\n");
                }
        }
    }


    internal class MatrixElementApplier<T, F> {
        public delegate dynamic ApplyDelegate(T Item1, F Item2);
        public Matrix<T> ApplyElementToElements(Matrix<T> MatrixToAdd, F ElementToAdd, ApplyDelegate Add){
            for(uint i = 0; i < MatrixToAdd.ToArray().GetLength(0); ++i){
                for(uint j = 0; j < MatrixToAdd.ToArray().GetLength(1); ++j){
                    MatrixToAdd[i, j] = Add(MatrixToAdd[i, j], ElementToAdd);
                }
            }
            return MatrixToAdd;
        }
    }
}







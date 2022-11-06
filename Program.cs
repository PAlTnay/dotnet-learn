// Складання матриці з числом зі строковим розподілом по стовпцях.
using System.Diagnostics;
using AdvancedMath;

Console.WriteLine(Environment.GetCommandLineArgs()[0]);

Matrix<int> matrixSample = new Matrix<int>(12,12);
MatrixElementApplier<int,int> adder = new MatrixElementApplier<int, int>();
adder.ApplyElementToElementsThreaded(matrixSample, 5, delegate(int a, int b){ return a + b;}).Print();

namespace AdvancedMath{
    internal class Matrix<T>{
        private T[,] _matrixField;
        public Matrix(int RowCount = 0, int ColumnCount = 0){
            _matrixField = new T[RowCount, ColumnCount];
        }

        public Matrix(Matrix<T> CopyMatrix){
            this._matrixField = CopyMatrix._matrixField;
        }

        public T this[int i, int j]
        {
            get { return _matrixField[i, j]; }
            set { _matrixField[i, j] = value; }
        }

        public T[,] ToArray(){
                return _matrixField;
        }

        public void Print(){
                for(int i = 0; i < this.ToArray().GetLength(0); ++i){
                    for(int j = 0; j < this.ToArray().GetLength(1); ++j){
                        Console.Write(_matrixField[i, j].ToString() + " ");
                    }
                    Console.Write("\n");
                }
        }
    }


    internal class MatrixElementApplier<T, F> {
        public delegate dynamic ApplyDelegate(T Item1, F Item2);
        public Matrix<T> ApplyElementToElements(Matrix<T> MatrixToAdd, F ElementToAdd, ApplyDelegate Add){
            for(int i = 0; i < MatrixToAdd.ToArray().GetLength(0); ++i){
                for(int j = 0; j < MatrixToAdd.ToArray().GetLength(1); ++j){
                    MatrixToAdd[i, j] = Add(MatrixToAdd[i, j], ElementToAdd);
                }
            }
            return MatrixToAdd;
        }

        record struct ThreadedColumnsApplierContainer(Matrix<T> MatrixHost, int ColumnsStartIndex, int ColumnsEndIndex, F ElementToApply, ApplyDelegate Delegate){
            public void ApplyToElements(){
                for(int columnIndex = ColumnsStartIndex; columnIndex < ColumnsEndIndex ; ++columnIndex){
                    if(columnIndex >= MatrixHost.ToArray().GetLength(1)) break;
                    
                    for(int rowIndes = 0; rowIndes < MatrixHost.ToArray().GetLength(0);++ rowIndes){
                        MatrixHost[rowIndes, columnIndex] = Delegate(MatrixHost[rowIndes, columnIndex],ElementToApply);
                    }
                }
            }
        }

        public Matrix<T> ApplyElementToElementsThreaded(Matrix<T> MatrixToAdd, F ElementToAdd, ApplyDelegate Add){

            int matrixColumnsCount = MatrixToAdd.ToArray().GetLength(1);
            int threadsCount = Environment.ProcessorCount;

            if(matrixColumnsCount < threadsCount){
               
                //TODO: make spliting by objects count 
                return ApplyElementToElements(MatrixToAdd, ElementToAdd, Add);;
            }


            int columnsPerThread = (matrixColumnsCount / threadsCount) + (matrixColumnsCount % threadsCount == 0 ? 0 : 1);
            Thread[] rowAditionThreads = new Thread[threadsCount];
            int columnIndexForNexThread = 0;

            for(int threadIndex = 0; threadIndex < threadsCount; ++threadIndex){
                ThreadedColumnsApplierContainer container = new ThreadedColumnsApplierContainer(
                    MatrixToAdd, columnIndexForNexThread, columnIndexForNexThread + columnsPerThread, ElementToAdd, Add);

                columnIndexForNexThread += columnsPerThread;

                rowAditionThreads[threadIndex] = new Thread(container.ApplyToElements);
                rowAditionThreads[threadIndex].Start();
            }

            //Console.WriteLine(Process.GetCurrentProcess().Threads.Count.ToString());

            foreach(Thread thread in rowAditionThreads){
                thread.Join();
            }
            
            
            return MatrixToAdd;
        }
    }
}







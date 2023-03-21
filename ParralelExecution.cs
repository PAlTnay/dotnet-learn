// Складання матриці з числом зі строковим розподілом по стовпцях.
using System;
using AdvancedMath;


class Lab1 : UserCustom.Lab{
    public void Entry(){
        Console.WriteLine("Matrix size:");
        string matrixSizeLine = Console.ReadLine();

        string[] size = matrixSizeLine.Split(' ', 2);
        int rowsCount = int.Parse(size[0]);
        int columnsCount = int.Parse(size[1]);

        Console.WriteLine("To add:");
        int numberToAdd = int.Parse(Console.ReadLine());


        Console.WriteLine("ShowMatrix?(true,false):");
        bool showMatrix = bool.Parse(Console.ReadLine());

        Matrix<int> matrixSample = new Matrix<int>(rowsCount,columnsCount, showMatrix);
        matrixSample.Print();
        MatrixElementApplier<int, int> adder = new MatrixElementApplier<int, int>();

        Console.WriteLine("Single thread:");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        adder.ApplyElementToElements(matrixSample, numberToAdd, delegate(int a, int b){ return a + b;}).Print();

        watch.Stop();
        double singleThreadTime = watch.Elapsed.TotalMilliseconds;
        Console.WriteLine($"Execution Time: {singleThreadTime} ms");
        matrixSample.Reset();

        Console.WriteLine("Multy thread:");
        watch.Reset();
        watch.Start();

        adder.ApplyElementToElementsThreaded(matrixSample, numberToAdd, delegate(int a, int b){ return a + b;}).Print();

        watch.Stop();
        double multiThreadTime = watch.Elapsed.TotalMilliseconds;
        Console.WriteLine($"Execution Time: {multiThreadTime} ms \n");
        Console.WriteLine($"Acceleration: {singleThreadTime / multiThreadTime}");
    }
}

namespace AdvancedMath{
    internal class Matrix<T>{
        private T[,] _matrixField;
        private bool _allowPrintMatrix = false;
        public Matrix(int RowCount = 0, int ColumnCount = 0, bool ShowMatrix = false){
            _matrixField = new T[RowCount, ColumnCount];
            _allowPrintMatrix = ShowMatrix;
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
            if(!_allowPrintMatrix) return; 

            for(int i = 0; i < this.ToArray().GetLength(0); ++i){
                for(int j = 0; j < this.ToArray().GetLength(1); ++j){
                    Console.Write(_matrixField[i, j].ToString() + " ");
                }
                Console.Write("\n");
            }
        }

        public void Reset(){
            for(int i = 0; i < this.ToArray().GetLength(0); ++i){
                for(int j = 0; j < this.ToArray().GetLength(1); ++j){
                    _matrixField[i, j] = default(T);
                }
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

            foreach(Thread thread in rowAditionThreads){
                thread.Join();
            }
            
            
            return MatrixToAdd;
        }
    }
}
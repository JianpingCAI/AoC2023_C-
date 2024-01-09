namespace AocLib.DataTypes
{
    public class DualMatrix<T>
    {
        private readonly T[][] _matrix;
        private readonly T[][] _transposedMatrix;
        private readonly int _rows;
        private readonly int _columns;

        public int Rows => _rows;

        public int Columns => _columns;

        public DualMatrix(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;

            _matrix = new T[rows][];
            _transposedMatrix = new T[columns][];

            for (int i = 0; i < rows; i++)
            {
                _matrix[i] = new T[columns];
            }

            for (int j = 0; j < columns; j++)
            {
                _transposedMatrix[j] = new T[rows];
            }
        }

        public void Set(int row, int column, T value)
        {
            _matrix[row][column] = value;
            _transposedMatrix[column][row] = value;
        }

        public T[] Row(int row)
        {
            return _matrix[row];
        }

        public T[] Column(int column)
        {
            return _transposedMatrix[column];
        }

        public void SetRow(int row, T[] newRowValues)
        {
            ArgumentNullException.ThrowIfNull(newRowValues);

            if (newRowValues.Length != Columns)
            {
                throw new ArgumentException("Length of newRowValues must be equal to the number of columns");
            }

            for (int col = 0; col < Columns; col++)
            {
                _matrix[row][col] = newRowValues[col];
                _transposedMatrix[col][row] = newRowValues[col];
            }
        }

        public void SetColumn(int col, T[] newColValues)
        {
            ArgumentNullException.ThrowIfNull(newColValues);

            if (newColValues.Length != Rows)
            {
                throw new ArgumentException("Length of newRowValues must be equal to the number of columns");
            }

            for (int row = 0; row < Columns; row++)
            {
                _matrix[row][col] = newColValues[row];
                _transposedMatrix[col][row] = newColValues[row];
            }
        }

        public override int GetHashCode()
        {
            HashCode hash = new();

            foreach (T[] row in _matrix)
            {
                foreach (T? c in row)
                {
                    hash.Add(c);
                }
            }

            return hash.ToHashCode();
        }

        public void Print()
        {
            foreach (T[] row in _matrix)
            {
                foreach (T t in row)
                {
                    Console.Write(t);
                }
                Console.WriteLine();
            }
        }
    }
}
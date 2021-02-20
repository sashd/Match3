
namespace Moves
{
    public struct Move
    {
        public Move(int column_1, int row_1, int column_2, int row_2)
        {
            this.column_1 = column_1;
            this.row_1 = row_1;
            this.column_2 = column_2;
            this.row_2 = row_2;
        }

        int column_1;
        int row_1;
        int column_2;
        int row_2;
    }
}

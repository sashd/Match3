namespace Matches
{
    public struct Match
    {
        public Match(int column, int row, int length, bool horizontal)
        {
            this.column = column;
            this.row = row;
            this.length = length;
            this.horizontal = horizontal;
        }

        public int column;
        public int row;
        public int length;
        public bool horizontal;
    }
}


using UnityEngine;

namespace Moves
{
    public struct Move
    {
        public Move(int x1, int y1, int x2, int y2)
        {
            from = new Vector2Int(x1, y1);
            to = new Vector2Int(x2, y2);
        }

        public Vector2Int from;
        public Vector2Int to;
    }
}

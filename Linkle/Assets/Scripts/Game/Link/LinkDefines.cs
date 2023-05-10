namespace Game.Link
{
    public enum Direction
    {
        None,
        Up,
        Right,
        Down,
        Left,
    }
    
    public enum TileState
    {
        Normal,
        Moving,
        AppendingSolved,
        Solved,
    }
    
    public class RobotData
    {
        public float minDuration;
        public float maxDuration;
    }

    public class TileData
    {
        public int tag;
        public readonly int key;

        public TileData(int key)
        {
            this.key = key;
        }

        public bool CanReachable()
        {
            return -1 == tag;
        }
    }
    
    public class LinkSolveResult
    {
        public bool success;
        public int[] path;
    }
        
    public class LinkHintResult
    {
        public bool success;
        public int[] path;
    }
    
    public class SearchingOption
    {
        public readonly Direction direction;
        public readonly int x;
        public readonly int y;

        public SearchingOption(Direction direction, int x, int y)
        {
            this.direction = direction;
            this.x = x;
            this.y = y;
        }
    }
}
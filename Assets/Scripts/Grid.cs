namespace Assets.Scripts
{
    public class Grid
    {
        public bool[,] Map;

        public Point Size;

        public Grid(Point size)
        {
            this.Size = size;
            this.Map = new bool[this.Size.X, this.Size.Y];
        }

        public void SetWall(int x, int y)
        {
            this.Map[x, y] = true;
        }

        public void SetWalkable(int x, int y)
        {
            this.Map[x, y] = false;
        }
    }
}
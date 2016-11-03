namespace Assets.Scripts
{
    public class Grid
    {
        public bool[,] Walls;

        public Point Size;

        public Grid(Point size)
        {
            this.Size = size;
            this.Walls = new bool[this.Size.X, this.Size.Y];
        }

        public void SetWall(int x, int y)
        {
            this.Walls[x, y] = true;
        }

        public void SetWalkable(int x, int y)
        {
            this.Walls[x, y] = false;
        }
    }
}
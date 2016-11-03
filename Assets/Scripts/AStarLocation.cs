namespace Assets.Scripts
{
    public class AStarLocation
    {
        public Point Position;

        public int F;
        public int G;
        public int H;
        public AStarLocation Parent;
    }
}
namespace Assets.Scripts
{
    public class WallTypeResult
    {
        public WallType Type;

        public int Rotation;

        public static implicit operator WallTypeResult(WallType type)
        {
            return new WallTypeResult()
                   {
                       Type = type
                   };
        }
    }
}
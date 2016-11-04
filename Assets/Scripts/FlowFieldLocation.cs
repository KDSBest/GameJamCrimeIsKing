using System.Collections.Generic;

namespace Assets.Scripts
{
    public class FlowFieldLocation
    {
        public Point Position;

        public int Cost = int.MaxValue;

        public List<FlowFieldLocation> Neighbours = new List<FlowFieldLocation>(4); 

        public void UpdateCost(int newCost)
        {
            if (this.Cost > newCost)
            {
                this.Cost = newCost;

                foreach (var neighbour in this.Neighbours)
                {
                    neighbour.UpdateCost(this.Cost + 1);
                }
            }
        }
    }
}
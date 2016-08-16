using System.Drawing;

namespace Project7_130716
{
    class LandMine
    {
        public Point
            Position;
        public Form1.LandMineType
            Type;
        public float
            Duration = 0f;
        public bool
            Exist = true;

        public LandMine(Point position, Form1.LandMineType type)
        {
            Position = position;
            Type = type;
        }

        public void Refresh(Region _Map)
        {
            Duration += 0.01f;
            for (int offset = 5; offset > 0; --offset)
            {
                Rectangle next = new Rectangle(new Point(Position.X, Position.Y + offset), Form1.LANDMINE_SIZE);
                if (!_Map.IsVisible(next))
                {
                    Position = next.Location;
                    break;
                }
            }
        }
    }
}

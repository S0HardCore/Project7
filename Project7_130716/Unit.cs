using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Project7_130716
{
    class Unit
    {
        public Character
            Owner;
        public List<Character>
            Players;
        public Point 
            Position;
        public Size 
            Dimensions;
        public Form1.SummonType
            Type;
        public float
            AbilityDuration = 0f,
            Duration = 0f,
            MaxDuration;
        public int
            HorizontalVelocity,
            VerticalVelocity;
        public Region
            AbilityRay = new Region(new Rectangle(-1, -1, 1, 1));
        public bool
            AbilityUsed = false,
            Exist = true;
        public string
            Command = "";
        

        public Unit(Point position, Size dimensions, Form1.SummonType type, float maxDuration, int horizontalVelocity, int verticalVelocity, Character owner, List<Character> players)
        {
            Position = position;
            Dimensions = dimensions;
            Type = type;
            MaxDuration = maxDuration;
            HorizontalVelocity = horizontalVelocity;
            VerticalVelocity = verticalVelocity;
            Owner = owner;
            Players = players;
        }
        
        public void Refresh(Region _Map)
        {
            if (Duration < MaxDuration)
            {
                Duration += 0.01f;
                switch (Command)
                {
                    case "Up":
                    case "Down":
                        for (int offset = VerticalVelocity; offset > 0; --offset)
                        {
                            Rectangle next = new Rectangle(new Point(Position.X, Position.Y + (Command == "Up" ? -offset : offset)), Dimensions);
                            if (!_Map.IsVisible(next))
                            {
                                Position = next.Location;
                                break;
                            }
                        }
                        break;
                    case "Left":
                    case "Right":
                        for (int offset = HorizontalVelocity; offset > 0; --offset)
                        {
                            Rectangle next = new Rectangle(new Point(Position.X + (Command == "Left" ? -offset : offset), Position.Y), Dimensions);
                            if (!_Map.IsVisible(next))
                            {
                                Position = next.Location;
                                break;
                            }
                        }
                        break;
                    case "Space":
                        AbilityUsed = true;
                        break;
                    case "E":
                        if (MaxDuration - Duration > 3)
                            Duration = MaxDuration - 3;
                        break;
                    case "Q":
                        Duration = MaxDuration;
                        Exist = false;
                        break;
                }
                if (AbilityUsed)
                {
                    GraphicsPath TGP = new GraphicsPath();
                    TGP.AddPie(Position.X - 18, Position.Y - 63, 100, 150, 65, 50);
                    AbilityRay.MakeEmpty();
                    if (AbilityDuration < 0.93f)
                        AbilityRay = new Region(TGP);
                    else
                        AbilityRay = new Region(new Rectangle(0, 0, 0, 0));
                    AbilityDuration += 0.01f;
                    if (AbilityDuration > 0.92f && AbilityDuration < 0.94f)
                        Duration = MaxDuration - 1.3f;
                        
                }
            }
            else
                Exist = false;
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project7_130716
{
    class KnockbackSystem
    {
        public Character Object;
        public float
            Velocity,
            Direction,
            Duration,
            MaxDuration;
        public bool
            Bounced = false,
            CanBounce = true,
            Exist = true;

        public KnockbackSystem(Character @object, float @velocity, float @direction, float @duration)
        {
            Object = @object;
            Velocity = @velocity;
            Direction = @direction;
            MaxDuration = @duration;
        }

        public void Refresh(Region _Map)
        {
            if (Duration < MaxDuration)
            {
                Duration += 0.01f;
                PointF next = Object.Position;
                for (float f = 0; f < Velocity; f += 0.5f)
                {
                    next.X += Form1.Cos(Direction) * 0.5f;
                    next.Y += Form1.Sin(Direction) * 0.5f;
                    if (_Map.IsVisible(next.X - 1f, next.Y - 1f, 27f, 55f))
                        if (CanBounce && !Bounced)
                        {
                            Bounced = true;
                            Velocity /= 2f;
                            Direction += (175f + Form1.getRandom.Next(11));
                        }
                        else
                        {
                            Duration = MaxDuration;
                            Exist = false;
                            break;
                        }
                }
                if (Velocity > 10f) Velocity -= 0.25f;
                Object.Position = Point.Round(next);
            }
            else
                Exist = false;
        }
    }
}

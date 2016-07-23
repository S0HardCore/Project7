using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project7_130716
{
    class Ray
    {
        public PointF Start;
        public Boolean
            Exist = true,
            Excluded = false;
        public float Direction;
        public Form1.RayType Type;
        public Character Initiator;
        public PointF End;
        public float Duration = 0f;
        public Region RayRegion = new Region(new Rectangle(-1, -1, 1, 1));
        public Ray(PointF _Start, float _Direction, Character _Initiator, Form1.RayType _Type, Region _Map)
        {
            Initiator = _Initiator;
            Start = _Start;
            Direction = _Direction;
            Type = _Type;
            PointF next = new PointF(Start.X + Form1.Cos(Direction), Start.Y + Form1.Sin(Direction));
        Mark:
            next = new PointF(next.X + Form1.Cos(Direction), next.Y + Form1.Sin(Direction));
            if ((Form1.MAP_INNER.Contains(Point.Round(next)) && Type == Form1.RayType.Railgun) ||
                (!_Map.IsVisible(next) && Type == Form1.RayType.Freeze))
                goto Mark;
            End = next;
        }
        public void Refresh(Character Player)
        {
            if (Exist)
            {
                RayRegion.MakeEmpty();
                if (Duration < Form1.RAY_DURATION)
                    Duration += 0.01f;
                else
                    Exist = false;
                GraphicsPath RayGP = new GraphicsPath();
                PointF[] RayPoints = new PointF[4]
                {
                    new PointF(Start.X + (Form1.RAY_SIZE / 2) * Form1.Cos(Direction - 90), Start.Y + (Form1.RAY_SIZE / 2) * Form1.Sin(Direction - 90)),
                    new PointF(Start.X + (Form1.RAY_SIZE / 2) * Form1.Cos(Direction + 90), Start.Y + (Form1.RAY_SIZE / 2) * Form1.Sin(Direction + 90)),
                    new PointF(End.X + (Form1.RAY_SIZE / 2) * Form1.Cos(Direction + 90), End.Y + (Form1.RAY_SIZE / 2) * Form1.Sin(Direction + 90)),
                    new PointF(End.X + (Form1.RAY_SIZE / 2) * Form1.Cos(Direction - 90), End.Y + (Form1.RAY_SIZE / 2) * Form1.Sin(Direction - 90))
                };
                RayGP.AddPolygon(RayPoints);
                RayRegion = new Region(RayGP);
            }
        }
    }
}

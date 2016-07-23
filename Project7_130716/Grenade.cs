using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project7_130716
{
    class Grenade
    {
        public Form1.GrenadeType Type;
        public Boolean
            Exist = true,
            Excluded = false;
        public Point Position;
        public float Direction;
        public float Timer = 0f;
        public float InitialSpeed = 10f;
        public PointF Offset;
        public Region HitBox = new Region();

        public Grenade(Form1.GrenadeType _Type, Point _Position, float _Direction, float _Timer)
        {
            Type = _Type;
            Position = _Position;
            Direction = _Direction;
            Timer = _Timer;
        }

        public void Move(Region _Map)
        {
            if (Timer >= (Type == Form1.GrenadeType.Explosive ? Form1.EXPLOSIVE_GRENADE_DURATION : Form1.BLINK_GRENADE_DURATION))
                Exist = false;
            else
            {
                if (InitialSpeed > 4)
                    if (Direction > 90 && 270 > Direction)
                        Direction -= 0.5f;
                    else
                        Direction += 0.5f;
                DirectionResetting();
                Offset = new PointF(Form1.Cos(Direction) * InitialSpeed, Form1.Sin(Direction) * (InitialSpeed + (Direction > 0 && 180 > Direction ? 3 : 0)));
                Point next = new Point(Position.X + (int)Offset.X, Position.Y + (int)Offset.Y);
                if (_Map.IsVisible(next.X, next.Y, (float)Form1.GRENADE_SIZE, (float)Form1.GRENADE_SIZE))
                {
                    Direction += 180;
                    if (InitialSpeed > 3.6f)
                        InitialSpeed -= 2f;
                    else
                    {
                        InitialSpeed = 0.9f;
                        float Temp = Direction;
                        Offset = new PointF(Form1.Cos(Direction) * InitialSpeed, Form1.Sin(Direction) * (InitialSpeed + (Direction > 0 && 180 > Direction ? 3 : 0)));
                        next = new Point(Position.X + (int)Offset.X, Position.Y + (int)Offset.Y);
                        Direction = 90f;
                        if (_Map.IsVisible(next.X, next.Y, (float)Form1.GRENADE_SIZE, (float)Form1.GRENADE_SIZE))
                            Direction = Temp;
                    }
                    DirectionResetting();
                    if (Direction > 90 && 270 > Direction)
                        Direction -= 30f;
                    else
                        Direction += 30f;
                    Offset = new PointF(Form1.Cos(Direction) * InitialSpeed, Form1.Sin(Direction) * InitialSpeed);
                    next = new Point(Position.X + (int)Offset.X, Position.Y + (int)Offset.Y);
                }
                Position.Offset(Point.Round(Offset));
                InitialSpeed -= 0.01f;
                Timer += 0.01f;
                HitBox.MakeEmpty();
                GraphicsPath GP = new GraphicsPath();
                GP.AddEllipse(Position.X - Form1.GRENADE_SIZE * 2, Position.Y - Form1.GRENADE_SIZE * 2, Form1.GRENADE_SIZE * 4, Form1.GRENADE_SIZE * 4);
                HitBox = new Region(GP);
            }
        }

        private void DirectionResetting()
        {
            if (Direction > 360)
                Direction -= 360;
            else
                if (Direction < 0)
                    Direction += 360;
        }
    }
}

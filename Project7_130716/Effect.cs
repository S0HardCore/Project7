﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project7_130716
{
    class Effect
    {
        public bool
            Exist = true,
            HitedOnce = false;
        public Form1.EffectType
            Type;
        public int
            Frames = 0,
            MaxFrames;
        public Rectangle
            Place;
        public Point
            Position;
        public Size
            Dimension;
        public Image
            Source;
        public Region
            HitBox = new Region(new Rectangle(-1, -1, 1, 1));

        public Effect(Form1.EffectType _Type, Point _Position, Size _Dimension, Image _Source, int _MaxFrames)
        {
            Type = _Type;
            Position = _Position;
            Dimension = _Dimension;
            Place = new Rectangle(Position, Dimension);
            Source = _Source;
            MaxFrames = _MaxFrames * 2;
        }
        public Effect(Form1.EffectType _Type, Rectangle _Place, Image _Source, int _MaxFrames)
        {
            Type = _Type;
            Place = _Place;
            Position = Place.Location;
            Dimension = Place.Size;
            Source = _Source;
            MaxFrames = _MaxFrames * 2;
        }
        public Point getCenter()
        {
            return new Point(Position.X + Dimension.Width / 2, Position.Y + Dimension.Height / 2);
        }
        public void Draw(Graphics g)
        {
            if (Frames < MaxFrames)
            {
                int FrameWidth = Source.Width / MaxFrames * 2;
                Rectangle EffectFrame = new Rectangle(FrameWidth * (Frames / 2), 0, FrameWidth, Source.Height);
                g.DrawImage(Source, Place, EffectFrame, GraphicsUnit.Pixel);
                Frames++;
            }
            else
            {
                HitBox.MakeEmpty();
                GraphicsPath GP = new GraphicsPath();
                switch (Type)
                {
                    case Form1.EffectType.GrenadeExplosion:
                        GP.AddEllipse(Position.X + Form1.GRENADE_SIZE, Position.Y + Form1.GRENADE_SIZE, Form1.GRENADE_SIZE * 4, Form1.GRENADE_SIZE * 4);
                        break;
                    case Form1.EffectType.UFOExplosion:
                        GP.AddEllipse(Position.X, Position.Y, Form1.GRENADE_SIZE * 6, Form1.GRENADE_SIZE * 6);
                        break;
                    case Form1.EffectType.LandMineExplosion:
                        GP.AddEllipse(Position.X, Position.Y, Form1.GRENADE_SIZE * 4, Form1.GRENADE_SIZE * 4);
                        break;
                }
                HitBox = new Region(GP);
                Exist = false;
            }
        }
    }
}

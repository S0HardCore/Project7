using System;
using System.Drawing;

namespace Project7_130716
{
    class Effect
    {
        public Boolean
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
                Exist = false;
        }
    }
}

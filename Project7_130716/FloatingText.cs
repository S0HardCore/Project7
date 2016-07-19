using System;
using System.Drawing;

namespace Project7_130716
{

    class FloatingText
    {
        public Point Position;
        public string Text;
        public Font _Font;
        public SolidBrush Brush;
        public float Duration = 0f;
        public float MaxDuration = Form1.FLOATING_TEXT_DURATION;
        public Boolean Exist = true;
        public FloatingText(Point _Position, SolidBrush InitialBrush, string _Text, Font __Font)
        {
            Position = _Position;
            Text = _Text;
            _Font = __Font;
            Brush = InitialBrush;
        }
        public FloatingText(Point _Position, Color InitialColor, string _Text, Font __Font, float _MaxDuration)
        {
            Position = _Position;
            Text = _Text;
            _Font = __Font;
            Brush = new SolidBrush(InitialColor);
            MaxDuration = _MaxDuration;
        }
        public void Refresh()
        {
            int AlphaDecrement;
            if (Duration > MaxDuration)
                Exist = false;
            else
                Duration += 0.01f;
            Position.Y--;
            if (Text.Contains("Respawn"))
            {
                if (Form1.getRandom.Next(4) == 1)
                    Position.Y--;
                Text = "Respawn in " + Math.Round(MaxDuration - Duration, 1).ToString();
            }
            if (MaxDuration > 1)
                AlphaDecrement = (int)(2.55f / MaxDuration);
            else
                AlphaDecrement = (int)(MaxDuration / 2.55f);
            Brush.Color = Color.FromArgb(Brush.Color.A - AlphaDecrement, Brush.Color.R, Brush.Color.G, Brush.Color.B);
        }
        public void Draw(Graphics g)
        {
            g.DrawString(Text, _Font, Brush, (PointF)Position);
        }
    }
}

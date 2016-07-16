﻿using System;
using System.Drawing;

namespace Project7_130716
{
    class Projectile
    {
        public PointF Position;
        public float Direction;
        public int Power = 1000;
        public int Size = Form1.PROJECTILE_SIZE;
        public Boolean RicochetedOnce = false;
        public Boolean Exist = true;
        public Projectile(PointF _Position, float _Direction)
        {
            Position = new PointF(Form1.Cos(_Direction) * 18 + _Position.X, Form1.Sin(_Direction) * 18 + _Position.Y);
            Direction = _Direction;
        }
        public PointF getNextPointF()
        {
            return new PointF(Position.X + Form1.Cos(Direction) * Form1.PROJECTILE_VELOCITY, Position.Y + Form1.Sin(Direction) * Form1.PROJECTILE_VELOCITY);
        }
        public void Move(Region _Map)
        {
            PointF next = new PointF(Position.X + Form1.Cos(Direction) * Form1.PROJECTILE_VELOCITY, Position.Y + Form1.Sin(Direction) * Form1.PROJECTILE_VELOCITY);
            if (_Map.IsVisible(next))
                if (!RicochetedOnce)
                {
                    Direction += 180f + Form1.getRandom.Next(-Form1.PROJECTILE_RICOCHET_RANGE, Form1.PROJECTILE_RICOCHET_RANGE + 1);
                    RicochetedOnce = true;
                    Size -= 2;
                    next = new PointF(Position.X + Form1.Cos(Direction) * Form1.PROJECTILE_VELOCITY, Position.Y + Form1.Sin(Direction) * Form1.PROJECTILE_VELOCITY);
                }
                else
                    Exist = false;
            if (Power > 0)
                Power -= Form1.PROJECTILE_VELOCITY;
            Position = next;
        }
    }
}

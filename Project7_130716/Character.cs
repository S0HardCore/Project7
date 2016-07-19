using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Project7_130716
{
    class Character
    {
        public Point 
            Position;
        public Byte
            Health = 100;
        public Boolean
            Rotation = true,
            AtGround = false,
            Backpack = false;
        public int 
            JumpProgress = -1;
        public float 
            RespawnTimer = -0.01f;
        public Region
            Body = new Region(new Rectangle(-1, -1, 1, 1)),
            Head = new Region(new Rectangle(-1, -1, 1, 1)),
            Arms = new Region(new Rectangle(-1, -1, 1, 1)),
            Chest = new Region(new Rectangle(-1, -1, 1, 1)),
            Legs = new Region(new Rectangle(-1, -1, 1, 1)),
            Shield = new Region(new Rectangle(-1, -1, 1, 1));
        public float
            ProjectileCooldown = Form1.PROJECTILE_COOLDOWN,
            PistolReload = Form1.PISTOL_RELOAD_COOLDOWN,
            ExplosiveGrenadeCooldown = Form1.EXPLOSIVE_GRENADE_COOLDOWN,
            ExplosiveGrenadeDown = -0.01f,
            BlinkGrenadeCooldown = Form1.BLINK_GRENADE_COOLDOWN,
            ShotgunReload = Form1.SHOTGUN_RELOAD_COOLDOWN,
            RailgunCooldown = Form1.RAILGUN_COOLDOWN,
            ShieldCooldown = Form1.SHIELD_COOLDOWN,
            ShieldDuration = -0.01f;
        
        public void RefreshBody()
        {
            Body.MakeEmpty();
            Head.MakeEmpty();
            Chest.MakeEmpty();
            Legs.MakeEmpty();
            if (RespawnTimer < 0)
            {
                GraphicsPath
                    GP_Head = new GraphicsPath(),
                    GP_Arms = new GraphicsPath(),
                    GP_Legs = new GraphicsPath();
                GP_Head.AddEllipse(Position.X + 5, Position.Y, 15, 15);
                Head = new Region(GP_Head);
                Chest = new Region(new Rectangle(Position.X + 5, Position.Y + 15, 16, 26));
                GP_Arms.AddRectangle(new Rectangle(Position.X + 4, Position.Y + 15, 1, 4));
                GP_Arms.AddRectangle(new Rectangle(Position.X + 21, Position.Y + 15, 1, 4));
                GP_Arms.AddRectangle(new Rectangle(Position.X - 1, Position.Y + 30, 1, 3));
                GP_Arms.AddRectangle(new Rectangle(Position.X, Position.Y + 15, 4, 19));
                GP_Arms.AddRectangle(new Rectangle(Position.X + 22, Position.Y + 15, 4, 19));
                GP_Arms.AddRectangle(new Rectangle(Position.X + 26, Position.Y + 30, 1, 3));
                Arms = new Region(GP_Arms);
                GP_Legs.AddRectangle(new Rectangle(Position.X + 7, Position.Y + 41, 4, 13));
                GP_Legs.AddRectangle(new Rectangle(Position.X + 15, Position.Y + 41, 4, 13));
                GP_Legs.AddRectangle(new Rectangle(Position.X + 5, Position.Y + 51, 2, 3));
                GP_Legs.AddRectangle(new Rectangle(Position.X + 19, Position.Y + 51, 2, 3));
                Legs = new Region(GP_Legs);
                Body.Union(Head);
                Body.Union(Chest);
                Body.Union(Arms);
                Body.Union(Legs);
                if (ShieldDuration > -0.01f)
                {
                    ShieldDuration += 0.01f;
                    GraphicsPath ShieldGP = new GraphicsPath();
                    ShieldGP.AddEllipse(Position.X - (Form1.SHIELD_SIZE - 26) / 2, Position.Y - (Form1.SHIELD_SIZE - 54) / 2, Form1.SHIELD_SIZE, Form1.SHIELD_SIZE);
                    Shield = new Region(ShieldGP);
                    if (ShieldDuration >= Form1.SHIELD_DURATION)
                        ShieldDuration = -0.01f;
                }
            }
            else
                if (RespawnTimer < Form1.CHARACTER_RESPAWN_DURATION)
                    RespawnTimer += 0.01f;
                else
                {
                    RespawnTimer = -0.01f;
                    Health = 100;
                    Position = Form1.getRandomLocationOnMap();
                }
        }
        public string WhereHit(PointF _Point)
        {
            RefreshBody();
            if (Head.IsVisible(_Point)) return "head";
            if (Arms.IsVisible(_Point) || Chest.IsVisible(_Point)) return "arm";
            if (Legs.IsVisible(_Point)) return "leg";
            return "none";
        }
        public Boolean DoDamage(int Damage)
        {
            if (ShieldDuration < 0)
            {
                int HealthAfterDamage = Health - Damage;
                Health = (Byte)HealthAfterDamage;
                if (HealthAfterDamage <= 0)
                {
                    RespawnTimer = 0f;
                    return false;
                }
            }
            return true;
        }

        public Character(Point _Position) { Position = _Position; RefreshBody(); }
        public Character(int x, int y) { Position = new Point(x, y); RefreshBody(); }

        public void StepLeftIfCan(Region _Map)
        {
            if (!_Map.IsVisible(new Rectangle(Position.X - 1 - Form1.CHARACTER_HORIZONTAL_VELOCITY, Position.Y, 2, 54)))
                Position.X -= Form1.CHARACTER_HORIZONTAL_VELOCITY;
            else
                if (!_Map.IsVisible(new Rectangle(Position.X - 1, Position.Y, 2, 54)))
                    Position.X--;
        }
        public void StepRightIfCan(Region _Map)
        {
            if (!_Map.IsVisible(new Rectangle(Position.X + 25 + Form1.CHARACTER_HORIZONTAL_VELOCITY, Position.Y, 2, 54)))
                Position.X += Form1.CHARACTER_HORIZONTAL_VELOCITY;
            else
                if (!_Map.IsVisible(new Rectangle(Position.X + 25, Position.Y, 2, 54)))
                    Position.X++;
        }
        public void FallIfCan(Region _Map)
        {
            if (JumpProgress == -1 || JumpProgress > Form1.CHARACTER_JUMP_LENGTH)
                if (!_Map.IsVisible(new Rectangle(Position.X, Position.Y + 53 + Form1.CHARACTER_VERTICAL_VELOCITY, 26, 2)))
                {
                    Position.Y += Form1.CHARACTER_VERTICAL_VELOCITY;
                    AtGround = false;
                }
                else
                    if (!_Map.IsVisible(new Rectangle(Position.X, Position.Y + 53, 26, 2)))
                    {
                        AtGround = false;
                        Position.Y++;
                    }
                    else
                        AtGround = true;
        }
        public void JumpIfCan(Region _Map)
        {
            if (JumpProgress > -1 && JumpProgress < Form1.CHARACTER_JUMP_LENGTH)
            {
                if (!_Map.IsVisible(new Rectangle(Position.X, Position.Y - 2 - Form1.CHARACTER_VERTICAL_VELOCITY, 26, 2)) && JumpProgress + Form1.CHARACTER_VERTICAL_VELOCITY * 2 < Form1.CHARACTER_JUMP_LENGTH)
                {
                    Position.Y -= Form1.CHARACTER_VERTICAL_VELOCITY * 2;
                    JumpProgress += Form1.CHARACTER_VERTICAL_VELOCITY * 2;
                }
                else
                    if (!_Map.IsVisible(new Rectangle(Position.X, Position.Y - 2, 26, 2)))
                    {
                        Position.Y--;
                        JumpProgress++;
                    }
                    else
                        JumpProgress = Form1.CHARACTER_JUMP_LENGTH;
            }
            if (JumpProgress > -1 && JumpProgress < Form1.CHARACTER_JUMP_LENGTH + 5)
            {
                JumpProgress++;
                if (JumpProgress == Form1.CHARACTER_JUMP_LENGTH + 5)
                    JumpProgress = -1;
            }
            
        }
        public Brush getHealthBrush()
        {
            if (Health > 90)
                return new SolidBrush(Color.Green);
            if (Health > 35)
                return new SolidBrush(Color.Orange);
            return new SolidBrush(Color.Red);
        }
        public RectangleF getHealthRectangleF()
        {
            return new RectangleF((float)Position.X - (Health > 99 ? 8 : 5), (float)Position.Y - 15, 35, 14);
        }
        public PointF getCenterPointF()
        {
            return new PointF((float)Position.X + 13, (float)Position.Y + 27);
        }
        public Rectangle getPlaceForSkull()
        {
            return new Rectangle(Position.X + 2, Position.Y + 32, 22, 22);
        }
    }
}

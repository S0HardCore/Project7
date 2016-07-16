using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Project7_130716
{
    public partial class Form1 : Form
    {
        public static readonly int
            CHARACTER_VERTICAL_VELOCITY = 4,
            CHARACTER_HORIZONTAL_VELOCITY = 4,
            CHARACTER_JUMP_LENGTH = 130,
            PROJECTILE_VELOCITY = 20,
            PROJECTILE_SIZE = 4,
            PROJECTILE_RICOCHET_RANGE = 5,
            SWORD_FRAMES = 6;
        public static readonly float
            CHARACTER_RESPAWN_DURATION = 2.1f,
            PROJECTILE_COOLDOWN = 0.25f,
            FLOATING_TEXT_DURATION = 0.83f;
        enum Inventory
        {
            Fists,
            Sword,
            Pistol,
            ExplosiveGrenade,
            BlinkGrenade
        }
        static Image
            iSkull = Properties.Resources.Skull,
            iCursor = Properties.Resources.cursorCustom,
            iLeftSword = Properties.Resources.LeftSword,
            iRightSword = Properties.Resources.RightSword,
            iConsole = Properties.Resources.glassPanelConsole;
        static readonly Rectangle
            MAP_REGION_RECTANGLE = new Rectangle(-250, -250, 2420, 1580),
            FPS_RECTANGLE = new Rectangle(-1, -2, 30, 22);
        static Timer
            updateTimer = new Timer();
        static string
            QuartzFont = "Quartz MS";
        static int
            lastTick, lastFrameRate, frameRate,
            swordFrames = 0;
        static float
            ProjectileCooldown = PROJECTILE_COOLDOWN;
        static Boolean
            SwordUsed = false;
        static HatchBrush
            MapBrush = new HatchBrush(HatchStyle.Trellis, Color.Black, Color.DarkSeaGreen);
        public static Size 
            Resolution = Screen.PrimaryScreen.Bounds.Size;
        static Character
            Player1;
        static StringFormat
            MiddleText = new StringFormat(),
            HorizontalMiddleText = new StringFormat();
        static Font
            Verdana13 = new Font("Verdana", 13);
        static Region
            Map = new Region();
        static Point
            ViewOffset;
        public static Random
            getRandom = new Random(DateTime.Now.Millisecond);
        static Inventory
            CurrentItem = Inventory.Sword;
        static List<Projectile>
            Projectiles = new List<Projectile>();
        static List<FloatingText>
            FloatingTexts = new List<FloatingText>();
        public static 
            Boolean ShowDI = false;
        static ConsolePrototype
            Console = new ConsolePrototype();
        static Boolean[] 
            KeysDown = new Boolean[3] { false, false, false };

        public Form1()
        {
            InitializeComponent();
            InitialSetup();
            HorizontalMiddleText.Alignment = MiddleText.Alignment = StringAlignment.Center;
            MiddleText.LineAlignment = StringAlignment.Center;
            foreach (FontFamily Family in FontFamily.Families)
                if (Family.Name.ToUpper() == "QUARTZ" || Family.Name.ToUpper() == "QUARTZ MS")
                    QuartzFont = Family.Name;
            Cursor.Hide();
            this.Size = Resolution;
            this.BackColor = Color.FromArgb(8, 8, 24);
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.Paint += pDraw;
            this.KeyDown += pKeyDown;
            this.KeyUp += pKeyUp;
            this.MouseMove += pMouseMove;
            this.MouseUp += pMouseUp;
            this.MouseDown += pMouseDown;
            updateTimer.Interval = 1;
            updateTimer.Tick += pUpdate;
            updateTimer.Start();
        }

        void InitialSetup()
        {
            Player1 = new Character(250, 50);
            ViewOffset = new Point(Resolution.Width / 2 - Player1.Position.X, Resolution.Height / 2 - Player1.Position.Y);
            Map.MakeEmpty();
            Map.Union(MAP_REGION_RECTANGLE);
            Map.Exclude(new Rectangle(-50, -50, 2020, 1180));
            Map.Union(new Rectangle(200, 350, 400, 50));
            Map.Union(new Rectangle(50, 500, 350, 30));
            Map.Union(new Rectangle(0, 650, 500, 20));
            Map.Union(new Rectangle(380, 450, 20, 50));
            Map.Union(new Rectangle(0, 600, 20, 50));
        }

        void pMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (Player1.RespawnTimer < 0)
                    switch (CurrentItem)
                    {
                        case Inventory.Pistol:
                            if (ProjectileCooldown >= PROJECTILE_COOLDOWN)
                            {
                                Projectiles.Add(new Projectile(Player1.Position, AngleBetween((PointF)OffsetByView(new Point(e.X + 16, e.Y + 16)), Player1.getCenterPointF())));
                                ProjectileCooldown = 0f;
                            }
                            break;
                        case Inventory.Sword:
                            if (!SwordUsed)
                            {
                                SwordUsed = true;
                                swordFrames = 0;
                            }
                            break;
                    }
        }

        void pMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                Player1.Position = OffsetByView(e.Location);
        }

        void pMouseMove(object sender, MouseEventArgs e)
        {
            
        }

        void pKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Tab:
                    if (!Console.Enabled)
                        Console.Enabled = true;
                    else
                    {
                        Console.Enabled = false;
                        if (!String.IsNullOrEmpty(Console.getString()))
                            Console.setString("");
                    }
                break;
                case Keys.A:
                    KeysDown[0] = false;
                    break;
                case Keys.D:
                    KeysDown[1] = false;
                    break;
                case Keys.W:
                    KeysDown[2] = false;
                    break;
            }
        }

        void pKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyData)
            {
                case Keys.A:
                    Player1.Rotation = false;
                    KeysDown[0] = true;
                    KeysDown[1] = false;
                    break;
                case Keys.D:
                    Player1.Rotation = true;
                    KeysDown[0] = false;
                    KeysDown[1] = true;
                    break;
                case Keys.W:
                    KeysDown[2] = true;
                    break;
            }
            if (Console.Enabled)
            #region Console
            {
                if ((e.KeyData >= Keys.A && e.KeyData <= Keys.Z) ||
                    (e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9))
                    Console.setString(Console.getString() + (char)e.KeyValue);
                switch (e.KeyData)
                {
                    case Keys.Back:
                        if (Console.getLength() > 0)
                            Console.setString(Console.getString().Remove(Console.getLength() - 1, 1));
                        break;
                    case Keys.Enter:
                        if (!String.IsNullOrEmpty(Console.getString()))
                            Console.applyCommand();
                        break;
                }
            }
            #endregion
        }

        void pUpdate(object sender, EventArgs e)
        {
            ProjectileCooldown += 0.01f;
            ViewOffset = new Point(Resolution.Width / 2 - Player1.Position.X, Resolution.Height / 2 - Player1.Position.Y);
            if (Player1.RespawnTimer < 0)
            {
                if (SwordUsed)
                    if (swordFrames < SWORD_FRAMES)
                        swordFrames++;
                    else
                    {
                        swordFrames = 0;
                        SwordUsed = false;
                    }
                if (KeysDown[0])
                    Player1.StepLeftIfCan(Map);
                if (KeysDown[1])
                    Player1.StepRightIfCan(Map);
                if (KeysDown[2])
                    if (Player1.JumpProgress == -1 && Player1.AtGround)
                        Player1.JumpProgress++;
            }
            if (Projectiles.Count > 0)
                for (int p = 0; p < Projectiles.Count; ++p)
                    if (Projectiles[p].Exist)
                    {
                        Projectiles[p].Move(Map);
                        if (Player1.Body.IsVisible(Projectiles[p].getNextPointF()))
                        {
                            string target = Player1.WhereHit(Projectiles[p].getNextPointF());
                            int damage = 0;
                            switch (target)
                            {
                                case "head":
                                    damage = 94 + Projectiles[p].Power / 100 + getRandom.Next(6);
                                    break;
                                case "arm":
                                    damage = 59 + Projectiles[p].Power / 100 + getRandom.Next(6);
                                    break;
                                case "leg":
                                    damage = 19 + Projectiles[p].Power / 100 + getRandom.Next(6);
                                    break;
                            }
                            Boolean LethalDamage = Player1.DoDamage(damage);
                            if (!LethalDamage)
                                FloatingTexts.Add(new FloatingText(new Point(Player1.Position.X - 25, Player1.Position.Y - 30), Color.FromArgb(255, 0, 192, 0), "Respawn", new Font(QuartzFont, 16), CHARACTER_RESPAWN_DURATION));                            
                            if (target != "none")
                                FloatingTexts.Add(new FloatingText(new Point(Player1.Position.X + 10, Player1.Position.Y - 10), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
                            Projectiles.Remove(Projectiles[p]);
                        }
                    }
                    else
                        Projectiles.Remove(Projectiles[p]);
            Player1.FallIfCan(Map);
            Player1.JumpIfCan(Map);
            foreach (FloatingText TFT in FloatingTexts)
                if (TFT.Exist)
                    TFT.Refresh();
            Player1.RefreshBody();
            Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(ViewOffset.X, ViewOffset.Y);
            g.FillRegion(MapBrush, Map);
            if (Player1.RespawnTimer < 0)
            {
                g.DrawString(Player1.Health.ToString(), new Font(QuartzFont, 11), Player1.getHealthBrush(), Player1.getHealthRectangleF(), MiddleText);
                g.FillRegion(Brushes.Orange, Player1.Body);
            }
            else
                g.DrawImage(iSkull, Player1.getPlaceForSkull());
            Rectangle SwordRectangle, SwordFrame;
            if (CurrentItem == Inventory.Sword)
                if (Player1.Rotation)
                {
                    SwordRectangle = new Rectangle(Player1.Position.X + 21, Player1.Position.Y + 6, 37, 28);
                    SwordFrame = new Rectangle(37 * (swordFrames / 2), 0, 37, 28);
                    g.DrawImage(iRightSword, SwordRectangle, SwordFrame, GraphicsUnit.Pixel);
                }
                else
                {
                    SwordRectangle = new Rectangle(Player1.Position.X - 32, Player1.Position.Y + 6, 37, 28);
                    SwordFrame = new Rectangle(74 - 37 * (swordFrames / 2), 0, 37, 28);
                    g.DrawImage(iLeftSword, SwordRectangle, SwordFrame, GraphicsUnit.Pixel);
                }

            foreach (Projectile TP in Projectiles)
                if (TP.Exist)
                    //g.DrawLine(Pens.LightGoldenrodYellow, TP.Position.X - 3 * Cos(TP.Direction), TP.Position.Y - 3 * Sin(TP.Direction), TP.Position.X + 3 * Cos(TP.Direction), TP.Position.Y + 3 * Sin(TP.Direction));
                    g.FillEllipse(Brushes.LightGoldenrodYellow, TP.Position.X, TP.Position.Y, TP.Size, TP.Size);
            foreach (FloatingText TFT in FloatingTexts)
                if (TFT.Exist)
                    TFT.Draw(g);
            g.ResetTransform();
            if (ShowDI)
            #region Debug Information
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(64, 0, 0, 0)), FPS_RECTANGLE);
                g.DrawString(CalculateFrameRate().ToString(), new Font(QuartzFont, 13), Brushes.White, (RectangleF)FPS_RECTANGLE, MiddleText);
                string output = "Player 1 Position: " + Player1.Position.ToString() + "\n" +
                    "View Offset: " + ViewOffset.ToString() + "\n" +
                    FloatingTexts.Count.ToString();
                g.DrawString(output, new Font(QuartzFont, 14), Brushes.Aqua, 0, FPS_RECTANGLE.Height);
            }
            #endregion
            if (Console.Enabled)
            #region Console
            {
                g.DrawString("Console: ", Verdana13, new SolidBrush(Color.FromArgb(155, 203, 219)), Console.getRegion().X + 3, Console.getRegion().Y + 25);
                g.DrawString(Console.getLog(), Verdana13, new SolidBrush(Color.FromArgb(155, 203, 219)), Console.getRegion().X + 3, Console.getRegion().Y);
                g.DrawString(Console.getPrevString(), Verdana13, new SolidBrush(Color.FromArgb(155, 203, 219)), Console.getRegionForPrevString(), HorizontalMiddleText);
                g.DrawString(Console.getString(), Verdana13, new SolidBrush(Color.FromArgb(155, 203, 219)), Console.getRegionForString());
                g.DrawImage(iConsole, Console.getRegion());
            }
            #endregion
            g.DrawImage(iCursor, MousePosition.X - 16, MousePosition.Y - 16);
        }

        int CalculateFrameRate()
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

        public Point OffsetByView(Point Input)
        {
            return new Point(Input.X - ViewOffset.X, Input.Y - ViewOffset.Y);
        }

        public float AngleBetween(PointF One, PointF Two)
        {
            double x1 = One.X,
                   x2 = Two.X,
                   y1 = One.Y,
                   y2 = Two.Y;
            double Angle = Math.Atan2(y1 - y2, x1 - x2) / Math.PI * 180;
            Angle = (Angle < 0) ? Angle + 360 : Angle;
            return (float)Angle;
        }

        public static float Cos(float _Direction)
        {
            return (float)Math.Cos(Math.PI * _Direction / 180);
        }

        public static float Sin(float _Direction)
        {
            return (float)Math.Sin(Math.PI * _Direction / 180);
        }

        public static Point getRandomLocationOnMap()
        {
            Rectangle TR;
        Mark:
            TR = new Rectangle(getRandom.Next(MAP_REGION_RECTANGLE.Left, MAP_REGION_RECTANGLE.Right), getRandom.Next(MAP_REGION_RECTANGLE.Top, MAP_REGION_RECTANGLE.Bottom), 54, 26);
                if (!Map.IsVisible(TR))
                    return TR.Location;
            goto Mark;
        }
    }
}

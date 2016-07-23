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
            CHARACTER_VERTICAL_VELOCITY = 5,
            CHARACTER_HORIZONTAL_VELOCITY = 5,
            CHARACTER_JUMP_LENGTH = 140,
            PROJECTILE_VELOCITY = 40,
            PROJECTILE_SIZE = 4,
            PROJECTILE_RICOCHET_RANGE = 5,
            PISTOL_CAGE = 6,
            SWORD_FRAMES = 6,
            EXPLOSION_FRAMES = 44,
            FREEZE_EXPLOSION_FRAMES = 25,
            GRENADE_SIZE = 16,
            RAY_SIZE = 4,
            SHIELD_SIZE = 100,
            INVENTORY_SLOTS_MARGIN = 40,
            INVENTORY_SLOT_SIZE = 100,
            INVENTORY_UI_SIZE = 100 * 4 + 40 * 3,
            BACKPACK_SIZE = 600;
        public static readonly float
            CHARACTER_RESPAWN_DURATION = 2.1f,
            PROJECTILE_COOLDOWN = 0.25f,
            PISTOL_RELOAD_COOLDOWN = 2.1f,
            EXPLOSIVE_GRENADE_COOLDOWN = 5.6f,
            EXPLOSIVE_GRENADE_DURATION = 2.1f,
            BLINK_GRENADE_COOLDOWN = 9.3f,
            BLINK_GRENADE_DURATION = 1.5f,
            SHOTGUN_RELOAD_COOLDOWN = 2.3f,
            RAY_DURATION = 0.05f,
            RAILGUN_COOLDOWN = 10.8f,
            SHIELD_DURATION = 1.7f,
            SHIELD_COOLDOWN = 12.5f,
            FREEZE_RIFLE_COOLDOWN = 17.3f,
            FREEZE_GRENADE_COOLDOWN = 13.4f,
            FREEZE_EFFECT_DURATION = 1.4f,
            STASIS_GRENADE_COOLDOWN = 8.3f,
            FLAME_GRENADE_COOLDOWN = 9.9f,
            SNIPER_RIFLE_COOLDOWN = 18.2f,
            FLOATING_TEXT_DURATION = 0.83f;
        public static readonly float[]
            ITEMS_COOLDOWN = new float[12]
            {
                0.25f,
                2.1f,
                5.6f,
                9.3f,
                2.3f,
                10.8f,
                12.5f,
                17.3f,
                13.4f,
                1f,
                1f,
                1f
            };
        public static readonly Size
            CHARACTER_SIZE = new Size(54, 26),
            InventorySlotSize = new Size(INVENTORY_SLOT_SIZE, INVENTORY_SLOT_SIZE),
            Resolution = Screen.PrimaryScreen.Bounds.Size;
        static readonly Point
            INVENTORY_INITIAL_POINT = new Point((Resolution.Width - INVENTORY_UI_SIZE) / 2, Resolution.Height - INVENTORY_SLOT_SIZE - (Resolution.Height > 1000 ? INVENTORY_SLOTS_MARGIN : 3 * INVENTORY_SLOTS_MARGIN / 4));
        public static readonly Rectangle
            BACKPACK_RECTANGLE = new Rectangle((Resolution.Width - BACKPACK_SIZE) / 2, INVENTORY_INITIAL_POINT.Y - BACKPACK_SIZE - INVENTORY_SLOTS_MARGIN / (Resolution.Height > 1000 ? 1 : 2), BACKPACK_SIZE, BACKPACK_SIZE),
            MAP_OUTTER = new Rectangle(-250, -250, 2420, 1580),
            MAP_INNER = new Rectangle(-50, -50, 2020, 1180),
            FPS_RECTANGLE = new Rectangle(-1, -2, 30, 22);
        static readonly Rectangle[]
            INVENTORY_SLOTS = new Rectangle[4],
            BACKPACK_SLOTS = new Rectangle[25];
        public enum Inventory
        {
            Sword,
            Pistol,
            Explosive_Grenade,
            Blink_Grenade,
            Shotgun,
            Railgun,
            Shield,
            Freeze_Rifle,
            Freeze_Grenade,
            Stasis_Grenade,
            Flame_Grenade,
            Sniper_Rifle
        }
        public enum GrenadeType
        {
            Explosive,
            Blink,
            Freeze,
            Flame
        }
        public enum GunType
        {
            Pistol,
            Shotgun,
            Sniper
        }
        public enum EffectType
        {
            Explosion,
            Teleportation,
            Freeze,
            Flame
        }
        public enum RayType
        {
            Railgun,
            Freeze
        }
        static Image[]
            BackpackIcons = new Image[12]
            {
                Properties.Resources.IconCrossedSword,
                Properties.Resources.IconPistol,
                Properties.Resources.IconGrenade,
                Properties.Resources.IconBlinkGrenade,
                Properties.Resources.IconShotgun,
                Properties.Resources.IconRailgun,
                Properties.Resources.EffectShield,
                Properties.Resources.IconFreezeRifle,
                Properties.Resources.IconFrostGrenade,
                Properties.Resources.IconStasisGrenade,
                Properties.Resources.IconFlameGrenade,
                Properties.Resources.IconSniperRifle
            },
            InventoryIcons = new Image[4];
        public static Image
            iSkull = Properties.Resources.CharacterSkull,
            iCursor = Properties.Resources.CursorCrosshair,
            iLeftSword = Properties.Resources.LeftSword,
            iRightSword = Properties.Resources.RightSword,
            iExplosiveGrenade = Properties.Resources.ExplosiveGrenade,
            iBlinkGrenade = Properties.Resources.BlinkGrenade,
            iEffectExplosion = Properties.Resources.EffectExplosion,
            iEffectTeleportation = Properties.Resources.EffectBlinkGrenade,
            iEffectShield = Properties.Resources.EffectShield,
            iEffectFreezeExplosion = Properties.Resources.EffectFreezeExplosion,
            iEffectFrozenBlock = Properties.Resources.EffectFrozenBlock,
            iBackPack = Properties.Resources.Backpack,
            iConsole = Properties.Resources.GlassPanelConsole;
        static Timer
            updateTimer = new Timer();
        static string
            QuartzFont = "Quartz MS";
        static int
            lastTick, lastFrameRate, frameRate,
            HoveredSlot = -1, BackpackCurrentSlot = 0,
            ammoInCage = 6,
            swordFrames = 0;
        public static Boolean
            SwordUsed = false,
            ModeWTF = false;
        static HatchBrush
            MapBrush = new HatchBrush(HatchStyle.Trellis, Color.Black, Color.DarkSeaGreen);
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
        static Inventory[]
            InventorySlots = new Inventory[4]
            {
                Inventory.Sword,
                Inventory.Pistol,
                Inventory.Explosive_Grenade,
                Inventory.Blink_Grenade
            };
        static Inventory
            CurrentItem = Inventory.Sword;
        static List<Projectile>
            Projectiles = new List<Projectile>();
        static List<FloatingText>
            FloatingTexts = new List<FloatingText>();
        static List<Grenade>
            Grenades = new List<Grenade>();
        static List<Effect>
            Effects = new List<Effect>();
        static List<Ray>
            Rays = new List<Ray>();
        public static
            Boolean ShowDI = false,
            InventoryControlMode = false;
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

        public static void InitialSetup()
        {
            Effects.Clear();
            Rays.Clear();
            Grenades.Clear();
            FloatingTexts.Clear();
            Projectiles.Clear();
            if (Player1 == null)
                Player1 = new Character(250, 50);
            ViewOffset = new Point(Resolution.Width / 2 - Player1.Position.X, Resolution.Height / 2 - Player1.Position.Y);
            Map.MakeEmpty();
            Map.Union(MAP_OUTTER);
            Map.Exclude(MAP_INNER);
            Map.Union(new Rectangle(200, 350, 400, 50));
            Map.Union(new Rectangle(50, 500, 350, 30));
            Map.Union(new Rectangle(0, 650, 500, 20));
            Map.Union(new Rectangle(380, 450, 20, 50));
            Map.Union(new Rectangle(0, 600, 20, 50));
            Map.Union(new Rectangle(480, 600, 20, 50));
            Map.Union(new Rectangle(480, 400, 20, 145));

            Map.Union(new Rectangle(800, 300, 300, 20));
            for (int a = 0; a < 4; ++a)
            {
                InventoryIcons[a] = BackpackIcons[a];
                INVENTORY_SLOTS[a] = new Rectangle(NewOffset(INVENTORY_INITIAL_POINT, a * (INVENTORY_SLOT_SIZE + INVENTORY_SLOTS_MARGIN), 0), InventorySlotSize);
            }
        }

        void pMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (Player1.FreezeDuration < 0 && Player1.RespawnTimer < 0 && !Player1.Backpack)
                    switch (CurrentItem)
                    {
                        case Inventory.Pistol:
                            if (Player1.Cooldowns[0] >= ITEMS_COOLDOWN[0])
                            {
                                Player1.Cooldowns[0] = 0f;
                                if (ammoInCage < 1)
                                    FloatingTexts.Add(new FloatingText(NewOffset(Player1.Position, -32, -25), Color.CadetBlue, "Reloading", new Font(QuartzFont, 13), 0.3f));
                                else
                                {
                                    ammoInCage--;
                                    Projectiles.Add(new Projectile(Player1.Position, AngleBetween((PointF)OffsetByView(new Point(e.X + 16, e.Y + 16)), Player1.getCenterPointF())));

                                    if (ammoInCage == 0)
                                        Player1.Cooldowns[(int)Inventory.Pistol] = 0f;
                                }
                            }
                            break;
                        case Inventory.Sword:
                            if (!SwordUsed)
                            {
                                SwordUsed = true;
                                swordFrames = 0;
                            }
                            break;
                        case Inventory.Explosive_Grenade:
                            if (Player1.Cooldowns[(int)Inventory.Explosive_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Explosive_Grenade])
                                Player1.ExplosiveGrenadeDown = 0f;
                            break;
                        case Inventory.Blink_Grenade:
                            if (Player1.Cooldowns[(int)Inventory.Blink_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Blink_Grenade])
                            {
                                Grenades.Add(new Grenade(GrenadeType.Blink, NewOffset(Player1.Position, 26, -5), AngleBetween((PointF)OffsetByView(new Point(MousePosition.X + 16, MousePosition.Y + 16)), Player1.getCenterPointF()), 0f));
                                Player1.Cooldowns[(int)Inventory.Blink_Grenade] = 0f;
                            }
                            break;
                        case Inventory.Shotgun:
                            if (Player1.Cooldowns[(int)Inventory.Shotgun] >= ITEMS_COOLDOWN[(int)Inventory.Shotgun])
                            {
                                Player1.Cooldowns[(int)Inventory.Shotgun] = 0f;
                                float middle = AngleBetween((PointF)OffsetByView(new Point(e.X + 16, e.Y + 16)), Player1.getCenterPointF());
                                for (float fi = (middle - 10f); fi < (middle + 10f); fi += 2.0f)
                                    Projectiles.Add(new Projectile(Player1.Position, fi, GunType.Shotgun));
                            }
                            break;
                        case Inventory.Railgun:
                            if (Player1.Cooldowns[(int)Inventory.Railgun] >= ITEMS_COOLDOWN[(int)Inventory.Railgun])
                            {
                                Player1.Cooldowns[(int)Inventory.Railgun] = 0f;
                                Rays.Add(new Ray(Player1.getCenterPointF(), AngleBetween((PointF)OffsetByView(new Point(MousePosition.X, MousePosition.Y)), Player1.getCenterPointF()), Player1, RayType.Railgun, Map));
                            }
                            break;
                        case Inventory.Shield:
                            if (Player1.Cooldowns[(int)Inventory.Shield] >= ITEMS_COOLDOWN[(int)Inventory.Shield])
                            {
                                Player1.Cooldowns[(int)Inventory.Shield] = Player1.ShieldDuration = 0f;
                            }
                            break;
                        case Inventory.Freeze_Rifle:
                            if (Player1.Cooldowns[(int)Inventory.Freeze_Rifle] >= ITEMS_COOLDOWN[(int)Inventory.Freeze_Rifle])
                            {
                                Player1.Cooldowns[(int)Inventory.Freeze_Rifle] = 0f;
                                Ray temp_ray = new Ray(Player1.getCenterPointF(), AngleBetween((PointF)OffsetByView(new Point(MousePosition.X, MousePosition.Y)), Player1.getCenterPointF()), Player1, RayType.Freeze, Map);
                                Rays.Add(temp_ray);
                                Effects.Add(new Effect(EffectType.Freeze, new Rectangle(NewOffset(temp_ray.End, -32, -32), new Size(64, 64)), iEffectFreezeExplosion, FREEZE_EXPLOSION_FRAMES));
                            }
                            break;
                    }
        }

        void pMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !Player1.Backpack)
                Player1.Position = OffsetByView(e.Location);
            if (e.Button == MouseButtons.Left)
                if (Player1.RespawnTimer < 0 && !Player1.Backpack)
                    switch (CurrentItem)
                    {
                        case Inventory.Explosive_Grenade:
                            if (Player1.ExplosiveGrenadeDown >= 0f && Player1.Cooldowns[(int)Inventory.Explosive_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Explosive_Grenade])
                                    GrenadeThrow(false);
                            break;
                    }
        }

        private static void GrenadeThrow(Boolean Over)
        {
            Grenades.Add(new Grenade(GrenadeType.Explosive, NewOffset(Player1.Position, 26, -5), AngleBetween((PointF)OffsetByView(new Point(MousePosition.X + 16, MousePosition.Y + 16)), Player1.getCenterPointF()), (Over ? (EXPLOSIVE_GRENADE_DURATION - 0.01f) : Player1.ExplosiveGrenadeDown)));
            Player1.ExplosiveGrenadeDown = -0.01f;
            Player1.Cooldowns[(int)Inventory.Explosive_Grenade] = 0f;
        }

        void pMouseMove(object sender, MouseEventArgs e)
        {
            if (!InventoryControlMode)
            {
                Rectangle NewBackPack = BACKPACK_RECTANGLE;
                NewBackPack.Location.Offset(10, 10);
                NewBackPack.Inflate(-10, -10);
                Boolean Determined = false;
                if (Player1.Backpack && NewBackPack.Contains(e.Location))
                    for (int a = 0; a < BackpackIcons.Length; ++a)
                        if (BACKPACK_SLOTS[a].Contains(e.Location))
                        {
                            HoveredSlot = a;
                            Determined = true;
                            break;
                        }
                if (!Determined)
                    HoveredSlot = -1;
            }
        }

        void pKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    if (Console.Enabled)
                        Console.Close();
                    else
                        if (Player1.Backpack)
                            Player1.Backpack = false;
                        else
                            Application.Exit();
                    break;
                case Keys.Tab:
                    if (!Console.Enabled)
                        Console.Enabled = true;
                    else
                        Console.Close();
                    break;
                case Keys.Left:
                case Keys.A:
                    KeysDown[0] = false;
                    break;
                case Keys.Right:
                case Keys.D:
                    KeysDown[1] = false;
                    break;
                case Keys.Up:
                case Keys.W:
                    KeysDown[2] = false;
                    break;
                case Keys.B:
                    if (!Console.Enabled)
                    {
                        Player1.Backpack = !Player1.Backpack;
                        if (Player1.Backpack)
                            HoveredSlot = (int)CurrentItem;
                        for (int a = 0; a < 3; ++a)
                            KeysDown[a] = false;
                    }
                    break;
                case Keys.D1:
                    BackpackCurrentSlot = 0;
                    if (!Player1.Backpack)
                        CurrentItem = InventorySlots[0];
                    else
                        if (HoveredSlot != -1)
                        {
                            InventorySlots[0] = (Inventory)HoveredSlot;
                            InventoryIcons[0] = BackpackIcons[HoveredSlot];
                            CurrentItem = (Inventory)HoveredSlot;
                        }
                    break;
                case Keys.D2:
                    BackpackCurrentSlot = 1;
                    if (!Player1.Backpack)
                        CurrentItem = InventorySlots[1];
                    else
                        if (HoveredSlot != -1)
                        {
                            InventorySlots[1] = (Inventory)HoveredSlot;
                            InventoryIcons[1] = BackpackIcons[HoveredSlot];
                            CurrentItem = (Inventory)HoveredSlot;
                        }
                    break;
                case Keys.D3:
                    BackpackCurrentSlot = 2;
                    if (!Player1.Backpack)
                        CurrentItem = InventorySlots[2];
                    else
                        if (HoveredSlot != -1)
                        {
                            InventorySlots[2] = (Inventory)HoveredSlot;
                            InventoryIcons[2] = BackpackIcons[HoveredSlot];
                            CurrentItem = (Inventory)HoveredSlot;
                        }
                    break;
                case Keys.D4:
                    BackpackCurrentSlot = 3;
                    if (!Player1.Backpack)
                        CurrentItem = InventorySlots[3];
                    else
                        if (HoveredSlot != -1)
                        {
                            InventorySlots[3] = (Inventory)HoveredSlot;
                            InventoryIcons[3] = BackpackIcons[HoveredSlot];
                            CurrentItem = (Inventory)HoveredSlot;
                        }
                    break;
            }
        }

        void pKeyDown(object sender, KeyEventArgs e)
        {
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
            else
                switch (e.KeyData)
                {
                    case Keys.Enter:
                        CurrentItem = (Inventory)HoveredSlot;
                        InventoryIcons[BackpackCurrentSlot] = BackpackIcons[HoveredSlot];
                        InventorySlots[BackpackCurrentSlot] = (Inventory)HoveredSlot;
                        break;
                    case Keys.Left:
                    case Keys.A:
                        if (!Player1.Backpack)
                        {
                            Player1.Rotation = false;
                            KeysDown[0] = true;
                            KeysDown[1] = false;
                        }
                        else
                            if (HoveredSlot % 5 == 0 || HoveredSlot < 0)
                                HoveredSlot += (HoveredSlot >= 5 ? (BackpackIcons.Length % 5 - 1) : 4);
                            else
                                HoveredSlot--;
                        break;
                    case Keys.Right:
                    case Keys.D:
                        if (!Player1.Backpack)
                        {
                            Player1.Rotation = true;
                            KeysDown[0] = false;
                            KeysDown[1] = true;
                        }
                        else
                            if (HoveredSlot % 5 == 4)
                                HoveredSlot -= 4;
                            else
                                if (HoveredSlot < BackpackIcons.Length - 1)
                                    HoveredSlot++;
                        break;
                    case Keys.Up:
                    case Keys.W:
                        if (!Player1.Backpack)
                            KeysDown[2] = true;
                        else
                            if (HoveredSlot < 5)
                            {
                                HoveredSlot += (BackpackIcons.Length / 5) * 5;
                                if (HoveredSlot >= BackpackIcons.Length)
                                    HoveredSlot = BackpackIcons.Length - 1;
                            }
                            else
                                HoveredSlot -= 5;
                        break;
                    case Keys.Down:
                    case Keys.S:
                        if (Player1.Backpack)
                            if (HoveredSlot > 20)
                                HoveredSlot -= (int)((BackpackIcons.Length / 5) * 5);
                            else
                                if (HoveredSlot + 5 < BackpackIcons.Length)
                                    HoveredSlot += 5;
                                else
                                    HoveredSlot = BackpackIcons.Length - 1;
                        break;
                }
        }

        void pUpdate(object sender, EventArgs e)
        {
            Player1.CooldownIncrease(0.01f);
            if (ModeWTF)
                Player1.CooldownIncrease(-1f);
            if (Player1.Cooldowns[(int)Inventory.Pistol] > PISTOL_RELOAD_COOLDOWN && ammoInCage < 1)
                ammoInCage = 6;
            if (Player1.ExplosiveGrenadeDown >= 0f && EXPLOSIVE_GRENADE_DURATION > Player1.ExplosiveGrenadeDown)
                Player1.ExplosiveGrenadeDown += 0.01f;
            if (Player1.ExplosiveGrenadeDown > EXPLOSIVE_GRENADE_DURATION - 0.01f)
                GrenadeThrow(true);
            ViewOffset = new Point(Resolution.Width / 2 - Player1.Position.X, Resolution.Height / 2 - Player1.Position.Y);
            if (Player1.RespawnTimer < 0 && Player1.FreezeDuration < 0)
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
            for (int b = 0; b < Rays.Count; ++b)
                if (!Rays[b].Exist)
                    Rays.Remove(Rays[b]);
            for (int b = 0; b < Grenades.Count; ++b)
                if (Grenades[b].Exist)
                    Grenades[b].Move(Map);
                else
                {
                    if (Grenades[b].Type == GrenadeType.Explosive)
                        Effects.Add(new Effect(EffectType.Explosion, NewOffset(Grenades[b].Position, -48, -48), new Size(96, 96), iEffectExplosion, 44));
                    else
                    {
                        Effects.Add(new Effect(EffectType.Teleportation, NewOffset(Player1.Position, -17, -6), new Size(64, 64), iEffectTeleportation, 16));
                        Image rev = iEffectTeleportation;
                        rev.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        Effects.Add(new Effect(EffectType.Teleportation, NewOffset(Grenades[b].Position, -45, -61), new Size(64, 64), rev, 16));
                    }
                    Grenades.Remove(Grenades[b]);
                }
            for (int p = 0; p < Projectiles.Count; ++p)
                if (Projectiles[p].Exist)
                {
                    Region TempRegion = new Region();
                    if (Player1.ShieldDuration < 0)
                    {
                        TempRegion.MakeEmpty();
                        Projectiles[p].Move(Map, TempRegion);
                    }
                    else
                        Projectiles[p].Move(Map, Player1.Shield);
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
                        if (target != "none" && Player1.ShieldDuration < 0)
                            FloatingTexts.Add(new FloatingText(new Point(Player1.Position.X + 10, Player1.Position.Y - 10), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
                        Projectiles.Remove(Projectiles[p]);
                    }
                }
                else
                    Projectiles.Remove(Projectiles[p]);
            Player1.FallIfCan(Map);
            Player1.JumpIfCan(Map);
            for (int a = 0; a < FloatingTexts.Count; ++a)
                if (FloatingTexts[a].Exist)
                    FloatingTexts[a].Refresh();
                else
                    FloatingTexts.Remove(FloatingTexts[a]);
            Player1.RefreshBody();
            Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (Control.IsKeyLocked(Keys.Scroll))
                g.ScaleTransform(0.5f, 0.5f);
            g.TranslateTransform(ViewOffset.X, ViewOffset.Y);
            foreach (Ray TR in Rays)
                if (TR.Exist)
                {
                    TR.Refresh(Player1);
                    if (TR.Type == RayType.Railgun)
                    {
                        if (TR.RayRegion.IsVisible(Player1.Body.GetBounds(g)) && Player1 != TR.Initiator)
                            Player1.DoDamage(33);
                        if (!TR.Excluded)
                        {
                            Map.Exclude(TR.RayRegion);
                            TR.Excluded = true;
                        }
                    }
                    else
                        if (TR.RayRegion.IsVisible(Player1.Body.GetBounds(g)) && Player1 != TR.Initiator)
                        {
                            Player1.FreezeDuration = 0f;
                        }
                    g.FillRegion(TR.Type == RayType.Railgun ? Brushes.CornflowerBlue : new LinearGradientBrush(TR.Start, TR.End, Color.LightCyan, Color.LightBlue) , TR.RayRegion);
                }
            g.FillRegion(MapBrush, Map);
            if (Player1.RespawnTimer < 0)
            {
                g.DrawString(Player1.Health.ToString(), new Font(QuartzFont, 11), Player1.getHealthBrush(), Player1.getHealthRectangleF(), MiddleText);
                g.FillRegion(Brushes.Orange, Player1.Body);
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
                if (Player1.FreezeDuration > -0.01f)
                    g.DrawImage(iEffectFrozenBlock, Player1.Position.X - 3, Player1.Position.Y + 22, 32, 32);
                if (Player1.ShieldDuration > -0.01f)
                    g.DrawImage(iEffectShield, Player1.Shield.GetBounds(g));
            }
            else
                g.DrawImage(iSkull, Player1.getPlaceForSkull());
            foreach (Grenade TG in Grenades)
                if (TG.Exist)
                    g.DrawImage(TG.Type == GrenadeType.Explosive ? iExplosiveGrenade : iBlinkGrenade, TG.Position.X, TG.Position.Y, GRENADE_SIZE, GRENADE_SIZE);
                else
                    if (TG.Type == GrenadeType.Explosive)
                        Map.Exclude(TG.HitBox);
            for (int b = 0; b < Effects.Count; ++b)
                if (Effects[b].Exist)
                {
                    Effects[b].Draw(g);
                    switch (Effects[b].Type)
                    {
                        case EffectType.Teleportation:
                            if (Effects[b].Frames == Effects[b].MaxFrames / 3)
                                Player1.Position = NewOffset(Effects[b].Position, 15, -15);
                            break;
                        case EffectType.Explosion:
                            if (Player1.Body.IsVisible(Effects[b].Place) && !Effects[b].HitedOnce && Player1.ShieldDuration < 0)
                            {
                                int damage = 1; Boolean calculated = false;
                                double distance1 = Math.Abs(Math.Sqrt(Math.Pow(Player1.Position.X - Effects[b].getCenter().X, 2) + Math.Pow(Player1.Position.Y - Effects[b].getCenter().Y, 2))),
                                    distance2 = Math.Abs(Math.Sqrt(Math.Pow(Player1.Position.X + CHARACTER_SIZE.Width - Effects[b].getCenter().X, 2) + Math.Pow(Player1.Position.Y + CHARACTER_SIZE.Height - Effects[b].getCenter().Y, 2)));
                                if (distance1 < Effects[b].Dimension.Width)
                                {
                                    damage = 150 - (int)distance1;
                                    calculated = true;
                                }
                                if (!calculated && distance2 < Effects[b].Dimension.Width)
                                    damage = 150 - (int)distance2;
                                damage -= Effects[b].Frames / 4;
                                Boolean LethalDamage = Player1.DoDamage(damage);
                                if (!LethalDamage)
                                    FloatingTexts.Add(new FloatingText(new Point(Player1.Position.X - 25, Player1.Position.Y - 30), Color.FromArgb(255, 0, 192, 0), "Respawn", new Font(QuartzFont, 16), CHARACTER_RESPAWN_DURATION));
                                FloatingTexts.Add(new FloatingText(new Point(Player1.Position.X + 10, Player1.Position.Y - 10), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
                                Effects[b].HitedOnce = true;
                            }
                            break;
                        case EffectType.Freeze:
                            if (Player1.ShieldDuration < 0 && Player1.Body.IsVisible(Effects[b].Place) && !Effects[b].HitedOnce)
                            {
                                Player1.FreezeDuration = 0f;
                                int damage = 30 + getRandom.Next(-10, 11);
                                if (Player1.Health - damage < 1)
                                    damage--;
                                Player1.DoDamage(damage);
                                FloatingTexts.Add(new FloatingText(new Point(Player1.Position.X + 10, Player1.Position.Y - 10), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION));
                                Effects[b].HitedOnce = true;
                            }
                            break;
                    }
                }
                else
                    Effects.Remove(Effects[b]);
            foreach (Projectile TP in Projectiles)
                if (TP.Exist)
                    g.FillEllipse(Brushes.LightGoldenrodYellow, TP.Position.X, TP.Position.Y, TP.Size, TP.Size);
            foreach (FloatingText TFT in FloatingTexts)
                if (TFT.Exist)
                    TFT.Draw(g);
            g.ResetTransform();
            for(int a = 0; a < 4; ++a)
            {
                g.DrawImage(InventoryIcons[a], INVENTORY_SLOTS[a]);
                if ((int)CurrentItem == (int)InventorySlots[a])
                    g.DrawRectangle(Pens.Gray, INVENTORY_SLOTS[a]);
                else
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 96, 96, 96)), INVENTORY_SLOTS[a]);
                    g.DrawRectangle(Pens.Black, INVENTORY_SLOTS[a]);
                }
                string CDoutput = "";
                if (a != 0)
                {
                    if (InventorySlots[a] == Inventory.Pistol)
                    {
                        if (ammoInCage < PISTOL_CAGE || CurrentItem == Inventory.Pistol)
                            if (ammoInCage > 0)
                                CDoutput = ammoInCage.ToString() + "/" + PISTOL_CAGE.ToString();
                            else
                                CDoutput = "[Reload]" + Math.Round(ITEMS_COOLDOWN[(int)Inventory.Pistol] - Player1.Cooldowns[(int)Inventory.Pistol], 1).ToString();
                    }
                    else
                        if (Player1.Cooldowns[(int)InventorySlots[a]] < ITEMS_COOLDOWN[(int)InventorySlots[a]])
                            CDoutput = Math.Round(ITEMS_COOLDOWN[(int)InventorySlots[a]] - Player1.Cooldowns[(int)InventorySlots[a]], 1).ToString();
                    g.DrawString(CDoutput, new Font("Tahoma", 14),
                                                new SolidBrush(Color.FromArgb(190, 190, 190)), NewOffset(INVENTORY_SLOTS[a].Location, 0, 102));
                }
                g.DrawString((a + 1).ToString(), new Font("Tahoma", 15), new SolidBrush(Color.FromArgb(190, 190, 190)), NewOffset(INVENTORY_SLOTS[a].Location, -16, 0));
            }
            if (Player1.Backpack)
            {
                int OX = 0, OY = 10;
                g.DrawImage(iBackPack, BACKPACK_RECTANGLE);
                for (int i = 0; i < BackpackIcons.Length; ++i)
                {
                    OX += 10;
                    BACKPACK_SLOTS[i] = new Rectangle(BACKPACK_RECTANGLE.X + OX + 100 * (i%5), BACKPACK_RECTANGLE.Y + OY, 100, 100);
                    if (i == HoveredSlot)
                        g.DrawRectangle(new Pen(Color.Goldenrod, 10), new Rectangle(NewOffset(BACKPACK_SLOTS[i].Location, -3, -3), new Size(106, 106)));
                    g.DrawImage(BackpackIcons[i], BACKPACK_SLOTS[i]);
                    if (i % 5 == 4)
                    {
                        OY += 120;
                        OX = -10;
                    }
                    OX += 10;
                }
            }
            if (ShowDI)
            #region Debug Information
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(64, 0, 0, 0)), FPS_RECTANGLE);
                g.DrawString(CalculateFrameRate().ToString(), new Font(QuartzFont, 13), Brushes.White, (RectangleF)FPS_RECTANGLE, MiddleText);
                string output = "Player 1 Position: " + Player1.Position.ToString() + "\n" +
                    "View Offset: " + ViewOffset.ToString() + "\n" + Player1.ExplosiveGrenadeDown.ToString() + "\n";
                for (int i = 0; i < 4; ++i)
                    output += InventorySlots[i].ToString() + "\n";
                for (int b = 0; b < BackpackIcons.Length; ++b)
                    output += Player1.Cooldowns[b].ToString() + "\n";
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

        public static Point NewOffset(PointF Initial, int x, int y)
        {
            return new Point((int)Initial.X + x, (int)Initial.Y + y);
        }

        public static Point NewOffset(Point Initial, Point Offset)
        {
            return new Point(Initial.X + Offset.X, Initial.Y + Offset.Y);
        }

        public static Point NewOffset(Point Initial, int x, int y)
        {
            return new Point(Initial.X + x, Initial.Y + y);
        }

        public static Point OffsetByView(Point Input)
        {
            return new Point(Input.X - ViewOffset.X, Input.Y - ViewOffset.Y);
        }

        public static float AngleBetween(PointF One, PointF Two)
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

        public static void RefreshInventory()
        {
            Player1.CooldownIncrease(60f);
        }

        public static Point getRandomLocationOnMap()
        {
            Rectangle TR;
        Mark:
            TR = new Rectangle(getRandom.Next(MAP_OUTTER.Left, MAP_OUTTER.Right), getRandom.Next(MAP_OUTTER.Top, MAP_OUTTER.Bottom), 54, 26);
                if (!Map.IsVisible(TR))
                    return TR.Location;
            goto Mark;
        }
    }
}

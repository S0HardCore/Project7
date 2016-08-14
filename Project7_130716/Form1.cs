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
            PROJECTILE_VELOCITY = 15,
            PROJECTILE_SIZE = 4,
            PROJECTILE_RICOCHET_RANGE = 5,
            PISTOL_CAGE = 6,
            SWORD_FRAMES = 6,
            EXPLOSION_FRAMES = 44,
            FREEZE_EXPLOSION_FRAMES = 25,
            STASIS_FRAMES = 16,
            GRENADE_SIZE = 16,
            RAY_SIZE = 4,
            SHIELD_SIZE = 100,
            JETPACK_MAX_FUEL = 100,
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
            STASIS_EFFECT_DURATION = 1.75f,
            FLAME_GRENADE_COOLDOWN = 9.9f,
            SNIPER_RIFLE_COOLDOWN = 18.2f,
            UFO_COOLDOWN = 60f,
            UFO_DURATION = 7f,
            FLOATING_TEXT_DURATION = 0.83f;
        public static readonly float[]
            ITEMS_COOLDOWN = new float[14]
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
                9.2f,
                14.3f,
                20.1f,
                14f,
                60f
            };
        public static readonly Size
            CHARACTER_SIZE = new Size(54, 26),
            UFO_SIZE = new Size(64, 24),
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
            Jet_Pack,
            Sniper_Rifle,
            Flaming_Fist,
            UFO
        }
        public enum GrenadeType
        {
            Explosive,
            Blink,
            Freeze,
            Stasis
        }
        public enum GunType
        {
            Pistol,
            Shotgun,
            Sniper
        }
        public enum EffectType
        {
            GrenadeExplosion,
            Teleportation,
            Freeze,
            Stasis,
            UFOExplosion
        }
        public enum RayType
        {
            Railgun,
            Freeze,
            Sniper
        }
        public enum SummonType
        {
            UFO
        }
        public static Image[]
            BackpackIcons = new Image[14]
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
                Properties.Resources.IconJetPack,
                Properties.Resources.IconSniperRifle,
                Properties.Resources.IconFlamingPunch,
                Properties.Resources.IconUFO
            },
            InventoryIcons = new Image[4];
        public static Image
            iSkull = Properties.Resources.CharacterSkull,
            iCursor = Properties.Resources.CursorCrosshair,
            iLeftSword = Properties.Resources.LeftSword,
            iRightSword = Properties.Resources.RightSword,
            iExplosiveGrenade = Properties.Resources.ExplosiveGrenade,
            iBlinkGrenade = Properties.Resources.BlinkGrenade,
            iFreezeGrenade = Properties.Resources.FreezeGrenade,
            iEffectExplosion = Properties.Resources.EffectExplosion,
            iEffectTeleportation = Properties.Resources.EffectBlinkGrenade,
            iEffectShield = Properties.Resources.EffectShield,
            iEffectFreezeExplosion = Properties.Resources.EffectFreezeExplosion,
            iEffectFrozenBlock = Properties.Resources.EffectFrozenBlock,
            iEffectStasis = Properties.Resources.EffectStasis,
            iEffectMiniStasis = Properties.Resources.EffectMiniStasis,
            iEffectFlameSmall = Properties.Resources.EffectFlameSmall,
            iUnitUFO = Properties.Resources.UFO,
            iBackPack = Properties.Resources.Backpack,
            iConsole = Properties.Resources.GlassPanelConsole;
        static Timer
            updateTimer = new Timer();
        static string
            QuartzFont = "Quartz MS";
        static int
            lastTick, lastFrameRate, frameRate,
            botIndex = 1,
            HoveredSlot = -1, BackpackCurrentSlot = 0;
        public static bool
            ModeWTF = false,
            ShowDI = false,
            InventoryControlMode = false;
        static HatchBrush
            MapBrush = new HatchBrush(HatchStyle.Trellis, Color.Black, Color.DarkSeaGreen);
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
        static List<KnockbackSystem>
            Knockbacks = new List<KnockbackSystem>();
        static List<Unit>
            Units = new List<Unit>();
        static List<Character>
            Players = new List<Character>();
        static ConsolePrototype
            Console = new ConsolePrototype();
        static bool[] 
            KeysDown = new bool[3] { false, false, false };

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
            this.StartPosition = FormStartPosition.Manual;
            Location = new Point();
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
            Units.Clear();
            Knockbacks.Clear();
            if (Players.Count == 0)
            {
                Players.Add(new Character(250, 50));
                Players.Add(new Character(400, 50));
                Players.Add(new Character(100, 50));
                Players.Add(new Character(700, 50));
                for (int a = 0; a < 4; ++a)
                {
                    InventoryIcons[a] = BackpackIcons[a];
                    INVENTORY_SLOTS[a] = new Rectangle(NewOffset(INVENTORY_INITIAL_POINT, a * (INVENTORY_SLOT_SIZE + INVENTORY_SLOTS_MARGIN), 0), InventorySlotSize);
                }
            }
            else
                Players[0].Health = 100;
            ViewOffset = new Point(Resolution.Width / 2 - Players[0].Position.X, Resolution.Height / 2 - Players[0].Position.Y);
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
            Map.Union(new Rectangle(1095, 300, 5, 300));
        }

        void pMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (Players[0].FreezeDuration < 0 && Players[0].RespawnTimer < 0 && !Players[0].Backpack)
                    switch (Players[0].CurrentItem)
                    {
                        case Inventory.Pistol:
                            if (Players[0].Cooldowns[0] >= ITEMS_COOLDOWN[0])
                            {
                                Players[0].Cooldowns[0] = 0f;
                                if (Players[0].ammoInCage < 1)
                                    FloatingTexts.Add(new FloatingText(NewOffset(Players[0].Position, -32, -25), Color.CadetBlue, "Reloading", new Font(QuartzFont, 13), 0.3f));
                                else
                                {
                                    Players[0].ammoInCage--;
                                    Projectiles.Add(new Projectile(Players[0].Position, AngleBetween(OffsetByView(new Point(e.X + 16, e.Y + 16)), Players[0].getCenterPointF())));

                                    if (Players[0].ammoInCage == 0)
                                        Players[0].Cooldowns[(int)Inventory.Pistol] = 0f;
                                }
                            }
                            break;
                        case Inventory.Sword:
                            if (!Players[0].SwordUsed)
                            {
                                Players[0].SwordUsed = true;
                                Players[0].swordFrames = 0;
                                foreach (Character TC in Players)
                                {
                                    Rectangle SwordHitbox = new Rectangle(NewOffset(Players[0].Position, Players[0].Rotation ? 26 : -26, 26), (Players[0].Rotation ? new Size(35, 35) : new Size(26, 26)));
                                    if (SwordHitbox.IntersectsWith(new Rectangle(TC.Position, new Size(26, 54))) && TC != Players[0] && TC.ShieldDuration < 0f && TC.RespawnTimer < 0f)
                                    {
                                        int damage = getRandom.Next(8) + 8;
                                        bool LethalDamage = TC.DoDamage(damage);
                                        FloatingTexts.Add(new FloatingText(new Point(TC.Position.X + 10, TC.Position.Y - 20), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
                                        if (!LethalDamage)
                                            FloatingTexts.Add(new FloatingText(new Point(TC.Position.X - 25, TC.Position.Y - 30), Color.FromArgb(255, 0, 192, 0), "Respawn", new Font(QuartzFont, 16), CHARACTER_RESPAWN_DURATION));
                                    }
                                }
                            }
                            break;
                        case Inventory.Explosive_Grenade:
                            if (Players[0].Cooldowns[(int)Inventory.Explosive_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Explosive_Grenade])
                                Players[0].ExplosiveGrenadeDown = 0f;
                            break;
                        case Inventory.Blink_Grenade:
                            if (Players[0].Cooldowns[(int)Inventory.Blink_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Blink_Grenade])
                            {
                                Grenades.Add(new Grenade(GrenadeType.Blink, NewOffset(Players[0].Position, 26, -5), AngleBetween(OffsetByView(NewOffset(MousePosition, 16, 16)), Players[0].getCenterPointF()), 0f));
                                Players[0].Cooldowns[(int)Inventory.Blink_Grenade] = 0f;
                            }
                            break;
                        case Inventory.Shotgun:
                            if (Players[0].Cooldowns[(int)Inventory.Shotgun] >= ITEMS_COOLDOWN[(int)Inventory.Shotgun])
                            {
                                Players[0].Cooldowns[(int)Inventory.Shotgun] = 0f;
                                float middle = AngleBetween((PointF)OffsetByView(new Point(e.X + 16, e.Y + 16)), Players[0].getCenterPointF());
                                for (float fi = (middle - 10f); fi < (middle + 10f); fi += 2.0f)
                                    Projectiles.Add(new Projectile(Players[0].Position, fi, GunType.Shotgun));
                            }
                            break;
                        case Inventory.Railgun:
                            if (Players[0].Cooldowns[(int)Inventory.Railgun] >= ITEMS_COOLDOWN[(int)Inventory.Railgun])
                            {
                                Players[0].Cooldowns[(int)Inventory.Railgun] = 0f;
                                Rays.Add(new Ray(Players[0].getCenterPointF(), AngleBetween(OffsetByView(new Point(MousePosition.X, MousePosition.Y)), Players[0].getCenterPointF()), Players, Players[0], RayType.Railgun, Map));
                            }
                            break;
                        case Inventory.Shield:
                            if (Players[0].Cooldowns[(int)Inventory.Shield] >= ITEMS_COOLDOWN[(int)Inventory.Shield])
                            {
                                Players[0].Cooldowns[(int)Inventory.Shield] = Players[0].ShieldDuration = 0f;
                            }
                            break;
                        case Inventory.Freeze_Rifle:
                            if (Players[0].Cooldowns[(int)Inventory.Freeze_Rifle] >= ITEMS_COOLDOWN[(int)Inventory.Freeze_Rifle])
                            {
                                Players[0].Cooldowns[(int)Inventory.Freeze_Rifle] = 0f;
                                Ray temp_ray = new Ray(Players[0].getCenterPointF(), AngleBetween(OffsetByView(new Point(MousePosition.X, MousePosition.Y)), Players[0].getCenterPointF()), Players, Players[0], RayType.Freeze, Map);
                                Rays.Add(temp_ray);
                                Effects.Add(new Effect(EffectType.Freeze, new Rectangle(NewOffset(temp_ray.End, -32, -32), new Size(64, 64)), iEffectFreezeExplosion, FREEZE_EXPLOSION_FRAMES));
                            }
                            break;
                        case Inventory.Freeze_Grenade:
                            if (Players[0].Cooldowns[(int)Inventory.Freeze_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Freeze_Grenade])
                            {
                                Players[0].Cooldowns[(int)Inventory.Freeze_Grenade] = 0f;
                                Grenades.Add(new Grenade(GrenadeType.Freeze, NewOffset(Players[0].Position, 26, -5), AngleBetween(OffsetByView(NewOffset(MousePosition, 16, 16)), Players[0].getCenterPointF()), 0f));
                            }
                            break;
                        case Inventory.Stasis_Grenade:
                            if (Players[0].Cooldowns[(int)Inventory.Stasis_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Stasis_Grenade])
                            {
                                Players[0].Cooldowns[(int)Inventory.Stasis_Grenade] = 0f;
                                Grenades.Add(new Grenade(GrenadeType.Stasis, NewOffset(Players[0].Position, 26, -5), AngleBetween(OffsetByView(NewOffset(MousePosition, 16, 16)), Players[0].getCenterPointF()), 0f));
                            }
                            break;
                        case Inventory.Sniper_Rifle:
                            if (Players[0].Cooldowns[(int)Inventory.Sniper_Rifle] >= ITEMS_COOLDOWN[(int)Inventory.Sniper_Rifle])
                            {
                                Players[0].Cooldowns[(int)Inventory.Sniper_Rifle] = 0f;
                                for (int a = 0; a < Rays.Count; ++a)
                                    if (Rays[a].Type == RayType.Sniper && Rays[a].Initiator == Players[0])
                                        foreach (Character TC in Players)
                                            if (Rays[a].RayRegion.IsVisible(new Rectangle(TC.Position, new Size(26, 54))) && TC != Rays[a].Initiator && TC.RespawnTimer <0f)
                                            {
                                                bool LethalDamage = TC.DoDamage(100);
                                                if (!LethalDamage)
                                                    FloatingTexts.Add(new FloatingText(new Point(TC.Position.X - 25, TC.Position.Y - 30), Color.FromArgb(255, 0, 192, 0), "Respawn", new Font(QuartzFont, 16), CHARACTER_RESPAWN_DURATION));
                                                FloatingTexts.Add(new FloatingText(new Point(TC.Position.X + 10, TC.Position.Y - 10), Color.FromArgb(255, 192, 0, 0), "100", new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
                                            }
                            }
                            break;
                        case Inventory.Flaming_Fist:
                            if (Players[0].Cooldowns[(int)Inventory.Flaming_Fist] >= ITEMS_COOLDOWN[(int)Inventory.Flaming_Fist])
                            {
                                Players[0].Cooldowns[(int)Inventory.Flaming_Fist] = 0f;
                                foreach (Character TC in Players)
                                {
                                    Rectangle FistHitbox = new Rectangle(NewOffset(Players[0].Position, Players[0].Rotation ? 20 : -14, 20), (Players[0].Rotation ? new Size(30, 30) : new Size(26, 26)));
                                    if (FistHitbox.IntersectsWith(new Rectangle(TC.Position, new Size(26, 54))) && TC != Players[0] && TC.ShieldDuration < 0f && TC.RespawnTimer < 0f)
                                    {
                                        float Direction = AngleBetween(OffsetByView(NewOffset(MousePosition, 16, 16)), Players[0].getCenterPointF());
                                        //MessageBox.Show(Direction.ToString());
                                        Knockbacks.Add(new KnockbackSystem(TC, 35f, Direction, 1f));
                                        int damage = getRandom.Next(10) + 30;
                                        bool LethalDamage = TC.DoDamage(damage);
                                        FloatingTexts.Add(new FloatingText(new Point(TC.Position.X + 10, TC.Position.Y - 20), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
                                        if (!LethalDamage)
                                            FloatingTexts.Add(new FloatingText(new Point(TC.Position.X - 25, TC.Position.Y - 30), Color.FromArgb(255, 0, 192, 0), "Respawn", new Font(QuartzFont, 16), CHARACTER_RESPAWN_DURATION));
                                    }
                                }
                            }
                            break;
                        case Inventory.UFO:
                            if (Players[0].Cooldowns[(int)Inventory.UFO] >= ITEMS_COOLDOWN[(int)Inventory.UFO])
                            {
                                Players[0].Cooldowns[(int)Inventory.UFO] = 0f;
                                Units.Add(new Unit(NewOffset(Players[0].Position, -19, +30), UFO_SIZE, SummonType.UFO, UFO_DURATION, 10, 12, Players[0], Players));
                            }
                            break;
                    }
        }

        void pMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                Players[botIndex].Position = OffsetByView(e.Location);
            else
            if (e.Button == MouseButtons.Right && !Players[0].Backpack)
                Players[0].Position = OffsetByView(e.Location);
            else
            if (e.Button == MouseButtons.Left)
                if (Players[0].RespawnTimer < 0 && !Players[0].Backpack)
                    switch (Players[0].CurrentItem)
                    {
                        case Inventory.Explosive_Grenade:
                            if (Players[0].ExplosiveGrenadeDown >= 0f && Players[0].Cooldowns[(int)Inventory.Explosive_Grenade] >= ITEMS_COOLDOWN[(int)Inventory.Explosive_Grenade])
                                GrenadeThrow(false);
                            break;
                    }
        }

        private static void GrenadeThrow(bool Over)
        {
            Grenades.Add(new Grenade(GrenadeType.Explosive, NewOffset(Players[0].Position, 26, -5), AngleBetween(OffsetByView(new Point(MousePosition.X + 16, MousePosition.Y + 16)), Players[0].getCenterPointF()), (Over ? (EXPLOSIVE_GRENADE_DURATION - 0.01f) : Players[0].ExplosiveGrenadeDown)));
            Players[0].ExplosiveGrenadeDown = -0.01f;
            Players[0].Cooldowns[(int)Inventory.Explosive_Grenade] = 0f;
        }

        void pMouseMove(object sender, MouseEventArgs e)
        {
            if (!InventoryControlMode)
            {
                Rectangle NewBackPack = BACKPACK_RECTANGLE;
                NewBackPack.Location.Offset(10, 10);
                NewBackPack.Inflate(-10, -10);
                bool Determined = false;
                if (Players[0].Backpack && NewBackPack.Contains(e.Location))
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
                case Keys.F9:
                    if (Players.Count > 1)
                        botIndex = 1;
                    break;
                case Keys.F10:
                    if (Players.Count > 2)
                        botIndex = 2;
                    break;
                case Keys.F11:
                    if (Players.Count > 3)
                        botIndex = 3;
                    break;
                case Keys.Escape:
                    if (Console.Enabled)
                        Console.Close();
                    else
                        if (Players[0].Backpack)
                        Players[0].Backpack = false;
                    else
                        Application.Exit();
                    break;
                case Keys.Tab:
                    if (!Console.Enabled)
                        Console.Enabled = true;
                    else
                        Console.Close();
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
                case Keys.B:
                    if (!Console.Enabled)
                    {
                        Players[0].Backpack = !Players[0].Backpack;
                        if (Players[0].Backpack)
                            HoveredSlot = (int)Players[0].CurrentItem;
                        for (int a = 0; a < 3; ++a)
                            KeysDown[a] = false;
                    }
                    break;
                case Keys.D1:
                    BackpackCurrentSlot = 0;
                    if (!Players[0].Backpack)
                        Players[0].CurrentItem = InventorySlots[0];
                    else
                        if (HoveredSlot != -1)
                    {
                        InventorySlots[0] = (Inventory)HoveredSlot;
                        InventoryIcons[0] = BackpackIcons[HoveredSlot];
                        Players[0].CurrentItem = (Inventory)HoveredSlot;
                    }
                    break;
                case Keys.D2:
                    BackpackCurrentSlot = 1;
                    if (!Players[0].Backpack)
                        Players[0].CurrentItem = InventorySlots[1];
                    else
                        if (HoveredSlot != -1)
                    {
                        InventorySlots[1] = (Inventory)HoveredSlot;
                        InventoryIcons[1] = BackpackIcons[HoveredSlot];
                        Players[0].CurrentItem = (Inventory)HoveredSlot;
                    }
                    break;
                case Keys.D3:
                    BackpackCurrentSlot = 2;
                    if (!Players[0].Backpack)
                        Players[0].CurrentItem = InventorySlots[2];
                    else
                        if (HoveredSlot != -1)
                    {
                        InventorySlots[2] = (Inventory)HoveredSlot;
                        InventoryIcons[2] = BackpackIcons[HoveredSlot];
                        Players[0].CurrentItem = (Inventory)HoveredSlot;
                    }
                    break;
                case Keys.D4:
                    BackpackCurrentSlot = 3;
                    if (!Players[0].Backpack)
                        Players[0].CurrentItem = InventorySlots[3];
                    else
                        if (HoveredSlot != -1)
                    {
                        InventorySlots[3] = (Inventory)HoveredSlot;
                        InventoryIcons[3] = BackpackIcons[HoveredSlot];
                        Players[0].CurrentItem = (Inventory)HoveredSlot;
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
                    case Keys.Up:
                    case Keys.Right:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Space:
                    case Keys.Q:
                    case Keys.E:
                        foreach (Unit TU in Units)
                            if (TU.Type == SummonType.UFO && TU.Owner.Equals(Players[0]))
                                TU.Command = e.KeyData.ToString();
                        break;
                    case Keys.Enter:
                        Players[0].CurrentItem = (Inventory)HoveredSlot;
                        InventoryIcons[BackpackCurrentSlot] = BackpackIcons[HoveredSlot];
                        InventorySlots[BackpackCurrentSlot] = (Inventory)HoveredSlot;
                        break;
                    case Keys.A:
                        if (!Players[0].Backpack)
                        {
                            Players[0].Rotation = false;
                            KeysDown[0] = true;
                            KeysDown[1] = false;
                        }
                        else
                            if (HoveredSlot % 5 == 0 || HoveredSlot < 0)
                                HoveredSlot += (HoveredSlot >= 5 ? (BackpackIcons.Length % 5 - 1) : 4);
                            else
                                HoveredSlot--;
                        break;
                    case Keys.D:
                        if (!Players[0].Backpack)
                        {
                            Players[0].Rotation = true;
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
                    case Keys.W:
                        if (!Players[0].Backpack)
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
                    case Keys.S:
                        if (Players[0].Backpack)
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
            Point LastPosition = Players[0].Position;
            Players[0].CooldownIncrease(0.01f);
            if (ModeWTF)
                Players[0].CooldownIncrease(-1f);
            if (Players[0].Cooldowns[(int)Inventory.Pistol] > PISTOL_RELOAD_COOLDOWN && Players[0].ammoInCage < 1)
                Players[0].ammoInCage = 6;
            if (Players[0].ExplosiveGrenadeDown >= 0f && EXPLOSIVE_GRENADE_DURATION > Players[0].ExplosiveGrenadeDown)
                Players[0].ExplosiveGrenadeDown += 0.01f;
            if (Players[0].ExplosiveGrenadeDown > EXPLOSIVE_GRENADE_DURATION - 0.01f)
                GrenadeThrow(true);
            ViewOffset = new Point(Resolution.Width / 2 - Players[0].Position.X, Resolution.Height / 2 - Players[0].Position.Y);
            if (Players[0].RespawnTimer < 0 && Players[0].FreezeDuration < 0 && Players[0].StasisDuration < 0)
            {
                if (Players[0].SwordUsed)
                    if (Players[0].swordFrames < SWORD_FRAMES)
                    {
                        Players[0].swordFrames++;
                        
                    }
                    else
                    {
                        Players[0].swordFrames = 0;
                        Players[0].SwordUsed = false;
                    }
                if (KeysDown[0])
                    Players[0].StepLeftIfCan(Map);
                if (KeysDown[1])
                    Players[0].StepRightIfCan(Map);
                if (KeysDown[2])
                    if (Players[0].CurrentItem == Inventory.Jet_Pack)
                    {
                        if (Players[0].JetPackFuel > 5f)
                            Players[0].JumpProgress = 50;
                        if (Players[0].JetPackFuel > 0f)
                            Players[0].JetPackFuel -= (1f + getRandom.Next(0, 7) / 10f);
                    }
                    else
                        if (Players[0].JumpProgress == -1 && Players[0].AtGround)
                        Players[0].JumpProgress = 0;
            }
            for (int a = 0; a < Units.Count; ++a)
                if (Units[a].Exist)
                {
                    foreach (Character TC in Players)
                        if (Units[a].AbilityUsed)
                            if (Units[a].AbilityRay.IsVisible(new Rectangle(TC.Position, CHARACTER_SIZE)))
                            {
                                DoDamageWithText(TC,1);
                            }
                    Units[a].Refresh(Map);
                    string lifetime = (UFO_DURATION - (int)Units[a].Duration).ToString();
                    foreach (FloatingText TFT in FloatingTexts)
                        if (TFT.Text == lifetime && TFT.MaxDuration == 0.01f)
                            break;
                    if (lifetime != "0")
                        FloatingTexts.Add(new FloatingText(NewOffset(Units[a].Position, 25, -25), Color.CornflowerBlue, lifetime, new Font(QuartzFont, 13), 0.01f));
                }
                else
                {
                    Effects.Add(new Effect(EffectType.UFOExplosion, NewOffset(Units[a].Position, 0, -16), new Size(96, 96), iEffectExplosion, 44));
                    Units.Remove(Units[a]);
                }
            for (int a = 0; a < Knockbacks.Count; ++a)
                if (Knockbacks[a].Exist)
                    Knockbacks[a].Refresh(Map);
                else
                    Knockbacks.Remove(Knockbacks[a]);
            int temp = 0;
            for (int a = 0; a < Rays.Count; ++a)
                if (Rays[a].Type == RayType.Sniper)
                    temp++;
            for (int b = 0; b < Rays.Count; ++b)
                if (!Rays[b].Exist || (Rays[b].Type == RayType.Sniper && (Players[0].CurrentItem != Inventory.Sniper_Rifle || temp > 0)))
                    Rays.Remove(Rays[b]);
            for (int b = 0; b < Grenades.Count; ++b)
                if (Grenades[b].Exist)
                    Grenades[b].Move(Map);
                else
                {
                    switch (Grenades[b].Type)
                    {
                        case GrenadeType.Explosive:
                            Effects.Add(new Effect(EffectType.GrenadeExplosion, NewOffset(Grenades[b].Position, -48, -48), new Size(96, 96), iEffectExplosion, 44));
                            break;
                        case GrenadeType.Blink:
                            Effects.Add(new Effect(EffectType.Teleportation, NewOffset(Players[0].Position, -17, -6), new Size(64, 64), iEffectTeleportation, 16));
                            Image rev = iEffectTeleportation;
                            rev.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            Effects.Add(new Effect(EffectType.Teleportation, NewOffset(Grenades[b].Position, -45, -61), new Size(64, 64), rev, 16));
                            break;
                        case GrenadeType.Freeze:
                            Effects.Add(new Effect(EffectType.Freeze, new Rectangle(NewOffset(Grenades[b].Position, -48, -48), new Size(96, 96)), iEffectFreezeExplosion, FREEZE_EXPLOSION_FRAMES));
                            break;
                        case GrenadeType.Stasis:
                            Effects.Add(new Effect(EffectType.Stasis, new Rectangle(NewOffset(Grenades[b].Position, -64, -64), new Size(128, 128)), iEffectStasis, STASIS_FRAMES));
                            break;
                    }
                    Grenades.Remove(Grenades[b]);
                }
            foreach (Character TC in Players)
                for (int p = 0; p < Projectiles.Count; ++p)
                    if (Projectiles[p].Exist)
                    {
                        Region TempRegion = new Region();
                        if (TC.ShieldDuration < 0)
                        {
                            TempRegion.MakeEmpty();
                            Projectiles[p].Move(Map, TempRegion);
                        }
                        else
                            Projectiles[p].Move(Map, TC.Shield);
                        Rectangle tmp = new Rectangle(TC.Position, new Size(26, 54));
                        if (tmp.Contains(Point.Round(Projectiles[p].getNextPointF())) && TC.RespawnTimer < 0f)
                        {
                            string target = TC.WhereHit(Projectiles[p].getNextPointF());
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
                            if (target != "none")
                            DoDamageWithText(TC, damage);
                            Projectiles.Remove(Projectiles[p]);
                        }
                    }
                    else
                        Projectiles.Remove(Projectiles[p]);
            for (int a = 0; a < FloatingTexts.Count; ++a)
                if (FloatingTexts[a].Exist)
                    FloatingTexts[a].Refresh();
                else
                    FloatingTexts.Remove(FloatingTexts[a]);
            foreach (Character TC in Players)
            {
                if (TC.StasisDuration < 0)
                {
                    TC.FallIfCan(Map);
                    TC.JumpIfCan(Map);
                }
                TC.RefreshBody();
            }
            Players[0].Speedometer = (float)Math.Abs(Math.Sqrt(Math.Pow(Players[0].Position.X - LastPosition.X, 2) + Math.Pow(Players[0].Position.Y - LastPosition.Y, 2)));
            Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (IsKeyLocked(Keys.Scroll))
                g.ScaleTransform(0.5f, 0.5f);
            g.TranslateTransform(ViewOffset.X, ViewOffset.Y);
            g.FillRegion(MapBrush, Map);
            foreach (Character TC in Players)
            {
                foreach (Ray TR in Rays)
                    if (TR.Exist)
                    {
                        TR.Refresh(TC);
                        switch (TR.Type)
                        {
                            case RayType.Railgun:
                                if (TR.RayRegion.IsVisible(TC.Body.GetBounds(g)) && TC != TR.Initiator)
                                    TC.DoDamage(33);
                                if (!TR.Excluded)
                                {
                                    Map.Exclude(TR.RayRegion);
                                    TR.Excluded = true;
                                }
                                g.FillRegion(Brushes.CornflowerBlue, TR.RayRegion);
                                break;
                            case RayType.Freeze:
                                if (TR.RayRegion.IsVisible(TC.Body.GetBounds(g)) && TC != TR.Initiator && TC.FreezeDuration < 0f)
                                {
                                    Effects.Add(new Effect(EffectType.Freeze, new Rectangle(NewOffset(TC.Position, -5, -20), new Size(64, 64)), iEffectFreezeExplosion, FREEZE_EXPLOSION_FRAMES));
                                    TC.FreezeDuration = 0f;
                                }
                                g.FillRegion(new LinearGradientBrush(TR.Start, TR.End, Color.LightCyan, Color.LightBlue), TR.RayRegion);
                                break;
                            case RayType.Sniper:
                                g.DrawLine(Pens.Red, TR.Start, TR.End);
                                break;
                        }
                    }
                if (TC.RespawnTimer < 0)
                {
                    g.DrawString(TC.Health.ToString(), new Font(QuartzFont, 11), TC.getHealthBrush(), TC.getHealthRectangleF(), MiddleText);
                    if (TC.CurrentItem == Inventory.Jet_Pack && KeysDown[2])
                    {
                        g.DrawImage(iEffectFlameSmall, TC.Position.X + 2, TC.Position.Y + 30);
                        g.DrawImage(iEffectFlameSmall, TC.Position.X + 19, TC.Position.Y + 30);
                    }
                    g.FillRegion(Brushes.LightSlateGray, TC.Body);
                    Rectangle SwordRectangle, SwordFrame;
                    switch (TC.CurrentItem)
                    {
                        case Inventory.Sword:
                            if (TC.Rotation)
                            {
                                SwordRectangle = new Rectangle(TC.Position.X + 21, TC.Position.Y + 6, 37, 28);
                                SwordFrame = new Rectangle(37 * (TC.swordFrames / 2), 0, 37, 28);
                                g.DrawImage(iRightSword, SwordRectangle, SwordFrame, GraphicsUnit.Pixel);
                            }
                            else
                            {
                                SwordRectangle = new Rectangle(TC.Position.X - 32, TC.Position.Y + 6, 37, 28);
                                SwordFrame = new Rectangle(74 - 37 * (TC.swordFrames / 2), 0, 37, 28);
                                g.DrawImage(iLeftSword, SwordRectangle, SwordFrame, GraphicsUnit.Pixel);
                            }
                            break;
                        case Inventory.Sniper_Rifle:
                            if (TC.Cooldowns[(int)Inventory.Sniper_Rifle] >= ITEMS_COOLDOWN[(int)Inventory.Sniper_Rifle])
                                Rays.Add(new Ray(TC.getCenterPointF(), AngleBetween(OffsetByView(NewOffset(MousePosition, 16, 16)), TC.getCenterPointF()), Players, TC, RayType.Sniper, Map));
                            break;
                        case Inventory.Jet_Pack:
                            g.DrawLine(new Pen(Color.Red, 2), TC.Position.X + 7, TC.Position.Y + 15, TC.Position.X + 7, TC.Position.Y + 32);
                            g.DrawLine(new Pen(Color.Red, 2), TC.Position.X + 5, TC.Position.Y + 32, TC.Position.X + 8, TC.Position.Y + 32);
                            g.DrawLine(new Pen(Color.Red, 2), TC.Position.X + 19, TC.Position.Y + 15, TC.Position.X + 19, TC.Position.Y + 32);
                            g.DrawLine(new Pen(Color.Red, 2), TC.Position.X + 18, TC.Position.Y + 32, TC.Position.X + 21, TC.Position.Y + 32);
                            break;
                        case Inventory.Flaming_Fist:
                            if (TC.Rotation)
                            {
                                g.FillRectangle(Brushes.OrangeRed, TC.Position.X + 22, TC.Position.Y + 30, 5, 3);
                                g.DrawLine(Pens.OrangeRed, TC.Position.X + 22, TC.Position.Y + 33, TC.Position.X + 25, TC.Position.Y + 33);
                            }
                            else
                            {
                                g.FillRectangle(Brushes.OrangeRed, TC.Position.X - 1, TC.Position.Y + 30, 5, 3);
                                g.DrawLine(Pens.OrangeRed, TC.Position.X, TC.Position.Y + 33, TC.Position.X + 3, TC.Position.Y + 33);
                            }
                            break;
                    }
                    if (TC.FreezeDuration > -0.01f)
                        g.DrawImage(iEffectFrozenBlock, TC.Position.X - 3, TC.Position.Y + 22, 32, 32);
                    if (TC.ShieldDuration > -0.01f)
                        g.DrawImage(iEffectShield, TC.Shield.GetBounds(g));
                    if (TC.StasisDuration > -0.01f)
                    {
                        Rectangle EffectFrame = new Rectangle(40 * (int)(TC.StasisDuration / 0.22f), 0, 40, 64);
                        g.DrawImage(iEffectMiniStasis, new Rectangle(TC.Position.X - 7, TC.Position.Y - 4, 40, 64), EffectFrame, GraphicsUnit.Pixel);
                    }
                }
                else
                    g.DrawImage(iSkull, TC.getPlaceForSkull());
                foreach (Grenade TG in Grenades)
                    if (TG.Exist)
                    {
                        Image GrenadeImage = iExplosiveGrenade;
                        switch (TG.Type)
                        {
                            case GrenadeType.Blink:
                                GrenadeImage = iBlinkGrenade;
                                break;
                            case GrenadeType.Freeze:
                                GrenadeImage = iFreezeGrenade;
                                break;
                            case GrenadeType.Stasis:
                                GrenadeImage = Properties.Resources.IconStasisGrenade;
                                break;
                        }
                        g.DrawImage(GrenadeImage, TG.Position.X, TG.Position.Y, GRENADE_SIZE, GRENADE_SIZE);
                    }
                for (int b = 0; b < Effects.Count; ++b)
                    if (Effects[b].Exist)
                    {
                        Effects[b].Draw(g);
                        switch (Effects[b].Type)
                        {
                            case EffectType.Teleportation:
                                if (Effects[b].Frames == Effects[b].MaxFrames / 3)
                                    TC.Position = NewOffset(Effects[b].Position, 15, -15);
                                break;
                            case EffectType.GrenadeExplosion:
                                if (TC.Body.IsVisible(Effects[b].Place) && !Effects[b].HitedOnce && TC.ShieldDuration < 0f)
                                {
                                    int damage = 1; bool calculated = false;
                                    double distance1 = Math.Abs(Math.Sqrt(Math.Pow(TC.Position.X - Effects[b].getCenter().X, 2) + Math.Pow(TC.Position.Y - Effects[b].getCenter().Y, 2))),
                                        distance2 = Math.Abs(Math.Sqrt(Math.Pow(TC.Position.X + CHARACTER_SIZE.Width - Effects[b].getCenter().X, 2) + Math.Pow(TC.Position.Y + CHARACTER_SIZE.Height - Effects[b].getCenter().Y, 2)));
                                    if (distance1 < Effects[b].Dimension.Width)
                                    {
                                        damage = 150 - (int)distance1;
                                        calculated = true;
                                    }
                                    if (!calculated && distance2 < Effects[b].Dimension.Width)
                                        damage = 150 - (int)distance2;
                                    damage -= Effects[b].Frames / 4;
                                    DoDamageWithText(TC, damage);
                                    Effects[b].HitedOnce = true;
                                }
                                break;
                            case EffectType.Freeze:
                                if (TC.ShieldDuration < 0f && TC.Body.IsVisible(Effects[b].Place) && !Effects[b].HitedOnce && TC.FreezeDuration < 1f)
                                {
                                    TC.FreezeDuration = 0f;
                                    int damage = 30 + getRandom.Next(-10, 11);
                                    if (TC.Health - damage < 1)
                                        damage--;
                                    DoDamageWithText(TC, damage);
                                    Effects[b].HitedOnce = true;
                                }
                                break;
                            case EffectType.Stasis:
                                if (TC.Body.IsVisible(Effects[b].Place) && !Effects[b].HitedOnce && TC.ShieldDuration < 0f)
                                {
                                    TC.StasisDuration = 0f;
                                    int damage = 15 + getRandom.Next(-5, 6);
                                    if (TC.Health - damage < 1)
                                        damage--;
                                    DoDamageWithText(TC, damage);
                                    Effects[b].HitedOnce = true;
                                }
                                break;
                        }
                    }
                    else
                    {
                        foreach (Character TC2 in Players)
                            if (TC2.Body.IsVisible(Effects[b].Place) && !Effects[b].HitedOnce && TC2.ShieldDuration < 0)
                                DoDamageWithText(TC2, 92 + getRandom.Next(18));
                        if (Effects[b].Type == EffectType.GrenadeExplosion || Effects[b].Type == EffectType.UFOExplosion)
                            Map.Exclude(Effects[b].HitBox);
                        Effects.Remove(Effects[b]);
                    }
            }
            foreach (Unit TU in Units)
                if (TU.Exist)
                    if (TU.Type == SummonType.UFO)
                    {
                        g.DrawImage(iUnitUFO, new Rectangle(TU.Position, new Size(64, 24)));
                        if (TU.AbilityUsed)
                            g.FillRegion(Brushes.LightBlue, TU.AbilityRay);
                    }
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
                if ((int)Players[0].CurrentItem == (int)InventorySlots[a])
                    g.DrawRectangle(Pens.LightGray, INVENTORY_SLOTS[a]);
                else
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 96, 96, 96)), INVENTORY_SLOTS[a]);
                    g.DrawRectangle(Pens.Black, INVENTORY_SLOTS[a]);
                }
                string CDoutput = "";
                if (InventorySlots[a] != Inventory.Sword)
                {
                    if (InventorySlots[a] == Inventory.Pistol)
                    {
                        if (Players[0].ammoInCage < PISTOL_CAGE || Players[0].CurrentItem == Inventory.Pistol)
                            if (Players[0].ammoInCage > 0)
                                CDoutput = Players[0].ammoInCage.ToString() + "/" + PISTOL_CAGE.ToString();
                            else
                                CDoutput = "[Reload]" + Math.Round(ITEMS_COOLDOWN[(int)Inventory.Pistol] - Players[0].Cooldowns[(int)Inventory.Pistol], 1).ToString();
                    }
                    else
                    if (InventorySlots[a] == Inventory.Jet_Pack)
                    {
                        if (Players[0].JetPackFuel < JETPACK_MAX_FUEL)
                            CDoutput = Math.Round(Math.Abs(Players[0].JetPackFuel)).ToString() + "/" + JETPACK_MAX_FUEL.ToString();
                    }
                    else
                        if (Players[0].Cooldowns[(int)InventorySlots[a]] < ITEMS_COOLDOWN[(int)InventorySlots[a]])
                        CDoutput = Math.Round(ITEMS_COOLDOWN[(int)InventorySlots[a]] - Players[0].Cooldowns[(int)InventorySlots[a]], 1).ToString();
                    g.DrawString(CDoutput, new Font("Tahoma", 14),
                                                new SolidBrush(Color.FromArgb(190, 190, 190)), NewOffset(INVENTORY_SLOTS[a].Location, 0, 102));
                }
                g.DrawString((a + 1).ToString(), new Font("Tahoma", 15), new SolidBrush(Color.FromArgb(190, 190, 190)), NewOffset(INVENTORY_SLOTS[a].Location, -16, 0));
            }
            if (Players[0].Backpack)
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
                g.DrawString(CalculateFrameRate().ToString(), new Font(QuartzFont, 13), Brushes.White, FPS_RECTANGLE, MiddleText);
                string output = "Velocity: " + Math.Round(Players[0].Speedometer, 2).ToString() + "\nPosition: " + Players[0].Position.ToString() + "\n" +
                    "View Offset: " + ViewOffset.ToString() + "\n" + Rays.Count.ToString() + "\n";
                for (int i = 0; i < 4; ++i)
                    output += InventorySlots[i].ToString() + "\n";
                for (int b = 0; b < BackpackIcons.Length; ++b)
                    output += Players[0].Cooldowns[b].ToString() + "\n";
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

        static void DoDamageWithText(Character TC, int damage)
        {
            if (TC.ShieldDuration < 0f)
            {
                bool LethalDamage = TC.DoDamage(damage);
                if (!LethalDamage)
                    FloatingTexts.Add(new FloatingText(new Point(TC.Position.X - 25, TC.Position.Y - 30), Color.FromArgb(255, 0, 192, 0), "Respawn", new Font(QuartzFont, 16), CHARACTER_RESPAWN_DURATION));
                FloatingTexts.Add(new FloatingText(new Point(TC.Position.X + 10, TC.Position.Y - 10), Color.FromArgb(255, 192, 0, 0), damage.ToString(), new Font(QuartzFont, 14), FLOATING_TEXT_DURATION / (LethalDamage ? 3 : 1)));
            }
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
            Players[0].CooldownIncrease(-1f);
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

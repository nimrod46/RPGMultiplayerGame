using Microsoft.Xna.Framework;
using NetworkingLib;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public abstract class Weapon : InteractiveItem, IDamageInflicter
    {
        public float Damage { get; set; }
        [XmlIgnore]
        public Entity? Attacker { get; set; }
        [XmlIgnore]
        public Direction Direction { get => Attacker!.SyncCurrentDirection; set => Attacker!.SyncCurrentDirection = value; }

        protected List<Type> specielWeaponEffects;
        protected double coolDownTime;
        private readonly UiTextureComponent coolDownCover;
        private double currentCoolDownTime;
        [XmlIgnore]
        public bool IsInCoolDown { get; private set; }

        public Weapon(ItemType itemType, string name, float damage, double coolDownTime) : base(itemType, name)
        {
            Damage = damage;
            SyncName = name;
            this.coolDownTime = coolDownTime;
            currentCoolDownTime = 0;
            IsInCoolDown = false;
            specielWeaponEffects = new List<Type>();
            AddSpecielWeaponEffect<FlickerEffect>();
            coolDownCover = new UiTextureComponent((g) => Vector2.Zero, UiComponent.PositionType.TopLeft, false, ITEM_LAYER * 0.1f, UiManager.Instance.CoolDownCover);
        }

        public void AddSpecielWeaponEffect<T>() where T : ISpecielWeaponEffect
        {
            specielWeaponEffects.Add(typeof(T));
        }

        public override void SetAsUiItem(UiComponent uiParent, Func<Point, Vector2> origin, UiComponent.PositionType originType)
        {
            base.SetAsUiItem(uiParent, origin, originType);
            coolDownCover.Size = uiParent.Size;
            coolDownCover.Parent = uiParent;
        }

        public override void Update(GameTime gameTime)
        {
            if (!hasAuthority)
            {
                return;
            }
            if (IsInCoolDown)
            {
                currentCoolDownTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (currentCoolDownTime >= coolDownTime)
                {
                    IsInCoolDown = false;
                    currentCoolDownTime = 0;
                }
                else
                {
                    coolDownCover.IsVisible = true;
                    coolDownCover.RenderRigion = new Rectangle(coolDownCover.RenderRigion.Location, new Point((int)(coolDownCover.Size.X * (coolDownTime - currentCoolDownTime) / coolDownTime), (int)coolDownCover.Size.Y));
                }
            }
            else
            {
                coolDownCover.IsVisible = false;
            }

        }

        public bool IsAbleToAttack()
        {
            return !IsInCoolDown;
        }

        public void Attack()
        {
            if (isInServer)
            {
                if (IsAbleToAttack())
                {
                    PreformeAttack();
                }
            }
            if(hasAuthority)
            {
                IsInCoolDown = true;
            }
        }

        public abstract void PreformeAttack();

        public virtual void Hit(Entity victim)
        {
            if (victim.IsDamageable)
            {
                InvokeBroadcastMethodNetworkly(nameof(ActivateEffectsOn), victim, this);
                victim.InvokeBroadcastMethodNetworkly(nameof(victim.OnAttackedBy), Attacker, Damage);
            }
        }

        public void ActivateEffectsOn(Entity victim, IDamageInflicter damageInflicter)
        {
            lock (specielWeaponEffects)
            {
                foreach (var specielWeaponEffectType in specielWeaponEffects)
                {
                    Activator.CreateInstance(specielWeaponEffectType, victim, damageInflicter);
                }
            }
        }

        public abstract void UpdateWeaponLocation(Entity entity);

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Type: " + ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.OrangeRed, "Weapon") + "\n" +
                "Damage: " + Damage + "\n" +
                "Cooldown time: " + coolDownTime;
        }
    }
}

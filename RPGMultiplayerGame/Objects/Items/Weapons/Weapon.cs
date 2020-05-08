﻿using Microsoft.Xna.Framework;
using NetworkingLib;
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
        public float X { get; set; }
        public float Y { get; set; }
        public float Damage { get; set; }
        [XmlIgnore]
        public Entity Attacker { get; set; }
        [XmlIgnore]
        public Direction Direction { get => Attacker.SyncCurrentDirection; set => Attacker.SyncCurrentDirection = value; }
        public bool SyncIsInCoolDown
        {
            get => isInCoolDown; set
            {
                isInCoolDown = value;
                InvokeSyncVarNetworkly(nameof(SyncIsInCoolDown), isInCoolDown);
            }
        }

        public double SyncCurrentCoolDownTime
        {
            get => currentCoolDownTime; set
            {
                currentCoolDownTime = value;
                InvokeSyncVarNetworkly(nameof(SyncCurrentCoolDownTime), currentCoolDownTime, Server.NetworkInterfaceType.UDP);
            }
        }

        protected List<Type> specielWeaponEffects;
        protected double coolDownTime;
        private readonly UiTextureComponent coolDownCover;
        private double currentCoolDownTime;
        private bool isInCoolDown;

        public Weapon(ItemType itemType, string name, float damage, double coolDownTime) : base(itemType, name)
        {
            Damage = damage;
            SyncName = name;
            this.coolDownTime = coolDownTime;
            SyncCurrentCoolDownTime = 0;
            SyncIsInCoolDown = false;
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
            if (SyncIsInCoolDown)
            {
                if (isInServer)
                {
                    SyncCurrentCoolDownTime += gameTime.ElapsedGameTime.TotalSeconds;
                    if (SyncCurrentCoolDownTime >= coolDownTime)
                    {
                        SyncCurrentCoolDownTime = 0;
                        SyncIsInCoolDown = false;
                    }
                }
                else
                {
                    coolDownCover.IsVisible = true;
                    coolDownCover.RenderRigion = new Rectangle(coolDownCover.RenderRigion.Location, new Point((int)(coolDownCover.Size.X * (coolDownTime - SyncCurrentCoolDownTime) / coolDownTime), (int)coolDownCover.Size.Y));
                }
            }
            else
            {
                if (!isInServer)
                {
                    coolDownCover.IsVisible = false;
                }
            }

        }

        public bool IsAbleToAttack()
        {
            return !SyncIsInCoolDown;
        }

        public void Attack()
        {
            if (isInServer)
            {
                if (IsAbleToAttack())
                {
                    SyncIsInCoolDown = true;
                    PreformeAttack();
                }
            }
            else
            {
                throw new Exception("Method \"" + nameof(Attack) + "\" in Weapon class was called in client");
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
                "Type: " + "Weapon" + "\n" +
                "Damage: " + Damage + "\n" +
                "Cooldown time: " + coolDownTime;
        }
    }
}

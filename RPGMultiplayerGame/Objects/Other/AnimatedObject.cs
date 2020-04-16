using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class AnimatedObject : GraphicObject
    {
        public struct GameTexture
        {
            public Texture2D Texture { get; private set; }
            public Vector2 Offset { get; private set; }

            public GameTexture(Texture2D image, Vector2 offset)
            {
                Texture = image;
                Offset = offset;
            }
        }
        public enum Direction
        {
            Left,
            Up,
            Right,
            Down,
            Idle, //TODO: Delete
        }

        public enum EntityAnimation
        {
            WalkLeft,
            WalkUp,
            WalkRight,
            WalkDown,
            IdleLeft,
            IdleUp,
            IdleRight,
            IdleDown,
            AttackLeft,
            AttackUp,
            AttackRight,
            AttackDown,
        }

        protected int SyncCurrentAnimationType
        {
            get => syncCurrentAnimationType; set
            {
                syncCurrentAnimationType = value;
                UpdateTexture();
            }
        }

        public Direction SyncCurrentDirection { get; set; }

        protected Dictionary<int, List<GameTexture>> animationsByType = new Dictionary<int, List<GameTexture>>();
        private int syncCurrentAnimationType;
        protected double animationTimeDelay;
        protected double timeSinceLastFrame;
        protected int currentAnimationIndex;
        protected bool shouldLoopAnimation;
        protected float speed;


        public AnimatedObject(Dictionary<int, List<GameTexture>> animationsByType)
        {
            this.animationsByType = animationsByType;
            speed = 0.5f / 10;
            animationTimeDelay = 100;
            shouldLoopAnimation = true;
            SyncCurrentDirection = Direction.Down;
            currentAnimationIndex = 0;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            UpdateTexture();
            BaseSize = Size;
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceLastFrame += (speed * 500);
            if (timeSinceLastFrame >= animationTimeDelay)
            {
                timeSinceLastFrame = 0;
                if (GetIsLoopAnimationFinished())
                {
                    if (shouldLoopAnimation)
                    {
                        currentAnimationIndex = 0;
                    }
                }
                else
                {
                    currentAnimationIndex++;
                }
                UpdateTexture();
            }
            base.Update(gameTime);
        }

        public virtual void UpdateTexture()
        {
            GameTexture gameTexture = animationsByType[SyncCurrentAnimationType][currentAnimationIndex];
            texture = gameTexture.Texture;
            offset = gameTexture.Offset * scale;
            Size = (texture.Bounds.Size.ToVector2() * scale).ToPoint();
        }

        public void OnCurrentAnimationTypeSet()
        {
            currentAnimationIndex = 0;
            timeSinceLastFrame = 0;
            UpdateTexture();
        }

        protected bool GetIsLoopAnimationFinished()
        {
            return currentAnimationIndex + 1 >= animationsByType[SyncCurrentAnimationType].Count;
        }

        protected void AnimationAtDir(Direction direction, int dirToAnimationIndex, bool shouldLoopAnimation)
        {
            currentAnimationIndex = 0;
            SyncCurrentDirection = direction;
            SyncCurrentAnimationType = (int)direction + dirToAnimationIndex;
            this.shouldLoopAnimation = shouldLoopAnimation;
        }
    }
}

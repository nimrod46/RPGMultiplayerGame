using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.VisualEffects;
using System;
using System.Collections.Generic;

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

        protected int SyncCurrentAnimationType
        {
            get => syncCurrentAnimationType; set
            {
                syncCurrentAnimationType = value;
                UpdateTexture();
            }
        }

        public Direction SyncCurrentDirection { get; set; }
        public float SyncSpeed
        {
            get => syncSpeed; set
            {
                syncSpeed = value;
                InvokeSyncVarNetworkly(nameof(SyncSpeed), syncSpeed);
            }
        }

        protected Dictionary<int, List<GameTexture>> animationsByType = new Dictionary<int, List<GameTexture>>();
        private int syncCurrentAnimationType;
        protected double animationTimeDelay;
        protected double timeSinceLastFrame;
        protected int currentAnimationIndex;
        protected bool shouldLoopAnimation;
        private float syncSpeed;

        public AnimatedObject(Dictionary<int, List<GameTexture>> animationsByType)
        {
            this.animationsByType = animationsByType;
            SyncSpeed = 0.5f / 10;
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
            timeSinceLastFrame += (SyncSpeed * 500);
            if (timeSinceLastFrame >= animationTimeDelay)
            {
                timeSinceLastFrame = 0;
                if (IsLoopAnimationFinished())
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
            Texture = gameTexture.Texture;
            offset = gameTexture.Offset * scale;
            Size = (Texture.Bounds.Size.ToVector2() * scale).ToPoint();
        }

        public void OnCurrentAnimationTypeSet()
        {
            currentAnimationIndex = 0;
            timeSinceLastFrame = 0;
            UpdateTexture();
        }

        public bool IsLoopAnimationFinished()
        {
            return currentAnimationIndex + 1 >= animationsByType[SyncCurrentAnimationType].Count;
        }

        protected void AnimationAtDir(Direction direction, int dirToAnimationIndex, bool shouldLoopAnimation)
        {
            
            currentAnimationIndex = 0;
            SyncCurrentDirection = direction;
            SyncCurrentAnimationType = (int)SyncCurrentDirection + dirToAnimationIndex;
            this.shouldLoopAnimation = shouldLoopAnimation;
        }
    }
}

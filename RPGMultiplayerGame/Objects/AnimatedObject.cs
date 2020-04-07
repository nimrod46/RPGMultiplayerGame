using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects
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

        protected Dictionary<int, List<GameTexture>> animationsByType = new Dictionary<int, List<GameTexture>>();
        [SyncVar(shouldInvokeNetworkly = false)]
        protected int syncCurrentDirection;
        [SyncVar(shouldInvokeNetworkly = false, hook = "UpdateTexture")] //Live sync through BroadcastMethod, when first time connecting through SyncVar
        protected int syncCurrentAnimationType;
        protected EntityId entityId;
        protected double animationTimeDelay;
        protected double timeSinceLastFrame;
        protected int currentAnimationIndex;
        protected bool shouldLoopAnimation;
        protected float speed;

        public AnimatedObject(EntityId entityId)
        {
            this.entityId = entityId;
            animationsByType = new Dictionary<int, List<GameTexture>>(GameManager.Instance.animationsByEntities[entityId]);
            speed = 0.5f / 10;
            animationTimeDelay = 100;
            shouldLoopAnimation = true;
            syncCurrentDirection = (int)Direction.Down;
        }

        protected abstract void InitAnimationsList();

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            UpdateTexture();
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
            GameTexture gameTexture = animationsByType[syncCurrentAnimationType][currentAnimationIndex];
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
            return currentAnimationIndex + 1 >= animationsByType[syncCurrentAnimationType].Count;
        }

        protected void AnimationAtDir(Direction direction, int dirToAnimationIndex, bool shouldLoopAnimation)
        {
            currentAnimationIndex = 0;
            syncCurrentDirection = (int)direction;
            syncCurrentAnimationType = (int)direction + dirToAnimationIndex;
            this.shouldLoopAnimation = shouldLoopAnimation;
        }
    }
}

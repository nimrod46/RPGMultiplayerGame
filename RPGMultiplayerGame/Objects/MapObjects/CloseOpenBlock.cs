using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public abstract class CloseOpenBlock : SpecialBlock
    {
        private State currentState;

        public enum DoorAnimation
        {
            Opening,
            Open,
            Closing,
            Close,
        }

        public enum State
        {
            Opening,
            Open,
            Closing,
            Close,
        }

        public State SyncCurrentState
        {
            get => currentState; set
            {
                currentState = value;
                AnimationAtDir(Direction.Down, (int)SyncCurrentState - (int)Direction.Down, false);
                InvokeSyncVarNetworkly(nameof(SyncCurrentState), currentState);
            }
        }

        public CloseOpenBlock(Dictionary<int, List<GameTexture>> animationsByType) : base(animationsByType)
        {
            SyncCurrentState = State.Close;
        }

        public virtual void SetState(State state)
        {
            switch (state)
            {
                case State.Open:
                    SyncCurrentState = State.Open;
                    break;
                case State.Close:
                    SyncCurrentState = State.Close;
                    break;
                case State.Opening:
                    SyncCurrentState = State.Opening;
                    break;
                case State.Closing:
                    SyncCurrentState = State.Closing;
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!hasAuthority)
            {
                return;
            }

            if(SyncCurrentState == State.Opening)
            {
                if(IsLoopAnimationFinished())
                {
                    SetState(State.Open);
                }
            }
            else if(SyncCurrentState == State.Closing)
            {
                if (IsLoopAnimationFinished())
                {
                    SetState(State.Close);
                }
            }
        }

        public void Open()
        {
            SetState(State.Opening);
        }

        public virtual void Close()
        {
            SetState(State.Closing);
        }

        public override void CmdEngage(Entity initiator)
        {
            InvokeCommandMethodNetworkly(nameof(CmdEngage), initiator);
            if(!isInServer)
            {
                return;
            }

            if (SyncCurrentState == State.Close)
            {
                Open();
            }
            else if (SyncCurrentState == State.Open)
            {
                Close();
            }
        }


        public override void OnTextureIndexSet()
        {
        }
    }
}

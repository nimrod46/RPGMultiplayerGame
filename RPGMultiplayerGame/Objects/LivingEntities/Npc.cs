using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Objects.Other;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public abstract class Npc : Human
    {
        protected ComplexDialog dialog;
        protected ComplexDialog currentDialog;
        protected SimpleDialog currentSimpleDialog;
        protected Player currentInteractingPlayer;
        private Vector2 dialogOffset;

        public Npc(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont)
        {
            speed *= 0.5f;
        }

        public override void OnNameSet()
        {
            base.OnNameSet();
            dialogOffset = nameOffset + new Vector2(0, -nameFontSize.Y);
        }

        internal abstract void ChooseDialogOption(int index);

        public abstract void InteractWithPlayer(Player player);

        public virtual void StopInteractWithPlayer(Player player)
        {
            if (player.hasAuthority)
            {
                player.StopInteractingWithNpc();
                currentDialog = null;
                currentInteractingPlayer = null;
            }
            currentSimpleDialog = null;
            StopLookingAtGameObject();
        }


        protected override void LookAtGameObject(GameObject gameObject)
        {
            if (!IsLookingAtPlayer && gameObject is Player)
            {
                InteractWithPlayer(gameObject as Player);
                base.LookAtGameObject(gameObject);
            }
        }

        //[BroadcastMethod]
        protected override void StopLookingAtGameObject()
        {
            if (isInServer)
            {
                base.StopLookingAtGameObject();
            }
            if (currentInteractingPlayer != null)
            {
                StopInteractWithPlayer(currentInteractingPlayer);
            }
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            currentDialog?.DrawAt(sprite, Location + dialogOffset);
            currentSimpleDialog?.DrawAt(sprite, Location + dialogOffset);
        }
    }
}

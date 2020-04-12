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
        private Vector2 dialogOffset;

        public Npc(EntityId entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth, SpriteFont nameFont) : base(entityID, collisionOffsetX, collisionOffsetY, maxHealth, nameFont, false)
        {
            speed *= 0.5f;
        }

        public override void OnNameSet()
        {
            base.OnNameSet();
            dialogOffset = nameOffset + new Vector2(0, -nameFontSize.Y);
        }

        public abstract void InteractWithPlayer(Player player);

        internal abstract void CmdChooseDialogOption(Player player, int index);


        public abstract void CmdStopInteractWithPlayer(Player player);
       


        protected override void LookAtGameObject(GameObject gameObject)
        {
            if (gameObject is Player player)
            {
                InteractWithPlayer(player);
            }
                base.LookAtGameObject(gameObject);
        }

        protected override void StopLookingAtGameObject(GameObject gameObject)
        {
            if (isInServer)
            {
                base.StopLookingAtGameObject(gameObject);
            }
            if (gameObject is Player player)
            {
                CmdStopInteractWithPlayer(player);
            }
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            currentDialog?.DrawAt(sprite, Location + dialogOffset);
        }
    }
}

using RPGMultiplayerGame.Graphics.Ui.Dialogs;
using RPGMultiplayerGame.Graphics.Ui.QuestsObjects.Quests;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Dialogs;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Joe : MultipleInteractionNpc
    {
        public Joe() : base(GraphicManager.EntityId.Player, 0, 0, 100, GraphicManager.Instance.PlayerNameFont)
        {
            SyncName = "Joe";
            minDistanceForObjectInteraction = 40;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            dialog = new ComplexDialog(SyncName, "Hi! can you help me?", false);
            dialog
                .AddAnswerOption("Yes", "Thank You!", false)
                .AddAnswerOption("....", new QuestDialog("Lets get started", new JoeKillQuest(), "So you need to kill for me some bats", "Tell me when you are done", "Ammmm.... seams like you did not finished", "Good job!! here is your reward"))
                .AddAnswerOption("Thank you", "Be well", true);
            dialog.AddAnswerOption("No", "Ok", false);
        }
    }
}

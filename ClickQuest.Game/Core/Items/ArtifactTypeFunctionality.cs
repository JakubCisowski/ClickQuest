using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Items
{
    public class ArtifactTypeFunctionality
    {
        public ArtifactType ArtifactType { get; set; }
        public string Description { get; set; }

        public virtual bool CanBeEquipped()
        {
            return true;
        }
    }
}
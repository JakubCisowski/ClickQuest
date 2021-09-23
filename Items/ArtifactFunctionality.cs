using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public class ArtifactFunctionality
	{
		public string Name { get; set; }

		public int ArtifactSlotsRequired { get; set; } = 1;

		// Use when trying to equip an artifact to determine if it can be equipped.
		public virtual bool CanBeEquipped()
		{
			int equippedArtifactsSlots = User.Instance.CurrentHero.EquippedArtifacts.Sum(x => x.ArtifactFunctionality.ArtifactSlotsRequired);

			if (3 - equippedArtifactsSlots < ArtifactSlotsRequired)
			{
				AlertBox.Show($"This artifact cannot be equipped right now - it requires {ArtifactSlotsRequired} free slots to use.", MessageBoxButton.OK);
				return false;
			}

			return true;
		}
		
		// Use when increasing base stats.
		public virtual void OnEquip() { }
        
        // Use to decrease base stats that have previously been increased.
        public virtual void OnUnequip() { }

        // Use to deal bonus damage upon clicking.
        public virtual void OnEnemyClick() { }
        
        // Use to increase ALL damage dealt (eg. by a percentage).
        public virtual void OnDealingDamage(int baseDamage) { }

        // Use to increase poison damage dealt (eg. by a percentage).
		public virtual void OnDealingPoisonDamage(int poisonDamage) { }

		// Use to trigger on-kill effects.
		public virtual void OnKill() { }
        
        // Use to trigger region-based utility effects (eg. increased drop rate).
        public virtual void OnRegionEnter() { }
        
        // Use to revert the above utility effects.
        public virtual void OnRegionLeave() { }
	}
}
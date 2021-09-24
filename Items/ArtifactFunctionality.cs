using System.Linq;
using System.Windows;
using ClickQuest.Adventures;
using ClickQuest.Controls;
using ClickQuest.Data.GameData;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Pages;
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
			bool isFighting = GameData.CurrentPage is RegionPage or DungeonBossPage;

			if (isFighting)
			{
				AlertBox.Show($"You cannot equip artifacts while in combat.", MessageBoxButton.OK);
				return false;
			}
			
			int equippedArtifactsSlots = User.Instance.CurrentHero.EquippedArtifacts.Sum(x => x.ArtifactFunctionality.ArtifactSlotsRequired);

			string slotText = ArtifactSlotsRequired == 1 ? "slot" : "slots";

			if (3 - equippedArtifactsSlots < ArtifactSlotsRequired)
			{
				AlertBox.Show($"This artifact cannot be equipped right now - it requires {ArtifactSlotsRequired} free {slotText} to use.", MessageBoxButton.OK);
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
		public virtual void OnDealingDamage(ref int baseDamage) { }
		
		// Use to increase click damage dealt (eg. by a percentage).
		public virtual void OnDealingClickDamage(ref int clickDamage) { }

		// Use to increase poison damage dealt (eg. by a percentage).
		public virtual void OnDealingPoisonDamage(ref int poisonDamage) { }

		// Use to trigger on-kill effects.
		public virtual void OnKill() { }

		// Use to trigger region-based utility effects (eg. increased drop rate).
		public virtual void OnRegionEnter() { }

		// Use to revert the above utility effects.
		public virtual void OnRegionLeave() { }
		
		// Use for effects that trigger on experience gained (eg. bonus experience).
		public virtual void OnExperienceGained(int experienceGained) { }
		
		// Use for effects that increase blessing effectiveness.
		public virtual void OnBlessingStarted(Blessing blessing) { }
		
		// Use for effects that empower quests (eg. decrease duration).
		public virtual void OnQuestStarted(Quest quest) { }
	}
}
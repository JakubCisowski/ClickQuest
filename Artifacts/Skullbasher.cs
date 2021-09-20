using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Pages;

namespace ClickQuest.Artifacts
{
	public class Skullbasher : ArtifactFunctionality
	{
		private const int DamageDealt = 10;
		private int _clickCounter;

		public override void OnClick()
		{
			if (_clickCounter++ == 0)
			{
				return;
			}

			_clickCounter = 0;

			var monsterButton = CombatController.GetCurrentMonsterButton();
			
			monsterButton?.DealBonusArtifactDamageToMonster(DamageDealt);
		}
		
		public Skullbasher()
		{
			Name = "Skullbasher";
		}
	}
}
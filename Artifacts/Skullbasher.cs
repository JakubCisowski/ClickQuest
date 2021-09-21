using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Pages;

namespace ClickQuest.Artifacts
{
	public class Skullbasher : ArtifactFunctionality
	{
		private const int DamageDealt = 10;
		private int _clickCounter;

		public override void OnEnemyClick()
		{
			if (_clickCounter++ == 0)
			{
				return;
			}

			_clickCounter = 0;

			var monsterButton = InterfaceController.CurrentMonsterButton;
			
			monsterButton?.DealBonusArtifactDamageToMonster(DamageDealt);
		}
		
		public Skullbasher()
		{
			Name = "Skullbasher";
		}
	}
}
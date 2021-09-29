using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Extensions.Game
{
	public static class GameController
	{
		public static void OnHeroExit()
		{
			User.Instance.CurrentHero?.UpdateTimePlayed();
			User.Instance.CurrentHero?.PauseBlessing();
			User.Instance.CurrentHero?.UnequipArtfacts();
		}
	}
}
using ClickQuest.Player;

namespace ClickQuest.Extensions.GameManager
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
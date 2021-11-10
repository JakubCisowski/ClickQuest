using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;

namespace ClickQuest.Game.Core.Places
{
	public class MonsterSpawnPattern
	{
		public int MonsterId { get; set; }
		public double Frequency { get; set; }

		public Monster Monster => GameAssets.Monsters.FirstOrDefault(x => x.Id == MonsterId);
	}
}
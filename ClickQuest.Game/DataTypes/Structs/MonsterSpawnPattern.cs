using System.Linq;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Models;

namespace ClickQuest.Game.DataTypes.Structs;

public class MonsterSpawnPattern
{
	public int MonsterId { get; set; }
	public double Frequency { get; set; }

	public Monster Monster
	{
		get
		{
			return GameAssets.Monsters.FirstOrDefault(x => x.Id == MonsterId);
		}
	}
}
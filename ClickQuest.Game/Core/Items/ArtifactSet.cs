using System.Collections.Generic;

namespace ClickQuest.Game.Core.Items
{
	public class ArtifactSet
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<int> ArtifactIds{ get; set; }

		public ArtifactSet()
		{
			ArtifactIds = new List<int>();
		}
	}
}
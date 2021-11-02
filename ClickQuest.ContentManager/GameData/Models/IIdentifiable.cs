using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickQuest.ContentManager.GameData.Models
{
	public interface IIdentifiable 
	{
		int Id { get; set; }
		string Name { get; set; }
	}
}

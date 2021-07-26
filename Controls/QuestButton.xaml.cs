using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Adventures;
using ClickQuest.Player;

namespace ClickQuest.Controls
{
	public partial class QuestButton : UserControl, INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;

		private readonly Quest _quest;

		public QuestButton(Quest quest)
		{
			InitializeComponent();

			_quest = quest;
			DataContext = _quest;
		}

		private void QuestButton_Click(object sender, RoutedEventArgs e)
		{
			// Start this quest (if another one isnt currently assigned).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
			{
				_quest.StartQuest();
			}
		}
	}
}
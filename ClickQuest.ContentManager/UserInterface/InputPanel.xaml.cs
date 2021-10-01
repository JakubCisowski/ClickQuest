using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface
{
	// zrobić jakiś osobny control typu StackPanel i tam, w zależności od datacontext (typu obiektu i jego pól) generować textboxy itp
	public partial class InputPanel : UserControl
	{
		private object _dataContext;
		
		public InputPanel()
		{
			InitializeComponent();
		}

		public void RefreshTextBoxes()
		{
			// todo: jak to zrobic programistycznie
			var idBox = new TextBox()
				{ };
			
		}
	}
}
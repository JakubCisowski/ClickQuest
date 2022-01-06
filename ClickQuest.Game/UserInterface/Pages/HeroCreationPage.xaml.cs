using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.UserInterface.Pages;

public partial class HeroCreationPage : Page
{
	public HeroCreationPage()
	{
		InitializeComponent();

		HeroClassBox.ItemsSource = Enum.GetValues(typeof(HeroClass)).Cast<HeroClass>().Where(x => x != HeroClass.All);
		HeroRaceBox.ItemsSource = Enum.GetValues(typeof(HeroRace)).Cast<HeroRace>();

		HeroNameBox.Focus();
	}

	public void CreateHeroButton_Click(object sender, RoutedEventArgs e)
	{
		var isHeroClassSelected = HeroClassBox.SelectedItems.Count > 0;
		var isHeroRaceSelected = HeroRaceBox.SelectedItems.Count > 0;

		if (isHeroClassSelected && isHeroRaceSelected)
		{
			var isHeroNameAlphanumericWithSpaces = HeroNameBox.Text.Trim().All(x => char.IsLetterOrDigit(x) || x == ' ');
			var isHeroNameLengthCorrect = HeroNameBox.Text.Trim().Length is > 0 and <= 15;

			if (!isHeroNameAlphanumericWithSpaces || !isHeroNameLengthCorrect)
			{
				AlertBox.Show("Hero name can contain up to 15 characters.\nValid characters: A-Z, a-z, 0-9, space.", MessageBoxButton.OK);
				return;
			}

			CreateHero();

			GameAssets.RefreshPages();

			InterfaceHelper.ChangePage(GameAssets.Pages["Town"], "Town");
		}
		else
		{
			AlertBox.Show("You must choose both race and class for your hero!", MessageBoxButton.OK);
		}
	}

	private void CreateHero()
	{
		var newHero = new Hero((HeroClass)Enum.Parse(typeof(HeroClass), HeroClassBox.SelectedValue.ToString()), (HeroRace)Enum.Parse(typeof(HeroRace), HeroRaceBox.SelectedValue.ToString()), HeroNameBox.Text);

		User.Instance.Heroes.Add(newHero);

		// Select current hero from entity database, because variable 'hero' doesn't represent the same hero that was loaded from entity in LoadGame().
		User.Instance.CurrentHero = User.Instance.Heroes.FirstOrDefault(x => x.Id == newHero.Id);

		User.Instance.CurrentHero?.Specializations.UpdateBuffs();

		newHero.SessionStartDate = DateTime.Now;

		SeedArtifacts();
	}

	// [PRERELEASE]
	private static void SeedArtifacts()
	{
		foreach (var artifact in GameAssets.Artifacts)
		{
			artifact.CreateMythicTag("FunctionSeedingArtifacts");

			CollectionsHelper.AddItemToCollection(artifact, User.Instance.CurrentHero.Artifacts);
		}
	}

	public void NoButton_Click(object sender, RoutedEventArgs e)
	{
		InterfaceHelper.ChangePage(GameAssets.Pages["MainMenu"], "");
	}
}
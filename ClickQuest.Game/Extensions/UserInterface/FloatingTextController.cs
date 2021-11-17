using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;
using Colors = ClickQuest.Game.Extensions.UserInterface.ColorsController;

namespace ClickQuest.Game.Extensions.UserInterface
{
	public static class FloatingTextController
	{
		public const int IngotDungeonKeyOffset = 22;
		public static Point GoldPositionPoint = new Point() { X = 193, Y = 232 };
		public static Point ExperiencePositionPoint = new Point() { X = 200, Y = 50 };
		public static Point IngotGeneralPositionPoint = new Point() { X = 1, Y = 257 };
		public static Point DungeonKeyGeneralPositionPoint = new Point() { X = 265, Y = 257 };


		public static Point IngotFinePositionPoint = new Point() { X = IngotGeneralPositionPoint.X, Y = IngotGeneralPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point IngotSuperiorPositionPoint = new Point() { X = IngotGeneralPositionPoint.X, Y = IngotFinePositionPoint.Y + IngotDungeonKeyOffset };
		public static Point IngotExceptionalPositionPoint = new Point() { X = IngotGeneralPositionPoint.X, Y = IngotSuperiorPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point IngotMasterworkPositionPoint = new Point() { X = IngotGeneralPositionPoint.X, Y = IngotExceptionalPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point IngotMythicPositionPoint = new Point() { X = IngotGeneralPositionPoint.X, Y = IngotMasterworkPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point DungeonKeyFinePositionPoint = new Point() { X = DungeonKeyGeneralPositionPoint.X, Y = DungeonKeyGeneralPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point DungeonKeySuperiorPositionPoint = new Point() { X = DungeonKeyGeneralPositionPoint.X, Y = DungeonKeyFinePositionPoint.Y + IngotDungeonKeyOffset };
		public static Point DungeonKeyExceptionalPositionPoint = new Point() { X = DungeonKeyGeneralPositionPoint.X, Y = DungeonKeySuperiorPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point DungeonKeyMasterworkPositionPoint = new Point() { X = DungeonKeyGeneralPositionPoint.X, Y = DungeonKeyExceptionalPositionPoint.Y + IngotDungeonKeyOffset };
		public static Point DungeonKeyMythicPositionPoint = new Point() { X = DungeonKeyGeneralPositionPoint.X, Y = DungeonKeyMasterworkPositionPoint.Y + IngotDungeonKeyOffset };


		public static Point EnemyCenterPoint = new Point() { X = 683, Y = 384 };
		public static Point LootEndPositionPoint = new Point() { X = EnemyCenterPoint.X + 200, Y = EnemyCenterPoint.Y - 200 };

		public static DoubleAnimation CreateTextOpacityAnimation(int durationInSeconds)
		{
			var textVisibilityAnimation = new DoubleAnimation
			{
				Name = "ClickTextVisibilityAnimation",
				Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
				From = 1.0,
				To = 0.0
			};

			return textVisibilityAnimation;
		}

		public static (double X, double Y) RandomizeFloatingTextPathPosition(Point mousePosition, double canvasActualWidth, double canvasActualHeight, int maximumPositionOffset)
		{
			double randomizedPositionX = mousePosition.X + RNG.Next(-maximumPositionOffset, maximumPositionOffset + 1);
			double randomizedPositionY = mousePosition.Y + RNG.Next(-maximumPositionOffset, maximumPositionOffset + 1);

			// Clamp the positions, so that floating numbers do not follow cursor when user hovers over stats panel, equipment panel, etc.
			randomizedPositionX = Math.Clamp(randomizedPositionX, -90, canvasActualWidth + 45);
			randomizedPositionY = Math.Clamp(randomizedPositionY, -115, canvasActualHeight + 90);

			return (randomizedPositionX, randomizedPositionY);
		}

		public static Border CreateFloatingTextCombatBorder(int damageValue, DamageType damageType)
		{
			var border = new Border
			{
				CornerRadius = new CornerRadius(20),
				BorderThickness = new Thickness(5),
				Padding = new Thickness(2),
				IsHitTestVisible = false
			};

			var stackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};

			var textBrush = Colors.GetDamageTypeColor(damageType);

			var icon = GetFloatingCombatIcon(damageType);
			icon.Foreground = textBrush;
			icon.VerticalAlignment = VerticalAlignment.Center;

			stackPanel.Children.Add(icon);

			var text = new TextBlock
			{
				Text = damageValue.ToString(),
				Foreground = textBrush,
				FontSize = 28,
				VerticalAlignment = VerticalAlignment.Center
			};

			if (damageType == DamageType.Critical)
			{
				text.FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");
			}

			stackPanel.Children.Add(text);

			border.Child = stackPanel;

			return border;
		}

		public static Border CreateFloatingTextUtilityBorder(string value, SolidColorBrush textBrush)
		{
			var border = new Border
			{
				CornerRadius = new CornerRadius(20),
				BorderThickness = new Thickness(5),
				Padding = new Thickness(2),
				IsHitTestVisible = false
			};

			var stackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};

			var text = new TextBlock
			{
				Text = value,
				Foreground = textBrush,
				FontSize = 28,
				VerticalAlignment = VerticalAlignment.Center
			};

			stackPanel.Children.Add(text);

			border.Child = stackPanel;

			return border;
		}

		public static Border CreateFloatingTextLootBorder(Item item, int quantity = 1)
		{
			var border = new Border
			{
				CornerRadius = new CornerRadius(20),
				BorderThickness = new Thickness(5),
				Padding = new Thickness(2),
				IsHitTestVisible = false
			};

			var stackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};

			var itemBlock = new TextBlock
			{
				Text = quantity + "x " + item.Name,
				Foreground = ColorsController.GetRarityColor(item.Rarity),
				FontSize = 28,
				VerticalAlignment = VerticalAlignment.Center
			};


			stackPanel.Children.Add(itemBlock);

			border.Child = stackPanel;

			return border;
		}

		public static PackIcon GetFloatingCombatIcon(DamageType damageType)
		{
			var icon = new PackIcon
			{
				Width = 28,
				Height = 28,
				VerticalAlignment = VerticalAlignment.Center
			};

			switch (damageType)
			{
				case DamageType.Normal:
					icon.Kind = PackIconKind.CursorDefault;
					break;

				case DamageType.Critical:
					icon.Kind = PackIconKind.CursorDefaultClick;
					break;

				case DamageType.Poison:
					icon.Kind = PackIconKind.Water;
					break;

				case DamageType.Aura:
					icon.Kind = PackIconKind.Brightness5;
					break;

				case DamageType.OnHit:
					icon.Kind = PackIconKind.CursorDefaultOutline;
					break;

				case DamageType.Artifact:
					icon.Kind = PackIconKind.DiamondStone;
					break;
			}

			return icon;
		}

		public static Point GetIngotRarityPosition(Rarity rarity)
		{
			Point point;

			switch (rarity)
			{
				case Rarity.General:
					point = IngotGeneralPositionPoint;
					break;

				case Rarity.Fine:
					point = IngotFinePositionPoint;
					break;

				case Rarity.Superior:
					point = IngotSuperiorPositionPoint;
					break;

				case Rarity.Exceptional:
					point = IngotExceptionalPositionPoint;
					break;

				case Rarity.Masterwork:
					point = IngotMasterworkPositionPoint;
					break;

				case Rarity.Mythic:
					point = IngotMythicPositionPoint;
					break;
			}

			return point;
		}

		public static Point GetDungeonKeyRarityPosition(Rarity rarity)
		{
			Point point;

			switch (rarity)
			{
				case Rarity.General:
					point = DungeonKeyGeneralPositionPoint;
					break;

				case Rarity.Fine:
					point = DungeonKeyFinePositionPoint;
					break;

				case Rarity.Superior:
					point = DungeonKeySuperiorPositionPoint;
					break;

				case Rarity.Exceptional:
					point = DungeonKeyExceptionalPositionPoint;
					break;

				case Rarity.Masterwork:
					point = DungeonKeyMasterworkPositionPoint;
					break;

				case Rarity.Mythic:
					point = DungeonKeyMythicPositionPoint;
					break;
			}

			return point;
		}
	}
}
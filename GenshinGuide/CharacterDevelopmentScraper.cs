using System;
using System.Drawing;

namespace GenshinGuide
{
	public struct CharacterDevelopment
	{
		public int name;
		public int count;
	}

	public static class CharacterDevelopmentScraper
	{
		public static void Scan_Items()
		{
			int maxColumns = 7;
			int maxRows = 4;
			int currentColumn = 0;
			int currentRow = 0;

			// offset used to move mouse to other artifacts
			int xOffset = Convert.ToInt32(Navigation.GetArea().right * ((Double)12.25 / 160));
			int yOffset = Convert.ToInt32(Navigation.GetArea().bottom * ((Double)14.5 / 90));

			// Scan first Item to check if empty
			CharacterDevelopment item = Scan_Item();
			currentColumn++;

			// Keep scanning while not repeating any items names
			while (true)
			{

			}

			// Scan the last of the inventory items

			// stop when repeated again or until max of 28
		}

		public static CharacterDevelopment Scan_Item()
		{
			CharacterDevelopment item = new CharacterDevelopment();
			int xOffset = 10; int yOffset = 7;
			int width = 325; int height = 560;

			// Grab item portrait on Right
			Double itemLocation_X = Navigation.GetArea().right * (108 / (Double)160);
			Double itemLocation_Y = Navigation.GetArea().bottom * (10 / (Double)90);

			Bitmap bm = new Bitmap(width - (xOffset*2), 7);
			Graphics g = Graphics.FromImage(bm);
			int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(itemLocation_X);
			int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(itemLocation_Y);
			g.CopyFromScreen(screenLocation_X + xOffset, screenLocation_Y, 0, 0, bm.Size);

			// Scan Item Name

			// Scan Item Number

			return item;
		}
	}
}

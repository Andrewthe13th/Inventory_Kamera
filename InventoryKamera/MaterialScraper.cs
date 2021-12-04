using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace InventoryKamera
{
    public struct Material
    {
        public int name;
        public int count;

        public Material(int _name, int _count)
        {
            name = _name;
            count = _count;
        }

    }

    public enum InventorySection { 
        CharacterDevelopmentItems,
        Food,
        Materials,
        Furnishings,
    }
        

    public static class MaterialScraper
    {
        public static List<Material> Scan_Materials(InventorySection section)
        {
            int maxColumns = 7;
            int maxRows = 4;
            int currentColumn = 0;
            int currentRow = 0;
            int scrollCount = 0;
            int count = 0;

            Double materialLocation_X = (Double)Navigation.GetArea().right * ((Double)21 / (Double)160);
            Double materialLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)14 / (Double)90);
            // offset used to move mouse to other artifacts
            int xOffset = Convert.ToInt32((Double)Navigation.GetArea().right * ((Double)12.25 / (Double)160));
            int yOffset = Convert.ToInt32((Double)Navigation.GetArea().bottom * ((Double)14.5 / (Double)90));

            // Scan first Item to check if empty
            List<Material> materials = new List<Material>();
            Material material = new Material(0,0);
            Material previousMaterial = new Material(-1,-1);
            Material previousRowMaterial = new Material(-1,-1);

            // Keep scanning while not repeating any items names
            while (true)
            {
                // Select Material
                Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(materialLocation_X) + (xOffset * (count % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(materialLocation_Y));
                Navigation.sim.Mouse.LeftButtonClick();
                Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

                // Scan Material Name
                material.name = ScanMaterialName(section);
                material.count = 0;

                // Save the first name of each column to check for repeats
                if(currentColumn == 0)
                {
                    if (material.name == previousRowMaterial.name)
                        break;
                    previousRowMaterial.name = material.name;
                }


                // Check if new material has been found
                if(material.name == previousMaterial.name)
                {
                    break;
                }
                else
                {
                    if (material.name != -1)
                    {
                        // Scan Material Number
                        material.count = ScanMaterialCount(currentColumn, currentRow, scrollCount);
                        materials.Add(material);
                    }
                    previousMaterial.name = material.name;
                }

                count++;
                currentColumn++;

                // reach end of row
                if (currentColumn == maxColumns)
                {
                    // reset mouse pointer and scroll down weapon list
                    currentColumn = 0;
                    scrollCount++;

                    // scroll down
                    for (int k = 0; k < 10; k++)
                    {
                        Navigation.sim.Mouse.VerticalScroll(-1);
                        // skip a scroll
                        if ((k == 7) && ((scrollCount % 3) == 0))
                        {
                            k++;
                            if (scrollCount % 9 == 0)
                            {
                                if (scrollCount == 18)
                                {
                                    scrollCount = 0;
                                }
                                else
                                {
                                    Navigation.sim.Mouse.VerticalScroll(-1);
                                    Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
                                }
                            }
                        }
                        Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
                    }
                    Navigation.SystemRandomWait(Navigation.Speed.Fast);
                }
            }

            // scroll down as much as possible
            for (int i = 0; i < 20; i++)
            {
                Navigation.sim.Mouse.VerticalScroll(-1);
                Navigation.SystemRandomWait(Navigation.Speed.InventoryScroll);
            }
            Navigation.SystemRandomWait(Navigation.Speed.Normal);
            Navigation.SystemRandomWait(Navigation.Speed.Normal);

            // Scan the last of the material items and stop when repeated again or until max of 28
            int startPostion = 1;
            currentColumn = 0;
            currentRow = 0;
            for (int i = startPostion; i < 5; i++)
            {
                for (int k = 0; k < maxColumns; k++)
                {
                    // Select Material
                    Navigation.SetCursorPos(Navigation.GetPosition().left + Convert.ToInt32(materialLocation_X) + (xOffset * (k % maxColumns)), Navigation.GetPosition().top + Convert.ToInt32(materialLocation_Y) + (yOffset * (i % (maxRows + 1))));
                    Navigation.sim.Mouse.LeftButtonClick();
                    Navigation.SystemRandomWait(Navigation.Speed.SelectNextInventoryItem);

                    // Scan Material Name
                    material.name = ScanMaterialName(section);
                    material.count = 0;

                    // Check if new material has been found
                    if (material.name == previousMaterial.name || material.name == -1)
                    {
                        break;
                    }
                    else
                    {
                        // Scan material number
                        material.count = ScanMaterialCountEnd(k, i-1);
                        if(material.count != -1)
                        {
                            materials.Add(material);
                            previousMaterial.name = material.name;
                        }
                    }

                    count++;
                }
            }

            return materials;
        }

        public static int ScanMaterialName(InventorySection section)
        {
            int xOffset = 10;
            int width = 325;

            // Grab item portrait on Right
            Double itemLocation_X = (Double)Navigation.GetArea().right * ((Double)108 / (Double)160);
            Double itemLocation_Y = (Double)Navigation.GetArea().bottom * ((Double)10 / (Double)90);
            
            Bitmap bm = new Bitmap(width - (xOffset*2), 37);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(itemLocation_X);
            int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(itemLocation_Y);
            g.CopyFromScreen(screenLocation_X + xOffset, screenLocation_Y, 0, 0, bm.Size);

            // Alter Image 
            Scraper.SetGamma(0.2, 0.2, 0.2, ref bm);
            Scraper.SetGrayscale(ref bm);
            Scraper.SetInvert(ref bm);

            string text = Scraper.AnalyzeText(bm);
            text = Regex.Replace(text, @"[\W]", "");
            text = text.ToLower();

            //UI
            UserInterface.Reset_Character();
            UserInterface.SetCharacter_NameAndElement(bm, text, "None");

            text = text.ToLower();

            if (section == InventorySection.CharacterDevelopmentItems)
                return Scraper.GetCharacterDevelopmentNameCode(text);

            if (section == InventorySection.Materials)
                return Scraper.GetMaterialCode(text);

            return -1;
        }

        public static int ScanMaterialCount(int column, int row, int additionalOffset = 0)
        {
            int[] scrollOffset = { 12, 7, 3, 16, 9, 5, 15, 12, 10, 8, 2, -2, 7, 4, 0, 10, 7, 10};
            //                      0   1  2  3  4  5   6   7   8  9 10  11 12  13 14  15  16  17 
            // get the picture
            int left = 139; int top = 70;
            int xOffset = 17; int yOffset = 10;
            int width = 81; int height = 20;

            // Grab item portrait on Right
            Double itemCount_X = 0;
            Double itemCount_Y = 82;

            Bitmap bm = new Bitmap(80, height);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(itemCount_X);
            int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(itemCount_Y);
            g.CopyFromScreen(left + screenLocation_X + ((column * width) + (column * xOffset)), top + screenLocation_Y + ((row * height) + (row * yOffset) + (scrollOffset[additionalOffset % 18])), 0, 0, bm.Size);

            // Image Processing
            Scraper.SetGrayscale(ref bm);
            Scraper.SetContrast(20.0, ref bm);

            string old_text = Scraper.AnalyzeText_SingleWord(bm);

            //replace T with 7
            old_text = old_text.Replace('T', '7');

            string text = Regex.Replace(old_text, @"[^\d]", "");
            

            // filter out
            int count = -1;
            if(int.TryParse(text,out count)){
                UserInterface.SetCharacter_Level(bm, count);
                return count;
            }
            else
            {
                UserInterface.SetCharacter_Level(bm, count);
                return 0;
            }
        }

        public static int ScanMaterialCountEnd(int column, int row)
        {
            // get the picture
            int left = 139; int top = 70;
            int xOffset = 17; int yOffset = 117;
            int width = 81; int height = 20;

            // Grab item portrait on Right
            Double itemCount_X = 0;
            Double itemCount_Y = 188;

            Bitmap bm = new Bitmap(80, height);
            Graphics g = Graphics.FromImage(bm);
            int screenLocation_X = Navigation.GetPosition().left + Convert.ToInt32(itemCount_X);
            int screenLocation_Y = Navigation.GetPosition().top + Convert.ToInt32(itemCount_Y);
            g.CopyFromScreen(left + screenLocation_X + ((column * width) + (column * xOffset)), top + screenLocation_Y + (row * yOffset) , 0, 0, bm.Size);

            // Image Processing
            Scraper.SetGrayscale(ref bm);

            string old_text = Scraper.AnalyzeText_SingleWord(bm);

            //replace T with 7
            old_text = old_text.Replace('T', '7');

            string text = Regex.Replace(old_text, @"[^\d]", "");

            // filter out
            int count = -1;
            if (int.TryParse(text, out count))
            {
                UserInterface.SetCharacter_Level(bm, count);
                return count;
            }
            else
            {
                UserInterface.SetCharacter_Level(bm, count);
                return -1;
            }
        }
    }
}

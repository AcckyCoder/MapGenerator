using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapGeneratorUtilite
{
    class Program
    {
        static string[] textures = new[]
            {
                "\"City.png\"", "\"coal.jpg\"", "\"grass.jpg\"", "\"rocks.jpg\"", "\"trees.jpg\"", "\"field.jpg\"", "\"gas.jpg\""
            };

        static int fieldSizeX = 30, fieldSizeY = 15;
        static int cities = 10, tree = 70, coal = 40, gas = 30, wheat = 100, rock = 60, grass = 140;
        static List<Cell> cells;


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to M.A.N.E.D. Map Generator");
            Console.WriteLine("© AcckyCoder, 2014");
            Console.WriteLine();

            cities = tree = coal = gas = wheat = rock = grass = 0;

            do
            {
                do
                {

                    GetCount(out fieldSizeX, "field wight");
                    GetCount(out fieldSizeY, "field height");


                    GetCount(out cities, "cities");
                    GetCount(out tree, "trees");
                    GetCount(out coal, "coals");
                    GetCount(out gas, "gas");
                    GetCount(out wheat, "wheats");
                    GetCount(out rock, "rocks");
                    GetCount(out grass, "grass");

                    int fieldCount = fieldSizeX * fieldSizeY;
                    int summ = (cities + tree + coal + gas + wheat + rock + grass);
                    if (fieldCount < summ)
                    {
                        Console.WriteLine("You want too much objects. There is no space to place it");
                        Console.WriteLine("Map generation will start again");
                    }
                    else
                    {
                        if (fieldCount > summ)
                        {
                            Console.WriteLine("There was not enoug field. I will add some grass for you.");
                            grass += (fieldCount - summ);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                } while (true);

                Console.WriteLine();
                Console.WriteLine("There is statistic about your map.");
                Console.WriteLine("Field size = " + fieldSizeX*fieldSizeY + " fields");
                Console.WriteLine("Field dimension = " + fieldSizeX + "X" + fieldSizeY );
                Console.WriteLine(string.Format("Cities = {0}\nTrees = {1}\nCoals = {2}\nGas = {3}\nWheat = {4}\nRocks = {5}\nGrass = {6}", cities, tree, coal, gas, wheat, rock, grass));

                Console.WriteLine();
                Console.WriteLine("Do you like it?");
                Console.WriteLine("Press ENTER to create map or press BACKSPACE to enter new parameters");

                ConsoleKeyInfo k = Console.ReadKey();

                if (k.Key != ConsoleKey.Backspace)
                {
                    break;
                }
            } while (true);

            Console.WriteLine("Good. Let's start generate a map");

            GenerateMap(fieldSizeX * fieldSizeY);

            Console.WriteLine("Now saving file...");

            SaveFile();

            Console.WriteLine("We save file map.json and will open it after close the programm.");

            Console.WriteLine("Thanks for using our Map Generator");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Process.Start("map.json");
        }

        private static void SaveFile()
        {
            using (FileStream fs = new FileStream("map.json", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("var map = [");

                    string[] strings = new string[cells.Count];

                    for (int i = 0; i < cells.Count; i++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("{\n\t\"type\": " + cells[i].type + ",");
                        sb.Append("\n\t\"texture\": " + cells[i].Texture + ",");
                        sb.Append("\n\t\"res_cnt\": " + cells[i].res_cnt + ",");
                        sb.Append("\n\t\"city\": " + cells[i].city +"\n}");
                        strings[i] = sb.ToString();

                    }

                    string s = string.Join(",\n", strings);
                    sw.WriteLine(s);

                    sw.WriteLine("]");
                }
            }
        }

        private static void GenerateMap(int fieldsCount)
        {
            cells = new List<Cell>(fieldsCount);
            for (int i = 0; i < fieldsCount; i++)
            {
                cells.Add(new Cell());
            }

            Random r = new Random();

            GenerateTexture(fieldsCount, cities, cells, r, 0);
            Console.WriteLine("Cities generated...");
            GenerateTexture(fieldsCount, tree, cells, r,1);
            Console.WriteLine("Trees generated...");
            GenerateTexture(fieldsCount, coal, cells, r,2);
            Console.WriteLine("Coal generated...");
            GenerateTexture(fieldsCount, gas, cells, r,3);
            Console.WriteLine("Gas generated...");
            GenerateTexture(fieldsCount, wheat, cells, r,4);
            Console.WriteLine("Wheat generated...");
            GenerateTexture(fieldsCount, rock, cells, r,5);
            Console.WriteLine("Rocks generated...");
            GenerateTexture(fieldsCount, grass, cells, r,6);
            Console.WriteLine("Grass generated...");
        }

        private static void GenerateTexture(int fieldsCount, int count, List<Cell> cells, Random r, int index)
        {
            for (int i = 0; i < count; i++)
            {
                int k;
                do
                {
                    k = r.Next(0, fieldsCount);
                } while (cells[k].Texture != string.Empty);
                cells[k].Texture = textures[index];
            }
            Thread.Sleep(count);
        }

        private static void GetCount(out int objects, string name)
        {
            do
            {
                Console.Write(string.Format("Enter {0} count: ", name));
                if (!Int32.TryParse(Console.ReadLine(), out objects))
                {
                    Console.WriteLine("Wrong number. Try agian");
                }
                else
                {
                    break;
                }
            } while (true);

        }

        class Cell
        {
            readonly public string type = "0";
            public string Texture = string.Empty;
            readonly public string res_cnt = "0";
            readonly public string city = "0";
        }
    }
}

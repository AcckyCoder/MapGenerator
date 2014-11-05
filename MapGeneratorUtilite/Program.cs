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
                "\"City.png\"","\"trees.jpg\"", "\"coal.jpg\"","\"gas.jpg\"" ,"\"wheat.jpg\"", "\"rocks.jpg\"", "\"grass.jpg\"" 
            };

        enum Textures {
           City, Tree, Coal, Gas, Wheat, Rock, Grass };

        static int fieldSizeX = 30, fieldSizeY = 15;
        static int cities = 10, tree = 70, coal = 40, gas = 30, wheat = 100, rock = 60, grass = 140;
        static List<List<Cell>> cells;


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

                    int fieldCount = fieldSizeX * fieldSizeY;
                    int summ = (cities + tree + coal + gas + wheat + rock);
                    if (fieldCount <= summ)
                    {
                        Console.WriteLine("You want too much objects. There is no space for grass");
                        Console.WriteLine("Map generation will start again");
                    }
                    else
                    {
                        grass = fieldCount - summ;
                        break;
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

            GenerateMap();

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

                    List<List<string>> strings = new List<List<string>>();

                    for (int i = 0; i < fieldSizeX; i++)
                    {
                        strings.Add(new List<string>());
                        for (int j = 0; j < fieldSizeY; j++)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{\n\t\"type\": " + cells[i][j].type + ",");
                            sb.Append("\n\t\"texture\": " + cells[i][j].Texture + ",");
                            sb.Append("\n\t\"res_cnt\": " + cells[i][j].res_cnt + ",");
                            sb.Append("\n\t\"city\": " + cells[i][j].city + "\n}");
                            strings[i].Add(sb.ToString());
                        }
                        if(i!= fieldSizeX-1)
                            strings[i][fieldSizeY-1] = strings[i].Last() + ",\n";
                    }

                    StringBuilder sb2 = new StringBuilder();
                    foreach (List<string> s in strings)
                    {
                        sb2.Append(string.Join(",\n", s));
                    }

                    sw.WriteLine(sb2.ToString());

                    sw.WriteLine("]");
                }
            }
        }

        private static void GenerateMap()
        {
            cells = new List<List<Cell>>();
            for (int i = 0; i < fieldSizeX; i++)
            {
                cells.Add(new List<Cell>());
                for (int j = 0; j < fieldSizeY; j++)
                {
                    cells[i].Add(new Cell());
                }
            }

            Random r = new Random();

            GenerateTexture(cities,  r, Textures.City);
            Console.WriteLine("Cities generated...");
            GenerateTexture( tree, r,1);
            Console.WriteLine("Trees generated...");
            GenerateTexture( coal,  r,Textures.Coal);
            Console.WriteLine("Coal generated...");
            GenerateTexture( gas,  r, Textures.Gas);
            Console.WriteLine("Gas generated...");
            GenerateTexture(wheat,  r,Textures.Wheat);
            Console.WriteLine("Wheat generated...");
            GenerateTexture( rock,  r,5);
            Console.WriteLine("Rocks generated...");
            GenerateTexture( grass, r,6);
            Console.WriteLine("Grass generated...");
        }

        private static void GenerateTexture(int count, Random r, int index)
        {
            for (int i = 0; i < count; i++)
            {
                int k, l;
                do
                {
                    k = r.Next(0, fieldSizeX);
                    l = r.Next(0, fieldSizeY);
                } while (cells[k][l].Texture != string.Empty);
                cells[k][l].Texture = textures[index];
            }
            Thread.Sleep(count);
        }


        private static void GenerateTexture(int count, Random r, Textures type)
        {
            int fieldsCount = fieldSizeX * fieldSizeY;
            for (int i = 0; i < count; i++)
            {
                int k,l;

                int ind = (int)type;

                do
                {
                    k = r.Next(0, fieldSizeX);
                    l = r.Next(0, fieldSizeY);
                    if (cells[k][l].Texture != string.Empty)
                        continue;

                    if (k > 0 && k < fieldSizeX - 1)
                    {
                        if (cells[k - 1][l].Texture == textures[ind])
                            continue;
                        if (cells[k + 1][l].Texture == textures[ind])
                            continue;
                    }
                    if(l > 0 && l < fieldSizeY - 1)
                    {
                        if (cells[k][l-1].Texture == textures[ind])
                            continue;
                        if (cells[k][l + 1].Texture == textures[ind])
                            continue;

                    }
                    if (k>0 && l >0 && l<fieldSizeY-1)
                    {
                        if (cells[k-1][l - 1].Texture == textures[ind])
                            continue;
                        if (cells[k-1][l + 1].Texture == textures[ind])
                            continue;

                    }

                    break;
                } while (true);
                cells[k][l].Texture = textures[ind];
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

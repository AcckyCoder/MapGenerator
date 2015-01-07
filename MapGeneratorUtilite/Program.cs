using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapGeneratorUtilite
{
    class Program
    {
        static string[] textures = new[]
            {
                "\"trees.jpg\"", "\"coal.jpg\"","\"gas.jpg\"" ,"\"wheat.jpg\"", "\"rocks.jpg\"", "\"grass.jpg\"", "\"City.png\""
            };

        enum Type
        {
            recource, city, production, road
        };

        enum RecourceType
        {
            tree, coal, gas, wheat, rock, grass, city
        };

        static int fieldSizeX = 30, fieldSizeY = 15;
        static int cities = 10, tree = 70, coal = 40, gas = 30, wheat = 100, rock = 60, grass = 140;
        static List<List<Cell>> cells;
        static List<string> citiesNames = new List<string>();


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to M.A.N.E.D. Map Generator");
            Console.WriteLine("© AcckyCoder, 2014");
            Console.WriteLine();

            //cities = tree = coal = gas = wheat = rock = grass = 0;

            do
            {
                citiesNames = new List<string> { "Винница", "Днепропетровск", "Донецк", "Житомир", "Запорожье", "Ивано-Франковск", "Киев", "Кировоград", "Луганск", "Луцк", "Львов", "Николаев", "Одесса", "Полтава", "Ровно", "Сумы", "Тернополь", "Ужгород", "Харьков", "Херсон", "Хмельницкий", "Черкассы", "Чернигов", "Черновцы" };

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
                Console.WriteLine("Field size = " + fieldSizeX * fieldSizeY + " fields");
                Console.WriteLine("Field dimension = " + fieldSizeX + "X" + fieldSizeY);
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

                    //List<List<string>> strings = new List<List<string>>();

                    //for (int i = 0; i < fieldSizeX; i++)
                    //{
                    //    strings.Add(new List<string>());
                    //    for (int j = 0; j < fieldSizeY; j++)
                    //    {
                    //        StringBuilder sb = new StringBuilder();
                    //        sb.Append("{\n\t\"type\": " + cells[i][j].type + ",");
                    //        sb.Append("\n\t\"texture\": " + cells[i][j].texture + ",");
                    //        sb.Append("\n\t\"res_cnt\": " + cells[i][j].res_cnt + ",");
                    //        sb.Append("\n\t\"recovery\": " + cells[i][j].recovery + ",");
                    //        sb.Append("\n\t\"owner\": " + cells[i][j].owner + ",");
                    //        sb.Append("\n\t\"production\": " + cells[i][j].production + ",");
                    //        sb.Append("\n\t\"level\": " + cells[i][j].level + ",");
                    //        sb.Append("\n\t\"cityname\": " + cells[i][j].CityName + "\n}");


                    //        strings[i].Add(sb.ToString());
                    //    }
                    //    if (i != fieldSizeX - 1)
                    //        strings[i][fieldSizeY - 1] = strings[i].Last() + ",\n";
                    //}



                    //StringBuilder sb2 = new StringBuilder();
                    //foreach (List<string> s in strings)
                    //{
                    //    sb2.Append(string.Join(",\n", s));
                    //}


                    List<string> param = new List<string>();
                    List<string> strings = new List<string>();

                    foreach (List<Cell> list in cells)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Cell cell = list[i];
                            param.Clear();
                            foreach (FieldInfo field in cell.GetType().GetFields())
                            {
                                param.Add("\n\t\"" + field.Name + "\": " + field.GetValue(cell).ToString());
                            }
                            strings.Add("{" + string.Join(",", param) + "\n},");
                        }
                    }

                    strings[strings.Count-1] = strings.Last().Remove(strings.Last().Length - 1);

                    foreach (string s in strings)
                    {
                        sw.WriteLine(s);
                    }

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

            generateCities(cities, r);
            Console.WriteLine("Cities generated...");
            GenerateRecources(tree, r, RecourceType.tree);
            Console.WriteLine("Trees generated...");
            GenerateRecources(coal, r, RecourceType.coal);
            Console.WriteLine("Coal generated...");
            GenerateRecources(gas, r, RecourceType.gas);
            Console.WriteLine("Gas generated...");
            GenerateRecources(wheat, r, RecourceType.wheat);
            Console.WriteLine("Wheat generated...");
            GenerateRecources(rock, r, RecourceType.rock);
            Console.WriteLine("Rocks generated...");
            GenerateGrass(grass, r);
            Console.WriteLine("Grass generated...");
        }

        private static void generateCities(int count, Random r)
        {
            int fieldsCount = fieldSizeX * fieldSizeY;
            for (int i = 0; i < count; i++)
            {
                int k, l;

                int ind = 6;

                do
                {
                    k = r.Next(0, fieldSizeX);
                    l = r.Next(0, fieldSizeY);
                    if (cells[k][l].texture != string.Empty)
                        continue;

                    if (k == 0 || l == 0 || k == fieldSizeX - 1 || k == fieldSizeY - 1)
                    {
                        continue;
                    }

                    if (k > 0 && k < fieldSizeX - 1)
                    {
                        if (cells[k - 1][l].type == textures[ind])
                            continue;
                        if (cells[k + 1][l].texture == textures[ind])
                            continue;
                    }

                    if (l > 0 && l < fieldSizeY - 1)
                    {
                        if (cells[k][l - 1].texture == textures[ind])
                            continue;
                        if (cells[k][l + 1].texture == textures[ind])
                            continue;
                    }
                    else
                    {
                        continue;
                    }

                    if (l % 2 == 0)
                    {
                        if (k > 0 && l > 0 && l < fieldSizeY - 1)
                        {
                            if (cells[k - 1][l - 1].texture == textures[ind])
                                continue;
                            if (cells[k - 1][l + 1].texture == textures[ind])
                                continue;
                        }
                    }
                    else
                    {
                        if (k < fieldSizeX - 1 && l > 0 && l < fieldSizeY - 1)
                        {
                            if (cells[k + 1][l - 1].texture == textures[ind])
                                continue;
                            if (cells[k + 1][l + 1].texture == textures[ind])
                                continue;
                        }

                    }

                    break;
                } while (true);
                cells[k][l].texture = textures[ind].ToString();
                cells[k][l].type = "\"" + RecourceType.city.ToString() + "\"";

                cells[k][l].cityName = getCityName(r);
                cells[k][l].level = r.Next(1, 7).ToString();

            }
        }


        private static void GenerateRecources(int count, Random r, RecourceType type)
        {
            int fieldsCount = fieldSizeX * fieldSizeY;
            for (int i = 0; i < count; i++)
            {
                int k, l;

                int ind = (int)type;

                do
                {
                    k = r.Next(0, fieldSizeX);
                    l = r.Next(0, fieldSizeY);
                    if (cells[k][l].texture != string.Empty)
                        continue;
                    
                    break;
                } while (true);

                cells[k][l].texture = textures[ind].ToString();
                cells[k][l].type = "\"" + type.ToString() + "\"";
                cells[k][l].resourceCount = r.Next(25000, 100000).ToString();
                cells[k][l].recovery = r.Next(30, 70).ToString();

            }
        }

        private static void GenerateGrass(int count, Random r)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (cells[i][j].type == "0")
                    {
                        cells[i][j].texture = textures[5];
                        cells[i][j].type = "\"" + RecourceType.grass.ToString() + "\""; ;
                    }
                }
            }
        }

        private static string getCityName(Random r)
        {
            int ind = r.Next(0, citiesNames.Count);

            string name = citiesNames[ind];
            citiesNames.RemoveAt(ind);
            return "\"" + name + "\"";
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
            //global
            public string type = "0";
            public string texture = "";

            //resource
            public string resourceCount = "0";
            public string recovery = "0";
            public string resourceType = "0";

            //production
            public string owner = "\"undefined\"";
            public string production = "0";

            //city
            public string cityName = "\"undefined\"";
            public string level = "1";
            public string health = "50";
            public string taxes = "10";
            public string crime = "50";
            public string unemployment = "50";
            public string happy = "50";



        }
    }
}

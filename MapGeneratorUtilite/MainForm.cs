using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapGeneratorUtilite
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        int fieldSizeX, fieldSizeY, fieldCount;
        enum Textures
        {
            City, Tree, Coal, Gas, Wheat, Rock, Grass
        };

        int cities = 10, tree = 70, coal = 40, gas = 30, wheat = 100, rock = 60, grass = 140;
        static string[] textures = new[]
            {
                "\"City.png\"","\"trees.jpg\"", "\"coal.jpg\"","\"gas.jpg\"" ,"\"wheat.jpg\"", "\"rocks.jpg\"", "\"grass.jpg\""
            };

        private void MainForm_Load(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged_1(null, null);   
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists("map.json"))
            {
                Process.Start("map.json");
            }
            else
            {
                MessageBox.Show("Файл не найден. Возможно, он был удален или перемещен");
            }
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            fieldSizeX = (int)numericUpDown1.Value;
            fieldSizeY = (int)numericUpDown2.Value;
            fieldCount = fieldSizeX * fieldSizeY;

            label5.Text = fieldCount.ToString();
            double percent = (fieldCount/100);

            try
            {
                numericUpDown3.Value = (int)Math.Round(2.222 * percent, MidpointRounding.AwayFromZero);
                numericUpDown4.Value = (int)Math.Round(15.555 * percent, MidpointRounding.AwayFromZero);

                numericUpDown5.Value = (int)Math.Round(8.888 * percent, MidpointRounding.AwayFromZero);

                numericUpDown6.Value = (int)Math.Round(6.666 * percent, MidpointRounding.AwayFromZero);

                numericUpDown7.Value = (int)Math.Round(22.222 * percent, MidpointRounding.AwayFromZero);

                numericUpDown8.Value = (int)Math.Round(13.333 * percent, MidpointRounding.AwayFromZero);

            }
            catch
            {
            }

        }

        static List<List<Cell>> cells;

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateMap();
            SaveFile();
            button1.Text = "Готово";
            button2.Enabled = true;
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            double percent = (100.0 / fieldCount);


            cities = (int)numericUpDown3.Value;
            tree = (int)numericUpDown4.Value;
            coal = (int)numericUpDown5.Value;
            gas = (int)numericUpDown6.Value;
            wheat = (int)numericUpDown7.Value;
            rock = (int)numericUpDown8.Value;

            int summ = (cities + tree + coal + gas + wheat + rock);
            if (summ >= fieldCount)
            {
                MessageBox.Show("Слишком много ячеек. не остается места для травы");
                button1.Enabled = false;
                
            }
            else
            {
                grass = fieldCount - summ;
                label14.Text = grass.ToString();

                button1.Enabled = true;
                button1.Text = "Сгенерировать";
                button2.Enabled = false;

            }
        }

        private static void SaveFile()
        {
            using (FileStream fs = new FileStream("map.json", FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("var map = [");

                    List<List<string>> strings = new List<List<string>>();

                    for (int i = 0; i < cells.Count; i++)
                    {
                        strings.Add(new List<string>());
                        for (int j = 0; j < cells[i].Count; j++)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{\n\t\"type\": " + cells[i][j].type + ",");
                            sb.Append("\n\t\"texture\": " + cells[i][j].Texture + ",");
                            sb.Append("\n\t\"res_cnt\": " + cells[i][j].res_cnt + ",");
                            sb.Append("\n\t\"city\": " + cells[i][j].city + "\n}");
                            strings[i].Add(sb.ToString());
                        }
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

        private void GenerateMap()
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

            GenerateTexture(cities, r, Textures.City);
            GenerateTexture(tree, r, 1);
            GenerateTexture(coal, r, Textures.Coal);
            GenerateTexture(gas, r, Textures.Gas);
            GenerateTexture(wheat, r, Textures.Wheat);
            GenerateTexture(rock, r, 5);
            GenerateTexture(grass, r, 6);
        }

        private void GenerateTexture(int count, Random r, int index)
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
        }


        private void GenerateTexture(int count, Random r, Textures type)
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
                    if (cells[k][l].Texture != string.Empty)
                        continue;

                    if (k > 0 && k < fieldSizeX - 1)
                    {
                        if (cells[k - 1][l].Texture == textures[ind])
                            continue;
                        if (cells[k + 1][l].Texture == textures[ind])
                            continue;
                    }
                    if (l > 0 && l < fieldSizeY - 1)
                    {
                        if (cells[k][l - 1].Texture == textures[ind])
                            continue;
                        if (cells[k][l + 1].Texture == textures[ind])
                            continue;

                    }
                    if (k > 0 && l > 0 && l < fieldSizeY - 1)
                    {
                        if (cells[k - 1][l - 1].Texture == textures[ind])
                            continue;
                        if (cells[k - 1][l + 1].Texture == textures[ind])
                            continue;

                    }

                    break;
                } while (true);
                cells[k][l].Texture = textures[ind];
            }
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

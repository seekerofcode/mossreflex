using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security.Policy;
using System.Windows.Controls.Primitives;

namespace BabbleSample
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private int order = 0;
        private Dictionary<string, List<string>> hashtable = new Dictionary<string, List<string>>();        //makes a new hash table

        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = String.Empty;
            if (order == 0)
            {
                textBlock1.Text += "Select a value greater than 0\n";
                return;
            }

            string lastKey = KeyGeneration(0);
            textBlock1.Text += lastKey;
            for (int i = 0; i < wordCount - 1; i++)
            {
                string nextWord = "";

                try
                {
                    nextWord = hashtable[lastKey][random.Next(hashtable[lastKey].Count)];
                }
                catch
                {
                    lastKey = KeyGeneration(0);
                    nextWord = hashtable[lastKey][random.Next(hashtable[lastKey].Count)];
                }

                textBlock1.Text += " " + nextWord;
                List<string> temp = new List<string>(lastKey.Split());
                temp.RemoveAt(0);
                temp.Add(nextWord);
                lastKey = String.Join(' ', temp);
            }

            textBlock1.Text += "\n---------------------------------------------------\n";
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            order = orderComboBox.SelectedIndex;

            if (order == 0)
                return;

            // Gives each key a new hash table
            for (int i = 0; i < words.Length - (order + 1); i++)
            {
                string key = KeyGeneration(i);
                if (!hashtable.ContainsKey(key))
                {
                    hashtable[key] = new List<string>();
                }
                hashtable[key].Add(words[i + order]);
            }

            textBlock1.Text += "Found " + words.Length + " total words\n";
            textBlock1.Text += "Found " + hashtable.Count + " unique keys\n";
            textBlock1.Text += "---------------------------------------------------\n";

            textBlock1.Text = String.Empty;
            foreach (string hash in hashtable.Keys)
            {
                textBlock1.Text += hash + " -> " + String.Join(", ", hashtable[hash]) + "\n";
            }

            textBlock1.Text += "---------------------------------------------------\n";
        }

        // makes a key with words and places them back together with a space
        private string KeyGeneration(int index)
        {
            string key = "";
            for (int i = 0; i < order - 1; i++)
            {
                key += words[index + i] + " ";
            }
            return key + words[index + order - 1];
        }
    }
}
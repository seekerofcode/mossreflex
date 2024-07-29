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
    // Babble framework
    // Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private int order = 0;
        private Dictionary<string, List<string>> hashtable = new Dictionary<string, List<string>>();        //Creates the hash table to be utilized

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

            //removed popup because it was annoying

            //Create a Key starting from the beginning of the list of words,
            //Grab a random value from the hashtable and the given key,
            //Delete the first word from the key and then append the last word to the list
            string nextKey = KeyGeneration(0);
            textBlock1.Text += nextKey;
            for (int i = 0; i < wordCount - 1; i++)
            {
                string nextWord = "";

                try
                {
                    nextWord = hashtable[nextKey][random.Next(hashtable[nextKey].Count)];
                }
                catch
                {
                    nextKey = KeyGeneration(0);
                    nextWord = hashtable[nextKey][random.Next(hashtable[nextKey].Count)];
                }

                textBlock1.Text += " " + nextWord;
                List<string> keyList = new List<string>(nextKey.Split());
                keyList.RemoveAt(0);
                keyList.Add(nextWord);
                nextKey = String.Join(' ', keyList);
            }

            textBlock1.Text += "\n---------------------------------------------------\n";
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            order = orderComboBox.SelectedIndex;

            if (order == 0)
                return;

            // Gives each created key a new generated hash table
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

            // Prints hashtable
            textBlock1.Text = String.Empty;
            foreach (string hash in hashtable.Keys)
            {
                textBlock1.Text += hash + " -> " + String.Join(", ", hashtable[hash]) + "\n";
            }

            textBlock1.Text += "---------------------------------------------------\n";
        }

        //Creates a key with the supplied words and given order
        //Puts the items back together with spaces to seperate each one
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
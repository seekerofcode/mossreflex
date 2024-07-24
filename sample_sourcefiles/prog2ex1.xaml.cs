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
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.Xml;
using System.Security.Policy;

namespace Babble
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>(); // hash table for babble

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

        // Dump function dumps all the key value and the number of items in the key
        private void dump(Dictionary<string, ArrayList> hashTable)

        {
            foreach (KeyValuePair<string, ArrayList> entry in hashTable)
            {
                textBlock1.Text += $"\n{entry.Key} -> {entry.Value.Count}";
            }
        }

        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
            }

            hashTable.Clear();

            //Loop through each word to fill the hashtable
            if (order > 0 && words != null)
            {
                textBlock1.Text = "";

                for (int i = 0; i < words.Count() - order; i++)
                {
                    string key = words[i];

                    //Adding the order amount of words as the key
                    for (int j = 1; j < order; j++)
                    {
                        key = key + " " + words[i + j];
                    }

                    if (!hashTable.ContainsKey(key))
                        hashTable.Add(key, new ArrayList());



                    hashTable[key].Add(words[i + order]);
                }

                textBlock1.Text += "Number of words: " + words.Count();
                textBlock1.Text += "\nNumber of keys: " + hashTable.Keys.Count();
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = ""; // Clear the output

            if (orderComboBox.SelectedIndex == 0)
            {
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    textBlock1.Text += " " + words[i];
            }

            if (orderComboBox.SelectedIndex == 1)
            {
                // Generate random text for first-order statistics
                Random rand = new Random();

                for (int i = 0; i < wordCount; i++)
                {
                    if (i < words.Length)
                    {
                        textBlock1.Text += " " + words[i];
                    }
                }
            }
            else if (orderComboBox.SelectedIndex > 0)
            {
                Random RNG = new Random();
                string babbleText = "";

                // Initialize the key based on the selected order
                string key = string.Join(" ", words, 0, orderComboBox.SelectedIndex);

                // Add the initial key to the babbleText
                babbleText += key;

                while (babbleText.Split(' ').Length < wordCount)
                {
                    if (hashTable.ContainsKey(key))
                    {
                        int rand = RNG.Next(hashTable[key].Count);
                        string nextWord = (string)hashTable[key][rand];

                        babbleText += " " + nextWord;

                        // Update the key by splitting, removing the first word, and joining again
                        string[] keyWords = key.Split(' ');
                        key = string.Join(" ", keyWords, 1, keyWords.Length - 1) + " " + nextWord;
                    }
                    else
                    {
                        // If the key is not in the hashtable, start over with a new key
                        key = string.Join(" ", words, 0, orderComboBox.SelectedIndex);
                        babbleText += " " + key;
                    }
                }

                textBlock1.Text = babbleText;

            }
        }
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
            dump(hashTable);
        }
    }
}


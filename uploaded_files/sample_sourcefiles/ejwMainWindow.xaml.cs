/* CS 212 Program 2: Babble
 * 
 * Emma Wang (ejw38)
 * Fri., Oct. 7, 2022
 * 
 * This program reads a text file and computes word co-occurrence statistics.
 * It generates meaningless babble that becomes less meaningless as higher orders of statistics is selected.
 * 
 * Used the starter and sample codes.
 * 
 */

using System;
using System.Collections;
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

namespace Babble
{
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private string babble;              // string of babbled words

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample";                                // Default file name
            ofd.DefaultExt = ".txt";                                // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt";             // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                //textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);   // read file
                words = Regex.Split(input, @"\s+");                 // split into array of words
                textBlock1.Text = "";                               // clear display
                babble = "";                                        // clear previous babble string
                orderComboBox.SelectedIndex = 0;                    // reset selected order to 0
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    textBlock1.Text += " " + words[i];              // display loaded file
            }
        }

        // This method creates a hash table of ArrayList object. 
        // The (int order) parameter determines how to create keys for each order.
        Dictionary<string, ArrayList> makeHashtable(int order)
        {
            Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
            for (int i = 0; i < words.Length; i++)                  // loop through array of words
            {
                if (order == 1)
                {
                    if (!hashTable.ContainsKey(words[i]))           // if the key doesn't already exist, create as new key
                        hashTable.Add(words[i], new ArrayList());
                    if (i + 1 < Math.Min(wordCount, words.Length))  // if the word isn't at the end of the array,
                        hashTable[words[i]].Add(words[i + 1]);      //     add the next word as the key's value
                }
                else if (order == 2)                                // repeat for all orders
                {
                    if (i + 1 < Math.Min(wordCount, words.Length))
                    {
                        string two = words[i] + " " + words[i + 1];
                        if (!hashTable.ContainsKey(two))
                            hashTable.Add(two, new ArrayList());
                        if (i + 2 < Math.Min(wordCount, words.Length))
                            hashTable[two].Add(words[i + 2]);
                    }
                }
                else if (order == 3)
                {
                    if (i + 2 < Math.Min(wordCount, words.Length))
                    {
                        string three = words[i] + " " + words[i + 1] + " " + words[i + 2];
                        if (!hashTable.ContainsKey(three))
                            hashTable.Add(three, new ArrayList());
                        if (i + 3 < Math.Min(wordCount, words.Length))
                            hashTable[three].Add(words[i + 3]);
                    }
                }
                else if (order == 4)
                {
                    if (i + 3 < Math.Min(wordCount, words.Length))
                    {
                        string four = words[i] + " " + words[i + 1] + " " + words[i + 2] + " " + words[i + 3];
                        if (!hashTable.ContainsKey(four))
                            hashTable.Add(four, new ArrayList());
                        if (i + 4 < Math.Min(wordCount, words.Length))
                            hashTable[four].Add(words[i + 4]);
                    }
                }
                else if (order == 5)
                {
                    if (i + 4 < Math.Min(wordCount, words.Length))
                    {
                        string five = words[i] + " " + words[i + 1] + " " + words[i + 2] + " " + words[i + 3] + " " + words[i + 4];
                        if (!hashTable.ContainsKey(five))
                            hashTable.Add(five, new ArrayList());
                        if (i + 5 < Math.Min(wordCount, words.Length))
                            hashTable[five].Add(words[i + 5]);
                    }
                }
            }
            return hashTable;
        }

        // This method displays the hashTable and constructs the babble string to display, based on selected order.
        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
                textBlock1.Text = "";                                               // clear display
                Dictionary<string, ArrayList> hashTable = makeHashtable(order);     // call method to create hashTable

                int numWords = words.Count();
                textBlock1.Text += "Total words: " + numWords + "\n";               // display total words in text file
                int keys = hashTable.Count - 1;
                textBlock1.Text += "Total unique keys: " + keys + "\n";             // display total unique keys in hashTable

                foreach (KeyValuePair<string, ArrayList> entry in hashTable)        // display hashTable
                {
                    textBlock1.Text += entry.Key + " -> ";
                    foreach (string word in entry.Value)
                        textBlock1.Text += word + " ";
                    textBlock1.Text += "\n";
                }

                // current keeps track of the most recent words added
                string current = words[0];
                for (int i = 1; i < order; i++)
                {
                    current += " " + words[i];
                }
                string firstKey = current;

                babble = current;
                for (int i = 0; i < wordCount; i++)
                {
                    // return to first key when the last word is current and append to babble
                    if (hashTable[current].Count < 1)
                    {
                        current = firstKey;
                        babble += " " + current;
                    }

                    // choose random value from the key's ArrayList and append to babble
                    ArrayList myArrayList = hashTable[current];
                    Random rnd = new Random();
                    int index = rnd.Next(0, myArrayList.Count);
                    babble += " " + myArrayList[index];

                    if (order == 1)
                    {
                        current = myArrayList[index].ToString();
                    }
                    // split babble into words so that each order can get its unique current words
                    if (order > 1)
                    {
                        string[] sift = Regex.Split(babble, @"\s+");
                        int lastIndex = sift.Count() - 1;
                        
                        if (order == 2)
                        {
                            current = sift[lastIndex - 1] + " " + sift[lastIndex];
                        }
                        else if (order == 3)
                        {
                            current = sift[lastIndex - 2] + " " + sift[lastIndex - 1] + " " + sift[lastIndex];
                        }
                        else if (order == 4)
                        {
                            current = sift[lastIndex - 3] + " " + sift[lastIndex - 2] + " " + sift[lastIndex - 1] + " " + sift[lastIndex];
                        }
                        else if (order == 5)
                        {
                            current = sift[lastIndex - 4] + " " + sift[lastIndex - 3] + " " + sift[lastIndex - 2] + " " + sift[lastIndex - 1] + " " + sift[lastIndex];
                        }
                    }
                }
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";                               // clear display
            if (babble == null)                                 // if no file has been loaded
            {
                textBlock1.Text = "No file loaded.";
            }
            else if (babble.Length <= 0)                        // if no order is chosen
            {
                textBlock1.Text = "Choose order to analyze.";
            }
            else
            {
                string[] babbler = Regex.Split(babble, @"\s+"); // splits babble into words so we loop only 200 words
                for (int i = 0; i < wordCount; i++)
                {
                    textBlock1.Text += " " + babbler[i];
                }
            }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }
    }
}

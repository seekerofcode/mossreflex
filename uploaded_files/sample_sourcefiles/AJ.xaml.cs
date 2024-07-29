/************************************************************
 * Babble framework
 * Author: Aryan Kumar Jha
 * Date: 09/30/2023
 * Fall 2023
 ************************************************************/

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

namespace BabbleSample
{
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private int order = 0;              // order stores the order ranging between 0 and 5 depending on what the user selects

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
                textBox1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
            }
        }

        //This function generates a HashTable for order 1
        private Dictionary<string, ArrayList> makeHashTableOrder1()
        {
            Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
            for (int i = 0; i < words.Length - 1; i++)
            {
                if (!hashTable.ContainsKey(words[i]))
                    hashTable.Add(words[i], new ArrayList());
                hashTable[words[i]].Add(words[i + 1]);

            }
            // adding last word separately to avoid array indexing issues.
            if (!hashTable.ContainsKey(words[words.Length - 1]))
            {
                hashTable.Add(words[words.Length - 1], new ArrayList());
                // "--EndOfFile--" is used to identify the end of original text. This is only added if the last key in the HashTable would be empty otherwise.
                hashTable[words[words.Length - 1]].Add("--EndOfFile--");
            }
            return hashTable;
        }

        //This function generates the Text to be Babbled for Order 1
        private string generateTextForOrder1()
        {
            Dictionary<string, ArrayList> hashTable = makeHashTableOrder1();
            string Text = "";
            string currentWord = words[0];
            for (int i = 0; i < 200; i++)
            {
                ArrayList nextWords = hashTable[currentWord];
                Random random = new Random();
                int randomIndex = random.Next(0, nextWords.Count);
                string nextWord = (String)nextWords[randomIndex];
                if (nextWord == "--EndOfFile--") //Checking if the key doesn't have any successive words.
                {
                    nextWord = words[0];
                }
                Text += " " + nextWord;
                currentWord = nextWord;
            }
            return words[0] + " " + Text;
        }

        //This function is generalized to create a HashTable for Order N
        private Dictionary<string, ArrayList> makeHashTableOrderN()
        {
            Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
            int i = 0;
            int j = order - 1; // Initialize j to order - 1
            string currentNWords = "";
            while (j < words.Length - 1)
            {
                currentNWords = string.Join(" ", words.Skip(i).Take(order)); // Combine N words
                if (!hashTable.ContainsKey(currentNWords))
                    hashTable.Add(currentNWords, new ArrayList());
                hashTable[currentNWords].Add(words[j + 1]);
                i++;
                j++;
            }
            currentNWords = string.Join(" ", words.Skip(i).Take(order));
            if (!hashTable.ContainsKey(currentNWords))
            {
                hashTable.Add(currentNWords, new ArrayList());
                // "--EndOfFile--" is used to identify the end of original text. This is only added if the last key in the HashTable would be empty otherwise.
                hashTable[currentNWords].Add("--EndOfFile--");
            }
            return hashTable;
        }

        //This function generates the Text to be Babbled for Order N
        private string generateTextForOrderN()
        {
            Dictionary<string, ArrayList> hashTable = makeHashTableOrderN();
            string Text = "";
            List<string> currentWords = new List<string>();
            // Initialize currentWords with the first N words
            for (int i = 0; i < order; i++)
            {
                currentWords.Add(words[i]);
            }

            for (int i = 0; i < 200; i++)
            {
                string start = string.Join(" ", currentWords);
                ArrayList nextWords = hashTable[start];
                Random random = new Random();
                int randomIndex = random.Next(0, nextWords.Count);
                string nextWord = (string)nextWords[randomIndex];

                if (nextWord == "--EndOfFile--") //Checking if the key doesn't have any successive words.
                {
                    // Reinitialize currentWords with the first N words
                    currentWords.Clear();
                    for (int j = 0; j < order; j++)
                    {
                        currentWords.Add(words[j]);
                    }
                    Text += " " + start;
                }
                else
                {
                    Text += " " + nextWord;
                    // Maintain the length of currentWords at N
                    UpdateWords(currentWords, nextWord);
                }
            }
            return string.Join(" ", words.Take(order)) + Text;
        }
        //Helper method for generateTextForOrderN()
        private void UpdateWords(List<string> wordsList, string nextWord)
        {
            if (wordsList.Count > 0)
            {
                wordsList.RemoveAt(0); // Remove the first word
            }
            wordsList.Add(nextWord); // Add the next word to the end
        }

        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
                //HashSet<string> wordSet = new HashSet<string>(words); //We can use a Set to find unique words for Order 1
                //But the following code works in general for all N:
                textBox1.Text = $"Total number of words in the chosen text file: {words.Length}\nNumber of unique terms for order {order} is: {makeHashTableOrderN().Count}"; //Counting number of key-value pairs for unique terms.
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            // If order is 0 then just display the actual text file.
            if (order == 0)
            {
                string text = "";
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                { text += " " + words[i]; }
                textBox1.Text = text;
            } // If order is 1, then use the generateNewText() method to generate 200 words to display.
            else if (order == 1)
            {
                textBox1.Text = generateTextForOrder1();
            } //If order >=2, then use the generateTest
            else
            {
                textBox1.Text = generateTextForOrderN();
            }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            order = orderComboBox.SelectedIndex; //Store the order when the user makes a selection in the UI. 
            analyzeInput(orderComboBox.SelectedIndex);
        }
    }
}

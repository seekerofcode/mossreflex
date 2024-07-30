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
using System.Xml.Linq;
using System.Linq.Expressions;

namespace BabbleSample
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private Dictionary<string, List<string>> hashTable = new Dictionary<string, List<string>>();  //stores the keys of words and their possible successors
        private string starterKey;          //serves as both the first words printed and the first key
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


        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                hashTable.Clear();
                textBlock1.Text = "";
                MessageBox.Show("Analyzing at order: " + order);

                string key; 
                List<string> keyArray = new List<string>();
                //goes through the array, taking a chunk of words, turning it into a key, and putting the subsequent word as one of it's possible successors
                for (int i = 0; i < words.Length - (order + 1); i++)
                {
                    keyArray.Clear();
                    //takes the next order of words, joins them into a string, and turns them into the key
                    //NOTE: I chose not to include the ending key (the key with no words after) because it simplifies some of the logic and is unnecessary.
                    for(int index = i; index < (i + order)%words.Length; index++)
                    {
                        keyArray.Add(words[index]);
                    }
                    key = String.Join(" ", keyArray);

                    //if the key is unique, makes a new key in the hashTable and assigns it an empty List<string>
                    if (!hashTable.ContainsKey(key))
                    {
                        hashTable.Add(key, new List<string>());
                    }                     
                    
                    //adds the word that comes after the key into the List<string> assigned to the key.
                    hashTable[key].Add(words[i+order]);
                    if(i == 0)
                    {
                        starterKey = key;
                    }
                }

                //Prints out unique keys + 1 (to account for the key without a subsequent element), number words in the document, and the hashTable
                textBlock1.Text += "Total # of words: " + words.Length + "\n" + "Unique orders of words: " + (hashTable.Keys.Count+1) + '\n';
                foreach (string hashKey in hashTable.Keys)
                {
                    textBlock1.Text += "+" + hashKey + ": " + String.Join(", ", hashTable[hashKey]) + '\n';
                }
            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        { 
            if (orderComboBox.SelectedIndex == 0)
            {
                textBlock1.Text = "";
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    textBlock1.Text += " " + words[i];
            }
            else
            {
                Random seed = new Random();
                //Initializes the textbox by clearing and printing the starter key
                textBlock1.Text = String.Empty;
                textBlock1.Text += starterKey;
                string key = starterKey;

                //Adds babble words to the textbox
                for (int i = 0; i < wordCount - 1; i++)
                {
                    int rand = seed.Next();
                    textBlock1.Text += " " + chooseWord(ref key, rand, ref i);
                }
            }
        }

        //Takes in the key to generate the next word
        //Params:
        //  Pass by Reference: key, wordCount (# words already printed)
        //  Pass by Value: rand (random integer to choose the next word)
        //Output:
        //  Returns: next word to be printed
        //  Modifications: changes the current key to reflect the new word, changes the wordcount if the starter key needed to be added.
        private string chooseWord(ref string key, int rand, ref int wordCount)
        {
            string word;
                               
            if (hashTable.ContainsKey(key))
            {
                int elemNum = rand % hashTable[key].Count;
                word = hashTable[key][elemNum];
                key = updateKey(word, key);                
            }
            else
            {
                //handles the situation wherein a key not found in the hashTable by resetting. 
                //This means redisplaying the starter words and changing the key back to the starterKey
                word = starterKey;
                key = starterKey;
                wordCount -= (orderComboBox.SelectedIndex - 1);
            }


            return (word);
        }

        //Updates the current key based on the new added word ("It was the" + "best" -> "was the best")
        //Params: newWord (word being added), currKey (the current key)
        //Output: Returns the new key as a string
        private string updateKey(string newWord, string currKey)
        {
            List<string> currKeyArray = new List<string>(currKey.Split(" "));
            currKeyArray.Add(newWord);
            currKeyArray.RemoveAt(0);
            string newKey = string.Join(" ", currKeyArray);
            return (newKey);
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }
    }
}

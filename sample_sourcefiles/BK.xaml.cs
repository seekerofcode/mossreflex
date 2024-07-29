using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BabbleSample
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment <summary>
    /// Babble framework
    /// Ben Kosters, Program 2, due Sept 30
    /// </summary>
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>(); // Hashtable to store all keys and predicted words
        private int order;      //size of n-grams


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
                MessageBox.Show("Analyzing at order: " + order);

            }
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";                     //clear text block for each time the button is clicked
            Random rnd = new Random();               //create a random object -- this will be used for picking the next word at random
            order = orderComboBox.SelectedIndex;    // required for computing key size
            string orderkey = "";                  //string used for computing each key(size determined by order variable)

            if (order == 0)   //If the order is 0, just print out the text up to the first 200 words
            {
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    textBlock1.Text += " " + words[i];
                return;
            }


            for (int i = 0; i < words.Length - order; i++)   // for each word in the text, check to see if the word is in the hash table
            {
                for (int j = i; j < i + order; j++)   //this loop is used to determine how many words are added to a key based on order
                {
                    if (j < order + i)
                    {
                        orderkey += words[j];
                        if (j < i + order - 1)   //a space should only exist between 2 words, not at the end thus it is a separate conditional
                        {
                            orderkey += " ";
                        }
                    }

                }
                if (!hashTable.ContainsKey(orderkey))   // if the word is not in the table, create a new key with that word
                {
                    hashTable.Add(orderkey, new ArrayList());
                }

                if (i + order + 1 < words.Length)   // as long as the word is not the last word in the text, add the next word into the arraylist
                {
                    hashTable[orderkey].Add(words[i + order]);
                }
                orderkey = "";
            }

            string currentword = "";
            for (int i = 0; i < order; i++) //the first n words is computed the same way as the keys were computed
            {
                currentword += words[i];
                if (i < order - 1)
                {
                    currentword += " ";
                }
            }
            textBlock1.Text += currentword;
            string[] temporaryWords;   //these two variables are used in finding the next word
            string nextword;


            for (int i = 0; i < Math.Min(wordCount, words.Length); i++) //only babble the number of words in the origional text or up to 200 words

                if (hashTable.ContainsKey(currentword)) // if hashtable contains that word, access it's arraylist value, and choose one at random
                {
                    ArrayList nextwords = hashTable[currentword];
                    if (nextwords.Count > 0)         // check if there are words in the arraylist-- if not, this is the last word
                    {
                        nextword = (string)nextwords[rnd.Next(nextwords.Count)];
                        textBlock1.Text += " " + nextword;
                        //the following part was mostly taken from ChatGPT, this deletes the first word and shifts up one word for the current key
                        temporaryWords = currentword.Split(' ');
                        List<string> remainingwords = new List<string>(temporaryWords.Skip(1));
                        remainingwords.Add(nextword);
                        currentword = String.Join(" ", remainingwords);

                    }
                    else
                    {
                        nextword = words[0]; // if the last word is reached, restart predicting from the first word again

                    }

                }


        }
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);
        }

        //This function was used to test whether the keys and values were properly placed in the hashtable,
        //I left it in the submission but it has no use in the final program
        public void printHashTable(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";  //clear text block
            order = orderComboBox.SelectedIndex;// required for computing key size
            string orderkey = "";

            if (order == 0) //If the order is 0, just print out the text up to the first 200 words
            {
                for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    textBlock1.Text += " " + words[i];
                return;
            }


            for (int i = 0; i < words.Length - order; i++) // for each word in the text, check to see if the word is in the hash table
            {
                for (int j = i; j < i + order; j++)
                {
                    orderkey += words[j];
                    if (j < i + order - 1)
                    {
                        orderkey += " ";
                    }
                }
                if (!hashTable.ContainsKey(orderkey))
                {// if the word is not in the table, create a new key with that word
                    hashTable.Add(orderkey, new ArrayList());
                }

                if (i + 1 < words.Length)
                {  // as long as the word is not the last word in the text, add the next word into the arraylist
                    hashTable[orderkey].Add(words[i + order]);
                }
                orderkey = "";
            }

            foreach (string key in hashTable.Keys)
            {
                textBlock1.Text += key + "-->";
                foreach (string value in hashTable[key])
                {
                    textBlock1.Text += value + " ";
                }
                textBlock1.Text += "\n";
            }

        }





    }
}
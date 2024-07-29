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
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        
        
        private int wordCount = 200;                // number of words to babble

        private string final;   //Store the final string holding all the babbles words



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
        Dictionary<string, ArrayList> makeHashtable(int order)
        {
            //Create a hashtable with arraylist values
            Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
            for (int i = 0; i < words.Length; i++)
            {
                if (order == 1)
                {
                    if (!hashTable.ContainsKey(words[i]))              //If hashtable doesnt contain the key then add a new one with empty arraylist
                        hashTable.Add(words[i], new ArrayList());
                    if (i + 1 < Math.Min(wordCount, words.Length))     //This fixes the problem when we are near the end of the words required to babble. Either 200 or the amount of keys in words, whatever is lowest.
                    {
                        hashTable[words[i]].Add(words[i + 1]);
                    }

                }
                //The next orders are basically using the same logic as order ==1 but, this time the key is n words and the values are the one word coming after that
                else if (order == 2)
                {
                    if (i + 1 < Math.Min(wordCount, words.Length))
                    {
                        string otwo = words[i] + " " + words[i + 1];
                        if (!hashTable.ContainsKey(otwo))
                            hashTable.Add(otwo, new ArrayList());
                        if (i + 2 < Math.Min(wordCount, words.Length))
                            hashTable[otwo].Add(words[i + 2]);          
                    }
                }
                else if (order == 3)
                {
                    if (i + 2 < Math.Min(wordCount, words.Length))
                    {
                        string othree = words[i] + " " + words[i + 1] + " " + words[i + 2];
                        if (!hashTable.ContainsKey(othree))
                            hashTable.Add(othree, new ArrayList());
                        if (i + 3 < Math.Min(wordCount, words.Length))
                            hashTable[othree].Add(words[i + 3]);
                    }
                }
                else if (order == 4)
                {
                    if (i + 3 < Math.Min(wordCount, words.Length))
                    {
                        string ofour = words[i] + " " + words[i + 1] + " " + words[i + 2] + " " + words[i + 3];
                        if (!hashTable.ContainsKey(ofour))
                            hashTable.Add(ofour, new ArrayList());
                        if (i + 4 < Math.Min(wordCount, words.Length))
                            hashTable[ofour].Add(words[i + 4]);
                    }
                }
                else if (order == 5)
                {
                    if (i + 4 < Math.Min(wordCount, words.Length))
                    {
                        string ofive = words[i] + " " + words[i + 1] + " " + words[i + 2] + " " + words[i + 3] + " " + words[i + 4];
                        if (!hashTable.ContainsKey(ofive))
                            hashTable.Add(ofive, new ArrayList());
                        if (i + 5 < Math.Min(wordCount, words.Length))
                            hashTable[ofive].Add(words[i + 5]);
                    }
                }


            }
            return hashTable;
        }
        private void analyzeInput(int order)
        {
            
            if (order > 0)
            {
                MessageBox.Show("Analyzing at order: " + order);
                textBlock1.Text = "";                                               
                Dictionary<string, ArrayList> hashTable = makeHashtable(order);    

                int numWords = words.Count();
                textBlock1.Text += "Total words: " + numWords + "\n";               
                int keys = hashTable.Count - 1;
                textBlock1.Text += "Total keys: " + keys + "\n";            

                foreach (KeyValuePair<string, ArrayList> entry in hashTable)        
                {
                    textBlock1.Text += entry.Key + " -> ";
                    foreach (string word in entry.Value)
                        textBlock1.Text += word + " ";
                    textBlock1.Text += "\n";
                }

                string current = words[0];

                for (int i = 1; i < order; i++)
                {
                    current += " " + words[i];
                }
                string first = current;

                final = current;

                //This loop will fill the final string with babbled words
                for (int i = 0; i < wordCount; i++)

                {
                    //if the arraylist has only one word then make current word the first one. And to the final string add the current word.
                    if (hashTable[current].Count < 1)
                    {
                        
                        current = first;
                        final += " " + current;
                    }
                    
                    ArrayList myArrayList = hashTable[current];     //For each key get the arraylist with words
                    Random rnd = new Random();
                    int index = rnd.Next(0, myArrayList.Count);     //Pick a random index that holds the random word
                    final += " " + myArrayList[index];              //Add the random word to the final string.


                    //Uodate the current string based on the order given
                    if (order == 1)
                    {
                        current = myArrayList[index].ToString();
                    }
                    else
                    {
                        string[] finalarray = Regex.Split(final, @"\s+");
                        int lastIndex = finalarray.Count() - 1;

                        if (order == 2) //If order is two, then we are using the last two words in the finalarray. Same logic is used for all the other orders
                        {
                            current = finalarray[lastIndex - 1] + " " + finalarray[lastIndex];
                        }
                        else if(order==3)
                        {
                            current = finalarray[lastIndex - 2]+ " " + finalarray[lastIndex-1]+" "+finalarray[lastIndex];
                        }
                        else if (order == 4)
                        {
                            current = finalarray[lastIndex - 3] + " " + finalarray[lastIndex - 2] + " " + finalarray[lastIndex-1] + " " + finalarray[lastIndex - 1];
                        }
                        else if (order == 5)
                        {
                            current = finalarray[lastIndex - 4] + " " + finalarray[lastIndex - 3] + " " + finalarray[lastIndex - 2] + " " + finalarray[lastIndex - 1] + " " + finalarray[lastIndex];
                        }

                    }
                    
                   
                }
            }
           
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";                               // clear display

            if (final == null)                                 // no file loaded
            {
                textBlock1.Text = "No file loaded.";
            }
            else if (final.Length <= 0)                        // no order is chosen
            {
                textBlock1.Text = "Choose order to analyze.";
            }
            else                              //If user already selected an order then output babbled text

                {
                    string[] babbler = Regex.Split(final, @"\s+"); // splits babble into words so we loop only 200 words
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



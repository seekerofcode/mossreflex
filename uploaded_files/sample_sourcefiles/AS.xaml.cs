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
using System.Diagnostics.Tracing;

namespace BabbleSample
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble

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
                Dictionary<string, ArrayList> hashTable;
                hashTable = HashText.makeHashtable(words);
                HashText.dump(hashTable, textBlock1);
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
            textBlock1.Clear();
            string word = words[0];
            string nextWord;
            int count = 0;
            Dictionary<string, ArrayList> hashTable;
            hashTable = HashText.makeHashtable(words);
            
            // Generate 200 "babble" words by choosing a random word from a
            // list specific to the previous word
            while (count < 200)
            {
                textBlock1.Text += word + " ";

                if (!hashTable.ContainsKey(word))
                {
                    nextWord = words[0];
                }
                else
                {
                    Random random = new Random();
                    ArrayList list = hashTable[word];
                    int randomIndex = random.Next(0, hashTable[word].Count);
                    nextWord = (string)hashTable[word][randomIndex];
                }
                word = nextWord;
                count++;
            }
        }

        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            analyzeInput(orderComboBox.SelectedIndex);

        }
    }

    class HashText
    {
        
        // Create the hash table of all unique words where
        // each unique word points to a list of all following words
        public static Dictionary<string, ArrayList> makeHashtable(string[] words)
        {
            Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
            for (int i = 0; i < words.Length; i++)
            {
                if (i >= words.Length - 1)
                {
                    hashTable.Add(words[i], new ArrayList());
                    hashTable[words[i]].Add(words[0]);
                }
                else
                {
                    if (!hashTable.ContainsKey(words[i]))
                        hashTable.Add(words[i], new ArrayList());
                    hashTable[words[i]].Add(words[i + 1]);
                }
            }
            return hashTable;
        }

        // Print out the hash table
        public static void dump(Dictionary<string, ArrayList> hashTable, TextBox textBlock)
        {
            foreach (KeyValuePair<string, ArrayList> entry in hashTable)
            {
                textBlock.Text += entry.Key + ": ";
                foreach (string word in entry.Value)
                    textBlock.Text += word + ", ";
                textBlock.Text += "\n";
            }
        }

        static void Main1(string[] args)
        {
            Dictionary<string, ArrayList> hashTable = makeHashtable(args);
            //dump(hashTable);
            Console.Write("\nPress enter to exit: ");
            Console.ReadLine();
        }
    }
}

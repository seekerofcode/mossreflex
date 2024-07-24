using System;
using System.Collections;
using System.Collections.Generic;

class HashExample
{
    static Dictionary<string,ArrayList> makeHashtable()
    {
        String[] names = { "one", "two", "three", "four", "five", "six",
                             "seven", "two", "ten", "four" };
        Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();

        foreach (string name in names)
        { 
            string firstLetter = name.Substring(0, 1);
            if (!hashTable.ContainsKey(firstLetter))
                hashTable.Add(firstLetter, new ArrayList());
            hashTable[firstLetter].Add(name);
        }
        return hashTable;
    }

    static void dump(Dictionary<string,ArrayList> hashTable)
    {
        foreach (KeyValuePair<string, ArrayList> entry in hashTable) {
            Console.Write("{0} -> ", entry.Key);
            foreach (string name in entry.Value)
                Console.Write("{0} ", name);
            Console.WriteLine();
        }
    }

    static void Main(string[] args)
    {
        Dictionary<string,ArrayList> hashTable = makeHashtable();
        dump(hashTable);
        Console.Write("\nPress enter to exit: ");
        Console.ReadLine();
    }
}

 
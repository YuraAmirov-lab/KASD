using System;
using System.Collections.Generic;
using System.Text;

public class Program
{
    public static void Main()
    {
        string p = Console.ReadLine();
        string t = Console.ReadLine();
        int n = p.Length;
        string s = p + "#" + t;
        int len = s.Length;
        int[] pi = new int[len];

        for (int i = 1; i < len; i++)
        {
            int j = pi[i - 1];
            while (j > 0 && s[i] != s[j])
                j = pi[j - 1];
            if (s[i] == s[j])
                j++;
            pi[i] = j;
        }

        List<int> positions = new List<int>();
        for (int i = n + 1; i < len; i++)
        {
            if (pi[i] == n)
                positions.Add(i - 2 * n + 1);
        }

        Console.WriteLine(positions.Count);
        if (positions.Count > 0)
            Console.WriteLine(string.Join(" ", positions));
    }
}
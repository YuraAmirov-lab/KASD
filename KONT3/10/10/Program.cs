using System;
using System.Linq;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        int n = s.Length;
        int[] sa = new int[n];
        int[] rank = new int[n];
        int[] tmp = new int[n];

        for (int i = 0; i < n; i++)
        {
            sa[i] = i;
            rank[i] = s[i];
        }

        for (int k = 1; k < n; k <<= 1)
        {
            Array.Sort(sa, (a, b) =>
            {
                if (rank[a] != rank[b])
                    return rank[a].CompareTo(rank[b]);
                int ra = a + k < n ? rank[a + k] : -1;
                int rb = b + k < n ? rank[b + k] : -1;
                return ra.CompareTo(rb);
            });

            tmp[sa[0]] = 0;
            for (int i = 1; i < n; i++)
            {
                tmp[sa[i]] = tmp[sa[i - 1]];
                if (rank[sa[i]] != rank[sa[i - 1]] ||
                    (sa[i] + k < n ? rank[sa[i] + k] : -1) !=
                    (sa[i - 1] + k < n ? rank[sa[i - 1] + k] : -1))
                    tmp[sa[i]]++;
            }

            for (int i = 0; i < n; i++)
                rank[i] = tmp[i];
        }

        int[] lcp = new int[n - 1];
        int[] pos = new int[n];
        for (int i = 0; i < n; i++)
            pos[sa[i]] = i;

        int h = 0;
        for (int i = 0; i < n; i++)
        {
            if (pos[i] == n - 1)
            {
                h = 0;
                continue;
            }

            int j = sa[pos[i] + 1];
            while (i + h < n && j + h < n && s[i + h] == s[j + h])
                h++;

            lcp[pos[i]] = h;
            if (h > 0) h--;
        }

        Console.WriteLine(string.Join(" ", sa.Select(x => x + 1)));
        Console.WriteLine(string.Join(" ", lcp));
    }
}
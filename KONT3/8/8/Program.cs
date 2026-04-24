using System;
using System.IO;

class Program
{
    static void Main()
    {
        using var reader = new StreamReader(Console.OpenStandardInput());
        using var writer = new StreamWriter(Console.OpenStandardOutput());

        int n = int.Parse(reader.ReadLine());
        int maxNodes = 1000005;
        int[] trie = new int[maxNodes * 26];
        int[] fail = new int[maxNodes];
        long[] count = new long[maxNodes];
        int[] patternNode = new int[n];
        int[] bfsOrder = new int[maxNodes];
        int[] q = new int[maxNodes];
        int nodesCount = 1;

        for (int i = 0; i < n; i++)
        {
            string s = reader.ReadLine();
            int u = 0;
            for (int j = 0; j < s.Length; j++)
            {
                int c = s[j] - 'a';
                int idx = u * 26 + c;
                if (trie[idx] == 0)
                    trie[idx] = nodesCount++;
                u = trie[idx];
            }
            patternNode[i] = u;
        }

        int head = 0, tail = 0;
        for (int c = 0; c < 26; c++)
            if (trie[c] != 0)
                q[tail++] = trie[c];

        int bfsIdx = 0;
        while (head < tail)
        {
            int u = q[head++];
            bfsOrder[bfsIdx++] = u;
            int uBase = u * 26;
            int fBase = fail[u] * 26;
            for (int c = 0; c < 26; c++)
            {
                if (trie[uBase + c] != 0)
                {
                    fail[trie[uBase + c]] = trie[fBase + c];
                    q[tail++] = trie[uBase + c];
                }
                else
                {
                    trie[uBase + c] = trie[fBase + c];
                }
            }
        }

        string t = reader.ReadLine();
        int curr = 0;
        for (int i = 0; i < t.Length; i++)
        {
            curr = trie[curr * 26 + t[i] - 'a'];
            count[curr]++;
        }

        for (int i = bfsIdx - 1; i >= 0; i--)
        {
            int u = bfsOrder[i];
            count[fail[u]] += count[u];
        }

        for (int i = 0; i < n; i++)
            writer.WriteLine(count[patternNode[i]]);
    }
}
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
        int[] patternNode = new int[n];
        int[] patternLen = new int[n];
        int[] bfsOrder = new int[maxNodes];
        int[] q = new int[maxNodes];

        int nodesCount = 1;

        for (int i = 0; i < n; i++)
        {
            string s = reader.ReadLine();
            patternLen[i] = s.Length;
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
                int v = trie[uBase + c];
                if (v != 0)
                {
                    fail[v] = trie[fBase + c];
                    q[tail++] = v;
                }
                else
                {
                    trie[uBase + c] = trie[fBase + c];
                }
            }
        }

        int[] left = new int[maxNodes];
        int[] right = new int[maxNodes];
        for (int i = 0; i < nodesCount; i++)
        {
            left[i] = int.MaxValue;
            right[i] = -1;
        }

        string t = reader.ReadLine();
        int curr = 0;

        for (int i = 0; i < t.Length; i++)
        {
            curr = trie[curr * 26 + t[i] - 'a'];
            if (left[curr] == int.MaxValue)
                left[curr] = i;
            right[curr] = i;
        }

        for (int i = bfsIdx - 1; i >= 0; i--)
        {
            int u = bfsOrder[i];
            int f = fail[u];

            if (left[u] != int.MaxValue)
            {
                if (left[f] > left[u])
                    left[f] = left[u];
            }

            if (right[u] > right[f])
                right[f] = right[u];
        }

        for (int i = 0; i < n; i++)
        {
            int node = patternNode[i];
            if (right[node] == -1)
            {
                writer.WriteLine("-1 -1");
            }
            else
            {
                int l = left[node] - patternLen[i] + 1;
                int r = right[node] - patternLen[i] + 1;
                writer.WriteLine($"{l} {r}");
            }
        }
    }
}
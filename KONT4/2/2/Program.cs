using System;
using System.Collections.Generic;

class Program
{
    static int m, n;
    static List<int>[] adj;
    static int[] matchR, matchL;
    static bool[] visited;

    static bool Dfs(int u)
    {
        foreach (int v in adj[u])
        {
            if (visited[v]) continue;
            visited[v] = true;
            if (matchR[v] == -1 || Dfs(matchR[v]))
            {
                matchR[v] = u;
                matchL[u] = v;
                return true;
            }
        }
        return false;
    }

    static void Dfs2(int u, bool[] visitedL, bool[] visitedR)
    {
        visitedL[u] = true;
        foreach (int v in adj[u])
        {
            if (!visitedR[v])
            {
                visitedR[v] = true;
                if (matchR[v] != -1 && !visitedL[matchR[v]])
                    Dfs2(matchR[v], visitedL, visitedR);
            }
        }
    }

    static void Main()
    {
        int k = int.Parse(Console.ReadLine());
        for (int test = 0; test < k; test++)
        {
            string[] mn = Console.ReadLine().Split();
            m = int.Parse(mn[0]);
            n = int.Parse(mn[1]);

            bool[,] known = null;
            if (m > 0 && n > 0) known = new bool[m, n];
            for (int i = 0; i < m; i++)
            {
                string[] parts = Console.ReadLine().Split();
                foreach (string p in parts)
                {
                    int v = int.Parse(p);
                    if (v == 0) break;
                    if (n > 0) known[i, v - 1] = true;
                }
            }

            adj = new List<int>[m];
            for (int i = 0; i < m; i++)
            {
                adj[i] = new List<int>();
                if (n > 0)
                {
                    for (int j = 0; j < n; j++)
                        if (!known[i, j]) adj[i].Add(j);
                }
            }

            matchR = new int[n];
            matchL = new int[m];
            for (int i = 0; i < n; i++) matchR[i] = -1;
            for (int i = 0; i < m; i++) matchL[i] = -1;

            for (int u = 0; u < m; u++)
            {
                visited = new bool[n];
                Dfs(u);
            }

            bool[] freeL = new bool[m];
            for (int u = 0; u < m; u++) freeL[u] = (matchL[u] == -1);

            bool[] visitedL = new bool[m];
            bool[] visitedR = new bool[n];

            for (int u = 0; u < m; u++)
                if (freeL[u] && !visitedL[u])
                    Dfs2(u, visitedL, visitedR);

            List<int> boys = new List<int>();
            List<int> girls = new List<int>();
            for (int u = 0; u < m; u++)
                if (visitedL[u]) boys.Add(u + 1);
            for (int v = 0; v < n; v++)
                if (!visitedR[v]) girls.Add(v + 1);

            int total = boys.Count + girls.Count;
            Console.WriteLine(total);
            Console.WriteLine($"{boys.Count} {girls.Count}");
            if (boys.Count > 0) Console.WriteLine(string.Join(" ", boys));
            else Console.WriteLine();
            if (girls.Count > 0) Console.WriteLine(string.Join(" ", girls));
            else Console.WriteLine();

            if (test != k - 1) Console.WriteLine();
        }
    }
}
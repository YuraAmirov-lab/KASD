using System;
using System.Collections.Generic;

class Program
{
    static int n, m;
    static List<int>[] adj;
    static int[] matchB;
    static bool[] visited;

    static bool Dfs(int u)
    {
        foreach (int v in adj[u])
        {
            if (visited[v]) continue;
            visited[v] = true;
            if (matchB[v] == 0 || Dfs(matchB[v]))
            {
                matchB[v] = u;
                return true;
            }
        }
        return false;
    }

    static void Main()
    {
        string[] nm = Console.ReadLine().Split();
        n = int.Parse(nm[0]);
        m = int.Parse(nm[1]);
        adj = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
            adj[i] = new List<int>();

        for (int i = 1; i <= n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            foreach (string part in parts)
            {
                int v = int.Parse(part);
                if (v == 0) break;
                adj[i].Add(v);
            }
        }

        matchB = new int[m + 1];
        int matching = 0;
        for (int u = 1; u <= n; u++)
        {
            visited = new bool[m + 1];
            if (Dfs(u))
                matching++;
        }

        Console.WriteLine(matching);
        for (int v = 1; v <= m; v++)
            if (matchB[v] != 0)
                Console.WriteLine($"{matchB[v]} {v}");
    }
}
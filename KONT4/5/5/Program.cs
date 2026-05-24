using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        List<Hor> horizontals = new List<Hor>();
        List<Ver> verticals = new List<Ver>();
        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            int x1 = int.Parse(parts[0]);
            int y1 = int.Parse(parts[1]);
            int x2 = int.Parse(parts[2]);
            int y2 = int.Parse(parts[3]);
            if (y1 == y2)
            {
                horizontals.Add(new Hor
                {
                    Y = y1,
                    Xmin = Math.Min(x1, x2),
                    Xmax = Math.Max(x1, x2)
                });
            }
            else
            {
                verticals.Add(new Ver
                {
                    X = x1,
                    Ymin = Math.Min(y1, y2),
                    Ymax = Math.Max(y1, y2)
                });
            }
        }

        int H = horizontals.Count;
        int V = verticals.Count;
        List<int>[] adj = new List<int>[H];
        for (int i = 0; i < H; i++) adj[i] = new List<int>();

        for (int i = 0; i < H; i++)
        {
            for (int j = 0; j < V; j++)
            {
                if (horizontals[i].Xmin <= verticals[j].X && verticals[j].X <= horizontals[i].Xmax &&
                    verticals[j].Ymin <= horizontals[i].Y && horizontals[i].Y <= verticals[j].Ymax)
                {
                    adj[i].Add(j);
                }
            }
        }

        int[] matchV = new int[V];
        for (int j = 0; j < V; j++) matchV[j] = -1;
        int[] visited = new int[V];
        int curVis = 0;
        int matching = 0;

        for (int u = 0; u < H; u++)
        {
            curVis++;
            if (Dfs(u, adj, matchV, visited, curVis)) matching++;
        }

        Console.WriteLine(n - matching);
    }

    static bool Dfs(int u, List<int>[] adj, int[] matchV, int[] visited, int curVis)
    {
        foreach (int v in adj[u])
        {
            if (visited[v] == curVis) continue;
            visited[v] = curVis;
            if (matchV[v] == -1 || Dfs(matchV[v], adj, matchV, visited, curVis))
            {
                matchV[v] = u;
                return true;
            }
        }
        return false;
    }

    struct Hor
    {
        public int Y;
        public int Xmin;
        public int Xmax;
    }

    struct Ver
    {
        public int X;
        public int Ymin;
        public int Ymax;
    }
}
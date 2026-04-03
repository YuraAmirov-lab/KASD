using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        var adj = new List<(int to, int weight)>[n];
        for (int i = 0; i < n; i++) adj[i] = new List<(int, int)>();

        for (int i = 0; i < m; i++)
        {
            var parts = Console.ReadLine().Split();
            int u = int.Parse(parts[0]) - 1;
            int v = int.Parse(parts[1]) - 1;
            int w = int.Parse(parts[2]);
            adj[u].Add((v, w));
            adj[v].Add((u, w));
        }

        var dist = new long[n];
        for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
        dist[0] = 0;

        var pq = new SortedSet<(long dist, int vertex)>(
            Comparer<(long dist, int vertex)>.Create((a, b) =>
            {
                int cmp = a.dist.CompareTo(b.dist);
                return cmp != 0 ? cmp : a.vertex.CompareTo(b.vertex);
            }));
        pq.Add((0, 0));

        while (pq.Count > 0)
        {
            var (d, v) = pq.Min;
            pq.Remove(pq.Min);
            if (d != dist[v]) continue;

            foreach (var (to, w) in adj[v])
            {
                long nd = d + w;
                if (nd < dist[to])
                {
                    dist[to] = nd;
                    pq.Add((nd, to));
                }
            }
        }

        Console.WriteLine(string.Join(" ", dist));
    }
}
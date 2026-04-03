using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var first = Console.ReadLine().Split();
        int n = int.Parse(first[0]);
        int m = int.Parse(first[1]);

        var graph = new List<(int to, long w)>[n + 1];
        for (int i = 1; i <= n; i++) graph[i] = new List<(int, long)>();

        for (int i = 0; i < m; i++)
        {
            var parts = Console.ReadLine().Split();
            int u = int.Parse(parts[0]);
            int v = int.Parse(parts[1]);
            long w = long.Parse(parts[2]);
            graph[u].Add((v, w));
            graph[v].Add((u, w));
        }

        var last = Console.ReadLine().Split();
        int a = int.Parse(last[0]);
        int b = int.Parse(last[1]);
        int c = int.Parse(last[2]);

        long[] distA = Dijkstra(n, graph, a);
        long[] distB = Dijkstra(n, graph, b);
        long[] distC = Dijkstra(n, graph, c);

        long dAB = distA[b];
        long dAC = distA[c];
        long dBC = distB[c];

        const long INF = long.MaxValue / 2;
        if (dAB >= INF || dAC >= INF || dBC >= INF)
        {
            Console.WriteLine(-1);
            return;
        }

        long ans = Math.Min(dAB + dBC, Math.Min(dAC + dBC, dAB + dAC));
        Console.WriteLine(ans);
    }

    static long[] Dijkstra(int n, List<(int to, long w)>[] graph, int start)
    {
        var dist = new long[n + 1];
        for (int i = 1; i <= n; i++) dist[i] = long.MaxValue / 2;
        dist[start] = 0;

        var pq = new SortedSet<(long d, int v)>(Comparer<(long d, int v)>.Create((x, y) =>
        {
            int cmp = x.d.CompareTo(y.d);
            return cmp != 0 ? cmp : x.v.CompareTo(y.v);
        }));
        pq.Add((0, start));

        while (pq.Count > 0)
        {
            var (d, u) = pq.Min;
            pq.Remove(pq.Min);
            if (d != dist[u]) continue;

            foreach (var (v, w) in graph[u])
            {
                long nd = d + w;
                if (nd < dist[v])
                {
                    dist[v] = nd;
                    pq.Add((nd, v));
                }
            }
        }
        return dist;
    }
}
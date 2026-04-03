using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);
        int s = int.Parse(input[2]);

        var edges = new List<(int u, int v, long w)>();
        var graph = new List<int>[n + 1];
        var reverseGraph = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
        {
            graph[i] = new List<int>();
            reverseGraph[i] = new List<int>();
        }

        for (int i = 0; i < m; i++)
        {
            var parts = Console.ReadLine().Split();
            int u = int.Parse(parts[0]);
            int v = int.Parse(parts[1]);
            long w = long.Parse(parts[2]);
            edges.Add((u, v, w));
            graph[u].Add(v);
            reverseGraph[v].Add(u);
        }

        var reachableFromS = new bool[n + 1];
        var stack = new Stack<int>();
        stack.Push(s);
        reachableFromS[s] = true;
        while (stack.Count > 0)
        {
            int u = stack.Pop();
            foreach (int v in graph[u])
            {
                if (!reachableFromS[v])
                {
                    reachableFromS[v] = true;
                    stack.Push(v);
                }
            }
        }
        const long INF = 1_000_000_000_000_000_000; 
        var dist = new long[n + 1];
        for (int i = 1; i <= n; i++) dist[i] = INF;
        dist[s] = 0;

        for (int i = 1; i < n; i++)
        {
            bool updated = false;
            foreach (var (u, v, w) in edges)
            {
                if (dist[u] != INF && dist[u] + w < dist[v])
                {
                    dist[v] = dist[u] + w;
                    updated = true;
                }
            }
            if (!updated) break;
        }

        var bad = new bool[n + 1];
        foreach (var (u, v, w) in edges)
        {
            if (dist[u] != INF && dist[u] + w < dist[v])
            {
                bad[v] = true;
            }
        }
        var queue = new Queue<int>();
        for (int i = 1; i <= n; i++)
            if (bad[i]) queue.Enqueue(i);
        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            foreach (int v in graph[u])
            {
                if (!bad[v])
                {
                    bad[v] = true;
                    queue.Enqueue(v);
                }
            }
        }

        for (int u = 1; u <= n; u++)
        {
            if (!reachableFromS[u])
                Console.WriteLine("*");
            else if (bad[u])
                Console.WriteLine("-");
            else
                Console.WriteLine(dist[u]);
        }
    }
}
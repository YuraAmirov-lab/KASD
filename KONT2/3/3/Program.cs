
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        const int NO_EDGE = 100000;

        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            for (int j = 0; j < n; j++)
            {
                int w = int.Parse(parts[j]);
                if (w != NO_EDGE)
                {
                    edges.Add(new Edge(i, j, w));
                }
            }
        }

        long[] dist = new long[n];
        int[] parent = new int[n];
        for (int i = 0; i < n; i++)
            parent[i] = -1;

        int x = -1;

        for (int i = 0; i < n; i++)
        {
            x = -1;
            foreach (var e in edges)
            {
                if (dist[e.To] > dist[e.From] + e.Weight)
                {
                    dist[e.To] = dist[e.From] + e.Weight;
                    parent[e.To] = e.From;
                    x = e.To;
                }
            }
        }

        if (x == -1)
        {
            Console.WriteLine("NO");
            return;
        }
        int y = x;
        for (int i = 0; i < n; i++)
            y = parent[y];

        List<int> cycle = new List<int>();
        int cur = y;

        do
        {
            cycle.Add(cur);
            cur = parent[cur];
        } while (cur != y);

        cycle.Reverse();

        Console.WriteLine("YES");
        Console.WriteLine(cycle.Count);
        foreach (int v in cycle)
            Console.Write((v + 1) + " ");
        Console.WriteLine();
    }

    class Edge
    {
        public int From, To, Weight;

        public Edge(int from, int to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }
}

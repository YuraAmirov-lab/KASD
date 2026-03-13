using System;
using System.IO;

class Program
{
    static void Main()
    {
        var reader = new StreamReader(Console.OpenStandardInput());
        var writer = new StreamWriter(Console.OpenStandardOutput());

        string[] nm = reader.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        Edge[] edges = new Edge[m];
        for (int i = 0; i < m; i++)
        {
            string[] parts = reader.ReadLine().Split();
            int u = int.Parse(parts[0]);
            int v = int.Parse(parts[1]);
            int w = int.Parse(parts[2]);
            edges[i] = new Edge(u, v, w);
        }

        Array.Sort(edges, (a, b) => a.w.CompareTo(b.w));

        int[] parent = new int[n + 1];
        int[] rank = new int[n + 1];
        for (int i = 1; i <= n; i++)
            parent[i] = i;

        long totalWeight = 0;
        int edgesUsed = 0;

        foreach (var e in edges)
        {
            int ru = Find(e.u, parent);
            int rv = Find(e.v, parent);
            if (ru != rv)
            {
                Union(ru, rv, parent, rank);
                totalWeight += e.w;
                edgesUsed++;
                if (edgesUsed == n - 1)
                    break;
            }
        }

        writer.WriteLine(totalWeight);
        writer.Flush();
    }

    struct Edge
    {
        public int u, v, w;
        public Edge(int u, int v, int w)
        {
            this.u = u;
            this.v = v;
            this.w = w;
        }
    }

    static int Find(int x, int[] parent)
    {
        if (parent[x] != x)
            parent[x] = Find(parent[x], parent);
        return parent[x];
    }

    static void Union(int x, int y, int[] parent, int[] rank)
    {
        if (rank[x] < rank[y])
        {
            parent[x] = y;
        }
        else if (rank[x] > rank[y])
        {
            parent[y] = x;
        }
        else
        {
            parent[y] = x;
            rank[x]++;
        }
    }
}
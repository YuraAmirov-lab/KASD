using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    struct Edge
    {
        public int u, v;
        public long w;
    }

    static void Main()
    {
        var reader = new StreamReader(Console.OpenStandardInput());
        var writer = new StreamWriter(Console.OpenStandardOutput());

        string[] nm = reader.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        var edges = new List<Edge>();
        var graph = new List<int>[n + 1];
        for (int i = 1; i <= n; i++) graph[i] = new List<int>();

        for (int i = 0; i < m; i++)
        {
            string[] parts = reader.ReadLine().Split();
            int a = int.Parse(parts[0]);
            int b = int.Parse(parts[1]);
            long w = long.Parse(parts[2]);
            if (a != b)
            {
                edges.Add(new Edge { u = a, v = b, w = w });
                graph[a].Add(b);
            }
        }

        bool[] reachable = new bool[n + 1];
        var queue = new Queue<int>();
        reachable[1] = true;
        queue.Enqueue(1);
        while (queue.Count > 0)
        {
            int v = queue.Dequeue();
            foreach (int to in graph[v])
            {
                if (!reachable[to])
                {
                    reachable[to] = true;
                    queue.Enqueue(to);
                }
            }
        }
        for (int i = 1; i <= n; i++)
        {
            if (!reachable[i])
            {
                writer.WriteLine("NO");
                writer.Flush();
                return;
            }
        }

        long result = Edmonds(1, n, edges);
        writer.WriteLine("YES");
        writer.WriteLine(result);
        writer.Flush();
    }

    static long Edmonds(int root, int n, List<Edge> edges)
    {
        const long INF = long.MaxValue / 2;
        long[] minIn = new long[n + 1];
        int[] from = new int[n + 1];

        while (true)
        {
            for (int i = 1; i <= n; i++)
            {
                minIn[i] = INF;
                from[i] = -1;
            }
            foreach (var e in edges)
            {
                if (e.u != e.v && e.v != root && e.w < minIn[e.v])
                {
                    minIn[e.v] = e.w;
                    from[e.v] = e.u;
                }
            }

            int[] vis = new int[n + 1];
            int[] cycle = new int[n + 1];
            int cycleCnt = 0;
            bool hasCycle = false;

            for (int v = 1; v <= n; v++)
            {
                if (v == root || vis[v] != 0) continue;
                int u = v;
                while (u != root && vis[u] == 0)
                {
                    vis[u] = 1;
                    u = from[u];
                }
                if (u != root && vis[u] == 1)
                {
                    hasCycle = true;
                    cycleCnt++;
                    int w = u;
                    do
                    {
                        cycle[w] = cycleCnt;
                        w = from[w];
                    } while (w != u);
                }
                u = v;
                while (u != root && vis[u] != 2)
                {
                    vis[u] = 2;
                    u = from[u];
                }
            }

            if (!hasCycle)
            {
                long sum = 0;
                for (int v = 1; v <= n; v++)
                    if (v != root)
                        sum += minIn[v];
                return sum;
            }

            int[] newId = new int[n + 1];
            int nextId = cycleCnt;
            for (int v = 1; v <= n; v++)
            {
                if (v == root) continue;
                if (cycle[v] == 0)
                    newId[v] = ++nextId;
                else
                    newId[v] = cycle[v];
            }
            newId[root] = ++nextId;
            int newN = nextId;

            long cycleSum = 0;
            for (int v = 1; v <= n; v++)
                if (cycle[v] != 0)
                    cycleSum += minIn[v];

            var newEdges = new List<Edge>();
            foreach (var e in edges)
            {
                int nu = newId[e.u];
                int nv = newId[e.v];
                if (nu == nv) continue;
                long w = e.w;
                if (cycle[e.v] != 0)
                    w -= minIn[e.v];
                newEdges.Add(new Edge { u = nu, v = nv, w = w });
            }

            long res = Edmonds(newId[root], newN, newEdges);
            return res + cycleSum;
        }
    }
}
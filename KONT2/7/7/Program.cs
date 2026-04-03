using System;
using System.Collections.Generic;

class Program
{
    class MinHeap
    {
        private long[] dists;
        private int[] states;
        private int count;

        public MinHeap(int initialCapacity)
        {
            dists = new long[initialCapacity];
            states = new int[initialCapacity];
            count = 0;
        }

        public int Count => count;

        public void Push(long d, int state)
        {
            if (count + 1 >= dists.Length)
            {
                Array.Resize(ref dists, dists.Length * 2);
                Array.Resize(ref states, states.Length * 2);
            }

            count++;
            int i = count;
            while (i > 1 && dists[i / 2] > d)
            {
                dists[i] = dists[i / 2];
                states[i] = states[i / 2];
                i /= 2;
            }
            dists[i] = d;
            states[i] = state;
        }

        public void Pop(out long d, out int state)
        {
            d = dists[1];
            state = states[1];
            long lastD = dists[count];
            int lastS = states[count];
            count--;

            if (count == 0) return;

            int i = 1;
            while (i * 2 <= count)
            {
                int child = i * 2;
                if (child != count && dists[child + 1] < dists[child])
                {
                    child++;
                }
                if (lastD <= dists[child]) break;

                dists[i] = dists[child];
                states[i] = states[child];
                i = child;
            }
            dists[i] = lastD;
            states[i] = lastS;
        }
    }

    struct Edge
    {
        public int to;
        public long weight;
        public Edge(int t, long w) { to = t; weight = w; }
    }

    static void Main()
    {
        string[] line = ReadLineTokens();
        if (line == null) return;

        int n = int.Parse(line[0]);
        int m = int.Parse(line[1]);

        List<Edge>[] adj = new List<Edge>[n + 1];
        for (int i = 1; i <= n; i++) adj[i] = new List<Edge>();

        for (int i = 0; i < m; i++)
        {
            string[] parts = ReadLineTokens();
            int u = int.Parse(parts[0]);
            int v = int.Parse(parts[1]);
            long w = long.Parse(parts[2]);
            adj[u].Add(new Edge(v, w));
            adj[v].Add(new Edge(u, w));
        }

        string[] tLine = ReadLineTokens();
        long t = long.Parse(tLine[0]);

        long minAdj1 = long.MaxValue;
        foreach (var edge in adj[1])
        {
            if (edge.weight < minAdj1)
            {
                minAdj1 = edge.weight;
            }
        }

        if (minAdj1 == long.MaxValue)
        {
            Console.WriteLine("Impossible");
            return;
        }

        int M = (int)(2 * minAdj1);
        long[,] dist = new long[n + 1, M];

        for (int i = 1; i <= n; i++)
        {
            for (int j = 0; j < M; j++)
            {
                dist[i, j] = long.MaxValue;
            }
        }

        dist[1, 0] = 0;
        MinHeap heap = new MinHeap(100000); 

        heap.Push(0, 1 * M + 0);

        while (heap.Count > 0)
        {
            long d;
            int state;
            heap.Pop(out d, out state);

            int u = state / M;
            int r = state % M;

            if (d > dist[u, r]) continue;

            foreach (var edge in adj[u])
            {
                int v = edge.to;
                long w = edge.weight;
                long nextDist = d + w;
                int nextR = (int)(nextDist % M);

                if (nextDist < dist[v, nextR])
                {
                    dist[v, nextR] = nextDist;
                    heap.Push(nextDist, v * M + nextR);
                }
            }
        }

        int targetR = (int)(t % M);
        if (dist[n, targetR] <= t)
        {
            Console.WriteLine("Possible");
        }
        else
        {
            Console.WriteLine("Impossible");
        }
    }

    static string[] ReadLineTokens()
    {
        string line = Console.ReadLine();
        while (line != null && line.Trim().Length == 0)
        {
            line = Console.ReadLine();
        }
        if (line == null) return null;
        return line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
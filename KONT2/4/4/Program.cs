using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var parts = Console.ReadLine().Split();
        int n = int.Parse(parts[0]);
        int m = int.Parse(parts[1]);
        int k = int.Parse(parts[2]);
        int s = int.Parse(parts[3]);

        var incoming = new List<(int from, int weight)>[n + 1];
        for (int i = 1; i <= n; i++) incoming[i] = new List<(int, int)>();

        for (int i = 0; i < m; i++)
        {
            var edge = Console.ReadLine().Split();
            int a = int.Parse(edge[0]);
            int b = int.Parse(edge[1]);
            int w = int.Parse(edge[2]);
            incoming[b].Add((a, w));
        }

        const long INF = long.MaxValue / 2;
        long[] dpPrev = new long[n + 1];
        long[] dpCurr = new long[n + 1];
        for (int i = 1; i <= n; i++) dpPrev[i] = INF;
        dpPrev[s] = 0;

        for (int step = 1; step <= k; step++)
        {
            for (int i = 1; i <= n; i++) dpCurr[i] = INF;
            for (int v = 1; v <= n; v++)
            {
                foreach (var (u, w) in incoming[v])
                {
                    if (dpPrev[u] != INF && dpPrev[u] + w < dpCurr[v])
                    {
                        dpCurr[v] = dpPrev[u] + w;
                    }
                }
            }
            var temp = dpPrev;
            dpPrev = dpCurr;
            dpCurr = temp;
        }

        for (int v = 1; v <= n; v++)
        {
            if (dpPrev[v] >= INF / 2) Console.WriteLine(-1);
            else Console.WriteLine(dpPrev[v]);
        }
    }
}
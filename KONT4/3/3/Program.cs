using System;
using System.Collections.Generic;

class Program
{
    static int n;
    static double v;
    static double[] time;
    static double[] x, y;
    static List<int>[] adj;
    static int[] matchR;
    static bool[] used;

    static bool Dfs(int u)
    {
        foreach (int v in adj[u])
        {
            if (used[v]) continue;
            used[v] = true;
            if (matchR[v] == -1 || Dfs(matchR[v]))
            {
                matchR[v] = u;
                return true;
            }
        }
        return false;
    }

    static void Main()
    {
        string[] first = Console.ReadLine().Split();
        n = int.Parse(first[0]);
        v = double.Parse(first[1]);

        time = new double[n];
        x = new double[n];
        y = new double[n];

        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            string[] hm = parts[0].Split(':');
            double hours = int.Parse(hm[0]) + int.Parse(hm[1]) / 60.0;
            time[i] = hours;
            x[i] = double.Parse(parts[1]);
            y[i] = double.Parse(parts[2]);
        }

        adj = new List<int>[n];
        for (int i = 0; i < n; i++) adj[i] = new List<int>();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j) continue;
                if (time[i] < time[j])
                {
                    double dt = time[j] - time[i];
                    double dx = x[i] - x[j];
                    double dy = y[i] - y[j];
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= v * dt + 1e-9)
                        adj[i].Add(j);
                }
            }
        }

        matchR = new int[n];
        for (int i = 0; i < n; i++) matchR[i] = -1;

        int maxMatch = 0;
        for (int i = 0; i < n; i++)
        {
            used = new bool[n];
            if (Dfs(i)) maxMatch++;
        }

        Console.WriteLine(n - maxMatch);
    }
}
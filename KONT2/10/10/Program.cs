using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        var reader = new StreamReader(Console.OpenStandardInput());
        var writer = new StreamWriter(Console.OpenStandardOutput());

        var first = reader.ReadLine().Split();
        int n = int.Parse(first[0]);
        int m = int.Parse(first[1]);

        var adj = new List<int>[n + 1];
        for (int i = 1; i <= n; i++) adj[i] = new List<int>();

        int[] indegree = new int[n + 1];
        for (int i = 0; i < m; i++)
        {
            var parts = reader.ReadLine().Split();
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            adj[x].Add(y);
            indegree[y]++;
        }

        var queue = new Queue<int>();
        for (int i = 1; i <= n; i++)
            if (indegree[i] == 0)
                queue.Enqueue(i);

        var topo = new List<int>();
        while (queue.Count > 0)
        {
            int v = queue.Dequeue();
            topo.Add(v);
            foreach (int to in adj[v])
            {
                indegree[to]--;
                if (indegree[to] == 0)
                    queue.Enqueue(to);
            }
        }

        int[] grundy = new int[n + 1];
        bool[] used = new bool[n + 2];
        var marked = new List<int>();

        for (int i = topo.Count - 1; i >= 0; i--)
        {
            int v = topo[i];
            foreach (int to in adj[v])
            {
                int g = grundy[to];
                if (!used[g])
                {
                    used[g] = true;
                    marked.Add(g);
                }
            }
            int mex = 0;
            while (used[mex]) mex++;
            grundy[v] = mex;

            foreach (int g in marked)
                used[g] = false;
            marked.Clear();
        }

        var sb = new StringBuilder();
        for (int i = 1; i <= n; i++)
        {
            if (i > 1) sb.Append(' ');
            sb.Append(grundy[i]);
        }
        writer.WriteLine(sb.ToString());

        writer.Close();
    }
}
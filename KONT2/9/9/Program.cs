using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        var reader = new StreamReader(Console.OpenStandardInput());
        int t = int.Parse(reader.ReadLine());
        var output = new StringBuilder();

        for (int test = 0; test < t; test++)
        {
            var parts = reader.ReadLine().Split();
            int n = int.Parse(parts[0]);
            int m = int.Parse(parts[1]);

            var radj = new List<int>[n + 1];
            int[] outdegree = new int[n + 1];
            for (int i = 1; i <= n; i++) radj[i] = new List<int>();

            for (int i = 0; i < m; i++)
            {
                parts = reader.ReadLine().Split();
                int a = int.Parse(parts[0]);
                int b = int.Parse(parts[1]);
                radj[b].Add(a);
                outdegree[a]++;
            }

            int[] status = new int[n + 1];
            int[] curOutdegree = new int[n + 1];
            Array.Copy(outdegree, curOutdegree, n + 1);
            var queue = new Queue<int>();

            for (int i = 1; i <= n; i++)
                if (outdegree[i] == 0)
                {
                    status[i] = 2;
                    queue.Enqueue(i);
                }

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();
                foreach (int v in radj[u])
                {
                    if (status[v] != 0) continue;
                    if (status[u] == 2)
                    {
                        status[v] = 1;
                        queue.Enqueue(v);
                    }
                    else if (status[u] == 1)
                    {
                        curOutdegree[v]--;
                        if (curOutdegree[v] == 0)
                        {
                            status[v] = 2;
                            queue.Enqueue(v);
                        }
                    }
                }
            }

            for (int i = 1; i <= n; i++)
            {
                if (status[i] == 1) output.AppendLine("FIRST");
                else if (status[i] == 2) output.AppendLine("SECOND");
                else output.AppendLine("DRAW");
            }
            if (test != t - 1) output.AppendLine();
        }

        Console.Write(output.ToString());
    }
}
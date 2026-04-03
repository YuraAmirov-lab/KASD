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

        var adj = new List<int>[n + 1];
        var indeg = new int[n + 1];
        for (int i = 1; i <= n; i++) adj[i] = new List<int>();

        for (int i = 0; i < m; i++)
        {
            var e = Console.ReadLine().Split();
            int u = int.Parse(e[0]);
            int v = int.Parse(e[1]);
            adj[u].Add(v);
            indeg[v]++;
        }

        var queue = new Queue<int>();
        for (int i = 1; i <= n; i++)
            if (indeg[i] == 0)
                queue.Enqueue(i);

        var order = new List<int>();
        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            order.Add(u);
            foreach (int v in adj[u])
            {
                indeg[v]--;
                if (indeg[v] == 0)
                    queue.Enqueue(v);
            }
        }

        var win = new int[n + 1];
        for (int i = order.Count - 1; i >= 0; i--)
        {
            int u = order[i];
            bool hasLosing = false;
            foreach (int v in adj[u])
            {
                if (win[v] == 0)
                {
                    hasLosing = true;
                    break;
                }
            }
            win[u] = hasLosing ? 1 : 0;
        }

        Console.WriteLine(win[s] == 1 ? "First player wins" : "Second player wins");
    }
}
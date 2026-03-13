using System;
using System.Collections.Generic;

class TopologicalSort
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        List<int>[] graph = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
            graph[i] = new List<int>();

        int[] indegree = new int[n + 1];
        for (int i = 0; i < m; i++)
        {
            string[] edge = Console.ReadLine().Split();
            int u = int.Parse(edge[0]);
            int v = int.Parse(edge[1]);
            graph[u].Add(v);
            indegree[v]++;
        }
        Queue<int> queue = new Queue<int>();
        for (int i = 1; i <= n; i++)
        {
            if (indegree[i] == 0)
                queue.Enqueue(i);
        }
        List<int> result = new List<int>();
        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            result.Add(u);

            foreach (int v in graph[u])
            {
                indegree[v]--;
                if (indegree[v] == 0)
                    queue.Enqueue(v);
            }
        }
        if (result.Count != n)
        {
            Console.WriteLine(-1);
        }
        else
        {
            Console.WriteLine(string.Join(" ", result) + " ");
        }
    }
}
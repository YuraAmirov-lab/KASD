using System;
using System.Collections.Generic;

class ArticulationPoints
{
    static void Main()
    {
        var input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        var graph = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
            graph[i] = new List<int>();

        for (int i = 0; i < m; i++)
        {
            var edge = Console.ReadLine().Split();
            int u = int.Parse(edge[0]);
            int v = int.Parse(edge[1]);
            graph[u].Add(v);
            graph[v].Add(u);
        }

        int[] tin = new int[n + 1];
        int[] low = new int[n + 1];
        bool[] visited = new bool[n + 1];
        bool[] isArticulation = new bool[n + 1];
        int timer = 0;

        for (int start = 1; start <= n; start++)
        {
            if (!visited[start])
            {
                var stack = new Stack<(int v, int parent, int next, int root)>();
                stack.Push((start, -1, 0, start));
                int rootChildren = 0;

                while (stack.Count > 0)
                {
                    var cur = stack.Peek();
                    int v = cur.v;
                    int parent = cur.parent;
                    int next = cur.next;
                    int root = cur.root;

                    if (!visited[v])
                    {
                        visited[v] = true;
                        tin[v] = low[v] = timer++;
                    }

                    if (next < graph[v].Count)
                    {
                        int to = graph[v][next];
                        stack.Pop();
                        stack.Push((v, parent, next + 1, root));

                        if (to == parent)
                            continue;

                        if (!visited[to])
                        {
                            if (parent == -1)
                                rootChildren++;
                            stack.Push((to, v, 0, root));
                        }
                        else
                        {
                            low[v] = Math.Min(low[v], tin[to]);
                        }
                    }
                    else
                    {
                        stack.Pop();

                        if (parent != -1)
                        {
                            low[parent] = Math.Min(low[parent], low[v]);
                            if (low[v] >= tin[parent] && parent != root)
                            {
                                isArticulation[parent] = true;
                            }
                        }
                    }
                }

                if (rootChildren > 1)
                    isArticulation[start] = true;
            }
        }

        List<int> result = new List<int>();
        for (int i = 1; i <= n; i++)
            if (isArticulation[i])
                result.Add(i);

        Console.WriteLine(result.Count);
        if (result.Count > 0)
            Console.WriteLine(string.Join(" ", result));
        else
            Console.WriteLine();
    }
}
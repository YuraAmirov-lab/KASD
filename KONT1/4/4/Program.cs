using System;
using System.Collections.Generic;

class EdgeBiconnectedComponents
{
    static void Main()
    {
        var input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        var graph = new List<(int, int)>[n + 1];
        for (int i = 1; i <= n; i++)
            graph[i] = new List<(int, int)>();

        for (int i = 1; i <= m; i++)
        {
            var edge = Console.ReadLine().Split();
            int u = int.Parse(edge[0]);
            int v = int.Parse(edge[1]);
            graph[u].Add((v, i));
            graph[v].Add((u, i));
        }

        int[] tin = new int[n + 1];
        int[] low = new int[n + 1];
        bool[] visited = new bool[n + 1];
        var bridges = new List<int>();
        int timer = 0;

        for (int start = 1; start <= n; start++)
        {
            if (!visited[start])
            {
                var stack = new Stack<(int v, int parent, int edgeId, int next)>();
                stack.Push((start, -1, -1, 0));

                while (stack.Count > 0)
                {
                    var cur = stack.Peek();
                    int v = cur.v;
                    int parent = cur.parent;
                    int edgeId = cur.edgeId;
                    int next = cur.next;

                    if (!visited[v])
                    {
                        visited[v] = true;
                        tin[v] = low[v] = timer++;
                    }

                    if (next < graph[v].Count)
                    {
                        var (to, eid) = graph[v][next];
                        stack.Pop();
                        stack.Push((v, parent, edgeId, next + 1));

                        if (to == parent)
                            continue;

                        if (!visited[to])
                        {
                            stack.Push((to, v, eid, 0));
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
                            var par = stack.Peek();
                            low[par.v] = Math.Min(low[par.v], low[v]);

                            if (low[v] > tin[par.v])
                                bridges.Add(edgeId);
                        }
                    }
                }
            }
        }

        bool[] isBridge = new bool[m + 1];
        foreach (int id in bridges)
            isBridge[id] = true;

        int[] comp = new int[n + 1];
        bool[] visited2 = new bool[n + 1];
        int compCount = 0;

        for (int i = 1; i <= n; i++)
        {
            if (!visited2[i])
            {
                compCount++;
                var queue = new Queue<int>();
                queue.Enqueue(i);
                visited2[i] = true;

                while (queue.Count > 0)
                {
                    int v = queue.Dequeue();
                    comp[v] = compCount;

                    foreach (var (to, eid) in graph[v])
                    {
                        if (!isBridge[eid] && !visited2[to])
                        {
                            visited2[to] = true;
                            queue.Enqueue(to);
                        }
                    }
                }
            }
        }

        Console.WriteLine(compCount);
        for (int i = 1; i <= n; i++)
            Console.Write(comp[i] + (i == n ? "" : " "));
        Console.WriteLine();
    }
}
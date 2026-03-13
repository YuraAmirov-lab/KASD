using System;
using System.Collections.Generic;

class Condensation
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        var graph = new List<int>[n + 1];
        var graphRev = new List<int>[n + 1];
        for (int i = 1; i <= n; i++)
        {
            graph[i] = new List<int>();
            graphRev[i] = new List<int>();
        }

        for (int i = 0; i < m; i++)
        {
            input = Console.ReadLine().Split();
            int u = int.Parse(input[0]);
            int v = int.Parse(input[1]);
            graph[u].Add(v);
            graphRev[v].Add(u);
        }

        var visited = new bool[n + 1];
        var order = new Stack<int>();

        for (int i = 1; i <= n; i++)
            if (!visited[i])
                Dfs1(i, graph, visited, order);

        var comp = new int[n + 1];
        int compId = 0;
        visited = new bool[n + 1];

        while (order.Count > 0)
        {
            int v = order.Pop();
            if (!visited[v])
            {
                compId++;
                Dfs2(v, graphRev, visited, comp, compId);
            }
        }

        var edges = new HashSet<(int, int)>();
        for (int u = 1; u <= n; u++)
            foreach (int v in graph[u])
            {
                int cu = comp[u];
                int cv = comp[v];
                if (cu != cv)
                    edges.Add((cu, cv));
            }

        Console.WriteLine(edges.Count);
    }

    static void Dfs1(int v, List<int>[] graph, bool[] visited, Stack<int> order)
    {
        visited[v] = true;
        foreach (int to in graph[v])
            if (!visited[to])
                Dfs1(to, graph, visited, order);
        order.Push(v);
    }

    static void Dfs2(int v, List<int>[] graphRev, bool[] visited, int[] comp, int compId)
    {
        visited[v] = true;
        comp[v] = compId;
        foreach (int to in graphRev[v])
            if (!visited[to])
                Dfs2(to, graphRev, visited, comp, compId);
    }
}
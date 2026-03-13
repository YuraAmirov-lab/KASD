using System;
using System.Collections.Generic;

class PartyPlanner
{
    static void Main()
    {
        string[] input = Console.ReadLine().Split();
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);

        var nameToId = new Dictionary<string, int>();
        for (int i = 1; i <= n; i++)
        {
            string name = Console.ReadLine().Trim();
            nameToId[name] = i;
        }

        int N = 2 * n;
        var graph = new List<int>[N + 1];
        var graphRev = new List<int>[N + 1];
        for (int i = 1; i <= N; i++)
        {
            graph[i] = new List<int>();
            graphRev[i] = new List<int>();
        }

        for (int i = 0; i < m; i++)
        {
            string line = Console.ReadLine().Trim();
            string[] parts = line.Split(' ');
            string lit1 = parts[0];
            string lit2 = parts[2];
            int v1 = GetVertex(lit1, nameToId, n);
            int v2 = GetVertex(lit2, nameToId, n);

            graph[v1].Add(v2);
            graphRev[v2].Add(v1);
            int nv2 = Neg(v2, n);
            int nv1 = Neg(v1, n);
            graph[nv2].Add(nv1);
            graphRev[nv1].Add(nv2);
        }

        bool[] visited = new bool[N + 1];
        var order = new Stack<int>();

        for (int i = 1; i <= N; i++)
            if (!visited[i])
                Dfs1(i, graph, visited, order);

        int[] comp = new int[N + 1];
        visited = new bool[N + 1];
        int compId = 0;

        while (order.Count > 0)
        {
            int v = order.Pop();
            if (!visited[v])
            {
                compId++;
                Dfs2(v, graphRev, visited, comp, compId);
            }
        }

        for (int i = 1; i <= n; i++)
        {
            if (comp[i] == comp[i + n])
            {
                Console.WriteLine(-1);
                return;
            }
        }

        var guests = new List<string>();
        foreach (var pair in nameToId)
        {
            int id = pair.Value;
            if (comp[id] > comp[id + n])
                guests.Add(pair.Key);
        }

        Console.WriteLine(guests.Count);
        foreach (string name in guests)
            Console.WriteLine(name);
    }

    static int GetVertex(string lit, Dictionary<string, int> nameToId, int n)
    {
        char sign = lit[0];
        string name = lit.Substring(1);
        int id = nameToId[name];
        return sign == '+' ? id : id + n;
    }

    static int Neg(int v, int n)
    {
        return v <= n ? v + n : v - n;
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
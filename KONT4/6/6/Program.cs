using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    class Edge
    {
        public int From;
        public int To;
        public int Cap;
        public int Flow;
        public int Cost;
        public int Rev;
    }

    static List<Edge>[] adj;

    static void AddEdge(int from, int to, int cap, int cost)
    {
        Edge a = new Edge { From = from, To = to, Cap = cap, Flow = 0, Cost = cost, Rev = adj[to].Count };
        Edge b = new Edge { From = to, To = from, Cap = 0, Flow = 0, Cost = -cost, Rev = adj[from].Count };
        adj[from].Add(a);
        adj[to].Add(b);
    }

    static void Main()
    {
        var tokenReader = new TokenReader(Console.In);
        string mStr = tokenReader.Next();
        if (string.IsNullOrEmpty(mStr)) return;

        int m = int.Parse(mStr);
        int k = int.Parse(tokenReader.Next());
        int n = int.Parse(tokenReader.Next());

        int t = int.Parse(tokenReader.Next());
        bool[,] forbidden = new bool[m + 1, m + k + 1];
        for (int i = 0; i < t; i++)
        {
            int g = int.Parse(tokenReader.Next());
            int y = int.Parse(tokenReader.Next());
            forbidden[g, y] = true;
        }

        int q = int.Parse(tokenReader.Next());
        bool[] isForced = new bool[m + k + 1];
        for (int i = 0; i < q; i++)
        {
            int id = int.Parse(tokenReader.Next());
            isForced[id] = true;
        }

        int source = 0;
        int sPrime = 1;
        int sink = m + k + 2;
        int numNodes = sink + 1;

        adj = new List<Edge>[numNodes];
        for (int i = 0; i < numNodes; i++)
        {
            adj[i] = new List<Edge>();
        }

        AddEdge(source, sPrime, n, 0);

        for (int g = 1; g <= m; g++)
        {
            int cost = isForced[g] ? -100000 : 0;
            AddEdge(sPrime, g + 1, 1, cost);
        }

        for (int y = m + 1; y <= m + k; y++)
        {
            int cost = isForced[y] ? -100000 : 0;
            AddEdge(y + 1, sink, 1, cost);
        }

        for (int g = 1; g <= m; g++)
        {
            for (int y = m + 1; y <= m + k; y++)
            {
                if (!forbidden[g, y])
                {
                    AddEdge(g + 1, y + 1, 1, 0);
                }
            }
        }

        int totalFlow = 0;

        while (totalFlow < n)
        {
            int[] dist = new int[numNodes];
            Array.Fill(dist, int.MaxValue);
            int[] parentNode = new int[numNodes];
            int[] parentEdge = new int[numNodes];
            bool[] inQueue = new bool[numNodes];

            Queue<int> queue = new Queue<int>();
            dist[source] = 0;
            queue.Enqueue(source);
            inQueue[source] = true;

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();
                inQueue[u] = false;

                for (int i = 0; i < adj[u].Count; i++)
                {
                    Edge e = adj[u][i];
                    if (e.Cap - e.Flow > 0 && dist[u] != int.MaxValue && dist[u] + e.Cost < dist[e.To])
                    {
                        dist[e.To] = dist[u] + e.Cost;
                        parentNode[e.To] = u;
                        parentEdge[e.To] = i;
                        if (!inQueue[e.To])
                        {
                            queue.Enqueue(e.To);
                            inQueue[e.To] = true;
                        }
                    }
                }
            }

            if (dist[sink] == int.MaxValue)
            {
                break;
            }

            int curr = sink;
            while (curr != source)
            {
                int p = parentNode[curr];
                int idx = parentEdge[curr];
                adj[p][idx].Flow += 1;
                adj[curr][adj[p][idx].Rev].Flow -= 1;
                curr = p;
            }
            totalFlow++;
        }

        if (totalFlow < n)
        {
            Console.WriteLine("NO");
            return;
        }

        for (int g = 1; g <= m; g++)
        {
            if (isForced[g])
            {
                bool covered = false;
                foreach (var e in adj[g + 1])
                {
                    if (e.To >= m + 2 && e.To <= m + k + 1 && e.Flow == 1)
                    {
                        covered = true;
                        break;
                    }
                }
                if (!covered)
                {
                    Console.WriteLine("NO");
                    return;
                }
            }
        }

        for (int y = m + 1; y <= m + k; y++)
        {
            if (isForced[y])
            {
                bool covered = false;
                foreach (var e in adj[y + 1])
                {
                    if (e.To == sink && e.Flow == 1)
                    {
                        covered = true;
                        break;
                    }
                }
                if (!covered)
                {
                    Console.WriteLine("NO");
                    return;
                }
            }
        }

        Console.WriteLine("YES");
        for (int g = 1; g <= m; g++)
        {
            foreach (var e in adj[g + 1])
            {
                if (e.To >= m + 2 && e.To <= m + k + 1 && e.Flow == 1)
                {
                    Console.WriteLine($"{g} {e.To - 1}");
                }
            }
        }
    }
}

class TokenReader
{
    private readonly TextReader _reader;
    private readonly Queue<string> _tokens = new Queue<string>();

    public TokenReader(TextReader reader)
    {
        _reader = reader;
    }

    public string Next()
    {
        while (_tokens.Count == 0)
        {
            string line = _reader.ReadLine();
            if (line == null) return null;
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                _tokens.Enqueue(part);
            }
        }
        return _tokens.Dequeue();
    }
}

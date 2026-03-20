using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Выберите задачу:");
            Console.WriteLine("1 - Транзитивное замыкание (DFS)");
            Console.WriteLine("2 - Максимальный поток (Эдмондс-Карп)");
            Console.WriteLine("3 - Максимальная клика (перебор)");
            Console.WriteLine("0 - Выход");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Task1();
            }
            else if (choice == "2")
            {
                Task2();
            }
            else if (choice == "3")
            {
                Task3();
            }
            else if (choice == "0")
            {
                break;
            }
            else
            {
                Console.WriteLine("Неверный выбор");
            }

            Console.WriteLine();
        }
    }

    static void DFS(int start, int v, List<int>[] graph, bool[] used, int[,] result)
    {
        used[v] = true;
        result[start, v] = 1;

        for (int i = 0; i < graph[v].Count; i++)
        {
            int to = graph[v][i];
            if (!used[to])
            {
                DFS(start, to, graph, used, result);
            }
        }
    }

    static void Task1()
    {
        Console.Write("Введите количество вершин: ");
        int n = int.Parse(Console.ReadLine());

        List<int>[] graph = new List<int>[n];
        for (int i = 0; i < n; i++)
        {
            graph[i] = new List<int>();
        }

        Console.Write("Введите количество рёбер: ");
        int m = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите рёбра графа (откуда куда), вершины с 0:");
        for (int i = 0; i < m; i++)
        {
            string[] s = Console.ReadLine().Split();
            int a = int.Parse(s[0]);
            int b = int.Parse(s[1]);
            graph[a].Add(b);
        }

        int[,] result = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            bool[] used = new bool[n];
            DFS(i, i, graph, used, result);
        }

        Console.WriteLine("Транзитивное замыкание:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(result[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static int BFS(int[,] capacity, int[,] flow, int s, int t, int[] parent, int n)
    {
        for (int i = 0; i < n; i++)
        {
            parent[i] = -1;
        }

        parent[s] = -2;

        Queue<int> q = new Queue<int>();
        q.Enqueue(s);

        int[] minCapacity = new int[n];
        minCapacity[s] = int.MaxValue;

        while (q.Count > 0)
        {
            int cur = q.Dequeue();

            for (int next = 0; next < n; next++)
            {
                if (parent[next] == -1 && capacity[cur, next] - flow[cur, next] > 0)
                {
                    parent[next] = cur;
                    minCapacity[next] = Math.Min(minCapacity[cur], capacity[cur, next] - flow[cur, next]);

                    if (next == t)
                    {
                        return minCapacity[t];
                    }

                    q.Enqueue(next);
                }
            }
        }

        return 0;
    }

    static void Task2()
    {
        Console.Write("Введите количество вершин: ");
        int n = int.Parse(Console.ReadLine());

        int[,] capacity = new int[n, n];
        int[,] flow = new int[n, n];

        Console.Write("Введите количество рёбер: ");
        int m = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите рёбра: откуда куда пропускная_способность");
        for (int i = 0; i < m; i++)
        {
            string[] s = Console.ReadLine().Split();
            int a = int.Parse(s[0]);
            int b = int.Parse(s[1]);
            int c = int.Parse(s[2]);

            capacity[a, b] = c;
        }

        Console.Write("Введите исток: ");
        int source = int.Parse(Console.ReadLine());

        Console.Write("Введите сток: ");
        int sink = int.Parse(Console.ReadLine());

        int maxFlow = 0;
        int[] parent = new int[n];

        while (true)
        {
            int addFlow = BFS(capacity, flow, source, sink, parent, n);

            if (addFlow == 0)
            {
                break;
            }

            maxFlow += addFlow;

            int cur = sink;
            while (cur != source)
            {
                int prev = parent[cur];
                flow[prev, cur] += addFlow;
                flow[cur, prev] -= addFlow;
                cur = prev;
            }
        }

        Console.WriteLine("Максимальный поток = " + maxFlow);
    }
    static void Task3()
    {
        Console.Write("Введите количество вершин: ");
        int n = int.Parse(Console.ReadLine());

        int[,] g = new int[n, n];

        Console.Write("Введите количество рёбер: ");
        int m = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите рёбра неориентированного графа:");
        for (int i = 0; i < m; i++)
        {
            string[] s = Console.ReadLine().Split();
            int a = int.Parse(s[0]);
            int b = int.Parse(s[1]);

            g[a, b] = 1;
            g[b, a] = 1;
        }

        int bestMask = 0;
        int bestSize = 0;

        int total = 1 << n;

        for (int mask = 1; mask < total; mask++)
        {
            bool isClique = true;
            int count = 0;

            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    count++;
                }
            }

            if (count <= bestSize)
            {
                continue;
            }

            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) == 0)
                {
                    continue;
                }

                for (int j = i + 1; j < n; j++)
                {
                    if ((mask & (1 << j)) == 0)
                    {
                        continue;
                    }

                    if (g[i, j] == 0)
                    {
                        isClique = false;
                        break;
                    }
                }

                if (!isClique)
                {
                    break;
                }
            }

            if (isClique)
            {
                bestSize = count;
                bestMask = mask;
            }
        }

        Console.WriteLine("Размер максимальной клики = " + bestSize);
        Console.Write("Вершины клики: ");

        for (int i = 0; i < n; i++)
        {
            if ((bestMask & (1 << i)) != 0)
            {
                Console.Write(i + " ");
            }
        }

        Console.WriteLine();
    }
}
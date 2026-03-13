using System;

class Program
{
    static int n;
    static int[,] a;

    static void Main()
    {
        n = int.Parse(Console.ReadLine());
        a = new int[n + 1, n + 1];

        for (int i = 1; i <= n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            for (int j = 1; j <= n; j++)
                a[i, j] = int.Parse(parts[j - 1]);
        }

        if (n == 1)
        {
            Console.WriteLine(0);
            return;
        }

        int l = 0, r = 1000000000;
        while (l < r)
        {
            int mid = (l + r) / 2;
            if (Check(mid))
                r = mid;
            else
                l = mid + 1;
        }
        Console.WriteLine(l);
    }

    static bool Check(int x)
    {
        bool[] vis = new bool[n + 1];
        Dfs(1, x, vis, true);
        for (int i = 1; i <= n; i++)
            if (!vis[i]) return false;

        vis = new bool[n + 1];
        Dfs(1, x, vis, false);
        for (int i = 1; i <= n; i++)
            if (!vis[i]) return false;

        return true;
    }

    static void Dfs(int u, int x, bool[] vis, bool forward)
    {
        vis[u] = true;
        if (forward)
        {
            for (int v = 1; v <= n; v++)
                if (!vis[v] && a[u, v] <= x)
                    Dfs(v, x, vis, true);
        }
        else
        {
            for (int v = 1; v <= n; v++)
                if (!vis[v] && a[v, u] <= x)
                    Dfs(v, x, vis, false);
        }
    }
}
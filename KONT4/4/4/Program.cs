using System;
using System.Collections.Generic;

class Program
{
    static int n, m, a, b;
    static char[][] grid;
    static int totalFree;
    static int L, R;
    static int[,] idL, idR;
    static List<int>[] adj;
    static int[] matchR;
    static int[] visited;
    static int cur;

    static bool Dfs(int u)
    {
        foreach (int v in adj[u])
        {
            if (visited[v] == cur) continue;
            visited[v] = cur;
            if (matchR[v] == -1 || Dfs(matchR[v]))
            {
                matchR[v] = u;
                return true;
            }
        }
        return false;
    }

    static void Main()
    {
        string[] first = Console.ReadLine().Split();
        n = int.Parse(first[0]);
        m = int.Parse(first[1]);
        a = int.Parse(first[2]);
        b = int.Parse(first[3]);
        grid = new char[n][];
        for (int i = 0; i < n; i++)
            grid[i] = Console.ReadLine().ToCharArray();

        totalFree = 0;
        idL = new int[n, m];
        idR = new int[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                idL[i, j] = -1;
                idR[i, j] = -1;
            }

        L = 0; R = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (grid[i][j] == '*')
                {
                    totalFree++;
                    if ((i + j) % 2 == 0)
                        idL[i, j] = L++;
                    else
                        idR[i, j] = R++;
                }
            }
        }

        adj = new List<int>[L];
        for (int i = 0; i < L; i++) adj[i] = new List<int>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (grid[i][j] == '*' && (i + j) % 2 == 0)
                {
                    int u = idL[i, j];
                    for (int d = 0; d < 4; d++)
                    {
                        int ni = i + dx[d];
                        int nj = j + dy[d];
                        if (ni >= 0 && ni < n && nj >= 0 && nj < m && grid[ni][nj] == '*')
                        {
                            int v = idR[ni, nj];
                            if (v != -1)
                                adj[u].Add(v);
                        }
                    }
                }
            }
        }

        matchR = new int[R];
        for (int i = 0; i < R; i++) matchR[i] = -1;
        visited = new int[R];
        cur = 0;
        int maxMatch = 0;
        for (int u = 0; u < L; u++)
        {
            cur++;
            if (Dfs(u)) maxMatch++;
        }

        long cost = (long)totalFree * b;
        if (a - 2 * b < 0)
        {
            cost += (long)(a - 2 * b) * maxMatch;
        }
        Console.WriteLine(cost);
    }
}
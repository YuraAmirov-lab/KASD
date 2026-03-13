using System;

class Program
{
    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int[] x = new int[n];
        int[] y = new int[n];
        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine().Split();
            x[i] = int.Parse(parts[0]);
            y[i] = int.Parse(parts[1]);
        }

        long[] dist = new long[n];
        bool[] used = new bool[n];

        for (int i = 1; i < n; i++)
        {
            long dx = x[i] - x[0];
            long dy = y[i] - y[0];
            dist[i] = dx * dx + dy * dy;
        }
        used[0] = true;
        dist[0] = 0;

        double total = 0.0;

        for (int i = 1; i < n; i++)
        {
            int minIdx = -1;
            long minDist = long.MaxValue;
            for (int j = 0; j < n; j++)
            {
                if (!used[j] && dist[j] < minDist)
                {
                    minDist = dist[j];
                    minIdx = j;
                }
            }

            used[minIdx] = true;
            total += Math.Sqrt(minDist);

            int cx = x[minIdx];
            int cy = y[minIdx];
            for (int j = 0; j < n; j++)
            {
                if (!used[j])
                {
                    long dx = cx - x[j];
                    long dy = cy - y[j];
                    long newDist = dx * dx + dy * dy;
                    if (newDist < dist[j])
                        dist[j] = newDist;
                }
            }
        }

        Console.WriteLine(total.ToString("F10"));
    }
}
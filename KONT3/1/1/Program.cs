using System;
using System.Text;

public class Program
{
    const int MOD1 = 1000000007;
    const int MOD2 = 1000000009;
    const int BASE = 91138233; 

    static void Main()
    {
        string s = Console.ReadLine();
        int n = s.Length;
        int m = int.Parse(Console.ReadLine());

        long[] pow1 = new long[n + 1];
        long[] pow2 = new long[n + 1];
        long[] h1 = new long[n + 1];
        long[] h2 = new long[n + 1];

        pow1[0] = 1;
        pow2[0] = 1;
        for (int i = 1; i <= n; i++)
        {
            pow1[i] = (pow1[i - 1] * BASE) % MOD1;
            pow2[i] = (pow2[i - 1] * BASE) % MOD2;
        }

        for (int i = 0; i < n; i++)
        {
            int val = s[i] - 'a' + 1;          
            h1[i + 1] = (h1[i] * BASE + val) % MOD1;
            h2[i + 1] = (h2[i] * BASE + val) % MOD2;
        }

        var output = new StringBuilder();

        for (int q = 0; q < m; q++)
        {
            string[] parts = Console.ReadLine().Split();
            int a = int.Parse(parts[0]) - 1;   
            int b = int.Parse(parts[1]) - 1;
            int c = int.Parse(parts[2]) - 1;
            int d = int.Parse(parts[3]) - 1;

            if (b - a != d - c)
            {
                output.AppendLine("No");
                continue;
            }

            int len = b - a + 1;

            long hash1_l = (h1[b + 1] - h1[a] * pow1[len]) % MOD1;
            if (hash1_l < 0) hash1_l += MOD1;
            long hash1_r = (h1[d + 1] - h1[c] * pow1[len]) % MOD1;
            if (hash1_r < 0) hash1_r += MOD1;

            long hash2_l = (h2[b + 1] - h2[a] * pow2[len]) % MOD2;
            if (hash2_l < 0) hash2_l += MOD2;
            long hash2_r = (h2[d + 1] - h2[c] * pow2[len]) % MOD2;
            if (hash2_r < 0) hash2_r += MOD2;

            if (hash1_l == hash1_r && hash2_l == hash2_r)
                output.AppendLine("Yes");
            else
                output.AppendLine("No");
        }

        Console.Write(output.ToString());
    }
}
using System;

class Program
{
    static void Main()
    {
        string s = Console.ReadLine();
        string t = Console.ReadLine();

        int n = s.Length;
        int m = t.Length;

        int total = n + 1 + m;
        int[] arr = new int[total];
        for (int i = 0; i < n; i++) arr[i] = s[i] - 'a' + 1;
        arr[n] = 27;
        for (int i = 0; i < m; i++) arr[n + 1 + i] = t[i] - 'a' + 1;

        int size = total + 1;

        int[] p = new int[size];
        int[] cls = new int[size];
        int[] pn = new int[size];
        int[] cn = new int[size];

        int alphabet = 28;
        int[] cnt = new int[Math.Max(alphabet, size)];

        for (int i = 0; i < size; i++)
        {
            int ch = (i == total) ? 0 : arr[i];
            cnt[ch]++;
        }

        for (int i = 1; i < alphabet; i++)
            cnt[i] += cnt[i - 1];

        for (int i = size - 1; i >= 0; i--)
        {
            int ch = (i == total) ? 0 : arr[i];
            p[--cnt[ch]] = i;
        }

        cls[p[0]] = 0;
        int classes = 1;

        for (int i = 1; i < size; i++)
        {
            int cur = p[i];
            int prev = p[i - 1];

            int a1 = (cur == total) ? 0 : arr[cur];
            int a2 = (prev == total) ? 0 : arr[prev];

            if (a1 != a2) classes++;
            cls[cur] = classes - 1;
        }

        int h = 0;
        while ((1 << h) < size)
        {
            int len = 1 << h;

            for (int i = 0; i < size; i++)
            {
                pn[i] = p[i] - len;
                if (pn[i] < 0) pn[i] += size;
            }

            Array.Clear(cnt, 0, classes);

            for (int i = 0; i < size; i++)
                cnt[cls[pn[i]]]++;

            for (int i = 1; i < classes; i++)
                cnt[i] += cnt[i - 1];

            for (int i = size - 1; i >= 0; i--)
                p[--cnt[cls[pn[i]]]] = pn[i];

            cn[p[0]] = 0;
            int newClasses = 1;

            for (int i = 1; i < size; i++)
            {
                int cur = p[i];
                int prev = p[i - 1];

                int mid1 = cur + len;
                if (mid1 >= size) mid1 -= size;

                int mid2 = prev + len;
                if (mid2 >= size) mid2 -= size;

                if (cls[cur] != cls[prev] || cls[mid1] != cls[mid2])
                    newClasses++;

                cn[cur] = newClasses - 1;
            }

            var tmp = cls;
            cls = cn;
            cn = tmp;

            classes = newClasses;
            h++;
        }

        int N = total;
        int[] sa = new int[N];
        for (int i = 1; i < size; i++)
            sa[i - 1] = p[i];

        int[] rank = new int[N];
        for (int i = 0; i < N; i++)
            rank[sa[i]] = i;

        int[] lcp = new int[Math.Max(0, N - 1)];
        int k = 0;
        for (int i = 0; i < N; i++)
        {
            int r = rank[i];
            if (r == N - 1)
            {
                k = 0;
                continue;
            }

            int j = sa[r + 1];
            while (i + k < N && j + k < N && arr[i + k] == arr[j + k])
                k++;

            lcp[r] = k;
            if (k > 0) k--;
        }

        int bestLen = 0;
        int bestPos = -1;

        for (int i = 1; i < N; i++)
        {
            int x = sa[i - 1];
            int y = sa[i];

            int ox = GetOwner(x, n, m);
            int oy = GetOwner(y, n, m);

            if (ox == -1 || oy == -1 || ox == oy)
                continue;

            int cur = lcp[i - 1];
            if (cur > bestLen)
            {
                bestLen = cur;
                bestPos = x;
            }
        }

        if (bestLen == 0)
        {
            Console.WriteLine();
            return;
        }

        if (bestPos < n)
            Console.WriteLine(s.Substring(bestPos, bestLen));
        else
            Console.WriteLine(t.Substring(bestPos - (n + 1), bestLen));
    }

    static int GetOwner(int pos, int n, int m)
    {
        if (pos < n) return 0;
        if (pos == n) return -1;
        if (pos < n + 1 + m) return 1;
        return -1;
    }
}
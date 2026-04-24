using System;

class Program
{
    static void Main()
    {
        string str = Console.ReadLine();
        int k = int.Parse(Console.ReadLine());

        int n = str.Length;

        int[] order = new int[n];
        int[] cls = new int[n];
        int[] newOrder = new int[n];
        int[] newCls = new int[n];

        int alphabet = 128;
        int[] count = new int[Math.Max(alphabet, n)];

        for (int i = 0; i < n; i++)
            count[str[i]]++;

        for (int i = 1; i < alphabet; i++)
            count[i] += count[i - 1];

        for (int i = n - 1; i >= 0; i--)
            order[--count[str[i]]] = i;

        cls[order[0]] = 0;
        int clsCount = 1;

        for (int i = 1; i < n; i++)
        {
            if (str[order[i]] != str[order[i - 1]])
                clsCount++;
            cls[order[i]] = clsCount - 1;
        }

        int len = 1;
        while (len < n)
        {
            for (int i = 0; i < n; i++)
            {
                newOrder[i] = order[i] - len;
                if (newOrder[i] < 0)
                    newOrder[i] += n;
            }

            Array.Clear(count, 0, clsCount);

            for (int i = 0; i < n; i++)
                count[cls[newOrder[i]]]++;

            for (int i = 1; i < clsCount; i++)
                count[i] += count[i - 1];

            for (int i = n - 1; i >= 0; i--)
                order[--count[cls[newOrder[i]]]] = newOrder[i];

            newCls[order[0]] = 0;
            int updated = 1;

            for (int i = 1; i < n; i++)
            {
                int cur = order[i];
                int prev = order[i - 1];

                int midCur = (cur + len) % n;
                int midPrev = (prev + len) % n;

                if (cls[cur] != cls[prev] || cls[midCur] != cls[midPrev])
                    updated++;

                newCls[cur] = updated - 1;
            }

            var tmp = cls;
            cls = newCls;
            newCls = tmp;

            clsCount = updated;
            len <<= 1;
        }

        if (k > clsCount)
        {
            Console.WriteLine("IMPOSSIBLE");
            return;
        }

        int need = 0;
        int startIndex = -1;

        for (int i = 0; i < n; i++)
        {
            if (i == 0 || cls[order[i]] != cls[order[i - 1]])
            {
                need++;
                if (need == k)
                {
                    startIndex = order[i];
                    break;
                }
            }
        }

        char[] result = new char[n];
        for (int i = 0; i < n; i++)
            result[i] = str[(startIndex + i) % n];

        Console.WriteLine(new string(result));
    }
}
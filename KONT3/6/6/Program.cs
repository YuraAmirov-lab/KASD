using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int k = int.Parse(Console.ReadLine());
        string[] arr = new string[k];
        for (int i = 0; i < k; i++)
            arr[i] = Console.ReadLine();

        Array.Sort(arr, (a, b) => a.Length.CompareTo(b.Length));
        string shortest = arr[0];
        int left = 0, right = shortest.Length;
        string answer = "";

        while (left <= right)
        {
            int mid = (left + right) / 2;
            if (Check(arr, mid, out string found))
            {
                answer = found;
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        Console.WriteLine(answer);
    }

    static bool Check(string[] arr, int len, out string result)
    {
        result = "";
        if (len == 0)
        {
            result = "";
            return true;
        }

        HashSet<string> current = new HashSet<string>();
        string first = arr[0];

        for (int i = 0; i + len <= first.Length; i++)
            current.Add(first.Substring(i, len));

        for (int i = 1; i < arr.Length; i++)
        {
            HashSet<string> next = new HashSet<string>();
            string s = arr[i];
            for (int j = 0; j + len <= s.Length; j++)
            {
                string sub = s.Substring(j, len);
                if (current.Contains(sub))
                    next.Add(sub);
            }
            current = next;
            if (current.Count == 0)
                return false;
        }

        foreach (var s in current)
        {
            result = s;
            return true;
        }

        return false;
    }
}
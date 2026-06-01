using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace task22
{
    public class Entry<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public Entry(K key, V value) { Key = key; Value = value; }
        public override bool Equals(object obj)
        {
            if (obj is Entry<K, V> other)
                return EqualityComparer<K>.Default.Equals(Key, other.Key) &&
                       EqualityComparer<V>.Default.Equals(Value, other.Value);
            return false;
        }
        public override int GetHashCode()
        {
            {
                int hash = 17;
                hash = hash * 23 + (Key == null ? 0 : Key.GetHashCode());
                hash = hash * 23 + (Value == null ? 0 : Value.GetHashCode());
                return hash;
            }
        }
        public override string ToString() => $"{Key} = {Value}";
    }

    class MyHashMap<K, V>
    {
        LinkedList<Entry<K, V>>[] table;
        int size;
        double loadFactor;

        public MyHashMap(int initialCapacity, double loadFactor)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("Начальная емкость должна быть больше нуля.");
            if (loadFactor <= 0 || loadFactor >= 1)
                throw new ArgumentException("Неверный loadFactor.");

            table = new LinkedList<Entry<K, V>>[initialCapacity];
            this.loadFactor = loadFactor;
            size = 0;

            for (int i = 0; i < table.Length; i++)
                table[i] = new LinkedList<Entry<K, V>>();
        }

        public MyHashMap() : this(16, 0.75) { }
        public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75) { }

        public void Clear()
        {
            for (int i = 0; i < table.Length; i++)
                table[i].Clear();
            size = 0;
        }

        private int GetIndex(K key)
        {
            int hash = key.GetHashCode();
            if (hash < 0) hash = -hash;
            return hash % table.Length;
        }

        public bool ContainsKey(object key)
        {
            if (key == null) return false;

            int index = GetIndex((K)key);

            foreach (var entry in table[index])
                if (entry.Key.Equals(key))
                    return true;

            return false;
        }

        public bool ContainsValue(object value)
        {
            foreach (var bucket in table)
                foreach (var entry in bucket)
                    if (entry.Value.Equals(value))
                        return true;

            return false;
        }

        public V Get(object key)
        {
            if (key == null) return default(V);

            int index = GetIndex((K)key);

            foreach (var entry in table[index])
                if (entry.Key.Equals(key))
                    return entry.Value;

            return default(V);
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public HashSet<K> KeySet()
        {
            HashSet<K> set = new HashSet<K>();

            foreach (var bucket in table)
                foreach (var entry in bucket)
                    set.Add(entry.Key);

            return set;
        }

        public HashSet<Entry<K, V>> EntrySet()
        {
            HashSet<Entry<K, V>> set = new HashSet<Entry<K, V>>();

            foreach (var bucket in table)
                foreach (var entry in bucket)
                    set.Add(entry);

            return set;
        }

        public void Put(K key, V value)
        {
            if (key == null)
                throw new ArgumentNullException("Ключ не может быть null.");

            int index = GetIndex(key);

            foreach (var entry in table[index])
            {
                if (entry.Key.Equals(key))
                {
                    entry.Value = value;
                    return;
                }
            }

            table[index].AddLast(new Entry<K, V>(key, value));
            size++;

            if ((double)size / table.Length >= loadFactor)
                Resize();
        }

        private void Resize()
        {
            LinkedList<Entry<K, V>>[] oldTable = table;

            table = new LinkedList<Entry<K, V>>[oldTable.Length * 2];

            for (int i = 0; i < table.Length; i++)
                table[i] = new LinkedList<Entry<K, V>>();

            foreach (var bucket in oldTable)
                foreach (var entry in bucket)
                {
                    int index = GetIndex(entry.Key);
                    table[index].AddLast(entry);
                }
        }

        public bool Remove(object key)
        {
            if (key == null) return false;

            int index = GetIndex((K)key);

            foreach (var entry in table[index])
            {
                if (entry.Key.Equals(key))
                {
                    table[index].Remove(entry);
                    size--;
                    return true;
                }
            }

            return false;
        }

        public int Size()
        {
            return size;
        }
    }

    internal class Program
    {
        static string NormalizeTag(string tag)
        {
            string content = tag.Trim('<', '>').TrimStart('/');
            return content.ToLowerInvariant();
        }

        static void Main(string[] args)
        {
            MyHashMap<string, int> tagCount = new MyHashMap<string, int>();
            try
            {
                using (StreamReader sr = new StreamReader("input.txt"))
                {
                    string line;
                    string pattern = @"<\/?[a-zA-Z][a-zA-Z0-9]*>";
                    while ((line = sr.ReadLine()) != null)
                    {
                        MatchCollection matches = Regex.Matches(line, pattern);
                        foreach (Match match in matches)
                        {
                            string normalized = NormalizeTag(match.Value);
                            int current = tagCount.Get(normalized);
                            tagCount.Put(normalized, current + 1);
                        }
                    }
                }
                var entries = tagCount.EntrySet();
                foreach (var entry in entries)
                    Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
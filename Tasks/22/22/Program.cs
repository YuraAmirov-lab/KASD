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
            unchecked
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
        private LinkedList<Entry<K, V>>[] table;
        private int size;
        private double loadFactor;

        public MyHashMap(int initialCapacity, double loadFactor)
        {
            if (initialCapacity <= 0) throw new ArgumentException("Начальная емкость должна быть больше нуля.");
            if (loadFactor <= 0 || loadFactor >= 1) throw new ArgumentException("LoadFactor должен быть в диапазоне (0,1).");
            size = 0;
            table = new LinkedList<Entry<K, V>>[initialCapacity];
            this.loadFactor = loadFactor;
            for (int i = 0; i < initialCapacity; i++) table[i] = new LinkedList<Entry<K, V>>();
        }
        public MyHashMap() : this(16, 0.75) { }
        public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75) { }

        private int GetIndex(K key)
        {
            int h1 = key?.GetHashCode() ?? 0;
            int h2 = h1 ^ (int)((uint)h1 >> 16);
            return (h2 & 0x7FFFFFFF) % table.Length;
        }

        public void Clear()
        {
            foreach (var bucket in table) bucket?.Clear();
            size = 0;
        }

        public bool ContainsKey(object key)
        {
            if (key is not K genericKey) return false;
            int index = GetIndex(genericKey);
            foreach (var entry in table[index])
                if (EqualityComparer<K>.Default.Equals(entry.Key, genericKey))
                    return true;
            return false;
        }

        public bool ContainsValue(object value)
        {
            if (value is not V genericValue) return false;
            foreach (var bucket in table)
                foreach (var entry in bucket)
                    if (EqualityComparer<V>.Default.Equals(entry.Value, genericValue))
                        return true;
            return false;
        }

        public HashSet<Entry<K, V>> EntrySet()
        {
            var set = new HashSet<Entry<K, V>>();
            foreach (var bucket in table)
                foreach (var entry in bucket)
                    set.Add(entry);
            return set;
        }

        public V Get(object key)
        {
            if (key is not K genericKey) return default;
            int index = GetIndex(genericKey);
            foreach (var entry in table[index])
                if (EqualityComparer<K>.Default.Equals(entry.Key, genericKey))
                    return entry.Value;
            return default;
        }

        public bool IsEmpty() => size == 0;

        public HashSet<K> KeySet()
        {
            var set = new HashSet<K>();
            foreach (var bucket in table)
                foreach (var entry in bucket)
                    set.Add(entry.Key);
            return set;
        }

        public void Put(K key, V value)
        {
            if (key == null) throw new ArgumentNullException("Ключ не может быть null.");
            int index = GetIndex(key);
            foreach (var entry in table[index])
            {
                if (EqualityComparer<K>.Default.Equals(entry.Key, key))
                {
                    entry.Value = value;
                    return;
                }
            }
            table[index].AddLast(new Entry<K, V>(key, value));
            size++;
            if ((double)size / table.Length >= loadFactor) Resize();
        }

        private void Resize()
        {
            var oldTable = table;
            table = new LinkedList<Entry<K, V>>[table.Length * 2];
            for (int i = 0; i < table.Length; i++) table[i] = new LinkedList<Entry<K, V>>();
            foreach (var bucket in oldTable)
                foreach (var entry in bucket)
                {
                    int newIndex = GetIndex(entry.Key);
                    table[newIndex].AddLast(entry);
                }
        }

        public bool Remove(object key)
        {
            if (key is not K genericKey) return false;
            int index = GetIndex(genericKey);
            var bucket = table[index];
            var node = bucket.First;
            while (node != null)
            {
                if (EqualityComparer<K>.Default.Equals(node.Value.Key, genericKey))
                {
                    bucket.Remove(node);
                    size--;
                    return true;
                }
                node = node.Next;
            }
            return false;
        }

        public int Size() => size;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace task23
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

    public enum VarType { Int, Float, Double, Unknown }

    public class VariableInfo
    {
        public VarType Type { get; set; }
        public string Value { get; set; }
        public int LineNumber { get; set; }
        public VariableInfo(VarType type, string value, int line)
        {
            Type = type;
            Value = value;
            LineNumber = line;
        }
        public override string ToString()
        {
            string typeStr = Type switch
            {
                VarType.Int => "int",
                VarType.Float => "float",
                VarType.Double => "double",
                _ => "unknown"
            };
            return $"{typeStr} => {Value}";
        }
    }

    internal class Program
    {
        static void Main()
        {
            MyHashMap<string, VariableInfo> map = new MyHashMap<string, VariableInfo>();
            List<string> errors = new List<string>();
            List<string> redefinitions = new List<string>();

            try
            {
                string content = File.ReadAllText("input.txt");
                string pattern = @"(?<type>int|float|double)\s+(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(?<value>[^;]+);";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(content);
                int lineCount = 1;
                string[] lines = File.ReadAllLines("input.txt");
                for (int idx = 0; idx < lines.Length; idx++)
                {
                    string line = lines[idx];
                    if (string.IsNullOrWhiteSpace(line)) { lineCount += line.Split('\n').Length; continue; }
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        string typeStr = match.Groups["type"].Value.ToLower();
                        string name = match.Groups["name"].Value;
                        string value = match.Groups["value"].Value.Trim();
                        VarType type = typeStr switch
                        {
                            "int" => VarType.Int,
                            "float" => VarType.Float,
                            "double" => VarType.Double,
                            _ => VarType.Unknown
                        };
                        if (!map.ContainsKey(name))
                        {
                            map.Put(name, new VariableInfo(type, value, idx + 1));
                        }
                        else
                        {
                            redefinitions.Add($"{name} (строка {idx + 1}) - оставлено первое значение");
                        }
                    }
                    else if (line.Trim().Length > 0 && !line.Trim().StartsWith("//"))
                    {
                        errors.Add($"строка {idx + 1}: {line.Trim()} - синтаксическая ошибка или неизвестный тип");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл input.txt не найден.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return;
            }

            Console.WriteLine("Результат:");
            foreach (var entry in map.EntrySet())
            {
                Console.WriteLine($"{entry.Value}");
            }

            if (errors.Count > 0)
            {
                Console.WriteLine("\nОбнаружены некорректные определения:");
                foreach (var err in errors) Console.WriteLine($"  {err}");
            }

            if (redefinitions.Count > 0)
            {
                Console.WriteLine("\nОбнаружены переопределения:");
                foreach (var red in redefinitions) Console.WriteLine($"  {red}");
            }
        }
    }
}
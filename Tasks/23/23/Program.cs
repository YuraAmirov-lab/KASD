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

            string pattern = @"(?<type>int|float|double)\s+(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(?<value>[^;]+);";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            try
            {
                string[] lines = File.ReadAllLines("input.txt");

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        continue;

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
                            map.Put(name, new VariableInfo(type, value, i + 1));
                        }
                        else
                        {
                            redefinitions.Add($"{name} (строка {i + 1}) - оставлено первое значение");
                        }
                    }
                    else
                    {
                        errors.Add($"строка {i + 1}: {line} - синтаксическая ошибка или неизвестный тип");
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
                Console.WriteLine(entry.Value);
            }

            if (errors.Count > 0)
            {
                Console.WriteLine("\nОбнаружены некорректные определения:");
                foreach (var err in errors)
                    Console.WriteLine("  " + err);
            }

            if (redefinitions.Count > 0)
            {
                Console.WriteLine("\nОбнаружены переопределения:");
                foreach (var red in redefinitions)
                    Console.WriteLine("  " + red);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;

namespace task21
{
    internal class Program
    {
        public class Entry<K, V>
        {
            public K Key { get; set; }
            public V Value { get; set; }

            public Entry(K key, V value)
            {
                Key = key;
                Value = value;
            }

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

            public override string ToString()
            {
                return $"{Key} = {Value}";
            }
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

        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование MyHashMap (задание 21) ===\n");
            var map = new MyHashMap<string, int>();
            Console.WriteLine("Создана пустая карта. Size = " + map.Size() + ", IsEmpty = " + map.IsEmpty());

            map.Put("один", 1);
            map.Put("два", 2);
            map.Put("три", 3);
            map.Put("четыре", 4);
            Console.WriteLine("После добавления 4 элементов. Size = " + map.Size());

            Console.WriteLine("\ncontainsKey(\"два\"): " + map.ContainsKey("два"));
            Console.WriteLine("containsKey(\"пять\"): " + map.ContainsKey("пять"));
            Console.WriteLine("containsValue(3): " + map.ContainsValue(3));
            Console.WriteLine("containsValue(99): " + map.ContainsValue(99));

            Console.WriteLine("\nGet(\"три\"): " + map.Get("три"));
            Console.WriteLine("Get(\"десять\"): " + map.Get("десять")); // default(int) = 0

            Console.Write("\nKeySet: ");
            foreach (var k in map.KeySet()) Console.Write(k + " ");
            Console.WriteLine();

            Console.WriteLine("EntrySet:");
            foreach (var e in map.EntrySet()) Console.WriteLine("  " + e);

            map.Remove("два");
            Console.WriteLine("\nПосле удаления \"два\". Size = " + map.Size());
            Console.WriteLine("containsKey(\"два\"): " + map.ContainsKey("два"));

            map.Clear();
            Console.WriteLine("\nПосле clear(): size = " + map.Size() + ", IsEmpty = " + map.IsEmpty());

            var doubleMap = new MyHashMap<double, string>(5);
            doubleMap.Put(3.14, "pi");
            doubleMap.Put(2.71, "e");
            Console.WriteLine("\nКарта с double -> string: " + doubleMap.Get(2.71));

            Console.WriteLine("\nВсе тесты завершены.");
        }
    }
}
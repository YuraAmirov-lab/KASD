using System;
using System.Collections.Generic;

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
                unchecked
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
                    throw new ArgumentException("LoadFactor должен быть в диапазоне (0,1).");

                size = 0;
                table = new LinkedList<Entry<K, V>>[initialCapacity];
                this.loadFactor = loadFactor;

                for (int i = 0; i < initialCapacity; i++)
                    table[i] = new LinkedList<Entry<K, V>>();
            }

            public MyHashMap() : this(16, 0.75) { }
            public MyHashMap(int initialCapacity) : this(initialCapacity, 0.75) { }

            public void Clear()
            {
                foreach (var bucket in table)
                    bucket?.Clear();
                size = 0;
            }

            private int GetIndex(K key)
            {
                int h1 = key?.GetHashCode() ?? 0;
                int h2 = h1 ^ (int)((uint)h1 >> 16);
                return (h2 & 0x7FFFFFFF) % table.Length;
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
                if ((double)size / table.Length >= loadFactor)
                    Resize();
            }

            private void Resize()
            {
                var oldTable = table;
                table = new LinkedList<Entry<K, V>>[table.Length * 2];
                for (int i = 0; i < table.Length; i++)
                    table[i] = new LinkedList<Entry<K, V>>();

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
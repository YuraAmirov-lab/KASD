using System;
using System.Collections.Generic;

public class MyTreeMap
{
    private class Node
    {
        public object Key;
        public object Value;
        public Node Left;
        public Node Right;

        public Node(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    private IComparer<object> comparer;
    private Node root;
    private int size;

    public MyTreeMap()
    {
        comparer = Comparer<object>.Default;
    }

    public MyTreeMap(IComparer<object> comp)
    {
        if (comp == null)
            throw new ArgumentNullException("comp");
        comparer = comp;
    }

    public void Clear()
    {
        root = null;
        size = 0;
    }

    public bool ContainsKey(object key)
    {
        return FindNode(key) != null;
    }

    public bool ContainsValue(object value)
    {
        return ContainsValue(root, value);
    }

    private bool ContainsValue(Node node, object value)
    {
        if (node == null)
            return false;
        if (Equals(value, node.Value))
            return true;
        if (ContainsValue(node.Left, value))
            return true;
        if (ContainsValue(node.Right, value))
            return true;
        return false;
    }

    public List<KeyValuePair<object, object>> EntrySet()
    {
        var list = new List<KeyValuePair<object, object>>();
        FillEntries(root, list);
        return list;
    }

    private void FillEntries(Node node, List<KeyValuePair<object, object>> list)
    {
        if (node == null)
            return;
        FillEntries(node.Left, list);
        list.Add(new KeyValuePair<object, object>(node.Key, node.Value));
        FillEntries(node.Right, list);
    }

    public object Get(object key)
    {
        var node = FindNode(key);
        if (node != null)
            return node.Value;
        return null;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    public List<object> KeySet()
    {
        var list = new List<object>();
        FillKeys(root, list);
        return list;
    }

    private void FillKeys(Node node, List<object> list)
    {
        if (node == null)
            return;
        FillKeys(node.Left, list);
        list.Add(node.Key);
        FillKeys(node.Right, list);
    }

    public object Put(object key, object value)
    {
        if (key == null)
            throw new ArgumentNullException("key");

        if (root == null)
        {
            root = new Node(key, value);
            size++;
            return null;
        }

        Node parent = null;
        Node current = root;
        int cmp = 0;

        while (current != null)
        {
            parent = current;
            cmp = comparer.Compare(key, current.Key);
            if (cmp == 0)
            {
                var old = current.Value;
                current.Value = value;
                return old;
            }
            else if (cmp < 0)
            {
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }

        var newNode = new Node(key, value);
        if (cmp < 0)
            parent.Left = newNode;
        else
            parent.Right = newNode;
        size++;
        return null;
    }

    public object Remove(object key)
    {
        var node = FindNode(key);
        if (node == null)
            return null;

        var old = node.Value;
        DeleteNode(node);
        size--;
        return old;
    }

    public int Size()
    {
        return size;
    }

    public object FirstKey()
    {
        if (root == null)
            throw new InvalidOperationException("Map is empty");
        var node = root;
        while (node.Left != null)
            node = node.Left;
        return node.Key;
    }

    public object LastKey()
    {
        if (root == null)
            throw new InvalidOperationException("Map is empty");
        var node = root;
        while (node.Right != null)
            node = node.Right;
        return node.Key;
    }

    public MyTreeMap HeadMap(object end)
    {
        var result = new MyTreeMap(comparer);
        AddHead(root, result, end);
        return result;
    }

    private void AddHead(Node node, MyTreeMap target, object end)
    {
        if (node == null)
            return;
        AddHead(node.Left, target, end);
        if (comparer.Compare(node.Key, end) < 0)
            target.Put(node.Key, node.Value);
        AddHead(node.Right, target, end);
    }

    public MyTreeMap SubMap(object start, object end)
    {
        if (comparer.Compare(start, end) >= 0)
            throw new ArgumentException("start must be less than end");
        var result = new MyTreeMap(comparer);
        AddSub(root, result, start, end);
        return result;
    }

    private void AddSub(Node node, MyTreeMap target, object start, object end)
    {
        if (node == null)
            return;
        AddSub(node.Left, target, start, end);
        if (comparer.Compare(node.Key, start) >= 0 && comparer.Compare(node.Key, end) < 0)
            target.Put(node.Key, node.Value);
        AddSub(node.Right, target, start, end);
    }

    public MyTreeMap TailMap(object start)
    {
        var result = new MyTreeMap(comparer);
        AddTail(root, result, start);
        return result;
    }

    private void AddTail(Node node, MyTreeMap target, object start)
    {
        if (node == null)
            return;
        AddTail(node.Left, target, start);
        if (comparer.Compare(node.Key, start) > 0)
            target.Put(node.Key, node.Value);
        AddTail(node.Right, target, start);
    }

    public bool TryLowerEntry(object key, out KeyValuePair<object, object> entry)
    {
        if (key == null)
            throw new ArgumentNullException("key");
        Node candidate = null;
        Node current = root;
        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, key);
            if (cmp < 0)
            {
                candidate = current;
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }
        if (candidate != null)
        {
            entry = new KeyValuePair<object, object>(candidate.Key, candidate.Value);
            return true;
        }
        entry = default(KeyValuePair<object, object>);
        return false;
    }

    public bool TryFloorEntry(object key, out KeyValuePair<object, object> entry)
    {
        if (key == null)
            throw new ArgumentNullException("key");
        Node candidate = null;
        Node current = root;
        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, key);
            if (cmp <= 0)
            {
                candidate = current;
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }
        if (candidate != null)
        {
            entry = new KeyValuePair<object, object>(candidate.Key, candidate.Value);
            return true;
        }
        entry = default(KeyValuePair<object, object>);
        return false;
    }

    public bool TryHigherEntry(object key, out KeyValuePair<object, object> entry)
    {
        if (key == null)
            throw new ArgumentNullException("key");
        Node candidate = null;
        Node current = root;
        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, key);
            if (cmp > 0)
            {
                candidate = current;
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }
        if (candidate != null)
        {
            entry = new KeyValuePair<object, object>(candidate.Key, candidate.Value);
            return true;
        }
        entry = default(KeyValuePair<object, object>);
        return false;
    }

    public bool TryCeilingEntry(object key, out KeyValuePair<object, object> entry)
    {
        if (key == null)
            throw new ArgumentNullException("key");
        Node candidate = null;
        Node current = root;
        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, key);
            if (cmp >= 0)
            {
                candidate = current;
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }
        if (candidate != null)
        {
            entry = new KeyValuePair<object, object>(candidate.Key, candidate.Value);
            return true;
        }
        entry = default(KeyValuePair<object, object>);
        return false;
    }

    public object LowerKey(object key)
    {
        if (TryLowerEntry(key, out var entry))
            return entry.Key;
        return null;
    }

    public object FloorKey(object key)
    {
        if (TryFloorEntry(key, out var entry))
            return entry.Key;
        return null;
    }

    public object HigherKey(object key)
    {
        if (TryHigherEntry(key, out var entry))
            return entry.Key;
        return null;
    }

    public object CeilingKey(object key)
    {
        if (TryCeilingEntry(key, out var entry))
            return entry.Key;
        return null;
    }

    public bool TryPollFirstEntry(out KeyValuePair<object, object> entry)
    {
        if (root == null)
        {
            entry = default(KeyValuePair<object, object>);
            return false;
        }
        var min = root;
        while (min.Left != null)
            min = min.Left;
        entry = new KeyValuePair<object, object>(min.Key, min.Value);
        DeleteNode(min);
        size--;
        return true;
    }

    public bool TryPollLastEntry(out KeyValuePair<object, object> entry)
    {
        if (root == null)
        {
            entry = default(KeyValuePair<object, object>);
            return false;
        }
        var max = root;
        while (max.Right != null)
            max = max.Right;
        entry = new KeyValuePair<object, object>(max.Key, max.Value);
        DeleteNode(max);
        size--;
        return true;
    }

    public bool TryFirstEntry(out KeyValuePair<object, object> entry)
    {
        if (root == null)
        {
            entry = default(KeyValuePair<object, object>);
            return false;
        }
        var min = root;
        while (min.Left != null)
            min = min.Left;
        entry = new KeyValuePair<object, object>(min.Key, min.Value);
        return true;
    }

    public bool TryLastEntry(out KeyValuePair<object, object> entry)
    {
        if (root == null)
        {
            entry = default(KeyValuePair<object, object>);
            return false;
        }
        var max = root;
        while (max.Right != null)
            max = max.Right;
        entry = new KeyValuePair<object, object>(max.Key, max.Value);
        return true;
    }

    private Node FindNode(object key)
    {
        var current = root;
        while (current != null)
        {
            int cmp = comparer.Compare(key, current.Key);
            if (cmp == 0)
                return current;
            else if (cmp < 0)
                current = current.Left;
            else
                current = current.Right;
        }
        return null;
    }

    private Node FindParent(object key)
    {
        if (root == null || comparer.Compare(key, root.Key) == 0)
            return null;

        var current = root;
        Node parent = null;
        while (current != null)
        {
            int cmp = comparer.Compare(key, current.Key);
            if (cmp == 0)
                return parent;
            parent = current;
            if (cmp < 0)
                current = current.Left;
            else
                current = current.Right;
        }
        return null;
    }

    private void DeleteNode(Node node)
    {
        var parent = FindParent(node.Key);
        bool isLeft = (parent != null && parent.Left == node);

        if (node.Left == null && node.Right == null)
        {
            if (parent == null)
                root = null;
            else if (isLeft)
                parent.Left = null;
            else
                parent.Right = null;
        }
        else if (node.Left == null)
        {
            if (parent == null)
                root = node.Right;
            else if (isLeft)
                parent.Left = node.Right;
            else
                parent.Right = node.Right;
        }
        else if (node.Right == null)
        {
            if (parent == null)
                root = node.Left;
            else if (isLeft)
                parent.Left = node.Left;
            else
                parent.Right = node.Left;
        }
        else
        {
            var minRight = node.Right;
            while (minRight.Left != null)
                minRight = minRight.Left;

            var minKey = minRight.Key;
            var minValue = minRight.Value;

            DeleteNode(minRight);

            node.Key = minKey;
            node.Value = minValue;
        }
    }
}

public class MyTreeSet
{
    private object DUMMY = new object();
    private MyTreeMap m;

    public MyTreeSet()
    {
        m = new MyTreeMap();
    }

    public MyTreeSet(MyTreeMap map)
    {
        if (map == null)
            throw new ArgumentNullException("map");
        m = map;
    }

    public MyTreeSet(IComparer<object> comparator)
    {
        if (comparator == null)
            throw new ArgumentNullException("comparator");
        m = new MyTreeMap(comparator);
    }

    public MyTreeSet(object[] a)
    {
        m = new MyTreeMap();
        if (a == null)
            throw new ArgumentNullException("a");
        for (int i = 0; i < a.Length; i++)
            m.Put(a[i], DUMMY);
    }

    public MyTreeSet(MyTreeSet s)
    {
        m = new MyTreeMap();
        if (s == null)
            throw new ArgumentNullException("s");
        var keys = s.m.KeySet();
        for (int i = 0; i < keys.Count; i++)
            m.Put(keys[i], DUMMY);
    }

    public bool Add(object e)
    {
        if (e == null)
            throw new ArgumentNullException("e");
        if (m.ContainsKey(e))
            return false;
        m.Put(e, DUMMY);
        return true;
    }

    public bool AddAll(object[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");
        bool modified = false;
        for (int i = 0; i < a.Length; i++)
        {
            if (Add(a[i]))
                modified = true;
        }
        return modified;
    }

    public void Clear()
    {
        m.Clear();
    }

    public bool Contains(object o)
    {
        return m.ContainsKey(o);
    }

    public bool ContainsAll(object[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");
        for (int i = 0; i < a.Length; i++)
        {
            if (!m.ContainsKey(a[i]))
                return false;
        }
        return true;
    }

    public bool IsEmpty()
    {
        return m.IsEmpty();
    }

    public bool Remove(object o)
    {
        if (!m.ContainsKey(o))
            return false;
        m.Remove(o);
        return true;
    }

    public bool RemoveAll(object[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");
        bool modified = false;
        for (int i = 0; i < a.Length; i++)
        {
            if (Remove(a[i]))
                modified = true;
        }
        return modified;
    }

    public bool RetainAll(object[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");
        var toKeep = new HashSet<object>();
        for (int i = 0; i < a.Length; i++)
            toKeep.Add(a[i]);

        var keys = m.KeySet();
        bool modified = false;
        for (int i = 0; i < keys.Count; i++)
        {
            if (!toKeep.Contains(keys[i]))
            {
                m.Remove(keys[i]);
                modified = true;
            }
        }
        return modified;
    }

    public int Size()
    {
        return m.Size();
    }

    public object[] ToArray()
    {
        var keys = m.KeySet();
        return keys.ToArray();
    }

    public object[] ToArray(object[] a)
    {
        var keys = m.KeySet();
        if (a == null || a.Length < keys.Count)
        {
            a = new object[keys.Count];
        }
        for (int i = 0; i < keys.Count; i++)
            a[i] = keys[i];
        return a;
    }

    public object First()
    {
        return m.FirstKey();
    }

    public object Last()
    {
        return m.LastKey();
    }

    public MyTreeSet SubSet(object fromElement, object toElement)
    {
        var sub = m.SubMap(fromElement, toElement);
        return new MyTreeSet(sub);
    }

    public MyTreeSet HeadSet(object toElement)
    {
        var head = m.HeadMap(toElement);
        return new MyTreeSet(head);
    }

    public MyTreeSet TailSet(object fromElement)
    {
        var tail = m.TailMap(fromElement);
        var result = new MyTreeSet();
        if (m.ContainsKey(fromElement))
            result.Add(fromElement);
        var keys = tail.KeySet();
        for (int i = 0; i < keys.Count; i++)
            result.Add(keys[i]);
        return result;
    }

    public object Ceiling(object obj)
    {
        return m.CeilingKey(obj);
    }

    public object Floor(object obj)
    {
        return m.FloorKey(obj);
    }

    public object Higher(object obj)
    {
        return m.HigherKey(obj);
    }

    public object Lower(object obj)
    {
        return m.LowerKey(obj);
    }

    public MyTreeSet HeadSet(object upperBound, bool incl)
    {
        var result = HeadSet(upperBound);
        if (incl && m.ContainsKey(upperBound))
            result.Add(upperBound);
        return result;
    }

    public MyTreeSet SubSet(object lowerBound, bool lowIncl, object upperBound, bool highIncl)
    {
        var allKeys = m.KeySet();
        var result = new MyTreeSet();
        var comp = Comparer<object>.Default;

        for (int i = 0; i < allKeys.Count; i++)
        {
            var k = allKeys[i];
            int cmpLow = comp.Compare(k, lowerBound);
            int cmpHigh = comp.Compare(k, upperBound);

            bool aboveLow = lowIncl ? cmpLow >= 0 : cmpLow > 0;
            bool belowHigh = highIncl ? cmpHigh <= 0 : cmpHigh < 0;

            if (aboveLow && belowHigh)
                result.Add(k);
        }
        return result;
    }

    public MyTreeSet TailSet(object fromElement, bool inclusive)
    {
        var allKeys = m.KeySet();
        var result = new MyTreeSet();
        var comp = Comparer<object>.Default;

        for (int i = 0; i < allKeys.Count; i++)
        {
            var k = allKeys[i];
            int c = comp.Compare(k, fromElement);
            if (inclusive ? c >= 0 : c > 0)
                result.Add(k);
        }
        return result;
    }

    public object PollLast()
    {
        if (m.IsEmpty())
            return null;
        if (m.TryPollLastEntry(out var entry))
            return entry.Key;
        return null;
    }

    public object PollFirst()
    {
        if (m.IsEmpty())
            return null;
        if (m.TryPollFirstEntry(out var entry))
            return entry.Key;
        return null;
    }

    public List<object> DescendingIterator()
    {
        var keys = m.KeySet();
        var result = new List<object>();
        for (int i = keys.Count - 1; i >= 0; i--)
            result.Add(keys[i]);
        return result;
    }

    public MyTreeSet DescendingSet()
    {
        var keys = m.KeySet();
        var reverseComp = new ReverseComparer();
        var result = new MyTreeSet(reverseComp);
        for (int i = 0; i < keys.Count; i++)
            result.Add(keys[i]);
        return result;
    }

    private class ReverseComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            return Comparer<object>.Default.Compare(y, x);
        }
    }
}
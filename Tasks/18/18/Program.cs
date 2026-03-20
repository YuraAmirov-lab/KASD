using System;
using System.Collections.Generic;

public class MyTreeMap
{
    class Node
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
}
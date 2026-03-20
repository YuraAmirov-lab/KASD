using System;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        MyTreeSet<int> set = new MyTreeSet<int>();

        set.Add(5);
        set.Add(2);
        set.Add(8);
        set.Add(1);
        set.Add(10);

        Console.WriteLine("Размер: " + set.Size());
        Console.WriteLine("Первый: " + set.First());
        Console.WriteLine("Последний: " + set.Last());

        Console.WriteLine("Содержит 8: " + set.Contains(8));
        Console.WriteLine("Ceiling(6): " + set.Ceiling(6));
        Console.WriteLine("Floor(6): " + set.Floor(6));
        Console.WriteLine("Higher(5): " + set.Higher(5));
        Console.WriteLine("Lower(5): " + set.Lower(5));

        int[] arr = set.ToArray();
        Console.WriteLine("Элементы:");
        for (int i = 0; i < arr.Length; i++)
            Console.Write(arr[i] + " ");
        Console.WriteLine();

        MyTreeSet<int> sub = set.SubSet(2, 10);
        Console.WriteLine("Подмножество [2;10):");
        int[] arr2 = sub.ToArray();
        for (int i = 0; i < arr2.Length; i++)
            Console.Write(arr2[i] + " ");
        Console.WriteLine();

        Console.WriteLine("PollFirst: " + set.PollFirst());
        Console.WriteLine("PollLast: " + set.PollLast());

        Console.WriteLine("После удаления первого и последнего:");
        int[] arr3 = set.ToArray();
        for (int i = 0; i < arr3.Length; i++)
            Console.Write(arr3[i] + " ");
        Console.WriteLine();

        Console.WriteLine("Обратный порядок:");
        List<int> desc = set.DescendingIterator();
        for (int i = 0; i < desc.Count; i++)
            Console.Write(desc[i] + " ");
        Console.WriteLine();
    }
}
public class MyTreeSet<E>
{
    private const bool RED = true;
    private const bool BLACK = false;

    private class Node
    {
        public E Key;
        public Node Left;
        public Node Right;
        public Node Parent;
        public bool Color;

        public Node(E key, bool color, Node parent)
        {
            Key = key;
            Color = color;
            Parent = parent;
        }
    }

    private Node root;
    private int size;
    private IComparer<E> comparer;

    public MyTreeSet()
    {
        comparer = Comparer<E>.Default;
    }

    public MyTreeSet(IComparer<E> comparator)
    {
        if (comparator == null)
            throw new ArgumentNullException("comparator");
        comparer = comparator;
    }

    public MyTreeSet(E[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");

        comparer = Comparer<E>.Default;

        for (int i = 0; i < a.Length; i++)
            Add(a[i]);
    }

    public MyTreeSet(MyTreeSet<E> s)
    {
        if (s == null)
            throw new ArgumentNullException("s");

        comparer = s.comparer;
        E[] arr = s.ToArray();
        for (int i = 0; i < arr.Length; i++)
            Add(arr[i]);
    }

    public bool Add(E e)
    {
        if (e == null)
            throw new ArgumentNullException("e");

        if (root == null)
        {
            root = new Node(e, BLACK, null);
            size = 1;
            return true;
        }

        Node parent = null;
        Node current = root;
        int cmp = 0;

        while (current != null)
        {
            parent = current;
            cmp = comparer.Compare(e, current.Key);

            if (cmp == 0)
                return false;
            else if (cmp < 0)
                current = current.Left;
            else
                current = current.Right;
        }

        Node newNode = new Node(e, RED, parent);

        if (cmp < 0)
            parent.Left = newNode;
        else
            parent.Right = newNode;

        FixAfterInsert(newNode);
        size++;
        return true;
    }

    public bool AddAll(E[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");

        bool changed = false;
        for (int i = 0; i < a.Length; i++)
        {
            if (Add(a[i]))
                changed = true;
        }
        return changed;
    }

    public void Clear()
    {
        root = null;
        size = 0;
    }

    public bool Contains(object o)
    {
        if (o == null)
            return false;

        return FindNode((E)o) != null;
    }

    public bool ContainsAll(E[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");

        for (int i = 0; i < a.Length; i++)
        {
            if (!Contains(a[i]))
                return false;
        }
        return true;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    public bool Remove(object o)
    {
        if (o == null)
            return false;

        Node p = FindNode((E)o);
        if (p == null)
            return false;

        DeleteNode(p);
        size--;
        return true;
    }

    public bool RemoveAll(E[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");

        bool changed = false;
        for (int i = 0; i < a.Length; i++)
        {
            if (Remove(a[i]))
                changed = true;
        }
        return changed;
    }

    public bool RetainAll(E[] a)
    {
        if (a == null)
            throw new ArgumentNullException("a");

        HashSet<E> keep = new HashSet<E>();
        for (int i = 0; i < a.Length; i++)
            keep.Add(a[i]);

        E[] arr = ToArray();
        bool changed = false;

        for (int i = 0; i < arr.Length; i++)
        {
            if (!keep.Contains(arr[i]))
            {
                Remove(arr[i]);
                changed = true;
            }
        }

        return changed;
    }

    public int Size()
    {
        return size;
    }

    public E[] ToArray()
    {
        List<E> list = new List<E>();
        InOrder(root, list);
        return list.ToArray();
    }

    public E[] ToArray(E[] a)
    {
        E[] data = ToArray();

        if (a == null || a.Length < data.Length)
            a = new E[data.Length];

        for (int i = 0; i < data.Length; i++)
            a[i] = data[i];

        return a;
    }

    public E First()
    {
        if (root == null)
            throw new InvalidOperationException("Set is empty");

        return Minimum(root).Key;
    }

    public E Last()
    {
        if (root == null)
            throw new InvalidOperationException("Set is empty");

        return Maximum(root).Key;
    }

    public MyTreeSet<E> SubSet(E fromElement, E toElement)
    {
        if (comparer.Compare(fromElement, toElement) > 0)
            throw new ArgumentException("fromElement > toElement");

        MyTreeSet<E> result = new MyTreeSet<E>(comparer);
        FillSubSet(root, result, fromElement, true, toElement, false);
        return result;
    }

    public MyTreeSet<E> HeadSet(E toElement)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(comparer);
        FillHeadSet(root, result, toElement, false);
        return result;
    }

    public MyTreeSet<E> TailSet(E fromElement)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(comparer);
        FillTailSet(root, result, fromElement, true);
        return result;
    }

    public E Ceiling(E obj)
    {
        Node current = root;
        Node candidate = null;

        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, obj);

            if (cmp == 0)
                return current.Key;

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

        if (candidate == null)
            return default(E);

        return candidate.Key;
    }

    public E Floor(E obj)
    {
        Node current = root;
        Node candidate = null;

        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, obj);

            if (cmp == 0)
                return current.Key;

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

        if (candidate == null)
            return default(E);

        return candidate.Key;
    }

    public E Higher(E obj)
    {
        Node current = root;
        Node candidate = null;

        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, obj);

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

        if (candidate == null)
            return default(E);

        return candidate.Key;
    }

    public E Lower(E obj)
    {
        Node current = root;
        Node candidate = null;

        while (current != null)
        {
            int cmp = comparer.Compare(current.Key, obj);

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

        if (candidate == null)
            return default(E);

        return candidate.Key;
    }

    public MyTreeSet<E> HeadSet(E upperBound, bool incl)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(comparer);
        FillHeadSet(root, result, upperBound, incl);
        return result;
    }

    public MyTreeSet<E> SubSet(E lowerBound, bool lowIncl, E upperBound, bool highIncl)
    {
        if (comparer.Compare(lowerBound, upperBound) > 0)
            throw new ArgumentException("lowerBound > upperBound");

        MyTreeSet<E> result = new MyTreeSet<E>(comparer);
        FillSubSet(root, result, lowerBound, lowIncl, upperBound, highIncl);
        return result;
    }

    public MyTreeSet<E> TailSet(E fromElement, bool inclusive)
    {
        MyTreeSet<E> result = new MyTreeSet<E>(comparer);
        FillTailSet(root, result, fromElement, inclusive);
        return result;
    }

    public E PollLast()
    {
        if (root == null)
            return default(E);

        Node max = Maximum(root);
        E ans = max.Key;
        DeleteNode(max);
        size--;
        return ans;
    }

    public E PollFirst()
    {
        if (root == null)
            return default(E);

        Node min = Minimum(root);
        E ans = min.Key;
        DeleteNode(min);
        size--;
        return ans;
    }

    public List<E> DescendingIterator()
    {
        List<E> list = new List<E>();
        ReverseInOrder(root, list);
        return list;
    }

    public MyTreeSet<E> DescendingSet()
    {
        MyTreeSet<E> result = new MyTreeSet<E>(new ReverseComparer(comparer));
        E[] arr = ToArray();
        for (int i = 0; i < arr.Length; i++)
            result.Add(arr[i]);
        return result;
    }

    private class ReverseComparer : IComparer<E>
    {
        private IComparer<E> comp;

        public ReverseComparer(IComparer<E> c)
        {
            comp = c;
        }

        public int Compare(E x, E y)
        {
            return comp.Compare(y, x);
        }
    }

    private void InOrder(Node node, List<E> list)
    {
        if (node == null)
            return;

        InOrder(node.Left, list);
        list.Add(node.Key);
        InOrder(node.Right, list);
    }

    private void ReverseInOrder(Node node, List<E> list)
    {
        if (node == null)
            return;

        ReverseInOrder(node.Right, list);
        list.Add(node.Key);
        ReverseInOrder(node.Left, list);
    }

    private void FillHeadSet(Node node, MyTreeSet<E> result, E bound, bool incl)
    {
        if (node == null)
            return;

        FillHeadSet(node.Left, result, bound, incl);

        int cmp = comparer.Compare(node.Key, bound);
        if ((incl && cmp <= 0) || (!incl && cmp < 0))
            result.Add(node.Key);

        FillHeadSet(node.Right, result, bound, incl);
    }

    private void FillTailSet(Node node, MyTreeSet<E> result, E bound, bool incl)
    {
        if (node == null)
            return;

        FillTailSet(node.Left, result, bound, incl);

        int cmp = comparer.Compare(node.Key, bound);
        if ((incl && cmp >= 0) || (!incl && cmp > 0))
            result.Add(node.Key);

        FillTailSet(node.Right, result, bound, incl);
    }

    private void FillSubSet(Node node, MyTreeSet<E> result, E low, bool lowIncl, E high, bool highIncl)
    {
        if (node == null)
            return;

        FillSubSet(node.Left, result, low, lowIncl, high, highIncl);

        int cmpLow = comparer.Compare(node.Key, low);
        int cmpHigh = comparer.Compare(node.Key, high);

        bool okLow = lowIncl ? cmpLow >= 0 : cmpLow > 0;
        bool okHigh = highIncl ? cmpHigh <= 0 : cmpHigh < 0;

        if (okLow && okHigh)
            result.Add(node.Key);

        FillSubSet(node.Right, result, low, lowIncl, high, highIncl);
    }

    private Node FindNode(E key)
    {
        Node current = root;

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

    private Node Minimum(Node node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    private Node Maximum(Node node)
    {
        while (node.Right != null)
            node = node.Right;
        return node;
    }

    private bool ColorOf(Node p)
    {
        if (p == null)
            return BLACK;
        return p.Color;
    }

    private Node ParentOf(Node p)
    {
        if (p == null)
            return null;
        return p.Parent;
    }

    private Node LeftOf(Node p)
    {
        if (p == null)
            return null;
        return p.Left;
    }

    private Node RightOf(Node p)
    {
        if (p == null)
            return null;
        return p.Right;
    }

    private void SetColor(Node p, bool c)
    {
        if (p != null)
            p.Color = c;
    }

    private void RotateLeft(Node p)
    {
        if (p == null)
            return;

        Node r = p.Right;
        p.Right = r.Left;

        if (r.Left != null)
            r.Left.Parent = p;

        r.Parent = p.Parent;

        if (p.Parent == null)
            root = r;
        else if (p.Parent.Left == p)
            p.Parent.Left = r;
        else
            p.Parent.Right = r;

        r.Left = p;
        p.Parent = r;
    }

    private void RotateRight(Node p)
    {
        if (p == null)
            return;

        Node l = p.Left;
        p.Left = l.Right;

        if (l.Right != null)
            l.Right.Parent = p;

        l.Parent = p.Parent;

        if (p.Parent == null)
            root = l;
        else if (p.Parent.Right == p)
            p.Parent.Right = l;
        else
            p.Parent.Left = l;

        l.Right = p;
        p.Parent = l;
    }

    private void FixAfterInsert(Node x)
    {
        x.Color = RED;

        while (x != null && x != root && ColorOf(ParentOf(x)) == RED)
        {
            if (ParentOf(x) == LeftOf(ParentOf(ParentOf(x))))
            {
                Node y = RightOf(ParentOf(ParentOf(x)));

                if (ColorOf(y) == RED)
                {
                    SetColor(ParentOf(x), BLACK);
                    SetColor(y, BLACK);
                    SetColor(ParentOf(ParentOf(x)), RED);
                    x = ParentOf(ParentOf(x));
                }
                else
                {
                    if (x == RightOf(ParentOf(x)))
                    {
                        x = ParentOf(x);
                        RotateLeft(x);
                    }

                    SetColor(ParentOf(x), BLACK);
                    SetColor(ParentOf(ParentOf(x)), RED);
                    RotateRight(ParentOf(ParentOf(x)));
                }
            }
            else
            {
                Node y = LeftOf(ParentOf(ParentOf(x)));

                if (ColorOf(y) == RED)
                {
                    SetColor(ParentOf(x), BLACK);
                    SetColor(y, BLACK);
                    SetColor(ParentOf(ParentOf(x)), RED);
                    x = ParentOf(ParentOf(x));
                }
                else
                {
                    if (x == LeftOf(ParentOf(x)))
                    {
                        x = ParentOf(x);
                        RotateRight(x);
                    }

                    SetColor(ParentOf(x), BLACK);
                    SetColor(ParentOf(ParentOf(x)), RED);
                    RotateLeft(ParentOf(ParentOf(x)));
                }
            }
        }

        root.Color = BLACK;
    }

    private void DeleteNode(Node p)
    {
        if (p.Left != null && p.Right != null)
        {
            Node s = Successor(p);
            p.Key = s.Key;
            p = s;
        }

        Node replacement;
        if (p.Left != null)
            replacement = p.Left;
        else
            replacement = p.Right;

        if (replacement != null)
        {
            replacement.Parent = p.Parent;

            if (p.Parent == null)
                root = replacement;
            else if (p == p.Parent.Left)
                p.Parent.Left = replacement;
            else
                p.Parent.Right = replacement;

            p.Left = null;
            p.Right = null;
            p.Parent = null;

            if (p.Color == BLACK)
                FixAfterDelete(replacement);
        }
        else if (p.Parent == null)
        {
            root = null;
        }
        else
        {
            if (p.Color == BLACK)
                FixAfterDelete(p);

            if (p.Parent != null)
            {
                if (p == p.Parent.Left)
                    p.Parent.Left = null;
                else if (p == p.Parent.Right)
                    p.Parent.Right = null;

                p.Parent = null;
            }
        }
    }

    private void FixAfterDelete(Node x)
    {
        while (x != root && ColorOf(x) == BLACK)
        {
            if (x == LeftOf(ParentOf(x)))
            {
                Node sib = RightOf(ParentOf(x));

                if (ColorOf(sib) == RED)
                {
                    SetColor(sib, BLACK);
                    SetColor(ParentOf(x), RED);
                    RotateLeft(ParentOf(x));
                    sib = RightOf(ParentOf(x));
                }

                if (ColorOf(LeftOf(sib)) == BLACK && ColorOf(RightOf(sib)) == BLACK)
                {
                    SetColor(sib, RED);
                    x = ParentOf(x);
                }
                else
                {
                    if (ColorOf(RightOf(sib)) == BLACK)
                    {
                        SetColor(LeftOf(sib), BLACK);
                        SetColor(sib, RED);
                        RotateRight(sib);
                        sib = RightOf(ParentOf(x));
                    }

                    SetColor(sib, ColorOf(ParentOf(x)));
                    SetColor(ParentOf(x), BLACK);
                    SetColor(RightOf(sib), BLACK);
                    RotateLeft(ParentOf(x));
                    x = root;
                }
            }
            else
            {
                Node sib = LeftOf(ParentOf(x));

                if (ColorOf(sib) == RED)
                {
                    SetColor(sib, BLACK);
                    SetColor(ParentOf(x), RED);
                    RotateRight(ParentOf(x));
                    sib = LeftOf(ParentOf(x));
                }

                if (ColorOf(RightOf(sib)) == BLACK && ColorOf(LeftOf(sib)) == BLACK)
                {
                    SetColor(sib, RED);
                    x = ParentOf(x);
                }
                else
                {
                    if (ColorOf(LeftOf(sib)) == BLACK)
                    {
                        SetColor(RightOf(sib), BLACK);
                        SetColor(sib, RED);
                        RotateLeft(sib);
                        sib = LeftOf(ParentOf(x));
                    }

                    SetColor(sib, ColorOf(ParentOf(x)));
                    SetColor(ParentOf(x), BLACK);
                    SetColor(LeftOf(sib), BLACK);
                    RotateRight(ParentOf(x));
                    x = root;
                }
            }
        }

        SetColor(x, BLACK);
    }

    private Node Successor(Node t)
    {
        if (t == null)
            return null;
        else if (t.Right != null)
        {
            Node p = t.Right;
            while (p.Left != null)
                p = p.Left;
            return p;
        }
        else
        {
            Node p = t.Parent;
            Node ch = t;
            while (p != null && ch == p.Right)
            {
                ch = p;
                p = p.Parent;
            }
            return p;
        }
    }
}
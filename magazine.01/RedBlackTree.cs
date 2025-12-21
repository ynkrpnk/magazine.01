using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text; // Необхідно для StringBuilder

namespace magazine._01
{
    public struct HashKey
    {
        public ulong Hash;
        public string Original;

        public HashKey(ulong hash, string original)
        {
            Hash = hash;
            Original = original;
        }

        public static int Compare(HashKey a, HashKey b)
        {
            int hashCompare = a.Hash.CompareTo(b.Hash);
            if (hashCompare != 0) return hashCompare;

            return string.Compare(a.Original, b.Original, StringComparison.Ordinal);
        }
    }


    // Перелік кольорів вузла
    public enum NodeColor
    {
        Black, Red
    }

    // Клас Вузла
    public class Node<T>
    {
        public HashKey Key;         // Хеш (використовується для побудови дерева)
        public T Value;         // Саме значення (MusicInstrument)
        public NodeColor Color;
        public Node<T> Parent;
        public Node<T> Left;
        public Node<T> Right;

        public Node(HashKey key, T value, NodeColor color)
        {
            Key = key;
            Value = value;
            Color = color;
        }

        // Метод для отримання "брата" вузла (потрібен для балансування)
        public Node<T> GetSibling()
        {
            if (Parent == null) throw new NullReferenceException("Parent was null");
            return Parent.Left == this ? Parent.Right : Parent.Left;
        }

        public bool IsBlack() => Color == NodeColor.Black;
        public bool IsRed() => Color == NodeColor.Red;
    }

    // Клас Червоно-Чорного дерева
    public class RedBlackTree<T>
    {
        public Node<T> Root;

        // Спеціальний "порожній" вузол (Sentinel)
        public static readonly Node<T> NullNode =
    new Node<T>(new HashKey(0, ""), default(T), NodeColor.Black);


        public RedBlackTree()
        {
            Root = NullNode;
        }

        public RedBlackTree(HashKey initialKey, T initialValue)
        {
            Root = new Node<T>(initialKey, initialValue, NodeColor.Black)
            {
                Left = NullNode,
                Right = NullNode
            };
        }

        // =========================================================
        //                 ПОШУК
        // =========================================================

        public (string els, string log) Find(string name, string category)
        {
            StringBuilder els = new StringBuilder();
            StringBuilder log = new StringBuilder();
            int steps = 0;
            int matches = 0;

            string searchCategory = category ?? string.Empty;
            ulong hash = Hash(name);
            HashKey searchKey = new HashKey(hash, name);

            Node<T> current = Root;

            while (current != NullNode)
            {
                steps++;
                int compare = HashKey.Compare(searchKey, current.Key);

                if (compare < 0)
                {
                    current = current.Left;
                }
                else if (compare > 0)
                {
                    current = current.Right;
                }
                else
                {
                    // Хеш совпал — проверяем категорию
                    MusicInstrument item = current.Value as MusicInstrument;
                    if (item != null)
                    {
                        bool categoryMatch = string.IsNullOrEmpty(searchCategory) ||
                                             item.Category.Equals(searchCategory, StringComparison.OrdinalIgnoreCase);

                        if (categoryMatch)
                        {
                            matches++;
                            log.AppendLine(
                                $" -> ЗНАЙДЕНО: ID={item.Id}, Name=\"{item.Name}\", Category=\"{item.Category}\", Price={item.Price}");
                            els.AppendLine($"ЗНАЙДЕНО: {item.Name} (Ціна: {item.Price})");
                        }
                    }

                    break; // после проверки категории выходим, других совпадений быть не может
                }
            }

            if (matches == 0)
                log.AppendLine(" -> Не знайдено жодного елемента.");
            else
                log.AppendLine($" -> Знайдено {matches} збіг(ів). Всього кроків: {steps}.");

            return (els.ToString(), log.ToString());
        }





        // =========================================================
        //            СТАНДАРТНІ МЕТОДИ (ВСТАВКА, ВИДАЛЕННЯ)
        // =========================================================

        public static ulong Hash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

                return BitConverter.ToUInt64(hash, 0);
            }
        }

        public void Clear()
        {
            Root = NullNode;
        }

        public void Insert(string keyString, T value)
        {
            ulong hash = Hash(keyString);
            HashKey key = new HashKey(hash, keyString);

            InsertKey(key, value);
        }

        private void InsertKey(HashKey key, T value)
        {
            Node<T> newNode = new Node<T>(key, value, NodeColor.Red)
            {
                Left = NullNode,
                Right = NullNode
            };

            if (Root == NullNode)
            {
                newNode.Color = NodeColor.Black;
                Root = newNode;
                return;
            }

            Node<T> current = Root;
            Node<T> parent = null;

            while (current != NullNode)
            {
                parent = current;

                int compare = HashKey.Compare(key, current.Key);

                if (compare < 0)
                {
                    current = current.Left;
                }
                else if (compare > 0)
                {
                    current = current.Right;
                }
                else
                {
                    throw new Exception("Duplicate key detected: identical string");
                }
            }

            newNode.Parent = parent;

            if (HashKey.Compare(key, parent.Key) < 0)
                parent.Left = newNode;
            else
                parent.Right = newNode;

            FixInsert(newNode);
        }


        private void FixInsert(Node<T> k)
        {
            while (k != Root && k.Parent.Color == NodeColor.Red)
            {
                if (k.Parent == k.Parent.Parent.Left)
                {
                    Node<T> uncle = k.Parent.Parent.Right;
                    if (uncle.Color == NodeColor.Red)
                    {
                        k.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Right)
                        {
                            k = k.Parent;
                            RotateLeft(k);
                        }
                        k.Parent.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        RotateRight(k.Parent.Parent);
                    }
                }
                else
                {
                    Node<T> uncle = k.Parent.Parent.Left;
                    if (uncle.Color == NodeColor.Red)
                    {
                        k.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Left)
                        {
                            k = k.Parent;
                            RotateRight(k);
                        }
                        k.Parent.Color = NodeColor.Black;
                        k.Parent.Parent.Color = NodeColor.Red;
                        RotateLeft(k.Parent.Parent);
                    }
                }
            }
            Root.Color = NodeColor.Black;
        }

        // --- Методи для видалення (потрібні для коректної роботи GridView) ---
        public void Delete(string keyString)
        {
            ulong hash = Hash(keyString);
            HashKey key = new HashKey(hash, keyString);

            try
            {
                Node<T> node = FindNode(key, Root);
                DeleteNode(node);
            }
            catch (KeyNotFoundException)
            {
                // Можно проигнорировать или вывести сообщение
            }
        }


        private Node<T> FindNode(HashKey key, Node<T> current)
        {
            while (current != NullNode)
            {
                int compare = HashKey.Compare(key, current.Key);

                if (compare == 0)
                    return current;
                else if (compare < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            throw new KeyNotFoundException();
        }


        private void DeleteNode(Node<T> node)
        {
            var parent = node.Parent;

            // Випадок 1: Немає дітей
            if (node.Left == NullNode && node.Right == NullNode)
            {
                if (node.IsBlack()) FixDoubleBlack(node);
                else
                {
                    Node<T> sibling = node.GetSibling();
                    if (sibling != NullNode) sibling.Color = NodeColor.Red;
                }

                if (parent.Left == node) parent.Left = NullNode;
                else parent.Right = NullNode;
                return;
            }

            // Випадок 2: Одна дитина (ліва)
            if (node.Left != NullNode && node.Right == NullNode)
            {
                var child = node.Left;
                child.Parent = parent;
                if (parent.Left == node) parent.Left = child;
                else if (parent.Right == node) parent.Right = child;

                if (node.IsBlack() && child.IsBlack()) FixDoubleBlack(child);
                else child.Color = NodeColor.Black;
                return;
            }

            // Випадок 2: Одна дитина (права)
            if (node.Right != NullNode && node.Left == NullNode)
            {
                var child = node.Right;
                child.Parent = parent;
                if (parent.Left == node) parent.Left = child;
                else if (parent.Right == node) parent.Right = child;

                if (node.IsBlack() && child.IsBlack()) FixDoubleBlack(child);
                else child.Color = NodeColor.Black;
                return;
            }

            // Випадок 3: Дві дитини
            Node<T> successor = node.Right;
            while (successor.Left != NullNode) successor = successor.Left;
            node.Key = successor.Key;
            node.Value = successor.Value;
            DeleteNode(successor);
        }

        private void FixDoubleBlack(Node<T> node)
        {
            if (node == Root) return;
            var sibling = node.GetSibling();
            var parent = node.Parent;

            if (sibling == NullNode) FixDoubleBlack(parent);
            else
            {
                if (sibling.IsRed())
                {
                    parent.Color = NodeColor.Red;
                    sibling.Color = NodeColor.Black;
                    if (sibling == sibling.Parent.Left) RotateRight(parent);
                    else RotateLeft(parent);
                    FixDoubleBlack(node);
                }
                else
                {
                    if (sibling.Left.IsRed() || sibling.Right.IsRed())
                    {
                        if (sibling.Left != NullNode && sibling.Left.IsRed())
                        {
                            if (sibling.Parent.Left == sibling)
                            {
                                sibling.Left.Color = sibling.Color;
                                sibling.Color = parent.Color;
                                RotateRight(parent);
                            }
                            else
                            {
                                sibling.Left.Color = parent.Color;
                                RotateRight(sibling);
                                RotateLeft(parent);
                            }
                        }
                        else
                        {
                            if (sibling.Parent.Left == sibling)
                            {
                                sibling.Right.Color = parent.Color;
                                RotateLeft(sibling);
                                RotateRight(parent);
                            }
                            else
                            {
                                sibling.Right.Color = sibling.Color;
                                sibling.Color = parent.Color;
                                RotateLeft(parent);
                            }
                        }
                        parent.Color = NodeColor.Black;
                    }
                    else
                    {
                        sibling.Color = NodeColor.Red;
                        if (parent.IsBlack()) FixDoubleBlack(parent);
                        else parent.Color = NodeColor.Black;
                    }
                }
            }
        }

        // --- Ротації ---
        private void RotateRight(Node<T> node)
        {
            var child = node.Left;
            node.Left = child.Right;
            if (child.Right != NullNode) child.Right.Parent = node;
            child.Parent = node.Parent;
            if (node.Parent == null) Root = child;
            else if (node == node.Parent.Right) node.Parent.Right = child;
            else node.Parent.Left = child;
            child.Right = node;
            node.Parent = child;
        }

        private void RotateLeft(Node<T> node)
        {
            var child = node.Right;
            node.Right = child.Left;
            if (child.Left != NullNode) child.Left.Parent = node;
            child.Parent = node.Parent;
            if (node.Parent == null) Root = child;
            else if (node == node.Parent.Left) node.Parent.Left = child;
            else node.Parent.Right = child;
            child.Left = node;
            node.Parent = child;
        }
    }
}
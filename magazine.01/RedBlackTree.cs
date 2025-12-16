using System;
using System.Collections.Generic;
using System.Text; // Необхідно для StringBuilder

namespace magazine._01
{
    // Перелік кольорів вузла
    public enum NodeColor
    {
        Black, Red
    }

    // Клас Вузла
    public class Node<T>
    {
        public int Key;         // ID (використовується для побудови дерева)
        public T Value;         // Саме значення (MusicInstrument)
        public NodeColor Color;
        public Node<T> Parent;
        public Node<T> Left;
        public Node<T> Right;

        public Node(int key, T value, NodeColor color)
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
        public static readonly Node<T> NullNode = new Node<T>(-1, default(T), NodeColor.Black);

        public RedBlackTree()
        {
            Root = NullNode;
        }

        public RedBlackTree(int initialKey, T initialValue)
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
            StringBuilder sb = new StringBuilder();
            StringBuilder els = new StringBuilder();

            sb.AppendLine("Дерево відсортоване по ID, робимо повний обхід");

            int steps = 0;
            int matches = 0;

            // Запуск рекурсивного обхода
            SearchRecursiveNameCategory(Root, name, category, sb, els, ref steps, ref matches);

            if (matches == 0)
            {
                sb.AppendLine(" -> Не знайдено жодного елемента.");
            }
            else
            {
                sb.AppendLine($" -> Знайдено {matches} збіг(ів). Всього кроків: {steps}.");
            }

            return (els.ToString(), sb.ToString());
        }

        private void SearchRecursiveNameCategory(
            Node<T> node,
            string searchName,
            string searchCategory,
            StringBuilder log,
            StringBuilder els,
            ref int steps,
            ref int matches)
        {
            if (node == NullNode) return;

            steps++;

            // Приводим значение к MusicInstrument
            MusicInstrument item = node.Value as MusicInstrument;
            if (item != null)
            {
                string itemName = item.Name ?? string.Empty;
                string itemCategory = item.Category ?? string.Empty;

                string name = searchName ?? string.Empty;
                string category = searchCategory ?? string.Empty;

                bool nameMatch = itemName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;
                bool categoryMatch = itemCategory.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0;

                if (nameMatch && categoryMatch)
                {
                    matches++;

                    log.AppendLine(
                        $" -> ЗНАЙДЕНО: ID={item.Id}, Name=\"{item.Name}\", Category=\"{item.Category}\", Price={item.Price}");

                    els.AppendLine($"ЗНАЙДЕНО: {item.Name} (Ціна: {item.Price})");
                }
            }

            // Идём влево
            SearchRecursiveNameCategory(node.Left, searchName, searchCategory, log, els, ref steps, ref matches);

            // Идём вправо
            SearchRecursiveNameCategory(node.Right, searchName, searchCategory, log, els, ref steps, ref matches);
        }



        // =========================================================
        //            СТАНДАРТНІ МЕТОДИ (ВСТАВКА, ВИДАЛЕННЯ)
        // =========================================================

        public void Clear()
        {
            Root = NullNode;
        }

        public void Insert(int key, T value)
        {
            Node<T> newNode = new Node<T>(key, value, NodeColor.Red)
            {
                Left = NullNode,
                Right = NullNode
            };

            if (Root == NullNode || Root is null)
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
                if (newNode.Key < current.Key) current = current.Left;
                else if (newNode.Key > current.Key) current = current.Right;
                else throw new Exception($"Key {newNode.Key} is already present");
            }

            newNode.Parent = parent;
            if (parent == null) Root = newNode;
            else if (newNode.Key < parent.Key) parent.Left = newNode;
            else parent.Right = newNode;

            if (newNode.Parent == null)
            {
                newNode.Color = NodeColor.Black;
                return;
            }
            if (newNode.Parent.Parent == null) return;

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
        public void Delete(int key)
        {
            try
            {
                DeleteNode(FindNode(key, Root));
            }
            catch (KeyNotFoundException) { }
        }

        private Node<T> FindNode(int key, Node<T> current)
        {
            while (true)
            {
                if (current == NullNode) throw new KeyNotFoundException();
                if (current.Key == key) return current;
                current = current.Key < key ? current.Right : current.Left;
            }
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
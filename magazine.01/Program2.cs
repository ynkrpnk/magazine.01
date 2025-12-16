//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace magazine._01
//{
//    class Program2
//    {
//    }
//}



//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using Linear;
//namespace RedBlackTree;

//class Program
//{
//    static void Main(string[] args)
//    {
//        var tree = new RedBlackTree<MusicInstrument>();
//        var linAlg = new LinearSearch();
//        //tree.Insert(50, null);

//        const int keysCount = 10000;
//        List<int> keysHistory = new(keysCount) { };
//        var random = new Random(10);

//        for (int i = 0; i < keysCount; i++) //цикл генерации елементов
//        {
//            int newKey = random.Next(0, 10000);
//            while (keysHistory.Contains(newKey))
//            {
//                newKey = random.Next(0, 10000);
//            }

//            tree.Insert(newKey, new MusicInstrument(newKey, $"id-{newKey}", newKey));
//            linAlg.Insert(new MusicInstrument(newKey, $"id-{newKey}", newKey));

//            //Console.WriteLine($"added {newKey}"); //временно для дебага
//            keysHistory.Add(newKey);
//        }


//        //для теста дерева/списка удаление елементов
//        /*for (int i = 0; i < 10; i++)
//        {
//            int key = keysHistory[random.Next(0, keysHistory.Count)];
//            Console.WriteLine($"deleted {key}"); //временно для дебага
//            tree.Delete(key);
//            linAlg.Delete(key);

//            keysHistory.Remove(key);
//        }*/

//        //RedBlackVisualizer.Visualize(tree.Root); //показывает структуру дерева


//        //показывает все ключи(проверить корректность удаления при дебаге)
//        /*foreach (var key in keysHistory) 
//        {
//            Console.WriteLine(key);
//        }*/


//        //ручные тесты получения елементов
//        // tree.Set(29, "asd");
//        /*Console.WriteLine(tree.Get(29));
//        Console.WriteLine(tree.Get(45));
//        Console.WriteLine(tree.Get(69));
//        Console.WriteLine(tree.Get(94));*/
//        //Console.WriteLine("-----");
//        //Console.WriteLine(linAlg.Get(94));
//        //linAlg.ShowAll(); //все елементы списка


//        //усредненное время поиска елементв
//        int testCount = 1000; // кол-во тестируемых елементов
//        long totalTicksLinear = 0;
//        long totalTicksTree = 0;

//        var rand = new Random(10);

//        for (int i = 0; i < testCount; i++)
//        {
//            int key = keysHistory[rand.Next(keysHistory.Count)];

//            // LinearSearch
//            Stopwatch sw = Stopwatch.StartNew();
//            var foundLinear = linAlg.Get(key);
//            sw.Stop();
//            totalTicksLinear += sw.ElapsedTicks;

//            // RedBlackTree
//            sw.Restart();
//            var foundTree = tree.Get(key);
//            sw.Stop();
//            totalTicksTree += sw.ElapsedTicks;
//        }

//        Console.WriteLine($"LinearSearch average: {totalTicksLinear / testCount} ticks");
//        Console.WriteLine($"RedBlackTree average: {totalTicksTree / testCount} ticks");
//        Console.WriteLine($"RedBlackTree is {(totalTicksLinear / testCount) / (totalTicksTree / testCount)} times faster");



//        // int[] keys = [20,30,40,];//50,60,70,80
//        // foreach (var key in keys)
//        // {
//        //     tree.Insert(key, $"string-{key}");
//        // }
//        // tree.Delete(30);
//        // RedBlackVisualizer.Visualize(tree.Root);

//        //Console.ReadLine();
//    }
//}

//public static class RedBlackVisualizer
//{
//    public static void Visualize<T>(Node<T> node, int indent = 0)
//    {
//        if (node is null || node == RedBlackTree<T>.NullNode) return;

//        for (int i = 0; i < indent; i++)
//        {
//            Console.ForegroundColor = ConsoleColor.Gray;
//            Console.Write(">");
//        }

//        if (node.Color == NodeColor.Black)
//        {
//            // Console.ForegroundColor = ConsoleColor.Black;
//            Console.ResetColor();
//        }
//        else
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//        }

//        Console.WriteLine($"{node.Key} - {node.Color}");

//        Console.ResetColor();

//        Visualize(node.Left, indent + 1);
//        Visualize(node.Right, indent + 1);
//    }
//}
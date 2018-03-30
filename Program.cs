using System;
using Logic_simulation.View;

namespace Logic_simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            TextUI.DisplayMenu();
            Console.Write("請按任意鑑繼續...");
            Console.ReadKey();
        }
    }
}

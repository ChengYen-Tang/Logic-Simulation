using System;
using Logic_simulation.Controller;
using Logic_simulation.Model;

namespace Logic_simulation.View
{
    public static class TextUI
    {
        static bool IsExit = false;

        /// <summary>
        /// Show Menu
        /// </summary>
        public static void DisplayMenu()
        {
            while(!IsExit)
            {
                Console.WriteLine("1. Load logic circuit file");
                Console.WriteLine("2. Simulation");
                Console.WriteLine("3. Display truth table");
                Console.WriteLine("4. Exit");
                Console.Write("Command:");
                ProcessCommand();
            }
        }

        /// <summary>
        /// 讀取Command並執行
        /// </summary>
        private static void ProcessCommand()
        {
            string Input = Console.ReadLine();
            int Command = int.MinValue;
            bool CommandCheck = false;
            
            if(int.TryParse(Input,out Command))
                if(Command >= 1 && Command <= 4)
                    CommandCheck = true;

            if(CommandCheck)
            {
                switch(Command)
                {
                    case 1:
                        Console.Write("Please key in a file path:");
                        if (!LogicSimulator.Load(Console.ReadLine()))
                            Console.WriteLine("File not found or file format error!!");
                        else
                            Console.WriteLine("Load file success!!");
                    break;

                    case 2:
                        if (LogicSimulator.LoadStatus)
                        {
                            bool[] Inputs = new bool[InputPin.PinsSignal.Length];
                            for (int i = 1; i < InputPin.PinsSignal.Length; i++)
                            {
                                Console.Write("Please key in the value of input pin {0}:", i);
                                try
                                {
                                    string Value = Console.ReadLine();
                                    if (Value == "0" || Value == "1")
                                    {
                                        Inputs[i] = Convert.ToBoolean(int.Parse(Value));
                                    }
                                    else
                                    {
                                        Console.WriteLine("The value of input pin must be 0/1");
                                        i--;
                                    }
                                    

                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("The value of input pin must be 0/1");
                                    i--;
                                }
                            }
                            Console.WriteLine(LogicSimulator.GetSimulationResult(Inputs));
                        }
                        else
                            Console.WriteLine("Please load an lcf file, before using this operation.");
                    break;

                    case 3:
                        if (LogicSimulator.LoadStatus)
                            Console.WriteLine(LogicSimulator.GetTruthTable());
                        else
                            Console.WriteLine("Please load an lcf file, before using this operation.");
                        break;

                    case 4:
                        Console.WriteLine("Goodbye, thanks for using LS.");
                        IsExit = true;
                    break;
                }
            }
            else
            {
                Console.WriteLine("Wrong command！！");
            }

            Console.WriteLine();
        }
    }
}
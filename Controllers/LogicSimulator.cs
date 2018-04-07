using System;
using System.Collections.Generic;
using System.IO;
using Logic_simulation.Model;

namespace Logic_simulation.Controller
{
    public static class LogicSimulator
    {
        private static bool IsLoadDone = false;
        private static dynamic[] circuit;
        public static IPin[] iPins;
        public static List<dynamic> oPins;

        public static bool Load(string FilePath)
        {
            List<double>[] ConnectionInformation;
            oPins = new List<dynamic>();
            try
            {
                // 讀檔
                using (StreamReader LogicCircuitFile = new StreamReader(@FilePath.Trim('"')))
                {
                    // 設定Input腳位數量
                    int iPinsCount = int.Parse(LogicCircuitFile.ReadLine());
                    iPins = new IPin[iPinsCount + 1];
                    // 設定邏輯閘數量
                    int GatesCount = int.Parse(LogicCircuitFile.ReadLine());
                    circuit = new Device[GatesCount + 1];
                    ConnectionInformation = new List<double>[GatesCount + 1];


                    // 初始化邏輯閘
                    for (int i = 1; i <= GatesCount; i++)
                    {

                        // 切割邏輯閘訊息，並檢查訊息結尾是否為0
                        string[] GateInformation = LogicCircuitFile.ReadLine().Split(" ");
                        if (int.Parse(GateInformation[GateInformation.Length - 1]) != 0)
                            return false;
                        // 產生邏輯閘物件
                        switch (int.Parse(GateInformation[0]))
                        {
                            case 1:
                                circuit[i] = new GateAND();
                                break;

                            case 2:
                                circuit[i] = new GateOR();
                                break;

                            case 3:
                                circuit[i] = new GateNOT();
                                break;

                            default:
                                return false;
                        }

                        // 紀錄此邏輯的關聯資訊
                        ConnectionInformation[i] = new List<double>();
                        for (int j = 1; j <= (GateInformation.Length - 2); j++)
                            ConnectionInformation[i].Add(double.Parse(GateInformation[j]));
                    }

                    // 關聯邏輯閘
                    for (int i = 1; i <= GatesCount; i++)
                        foreach (double InputNumber in ConnectionInformation[i])
                            if (InputNumber < 0)
                            {
                                if (iPins[Math.Abs(Convert.ToInt16(InputNumber))] == null)
                                    iPins[Math.Abs(Convert.ToInt16(InputNumber))] = new IPin();
                                circuit[i].AddInputPin(iPins[Math.Abs(Convert.ToInt16(InputNumber))]);
                            }
                            else
                            {
                                circuit[i].AddInputPin(circuit[Convert.ToInt16(Math.Floor(InputNumber))]);
                                circuit[Convert.ToInt16(Math.Floor(InputNumber))].IsOutput = false;
                            }

                    // 尋找電路的最末端邏輯閘，並關聯至電路輸出腳位
                    for (int i = 1; i <= GatesCount; i++)
                        if (circuit[i].IsOutput)
                        {
                            OPin oPin = new OPin();
                            oPin.AddInputPin(circuit[i]);
                            oPins.Add(oPin);
                        }
                }
                IsLoadDone = true;
                return true;
            }
            catch
            {
                IsLoadDone = false;
                return false;
            }
        }

        /// <summary>
        /// 輸出真值表
        /// </summary>
        /// <returns></returns>
        public static string GetTruthTable()
        {
            string Result = string.Empty;
            Result = "Truth table:\n";
            PrivtFieldName(ref Result);

            // 運行次數:2的N次方，N為Input pin數量
            for (int i = 0; i < Math.Pow(2, (iPins.Length - 1)); i++)
            {
                int k = i;
                // 10進制轉2進制，並送入電路的Inputs
                for (int j = 0; j < iPins.Length - 1; j++)
                {
                    iPins[((iPins.Length - 1) - j)].iPins = (Convert.ToBoolean(k % 2));
                    k /= 2;
                }
                // 輸出當前input的訊號 0 0 1
                for (int j = 1; j < iPins.Length; j++)
                    Result += (Convert.ToInt32(iPins[j].iPins) + " ");

                // 輸出結果
                Result += "|";
                foreach (dynamic OutputValue in oPins)
                    Result += (" " + Convert.ToInt32(OutputValue.GetOutput()));
                    

                Result += ("\n");
            }
            return (Result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InPins"></param>
        /// <returns></returns>
        public static string GetSimulationResult(bool[] InPins)
        {
            string Result = string.Empty;
            Result = "Simulation Result:\n";
            PrivtFieldName(ref Result);

            // 設定input value至input pins，並輸出input value
            for (int i = 1; i < InPins.Length; i++)
            {
                iPins[i].iPins = InPins[i];
                Result += Convert.ToInt32(InPins[i]) + " ";
            }

            // 輸出結果
            Result += "|";
            foreach (OPin OutputPin in oPins)
                Result += (" " + Convert.ToInt32(OutputPin.GetOutput()));

            Result += ("\n");

            return (Result);
        }

        /// <summary>
        /// 回傳LCF載入狀況
        /// </summary>
        public static bool LoadStatus { get { return IsLoadDone; } }

        private static void PrivtFieldName(ref string Result)
        {
            // 第一行 i i i | o
            for (int i = 1; i < iPins.Length; i++)
                Result += "i ";
            Result += "|";
            for (int i = 0; i < oPins.Count; i++)
                Result += " o";
            Result += "\n";
            // 第二行 1 2 3 | 1
            for (int i = 1; i < iPins.Length; i++)
                Result += (i + " ");
            Result += "|";
            for (int i = 0; i < oPins.Count; i++)
                Result += " " + (i + 1);
            Result += "\n";
            // 第三行 ------+--
            for (int i = 1; i < iPins.Length; i++)
                Result += ("--");
            Result += ("+");
            for (int i = 0; i < oPins.Count; i++)
                Result += "--";
            Result += ("\n");
        }
    }
}
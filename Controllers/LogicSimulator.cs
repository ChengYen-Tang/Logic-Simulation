using System;
using System.IO;
using Logic_simulation.Model;

namespace Logic_simulation.Controller
{
    public static class LogicSimulator
    {
        private static bool IsLoadDone = false;
        private static Device[] Gates;

        public static unsafe bool Load(string FilePath)
        {
            // 讀檔
            StreamReader LogicCircuitFile = new StreamReader(@FilePath.Trim('"'));
            try
            {
                // 設定Input腳位數量
                int iPinsCount = int.Parse(LogicCircuitFile.ReadLine());
                InputPin.PinsSignal = new bool[iPinsCount + 1];
                // 設定邏輯閘數量
                int GatesCount = int.Parse(LogicCircuitFile.ReadLine());
                Gates = new Device[GatesCount + 1];

                // 產生邏輯閘物件
                for (int i = 1; i <= GatesCount; i++)
                    Gates[i] = new Device();

                // 初始化邏輯閘
                for (int i = 1; i <= GatesCount; i++)
                {
                    
                    // 切割邏輯閘訊息，並檢查訊息結尾是否為0
                    string[] GateInformation = LogicCircuitFile.ReadLine().Split(" ");
                    if (int.Parse(GateInformation[GateInformation.Length - 1]) != 0)
                        return false;
                    // 設定邏輯閘型態
                    Gates[i].GateType = (LogicGate)int.Parse(GateInformation[0]);
                    // 設定邏輯閘輸入腳位的數量
                    // 邏輯閘資訊第一項目為邏輯閘型態，最後項目為0
                    Gates[i].iPins = new bool*[GateInformation.Length - 2];

                    // 設定指標，把邏輯閘的輸入與其它邏輯閘的輸出對接
                    for (int j = 1; j <= (GateInformation.Length - 2); j++)
                    {
                        // 判斷輸入腳位的目標，<0代表電路的輸入
                        if (double.Parse(GateInformation[j]) < 0)
                        {
                            // 設定輸入改變事件及腳位指標
                            InputPin.OutputChangedEvent +=
                                Gates[i].GetOutput;
                            fixed (bool* InPin =
                                &InputPin.PinsSignal[Math.Abs(Convert.ToInt16(double.Parse(GateInformation[j])))])
                                Gates[i].AddInputPin(InPin);
                        }
                        else
                        {
                            // 紀錄此閘輸出有接到其它閘的輸入
                            Gates[Convert.ToInt16(Math.Floor(double.Parse(GateInformation[j])))].IsOutput = false;

                            Gates[Convert.ToInt16(Math.Floor(double.Parse(GateInformation[j])))].OutputChangedEvent
                                += Gates[i].GetOutput;
                            fixed (bool* InPin =
                                    &Gates[Convert.ToInt16(Math.Floor(Convert.ToDouble(GateInformation[j])))].Output)
                                Gates[i].AddInputPin(InPin);
                        }
                    }
                }

                // 尋找電路的輸出腳位並設定指標
                OutputPin.LinkToDeviceOutput(Gates);
                // 釋放載入資源
                LogicCircuitFile.Close();
                IsLoadDone = true;
                return true;
            }
            catch
            {
                LogicCircuitFile.Close();
                IsLoadDone = false;
                return false;
            }
        }

        /// <summary>
        /// 輸出真值表
        /// </summary>
        /// <returns></returns>
        public static unsafe string GetTruthTable()
        {
            string Result = string.Empty;
            Result = "Truth table:\n";
            PrivtFieldName(ref Result);

            // 運行次數:2的N次方，N為Input pin數量
            for (int i = 0; i < Math.Pow(2, (InputPin.PinsSignal.Length - 1)); i++)
            {
                int k = i;
                // 10進制轉2進制，並送入電路的Inputs
                for (int j = 0; j < InputPin.PinsSignal.Length - 1; j++)
                {
                    InputPin.PinsSignal[((InputPin.PinsSignal.Length - 1) - j)] = Convert.ToBoolean(k % 2);
                    k /= 2;
                }
                // 輸出當前input的訊號 0 0 1
                for (int j = 1; j < InputPin.PinsSignal.Length; j++)
                    Result += (Convert.ToInt32(InputPin.PinsSignal[j]) + " ");

                // 開始運算
                InputPin.Changed();
                // 輸出結果
                Result += "|";
                foreach (bool OutputValue in OutputPin.OutPutSignals)
                    Result += (" " + Convert.ToInt32(OutputValue));

                Result += ("\n");
            }
            return (Result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InPins"></param>
        /// <returns></returns>
        public static unsafe string GetSimulationResult(bool[] InPins)
        {
            string Result = string.Empty;
            Result = "Simulation Result:\n";
            PrivtFieldName(ref Result);

            Array.Copy(InPins, InputPin.PinsSignal, InPins.Length);
            for (int i = 1; i < InputPin.PinsSignal.Length; i++)
                Result += Convert.ToInt32(InputPin.PinsSignal[i]) + " ";

            // 開始運算
            InputPin.Changed();
            // 輸出結果
            Result += "|";
            foreach (bool OutputValue in OutputPin.OutPutSignals)
                Result += (" " + Convert.ToInt32(OutputValue));

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
            for (int i = 1; i < InputPin.PinsSignal.Length; i++)
                Result += "i ";
            Result += "|";
            for (int i = 0; i < OutputPin.OutPutSignals.Length; i++)
                Result += " o";
            Result += "\n";
            // 第二行 1 2 3 | 1
            for (int i = 1; i < InputPin.PinsSignal.Length; i++)
                Result += (i + " ");
            Result += "|";
            for (int i = 0; i < OutputPin.OutPutSignals.Length; i++)
                Result += " " + (i + 1);
            Result += "\n";
            // 第三行 ------+--
            for (int i = 1; i < InputPin.PinsSignal.Length; i++)
                Result += ("--");
            Result += ("+");
            for (int i = 0; i < OutputPin.OutPutSignals.Length; i++)
                Result += "--";
            Result += ("\n");
        }
    }
}
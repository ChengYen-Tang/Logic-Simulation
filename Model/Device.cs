using System.Collections.Generic;

namespace Logic_simulation.Model
{
    /// <summary>
    /// 列舉邏輯閘
    /// </summary>
    public enum LogicGate
    {
        AND = 1,
        OR,
        NOT
    }

    public delegate void IutputChangedEventHandler();
    public delegate void OutputChangedEventHandler();

    /// <summary>
    /// 邏輯閘
    /// </summary>
    public unsafe class Device
    {
        public LogicGate GateType;
        public bool*[] iPins;
        public bool Output = false;
        public bool IsOutput = true;
        public event OutputChangedEventHandler OutputChangedEvent;

        /// <summary>
        /// 邏輯判斷並輸出
        /// </summary>
        internal void GetOutput()
        {
            switch (GateType)
            {
                case LogicGate.AND: //AND
                    GateAND();
                    break;

                case LogicGate.OR: //OR
                    GateOR();
                    break;

                case LogicGate.NOT: //NOT
                    GateNot();
                    break;
            }
            // 如果此邏輯閘的輸出腳位有連接其它邏輯閘輸入腳位，通知連接的邏輯閘
            OutputChangedEvent?.Invoke();
        }

        /// <summary>
        /// 新增邏輯閘的輸入腳，並連接至電路輸入或其它邏輯閘輸出腳位
        /// </summary>
        /// <param name="iPin"></param>
        public void AddInputPin(bool* iPin)
        {
            for (int i = 0; i < iPins.Length; i++)
                if (iPins[i] == null)
                {
                    iPins[i] = iPin;
                    break;
                }
        }

        private void GateAND()
        {
            Output = true;
            foreach (bool* iPin in iPins)
                if (*iPin == false)
                    Output = false;
        }

        private void GateOR()
        {
            Output = false;
            foreach (bool* iPin in iPins)
                if (*iPin == true)
                    Output = true;
        }

        private void GateNot()
        {
            Output = !*iPins[0];
        }
    }

    /// <summary>
    /// 電路的輸入腳位
    /// </summary>
    public static class InputPin
    {
        public static bool[] PinsSignal;

        public static event IutputChangedEventHandler OutputChangedEvent;

        /// <summary>
        /// 通知與電路輸入連接的邏輯閘，電路輸入腳位已變更
        /// </summary>
        internal static void Changed()
        {
            OutputChangedEvent();
        }
    }

    /// <summary>
    /// 電路的輸出腳位
    /// </summary>
    public static unsafe class OutputPin
    {
        private static bool*[] OutPutPins;

        /// <summary>
        /// 尋找電路輸出腳位並連結
        /// </summary>
        /// <param name="Gates"></param>
        public static void LinkToDeviceOutput(Device[] Gates)
        {
            List<Device> OutPutPinGate = new List<Device>();
            foreach (Device Gate in Gates)
                if(Gate != null)
                    if (Gate.IsOutput)
                        OutPutPinGate.Add(Gate);

            OutPutPins = new bool*[OutPutPinGate.Count];
            for (int i = 0; i < OutPutPinGate.Count; i++)
                fixed (bool* OutPutPin = &OutPutPinGate[i].Output)
                    OutPutPins[i] = OutPutPin;
        }

        /// <summary>
        /// 回傳電路輸出結果
        /// </summary>
        public static bool[] OutPutSignals
        {
            get
            {
                List<bool> OutPutValues = new List<bool>();
                foreach (bool* OutPutValue in OutPutPins)
                    OutPutValues.Add(*OutPutValue);

                return OutPutValues.ToArray();
            }
        }
    }
}

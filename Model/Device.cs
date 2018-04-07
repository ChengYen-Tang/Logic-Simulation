using System;
using System.Collections.Generic;

namespace Logic_simulation.Model
{
    /// <summary>
    /// 抽象父系類別
    /// </summary>
    public abstract class Device
    {
        protected List<dynamic> iPins;
        public bool IsOutput = true;

        public Device()
        {
            iPins = new List<dynamic>();
        }

        public void AddInputPin(Device iPin)
        {
            iPins.Add(iPin);
        }

        public bool GetOutput()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 電路輸出腳位
    /// </summary>
    public class OPin : Device
    {
        public new dynamic iPins;

        public void AddInputPin(dynamic iPin)
        {
            iPins = iPin;
        }

        public new bool GetOutput()
        {
            return iPins.GetOutput();
        }
    }

    /// <summary>
    /// 電路輸入腳位
    /// </summary>
    public class IPin : Device
    {
        public new bool iPins;

        public new bool GetOutput()
        {
            return iPins;
        }
    }

    /// <summary>
    /// 反向邏輯閘
    /// </summary>
    public class GateNOT : Device
    {
        public new bool GetOutput()
        {
            return !iPins[0].GetOutput();
        }
    }

    /// <summary>
    /// AND邏輯閘
    /// </summary>
    public class GateAND : Device
    {
        public new bool GetOutput()
        {
            bool Output = true;

            foreach (dynamic IPin in iPins)
                if (IPin.GetOutput() == false)
                    Output = false;

            return Output;
        }
    }

    /// <summary>
    /// OR邏輯閘
    /// </summary>
    public class GateOR : Device
    {
        public new bool GetOutput()
        {
            bool Output = false;

            foreach (dynamic IPin in iPins)
                if (IPin.GetOutput() == true)
                    Output = true;

            return Output;
        }
    }
}

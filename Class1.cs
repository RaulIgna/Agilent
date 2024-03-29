﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Driver;
//using Keysight.Kt34400;
using Agilent.Ag3446x.Interop;
using Ivi.Dmm;
using System.Threading;
using Agilent_34411A_LIB.AutoTest;



//namespace Agilent_34465A_LIB
namespace Agilent_34411A_LIB
{
    //public class DMM34465A
    public class DMM34410A
    {
        public string ResourceName;
        public string ID;
        public double Range;
        public double NPLC;
        public bool InputImpedance;
        public Ag3446xAutoZeroEnum AutoZero;
        public bool NullState;
        public double NullValue;
        public double Constant;
        public double RawValue;
        public bool Run;

        public Thread Work;
        //public Keysight.Kt34400.Kt34400 Driver = new Keysight.Kt34400.Kt34400();
        // public Keysight.Kt34400.Kt34400 Driver;
        public Agilent.Ag3446x.Interop.Ag3446x Driver;

        public DMM34410A(string lID) 
        { 
            ResourceName = "USB0::0x2A8D::0x0101::MY6009" + lID + "::0::INSTR";
            ID = lID;
            Range = 100;
            NPLC = 10;
        }

        public void DMM_SetID(string text)
        {
            ID = text;
            ResourceName =  "USB0::0x2A8D::0x0101::MY6009" + ID + "::0::INSTR";
        }

        public void DMM_SelectAutoRange(int AutoZeroID)
        {
            switch(AutoZeroID)
            {
                case 0:
                    AutoZero = Ag3446xAutoZeroEnum.Ag3446xAutoZeroOff;
                    break;
                case 1:
                    AutoZero = Ag3446xAutoZeroEnum.Ag3446xAutoZeroOnce;
                    break;
                case 2:
                    AutoZero = Ag3446xAutoZeroEnum.Ag3446xAutoZeroOn;
                    break;
                default:
                    AutoZero = Ag3446xAutoZeroEnum.Ag3446xAutoZeroOnce;
                    break;
            }
        }
        
        public void DMM_SelectRange(int SelectedIndex)
        {
            switch (SelectedIndex)
            {
                case 0:
                    Range = 0.1;
                    break;
                case 1:
                    Range = 1;
                    break;
                case 2:
                    Range = 10;
                    break;
                case 3:
                    Range = 100;
                    break;
                case 4:
                    Range = 1000;
                    break;
                default:
                    Range = 100;
                    break;
            }
        }

        public void DMM_SelectNPLC(int SelectedIndex)
        {
            switch (SelectedIndex)
            {
                case 0:
                    NPLC = 0.006;
                    break;
                case 1:
                    NPLC = 0.02;
                    break;
                case 2:
                    NPLC = 0.06;
                    break;
                case 3:
                    NPLC = 0.2;
                    break;
                case 4:
                    NPLC = 1;
                    break;
                case 5:
                    NPLC = 2;
                    break;
                case 6:
                    NPLC = 10;
                    break;
                case 7:
                    NPLC = 100;
                    break;
                default:
                    NPLC = 10;
                    break;
            }
        }
    }
    



    public class Agilent_34465A_LIB
    {
        private static Error err_rx = new Error(false,"Default Rx thread error");
        public static Error Open(DMM34410A DMM)
        {
            string pOptionString =
                "Cache=false, InterchangeCheck=false, QueryInstrStatus=true, RangeCheck=true, RecordCoercions=false, Simulate=false";
            //IdQuery
            //If this is enabled, the driver will query the instrument model and compare it with a list of instrument models that is supported by the driver. If the model is not supported, Initialize will fail with the E_IVI_ID_QUERY_FAILED error code. 
            bool pIdQuery = true;

            //Reset
            //If this is enabled, the driver will perform a reset of the instrument. If the reset fails, Initialize will fail with the E_IVI_RESET_FAILED error code.
            bool pReset = true;

            try// to close previous oppened
            {
                DMM.Driver.Close();
                DMM.Driver = null;
            }
            catch { }

            Error er = new Error();
            try
            {
                DMM.Driver = new Agilent.Ag3446x.Interop.Ag3446x();//(DMM.ResourceName, pIdQuery, pReset, pOptionString);
                DMM.Driver.Initialize(DMM.ResourceName,pIdQuery,pReset,pOptionString);
                er = getError("Initialize", DMM);
                if (!er.OK)
                {
                    return er;
                }

                //Resolution resolution = Resolution.Max;
                DMM.Driver.DCVoltage.Configure(DMM.Range, 4.5);

                er = getError("Configure(range, resolution)", DMM);
                if (!er.OK)
                {
                    return er;
                }


                DMM.Driver.DCVoltage.NullEnabled = DMM.NullState;

                er = getError("Null State", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.DCVoltage.NullValue = DMM.NullValue;

                er = getError("Null Value", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.DCVoltage.ImpedanceAutoEnabled = DMM.InputImpedance;

                er = getError("Input Impedance", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.DCVoltage.AutoZero= DMM.AutoZero;

                er = getError("AutoZero", DMM);
                if (!er.OK)
                {
                    return er;
                }


                DMM.Driver.DCVoltage.NPLC = DMM.NPLC;
                er = getError("DCVoltage.Aperture", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.Trigger.Source = Ag3446xTriggerSourceEnum.Ag3446xTriggerSourceImmediate;//TriggerSource.Immediate;
                er = getError(".Trigger.TriggerSource", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.Trigger.Count = 1;
                er = getError("", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.Trigger.Delay = 0; //PrecisionTimeSpan.Zero;
                er = getError("Trigger.TriggerDelay ", DMM);
                if (!er.OK)
                {
                    return er;
                }

                DMM.Driver.Trigger.SampleCount = 1;
                er = getError("Trigger.SampleCount", DMM);
                if (!er.OK)
                {
                    return er;
                }


                DMM.Driver.Display.Mode = Ag3446xDisplayModeEnum.Ag3446xDisplayModeNumeric;
                er = getError("DataFormat.DataFormat", DMM);
                if (!er.OK)
                {
                    return er;
                }
            }
            catch (Exception ex) {
                DMM.Run = false;
                return new Error(ex.Message);
            }

            return new Error(true, "No error");
        }
        public static void Close(DMM34410A DMM)
        {
            try
            {
                DMM.Driver.Close();
                DMM.Driver = null;
            }
            catch
            {

            }
        }

        public static void GetData(DMM34410A DMM, out double[] Data)
        {
            int dataPts;

            // Initiate the measurement
            //Changes the state of the triggering system from the 'idle' state to the 'wait-for-trigger' state.
            //Measurements will begin when the specified trigger conditions are satisfied following execution of this method. Note that this method also clears the previous set of readings from memory.
            DMM.Driver.Measurement.Initiate();

            // Slow down
            Thread.Sleep((int)(DMM.NPLC * 100));

            // Gets the total number of reading currently stored in reading memory
            dataPts = DMM.Driver.Measurement.ReadingCount;

            // If there is any data, read and remove the data
            // Otherwise, set Data to null
            Data = dataPts > 0 ? DMM.Driver.Measurement.RemoveReadings(dataPts) : null;
        }

        
        static private Error getError(string title, DMM34410A driver)
        {
            bool checkstat = false;

            try
            {
                // check for initial error
                int error = 0;
                string errorString = "";
                driver.Driver.Utility.ErrorQuery(error,errorString);
                    
                    //driver.Driver.Utility.ErrorQuery();

                while (error != 0)
                {
                    checkstat = true;

                    title += "Error in: " + title + "\n";
                    title += title + "Error Number: " + error + "\nError Message: " + errorString;

                    driver.Driver.Utility.ErrorQuery(error, errorString);
                   // res = driver.Driver.Utility.ErrorQuery();
                }

                // Exit program if error is determined
                if (checkstat)
                {
                    title += Environment.NewLine + "Idriver Closed in internal error";
                    driver.Driver.Close();
                    driver = null;
                    return new Error(title);

                    //Environment.Exit(0); //after message box
                }
            }
            catch (Exception e)
            {
                title += Environment.NewLine + e.Message + "Idriver Closed in exception";
                driver.Driver.Close();
                driver = null;
                return new Error(title);

                //Environment.Exit(0); //after message box
            }

            return new Error(true, "No error");
        }
    }

   
}

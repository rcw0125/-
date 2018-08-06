using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VehIC_Device;

namespace Talent.MH.MC9500
{
    public class FixRFIDReaderWrapper
    {
        public int port = 1;

        public int baud = 9600;

        public string errmsg = string.Empty;

        public IntPtr porthandle = IntPtr.Zero;

        public bool torun = true;

        public FixRFIDReaderWrapper(int portid, int baudrate)
        {
            this.port = portid;
            this.baud = baudrate;
        }

        public bool OpenDevice()
        {
            this.CloseDevice();
            bool result;
            try
            {
                this.porthandle = FRRAPT.OpenCardReader(this.port, this.baud);
                if (!this.ISConnected())
                {
                    this.errmsg = "打开读写器串口失败!";
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                this.porthandle = IntPtr.Zero;
                this.errmsg = ex.ToString();
                result = false;
            }
            return result;
        }

        public bool CloseDevice()
        {
            bool result;
            try
            {
                FRRAPT.CloseCardReader(this.porthandle);
                this.porthandle = IntPtr.Zero;
                result = true;
            }
            catch (Exception ex)
            {
                this.errmsg = ex.ToString();
                result = false;
            }
            finally
            {
                this.porthandle = IntPtr.Zero;
            }
            return result;
        }

        public bool Available()
        {
            return this.torun && this.ISConnected();
        }

        public bool ISConnected()
        {
            return this.porthandle != IntPtr.Zero && !(this.porthandle.ToString() == "0");
        }

        public void Dispose()
        {
            this.CloseDevice();
        }

        public string Excute()
        {
            string result;
            if (!this.Available())
            {
                result = string.Empty;
            }
            else
            {
                try
                {
                    string text = string.Empty;
                    byte[] array = new byte[128];
                    for (int i = 0; i < 128; i++)
                    {
                        array[i] = 0;
                    }
                    switch (FRRAPT.QueryRFCard(this.porthandle, 82, array))
                    {
                        case 254:
                        case 255:
                            this.errmsg = "通讯失败";
                            result = string.Empty;
                            break;
                        default:
                            if (array[2] == 0 && array[3] == 0 && array[4] == 0 && 0 == array[5])
                            {
                                this.errmsg = "没有卡片";
                                result = string.Empty;
                            }
                            else
                            {
                                string text2 = string.Empty;
                                string str = string.Empty;
                                for (int j = 0; j < 4; j++)
                                {
                                    text2 += array[j + 4].ToString("X2");
                                }
                                for (int j = 0; j < 7; j++)
                                {
                                    str += array[j + 4].ToString("X2");
                                }
                                byte b = array[1];
                                if (b <= 4)
                                {
                                    if (b != 1)
                                    {
                                        if (b == 4)
                                        {
                                            text = string.Empty;
                                            this.Beep(3, 200);
                                        }
                                    }
                                    else
                                    {
                                        text = string.Empty;
                                        this.Beep(3, 200);
                                    }
                                }
                                else if (b != 8)
                                {
                                    if (b == 16)
                                    {
                                        text = string.Empty;
                                        this.Beep(3, 200);
                                    }
                                }
                                else
                                {
                                    text = text2;
                                }
                                result = text;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.errmsg = ex.ToString();
                    result = null;
                }
            }
            return result;
        }

        public void Beep(int times, int timespan)
        {
            if (this.ISConnected())
            {
                try
                {
                    for (int i = 0; i < times; i++)
                    {
                        FRRAPT.RFReaderBeep(this.porthandle);
                        Thread.Sleep(timespan);
                    }
                }
                catch (Exception ex)
                {
                    this.errmsg = ex.ToString();
                }
            }
        }

        public void Lamp(int lamppos, int times)
        {
            if (this.ISConnected())
            {
                try
                {
                    for (int i = 0; i < times; i++)
                    {
                        FRRAPT.RFReaderLed(this.porthandle, lamppos, 1);
                        Thread.Sleep(500);
                        FRRAPT.RFReaderLed(this.porthandle, lamppos, 0);
                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    this.errmsg = ex.ToString();
                }
            }
        }

        public string Excute(int controlid)
        {
            return this.Excute();
        }

        public bool Kz(int controlid, int addr)
        {
            return false;
        }
    }
}

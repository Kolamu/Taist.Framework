namespace Mock.Tools.Controls
{
    using System;
    using System.Runtime.InteropServices;

    using Mock.Tools.Exception;
    using Mock.Nature.Native;
    internal class KeybordObject : WinObject
    {
        internal KeybordObject()
        {
        }
        
        [DllImport("user32.dll")]
        private static extern void keybd_event(Data.VK cVk, int bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(Data.VK keyCode, int uMapType);

        internal void Input(string s)
        {
            //NativeMethods.BlockInput(true);
            if (s == null || s == string.Empty)
            {
                return;
            }
            char []cs = s.ToCharArray();
            foreach (char c in cs)
            {
                byte[] bts = System.Text.Encoding.Default.GetBytes(c.ToString());
                System.Decimal MachineCode = 0;
                for (int i = 0; i < bts.Length; i++)
                {
                    MachineCode = MachineCode + (bts[i] << (bts.Length - i - 1) * 8);
                }

                keybd_event(Data.VK.ALT, MapVirtualKey(Data.VK.ALT, 0), 0x0, 0);
                char[] mcChar = MachineCode.ToString().ToCharArray();
                foreach (char mc in mcChar)
                {
                    Data.VK vk = (Data.VK)Enum.Parse(typeof(Data.VK), string.Format("NUMPAD{0}", mc));
                    keybd_event(vk, MapVirtualKey(vk, 0), 0, 0);
                    Wait(10);
                    keybd_event(vk, MapVirtualKey(vk, 0), 0x2, 0);
                    Wait(10);
                }
                keybd_event(Data.VK.ALT, MapVirtualKey(Data.VK.ALT, 0), 0x2, 0);
            }
        }

        internal void KeyDown(Mock.Data.VK vk)
        {
            //NativeMethods.BlockInput(true);
            keybd_event(vk, MapVirtualKey(vk, 0), 0, 0);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    LogManager.Debug("Input Unlock Fail");
            //}
        }

        internal void KeyUp(Mock.Data.VK vk)
        {
            //NativeMethods.BlockInput(true);
            keybd_event(vk, MapVirtualKey(vk, 0), 0x2, 0);
            //while (!NativeMethods.BlockInput(false))
            //{
            //    Wait(100);
            //    LogManager.Debug("Input Unlock Fail", NoteType.ERROR);
            //}
        }
    }
}

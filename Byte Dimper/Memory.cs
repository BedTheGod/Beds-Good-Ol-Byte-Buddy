using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Byte_Dimper
{
    public class Memory
    {
        public bool IsProcessRunning(string processName)
        {
            try
            {
                Process process = Process.GetProcessesByName(processName)[0];
                if (process.Handle.ToInt64() != 0L)
                {
                    this.BaseAddress = process.MainModule.BaseAddress.ToInt64();
                    this.ProcessID = process.Id;
                    this.ProcessHandle = process.Handle;
                    return true;
                }
            }
            catch (Exception)
            {
                this.BaseAddress = 0L;
                this.ProcessID = 0;
                this.ProcessHandle = IntPtr.Zero;
                return false;
            }
            return false;
        }

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [Out] byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

        public void WriteInt(long pAddress, int value)
        {
            try
            {
                uint num = 0U;
                Memory.WriteProcessMemory(this.ProcessHandle, pAddress, BitConverter.GetBytes(value), 4U, out num);
            }
            catch (Exception)
            {
            }
        }

        public void WriteByte(long _lpBaseAddress, byte _Value)
        {
            byte[] bytes = BitConverter.GetBytes((short)_Value);
            uint num = 0u;
            Memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, bytes, (uint)(bytes.Length - 1), out num);
        }

        public void WriteUInt(long pAddress, uint value)
        {
            try
            {
                uint num = 0U;
                Memory.WriteProcessMemory(this.ProcessHandle, pAddress, BitConverter.GetBytes(value), 4U, out num);
            }
            catch (Exception)
            {
            }
        }

        public long GetPointerInt(long add, long[] offsets, int level)
        {
            long num = add;
            for (int i = 0; i < level; i++)
            {
                num = this.ReadInt64(num) + offsets[i];
            }
            return num;
        }

        public void WriteBytes(long _lpBaseAddress, byte[] _Value)
        {
            for (int i = 0; i < _Value.Length; i++)
            {
                byte[] bytes = BitConverter.GetBytes((short)_Value[i]);
                uint num = 0u;
                Memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, bytes, (uint)(bytes.Length - 1), out num);
            }
        }

        public void WriteXBytes(long _lpBaseAddress, byte[] _Value)
        {
            uint zero = 0u;
            Memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, _Value, (uint)_Value.Length, out zero);
        }

        public long ReadInt64(long pAddress)
        {
            try
            {
                uint num = 0U;
                byte[] array = new byte[8];
                if (Memory.ReadProcessMemory(this.ProcessHandle, pAddress, array, 8U, out num))
                {
                    return BitConverter.ToInt64(array, 0);
                }
            }
            catch (Exception)
            {
            }
            return 0L;
        }

        public string ReadString(long pAddress)
        {
            try
            {
                byte[] array = new byte[1280];
                uint num = 0U;
                if (Memory.ReadProcessMemory(this.ProcessHandle, pAddress, array, 1280U, out num))
                {
                    string text = "";
                    int num2 = 0;
                    while (array[num2] != 0)
                    {
                        string str = text;
                        char c = (char)array[num2];
                        text = str + c.ToString();
                        num2++;
                    }
                    return text;
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public int ReadInt(long pAddress)
        {
            try
            {
                uint num = 0U;
                byte[] array = new byte[4];
                if (Memory.ReadProcessMemory(this.ProcessHandle, pAddress, array, 4U, out num))
                {
                    return BitConverter.ToInt32(array, 0);
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public void WriteFloat(long pAddress, float value)
        {
            try
            {
                uint num = 0U;
                Memory.WriteProcessMemory(this.ProcessHandle, pAddress, BitConverter.GetBytes(value), 4U, out num);
            }
            catch (Exception)
            {
            }
        }

        public float ReadFloat(long pAddress)
        {
            try
            {
                uint num = 0U;
                byte[] array = new byte[4];
                if (Memory.ReadProcessMemory(this.ProcessHandle, pAddress, array, 4U, out num))
                {
                    return BitConverter.ToSingle(array, 0);
                }
            }
            catch (Exception)
            {
            }
            return 0f;
        }

        public byte[] ReadBytes(long pAddress, int length)
        {
            byte[] array = new byte[length];
            uint num = 0U;
            Memory.ReadProcessMemory(this.ProcessHandle, pAddress, array, (uint)length, out num);
            return array;
        }

        public void WriteBool(long pAddress, bool value)
        {
            try
            {
                byte[] buff = new byte[] { value ? ((byte)1) : ((byte)0) };
                uint num = 0U;
                Memory.WriteProcessMemory(this.ProcessHandle, pAddress, buff, (uint)buff.Length, out num);
            }
            catch (Exception)
            {
            }
        }

        public void WriteString(long pAddress, string pString)
        {
            try
            {
                uint num = 0U;

                if (Memory.WriteProcessMemory(this.ProcessHandle, pAddress, Encoding.UTF8.GetBytes(pString), (uint)pString.Length, out num))
                {
                    byte[] lpBuffer = new byte[1];
                    Memory.WriteProcessMemory(this.ProcessHandle, pAddress + (long)pString.Length, lpBuffer, 1U, out num);
                }
            }
            catch (Exception)
            {
            }
        }

        public long BaseAddress;

        public int ProcessID;

        public IntPtr ProcessHandle;
    }
}
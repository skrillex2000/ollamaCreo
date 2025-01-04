using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp2
{
    public class Creo
    {
        [DllImport("ConsoleApplication3.dll")]
       public static extern bool CreateFile(string fileName, string type);

        [DllImport("ConsoleApplication3.dll")]
        public static extern int GetInfo(StringBuilder filename,StringBuilder type,StringBuilder count);

        [DllImport("ConsoleApplication3.dll")]
        public static extern int HighLightallCurveEdges();


        [DllImport("ConsoleApplication3.dll")]
        public static extern int FetchChildParts(out IntPtr data);
    }
}

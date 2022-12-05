using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Helpers
{
    internal class MathHelper
    {
        static public int GetLCM(int[] vals)
        {
            int currentLCM = 1;

            foreach (int val in vals)
                currentLCM = LCM(currentLCM, val);

            return currentLCM;
        }

        static public int GetGCD(int[] vals)
        {
            int currentGCD = vals[0];
            
            foreach (int val in vals)
                currentGCD = GCD(currentGCD, val);

            return currentGCD;
        }

        static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static int LCM(int a, int b)
        {
            return (a / GCD(a, b)) * b;
        }
    }
}

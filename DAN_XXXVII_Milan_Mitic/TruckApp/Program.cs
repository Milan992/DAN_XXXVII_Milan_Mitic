using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TruckApp
{
    class Program
    {
        static readonly object l = new object();
        static int[] possibleRoutes = new int[1000];
        static List<int> divisable = new List<int>();
        static int[] chosenRoutes = new int[10];
        static Random random = new Random();

        static void Main(string[] args)
        {
            Thread begin = new Thread(() => GenerateRoutNumber());
            Thread manager = new Thread(() => ChooseRoute());

            begin.Start();
            manager.Start();
            Console.ReadLine();
        }

        public static void GenerateRoutNumber()
        {
            using (StreamWriter sw = new StreamWriter(@"..\..\possibleRoutes.txt", true))
            {

                for (int i = 0; i < possibleRoutes.Length; i++)
                {
                    try
                    {
                        possibleRoutes[i] = random.Next(1, 5001);
                        sw.WriteLine(possibleRoutes[i]);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("\nRout numbers generated");
        }

        private static void ChooseRoute()
        {
            Thread.Sleep(3000);
            Console.WriteLine("\nManager notifies drivers that routes are chosen");
            try
            {
                using (StreamReader sr = new StreamReader(@"..\..\possibleRoutes.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (Convert.ToInt32(line) % 3 == 0)
                        {
                            divisable.Add(Convert.ToInt32(line));
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            for (int i = 0; i < chosenRoutes.Length; i++)
            {
                chosenRoutes[i] = divisable.Min();
                Console.WriteLine("\nRout {0} is chosen", chosenRoutes[i]);
                divisable.Remove(divisable.Min());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace TruckApp
{
    class Program
    {
        static readonly object l = new object();
        static int[] possibleRoutes = new int[1000];
        static List<int> divisable = new List<int>();
        static int[] chosenRoutes = new int[10];
        static Random random = new Random();
        static Thread[] trucks = new Thread[10];
        static SemaphoreSlim semaphore = new SemaphoreSlim(2);
        static Semaphore s = new Semaphore(10, 10);
        static int routTime;
        static int i = -1;

        static void Main(string[] args)
        {
            // clean txt file
            File.WriteAllText(@"..\..\possibleRoutes.txt", "");

            Thread begin = new Thread(() => GenerateRoutNumber());
            Thread manager = new Thread(() => ChooseRoute());

            begin.Start();
            manager.Start();
            manager.Join();

            Thread t = new Thread(() => FillTruck());
            for (int i = 0; i < 10; i++)
            {
                t = new Thread(() => FillTruck());
                t.Name = "Truck_" + Convert.ToString(i + 1);
                trucks[i] = t;
                t.Start();
            }
            t.Join();

            Console.ReadLine();
        }

        /// <summary>
        /// generates 1000 random numbers and writes them to a txt file.
        /// </summary>
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

        /// <summary>
        /// puts all the numbers divisable by 3 in a list.
        /// </summary>
        public static void ChooseRoute()
        {
            Thread.Sleep(random.Next(1, 3001));
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

            //get minimum value from the list and then remove it from the list as much times as the array's lenght is.
            for (int i = 0; i < chosenRoutes.Length; i++)
            {
                chosenRoutes[i] = divisable.Min();
                Console.WriteLine("\nRout {0} is chosen", chosenRoutes[i]);
                divisable.Remove(divisable.Min());
            }
        }

        /// <summary>
        /// simulates time needed to fill a truck.
        /// </summary>
        public static void FillTruck()
        {
            semaphore.Wait();
            int timeFilling = random.Next(500, 5001);
            Thread.Sleep(5000);
            Console.WriteLine("\n{0} was filled in {1} miliseconds", Thread.CurrentThread.Name, timeFilling);
            semaphore.Release();

            i++;
            Console.WriteLine("\n{0} gets the rout {1}", Thread.CurrentThread.Name, chosenRoutes[i]);

            while (i < 9)
            {
                s.WaitOne();
            }

            Thread.Sleep(4);
            routTime = random.Next(500, 5001);
            s.Release();
            Thread order = new Thread(() => Rout(routTime, timeFilling));
            order.Name = Thread.CurrentThread.Name;
            order.Start();
            Console.WriteLine("\n{0} notifies the order that he will arrive in {1} miliseconds", Thread.CurrentThread.Name, routTime);
        }

        /// <summary>
        /// simulates unloading a truck.
        /// </summary>
        /// <param name="routTime"></param>
        /// <param name="timeFilling"></param>
        public static void Rout(int routTime, int timeFilling)
        {
            if (routTime < 3000)
            {
                Thread.Sleep(routTime);
                Console.WriteLine("\n{0} arrived", Thread.CurrentThread.Name);
                double time = timeFilling / 1.5;
                Thread.Sleep(Convert.ToInt32(time));
                Console.WriteLine("\n{0} unloaded in {1} miliseconds", Thread.CurrentThread.Name, Convert.ToInt32(time));
            }
            else
            {
                Thread.Sleep(3000);
                Console.WriteLine("\nOrder canceled, {0} came back in 3 seconds", Thread.CurrentThread.Name);
            }
        }
    }
}

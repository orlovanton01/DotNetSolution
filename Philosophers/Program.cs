using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Philosophers
{
    class Philosopher
    {
        public string Name { get; }
        public int ThinkTime { get; }
        public int EatTime { get; }
        public PhilosopherState State { get; set; }
        public DateTime LastStateChangeTime { get; set; }

        public Philosopher(string name, int thinkTime, int eatTime)
        {
            Name = name;
            ThinkTime = thinkTime;
            EatTime = eatTime;
            LastStateChangeTime = DateTime.Now;
        }
    }

    enum PhilosopherState
    {
        Thinking,
        Hungry,
        Eating
    }

    class Waiter
    {
        private SemaphoreSlim semaphore;
        private int eatingPhilosophers = 0;
        private object lockObj = new object();

        // Конструктор для определения максимального количества философов, которые могут есть одновременно
        public Waiter(int numberOfPhilosophers)
        {
            int maxEating = (numberOfPhilosophers - 1) / 2; // Максимальное количество философов, которые могут есть одновременно
            semaphore = new SemaphoreSlim(maxEating, maxEating); // Устанавливаем лимит
        }

        public async Task<bool> TryEnterAsync()
        {
            await semaphore.WaitAsync(); // Блокируем официанта для получения вилок

            lock (lockObj)
            {
                eatingPhilosophers++; // Философ начинает есть
            }
            return true;
        }

        public void Leave()
        {
            lock (lockObj)
            {
                eatingPhilosophers--; // Философ закончил есть
            }
            semaphore.Release(); // Освобождаем вилку
        }
    }

    internal class Program
    {
        static List<Philosopher> philosophers = new List<Philosopher>();
        static Waiter waiter;

        static void PhilosopherLife(Philosopher philosopher, Waiter waiter)
        {
            while (true)
            {
                philosopher.State = PhilosopherState.Thinking;
                philosopher.LastStateChangeTime = DateTime.Now;
                Thread.Sleep(philosopher.ThinkTime);

                philosopher.State = PhilosopherState.Hungry;
                philosopher.LastStateChangeTime = DateTime.Now;

                // Проверяем, может ли философ начать есть
                if (waiter.TryEnterAsync().Result)
                {
                    philosopher.State = PhilosopherState.Eating;
                    philosopher.LastStateChangeTime = DateTime.Now;
                    Thread.Sleep(philosopher.EatTime);

                    waiter.Leave(); // Философ закончил есть
                }
            }
        }

        static void LoadPhilosophersFromXml(string path)
        {
            var doc = XDocument.Load(path);
            foreach (var elem in doc.Descendants("Philosopher"))
            {
                var name = elem.Element("Name")?.Value;
                var thinkTime = int.Parse(elem.Element("ThinkTime")?.Value);
                var eatTime = int.Parse(elem.Element("EatTime")?.Value);

                philosophers.Add(new Philosopher(name, thinkTime, eatTime));
            }
        }

        static void Main(string[] args)
        {
            // Загружаем философов из XML
            LoadPhilosophersFromXml("philosophers.xml");

            // Создаем официанта с количеством философов
            waiter = new Waiter(philosophers.Count);

            // Запускаем потоки для каждого философа
            foreach (var philosopher in philosophers)
            {
                Thread thread = new Thread(() => PhilosopherLife(philosopher, waiter));
                thread.Start();
            }

            // Основной цикл для вывода состояния философов
            while (true)
            {
                Console.Clear();
                foreach (var p in philosophers)
                {
                    TimeSpan elapsed = DateTime.Now - p.LastStateChangeTime;
                    Console.WriteLine($"{p.Name}: {p.State} ({elapsed.TotalSeconds:F1} сек)");
                }
                Thread.Sleep(1000);
            }
        }
    }
}

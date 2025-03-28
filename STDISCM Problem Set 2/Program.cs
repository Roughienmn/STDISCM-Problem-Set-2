using System.Collections.Concurrent;

class Program
{
    public static uint dungeonCount, tankCount, healerCount, dpsCount, t1, t2;
    public static bool emptyGroupQueue = false;
    private static Dungeon[] dungeons;
    private static ConcurrentQueue<Dungeon> emptyQueue = new ConcurrentQueue<Dungeon>();
    private static ConcurrentQueue<Dungeon> activeQueue = new ConcurrentQueue<Dungeon>();

    static bool ReadConfig()
    {
        uint n = 0, t = 0, h = 0, d = 0, t_1 = 0, t_2 = 0;
        string[] lines = File.ReadAllLines("..\\..\\..\\config.txt");
        foreach (string line in lines)
        {
            string commentLine = line.Split("//")[0];
            string[] parts = commentLine.Split('=');

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            if (parts.Length != 2)
            {
                continue;
            }

            if (!uint.TryParse(parts[1], out uint value))
            {
                Console.WriteLine($"Invalid value for {parts[0]}: {parts[1]}");
                return false;
            }

            switch (parts[0])
            {
                case "n":
                    n = value;
                    break;
                case "t":
                    t = value;
                    break;
                case "h":
                    h = value;
                    break;
                case "d":
                    d = value;
                    break;
                case "t1":
                    t_1 = value;
                    break;
                case "t2":
                    t_2 = value;
                    break;
            }
        }

        if (n == 0)
        {
            Console.WriteLine("Dungeon instances must be greater than 0.");
            return false;
        }

        if (t < 1 || h < 1 || d < 3)
        {
            Console.WriteLine("Not enough members to form a full group.");
            return false;
        }
        if (t_1 == 0 || t_2 == 0)
        {
            Console.WriteLine("Time to complete dungeon must be greater than 0");
            return false;
        }

        if (n > 1000)
        {
            Console.WriteLine("Dungeon count is too large.");
            return false;
        }

        if (t_2 > 15)
        {
            Console.WriteLine("Input value for t2 is greater than 15.");
            return false;
        }

        if (t_1 > t_2)
        {
            Console.WriteLine("Value for t1 cannot be greater than t2 nor greater than 15.");
            return false;
        }

        dungeonCount = n;
        tankCount = t;
        healerCount = h;
        dpsCount = d;
        t1 = t_1;
        t2 = t_2;

        return true;
    }
    
    static void Main(string[] args)
    {
        if (!ReadConfig())
        {
            Console.WriteLine("Failed to read config file.");
            return;
        }

        Console.WriteLine("Config file read successfully!");
        Console.WriteLine("Dungeon instance count: {0}", dungeonCount);
        Console.WriteLine("Tank player count: {0}", tankCount);
        Console.WriteLine("Healer player count: {0}", healerCount);
        Console.WriteLine("DPS player count: {0}", dpsCount);
        Console.WriteLine("Time to complete dungeon: {0} - {1}", t1, t2);

        dungeons = new Dungeon[dungeonCount];
        for (uint i = 0; i < dungeonCount; i++)
        {
            dungeons[i] = new Dungeon(i);
            dungeons[i].Start();
            emptyQueue.Enqueue(dungeons[i]);
        }


        while (tankCount >= 1 && healerCount >= 1 && dpsCount >= 3)
        {   
            if (activeQueue.TryDequeue(out Dungeon activeDungeon))
            {
                if(activeDungeon.CurrentState == Dungeon.State.EMPTY)
                {
                    emptyQueue.Enqueue(activeDungeon);
                }
                else
                {
                    activeQueue.Enqueue(activeDungeon);
                }
            }

            if (emptyQueue.TryDequeue(out Dungeon emptyDungeon))
            {
                emptyDungeon.AddParty();
                tankCount--;
                healerCount--;
                dpsCount -= 3;
                activeQueue.Enqueue(emptyDungeon);
            }

            Thread.Sleep(100);
        }

        emptyGroupQueue = true;

        // No more possible parties to add, wait for all dungeons to be empty
        bool allEmpty;
        do
        {
            allEmpty = true;
            foreach (var dungeon in dungeons)
            {
                if (dungeon.CurrentState != Dungeon.State.EMPTY)
                {
                    allEmpty = false;
                    break;
                }
            }
            if (!allEmpty)
            {
                Thread.Sleep(100);
            }
        } while (!allEmpty);


        foreach (var dungeon in dungeons)
        {
            dungeon.Join();
        }

        Console.WriteLine();

        Console.WriteLine("Dungeon Stats Summary:");

        foreach (var dungeon in dungeons)
        {
            dungeon.PrintStats();
        }

    }
}
class Program
{
    public static uint dungeonCount, tankCount, healerCount, dpsCount, t1, t2;
    public static bool emptyQueue = false;
    private static Dungeon[] dungeons;
    static bool ReadConfig()
    {
        uint n = 0, t = 0, h = 0, d = 0, t_1 = 0, t_2 = 0;
        string[] lines = File.ReadAllLines("config.txt");
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

        if (n == 0 || t == 0 || h == 0 || d == 0 || t_1 == 0 || t_2 == 0)
        {
            Console.WriteLine("Input values must be greater than 0");
            return false;
        }

        if (t_2 > 15)
        {
            Console.WriteLine("Input value for t2 is greater than 15. Setting to 15.");
            t_2 = 15;
        }

        if (t_1 > t_2)
        {
            Console.WriteLine("Value for t1 cannot be greater than t2. Setting t1 to be equal to t2");
            t_1 = t_2;
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
    
    public static void printDungeonStatus()
    {
        foreach (var dungeon in dungeons)
        {
            dungeon.PrintStatus();
        }
    }
    
    static void Main(string[] args)
    {
        if (!ReadConfig())
        {
            Console.WriteLine("Failed to read config file.");
            return;
        }

        Console.WriteLine("Config file read successfully!");

        dungeons = new Dungeon[dungeonCount];
        for (uint i = 0; i < dungeonCount; i++)
        {
            dungeons[i] = new Dungeon(i);
            dungeons[i].Start();
        }

        uint currIndex = 0;

        while (tankCount > 1 && healerCount > 1 && dpsCount > 3)
        {   
            currIndex = currIndex % dungeonCount;
            if (dungeons[currIndex].CurrentState == Dungeon.State.EMPTY)
            {
                dungeons[currIndex].AddParty();
                tankCount--;
                healerCount--;
                dpsCount -= 3;
            }

            currIndex++;
            Thread.Sleep(100);
        }

        emptyQueue = true;

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

        foreach (var dungeon in dungeons)
        {
            dungeon.PrintStats();
        }

    }
}
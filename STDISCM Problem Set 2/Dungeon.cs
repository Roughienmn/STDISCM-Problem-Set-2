using System;
using System.Threading;

public class Dungeon
{
    private uint id;
    private uint partyCount, totalTimeServed;
    private State state;
    private Thread thread;

    public enum State
    {
        EMPTY,
        ACTIVE
    }

    public Dungeon(uint id)
    {
        this.id = id;
        this.partyCount = 0;
        this.state = State.EMPTY;
        this.thread = new Thread(Run);
    }

    public void Start()
    {
        this.thread.Start();
    }

    public void AddParty()
    {
        this.state = State.ACTIVE;
    }

    private void Run()
    {
        while (!Program.emptyQueue)
        {
            if(this.state == State.EMPTY)
            {
                continue;
            }

            int sleepTime = new Random().Next((int)Program.t1, (int)Program.t2);
            this.partyCount++;
            this.totalTimeServed += (uint)sleepTime;
            Thread.Sleep(sleepTime*1000);

            this.state = State.EMPTY;
        }
    }

    public State CurrentState
    {
        get
        {
            return state;
        }
    }

    public void PrintStatus()
    {
        Console.WriteLine($"[Dungeon {id}] Status: {state}");
    }

    public void PrintStats()
    {
        Console.WriteLine($"[Dungeon {id}]");
        Console.WriteLine($"Total parties served: {partyCount}");
        Console.WriteLine($"Total time served: {totalTimeServed} seconds");
    }

    public void Join()
    {
        this.thread.Join();
    }
}
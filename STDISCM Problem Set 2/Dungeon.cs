using System;
using System.Threading;

public class Dungeon
{
    private uint id;
    private uint partyCount;
    private State state;
    private Thread thread;
    private readonly object stateLock = new object(); //lock for printing

    private enum State
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
        lock (stateLock)
        {
            this.state = State.ACTIVE;
        }
    }

    private void Run()
    {
        while (true)
        {
            lock (stateLock)
            {
                Console.WriteLine($"[Dungeon {id}] Current State: {state}");
            }

            Thread.Sleep(5000);

            lock (stateLock)
            {
                this.state = State.EMPTY;
            }
        }
    }

    public void PrintStatus()
    {
        lock (stateLock)
        {
            Console.WriteLine($"[Dungeon {id}] Status: {state}");
        }
    }
}
using System;

public class Dungeon
{
    private uint id;
    private State state;
    private Thread thread;
    enum State
    {
        EMPTY,
        ACTIVE
    }

    public Dungeon(uint id)
    {
        this.id = id;
        this.state = State.EMPTY;
        this.thread = new Thread(Run);
    }

    public void Start()
    {
        this.thread.Start();
    }

    private void Run()
    {
        
    }
}

 using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class LogHandler
{
    private Stopwatch watch;
    private StreamWriter writer;

    public LogHandler()
    {
        writer = File.AppendText(Application.persistentDataPath + "/log.txt");
        writer.Write("\n\n=============== Game started ================\n\n");
        watch = Stopwatch.StartNew();
    }

    public void Write(string message)
    {
        watch.Stop();
        writer.Write(string.Format("\n[{0} (Delta: {1})] {2}", DateTime.Now, watch.ElapsedMilliseconds, message));
        watch.Reset();
        watch.Start();
    }

    ~LogHandler()
    {
        watch.Stop();
        writer.Flush();
        writer.Close();
    }
}

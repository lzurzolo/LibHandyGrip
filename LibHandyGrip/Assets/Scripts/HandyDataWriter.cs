using System;
using System.Collections.Generic;
using System.IO;

class HandyDataWriter
{
    public HandyDataWriter(string logFileName, bool appendDateToFileName, List<string> columnNames)
    {
        var formattedFileName = appendDateToFileName ? string.Format(logFileName + DateTime.Today.ToString("_MMddyyyy") + ".txt") : string.Format(logFileName + ".txt");

        DirectoryInfo di = Directory.CreateDirectory("log/");

        string[] matchingFileNames = Directory.GetFiles(di.FullName, logFileName + "*", SearchOption.TopDirectoryOnly);
        int matchingFileNameCount = matchingFileNames.Length;
        
        if(matchingFileNameCount > 0)
        {
            formattedFileName = appendDateToFileName ? string.Format(logFileName + System.DateTime.Today.ToString("_MMddyyyy") + "(" + matchingFileNameCount + ").txt") : string.Format(logFileName + "(" + matchingFileNameCount + ").txt");
        }

        sw = new StreamWriter(di.FullName + formattedFileName, true);

        foreach(var c in columnNames)
        {
            sw.Write(c);
            if(columnNames.IndexOf(c) != columnNames.Count - 1)
            {
                sw.Write(",");
            }
        }
        sw.Write("\n");
        
        var eventFileName = appendDateToFileName ? string.Format("event_" + logFileName + DateTime.Today.ToString("_MMddyyyy") + ".txt") : string.Format("event_" + logFileName + ".txt");
        
        string[] matchingEventFileNames = Directory.GetFiles(di.FullName, "event_" + logFileName + "*", SearchOption.TopDirectoryOnly);
        int matchingEventFileNameCount = matchingEventFileNames.Length;
        
        if(matchingEventFileNameCount > 0)
        {
            eventFileName = appendDateToFileName ? string.Format("event_" + logFileName + System.DateTime.Today.ToString("_MMddyyyy") + "(" + matchingFileNameCount + ").txt") : string.Format("event_" + logFileName + "(" + matchingFileNameCount + ").txt");
        }
        
        eventsw = new StreamWriter(di.FullName + eventFileName, true);
    }

    public void WriteFloats(float ms, List<float> data, string identifier = default(string))
    {
        sw.Write(ms);
        foreach (float f in data)
        {
            sw.Write(",");
            sw.Write(f);
        }

        if (!string.IsNullOrEmpty(identifier))
        {
            sw.Write(",");
            sw.Write(identifier);
        }

        sw.Write("\n");
    }

    public void WriteEvent(float ms, List<string> data)
    {
        eventsw.Write(ms);

        foreach (var s in data)
        {
            eventsw.Write(",");
            eventsw.Write(s);
        }
        eventsw.Write("\n");
    }

    public void Close()
    {
        sw.Close();
        eventsw.Close();
    }

    private StreamWriter sw;
    private StreamWriter eventsw;
};
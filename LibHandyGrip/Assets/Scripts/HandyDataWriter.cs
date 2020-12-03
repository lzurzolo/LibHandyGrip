using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public void Close()
    {
        sw.Close();
    }

    private StreamWriter sw;
};
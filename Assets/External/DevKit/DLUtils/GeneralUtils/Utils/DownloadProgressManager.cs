using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DownloadProgressManager
{
    public static async void FakeProgressInSeconds(float start, float end, int step, float sec, PrintProgress PrintFunc)
    {
        var curP = start;
        var epoch = (end - start) / step;
        PrintFunc(curP);
        for (int i = 0; i < epoch; i++)
        {
            var t = sec / epoch * 1000;
            await Task.Delay((int)t);
            curP += step;
            PrintFunc(curP);
        }
        PrintFunc(end);
    }


    public static async void FakeProgressInSeconds(float start, float end, int step, float sec, string header="DownloadProgress")
    {
        void DefaultPrintFunction(float curP)
        {
            string msg = header+ "=>" + (int)curP;

            PHUtils.SendMessageToFlutter(msg, editorPrint:true);
        }

        var curP = start;
        var epoch = (end - start) / step;
        DefaultPrintFunction(curP);
        for (int i = 0; i < epoch; i++)
        {
            var t = sec / epoch * 1000;
            await Task.Delay((int)t);
            curP += step;
            //DefaultPrintFunction(curP);
        }
        DefaultPrintFunction(end);
    }

}
public delegate void PrintProgress(float progress);


public class DownloadProcessManager
{

    public bool isActivate = false;

    public int currentProgress = 1;

    public int Count = 1;
    public int currentNum = 1;

    public bool debug = true;

    public Dictionary<string, float> CostTime = new Dictionary<string, float>();
    public string downloaderName = "";

    public virtual void Prepare(int showMin, int showMax)
    {
        
    }
    

    
    public void Finish()
    {
        PrintMessage(100);

        PHUtils.SendMessageToFlutter(ConstDependency.DownloadSuccess, "Done", editorPrint: debug);

        isActivate = false;
        Clear();
    }


    protected void PrintMessage(int progress)
    {
        if (!isActivate || currentProgress > progress) return;
        currentProgress = progress;

        PHUtils.SendMessageToFlutter(ConstDependency.DownloadProgress, progress, editorPrint: debug);

    }

    protected void PrintMessage(float progress)
    {
        PrintMessage((int)progress);
    }


    public virtual void Clear()
    {
        CostTime.Clear();
    }

    public virtual void StartProgress(string i, string hint="")
    {
        TimeCollector.Start(i, hint);
    }
    public virtual void EndProgress(string i, string hint = "")
    {
        TimeCollector.End(i, hint);
    }
}


public class TimeCollector
{
    public static Dictionary<string, float> mem = new Dictionary<string, float>();

    public static void Start(string key, string hint="")
    {
        mem[key] = Time.time;
        Debug.LogFormat("TimeCollector=>{0}=>Start at {1}, hint:{2}",key, mem[key],hint);
    }
    public static void End(string key, string hint="")
    {
        Debug.LogFormat("TimeCollector=>{0}=>Cost {1}, hint:{2}", key, Time.time - mem[key], hint);
    }
}
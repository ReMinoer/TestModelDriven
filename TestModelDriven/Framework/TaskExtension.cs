using System.Runtime.ExceptionServices;
using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

static public class TaskExtension
{
    static public async void CaptureThrow(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
        }
    }

    static public async void CaptureThrow<T>(this Task<T> task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(e).Throw();
        }
    }
}
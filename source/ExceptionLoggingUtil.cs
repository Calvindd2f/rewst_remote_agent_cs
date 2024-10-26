using System;
using System.IO;
using System.Net;

namespace Rewst.RemoteAgent.Calvindd2f
{
    public static class ExceptionLoggingUtil
    {
        public static string GetExceptionInformation(System.Exception e)
        {
            try
            {
                if (e == null)
                {
                    return "";
                }
                string text = "";
                if (e.Message != null)
                {
                    text = text + "Exception Occured. Message: " + e.Message + "\r\n";
                }
                if (e is System.Net.WebException)
                {
                    try
                    {
                        using (
                            System.IO.Stream responseStream = (
                                (System.Net.WebException)e
                            ).Response.GetResponseStream()
                        )
                        {
                            using (
                                System.IO.StreamReader streamReader = new System.IO.StreamReader(
                                    responseStream
                                )
                            )
                            {
                                text = text + "Web Error Response: " + streamReader.ReadToEnd();
                            }
                        }
                    }
                    catch (System.Exception) { }
                }
                if (e.StackTrace != null)
                {
                    text = text + "Stack Trace: " + e.StackTrace + "\r\n";
                }
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message != null)
                    {
                        text =
                            text
                            + "Inner Exception Message: "
                            + e.InnerException.StackTrace
                            + "\r\n";
                    }
                    if (e.InnerException is System.Net.WebException)
                    {
                        try
                        {
                            using (
                                System.IO.Stream responseStream2 = (
                                    (System.Net.WebException)e.InnerException
                                ).Response.GetResponseStream()
                            )
                            {
                                using (
                                    System.IO.StreamReader streamReader2 =
                                        new System.IO.StreamReader(responseStream2)
                                )
                                {
                                    text =
                                        text
                                        + "Inner Exception Web Error: "
                                        + streamReader2.ReadToEnd();
                                }
                            }
                        }
                        catch (System.Exception) { }
                    }
                    if (e.InnerException.StackTrace != null)
                    {
                        text = text + "Inner Exception Stack Trace: " + e.InnerException.StackTrace;
                    }
                }
                return text;
            }
            catch (System.Exception) { }
            return "Error could not be determined.";
        }
    }
}

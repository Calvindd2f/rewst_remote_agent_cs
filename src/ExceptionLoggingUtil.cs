using System.Net;

namespace Rewst.RemoteAgent
{
    public static class ExceptionLoggingUtil
    {
        public static string GetExceptionInformation(Exception e)
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
                if (e is WebException)
                {
                    try
                    {
                        using Stream responseStream = ((WebException)e).Response.GetResponseStream();
                        using StreamReader streamReader = new(responseStream);
                        text = text + "Web Error Response: " + streamReader.ReadToEnd();
                    }
                    catch (Exception) { }
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
                    if (e.InnerException is WebException)
                    {
                        try
                        {
                            using Stream responseStream2 = (e.InnerException as WebException).Response.GetResponseStream();
                            using StreamReader streamReader2 = new(responseStream2);
                            text = text + "Inner Exception Web Error: " + streamReader2.ReadToEnd();
                        }
                        catch (Exception) { }
                    }
                    if (e.InnerException.StackTrace != null)
                    {
                        text = text + "Inner Exception Stack Trace: " + e.InnerException.StackTrace;
                    }
                }
                return text;
            }
            catch (Exception) { }
            return "Error could not be determined.";
        }
    }
}

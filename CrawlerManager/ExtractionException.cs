using System;
using System.Collections;

namespace CarBase.CrawlerManager
{
  public class ExtractionException : ApplicationException
  {
    public ExtractionException()
    {
    }

    public ExtractionException(string message) 
      : base(message)
    {
    }

    public ExtractionException(string message, Exception innerException)
      : base(message, innerException)
    {
      if (innerException.Data.Count > 0)
      {
        foreach (DictionaryEntry entry in innerException.Data)
        {
          if (!Data.Contains(entry.Key))
            Data.Add(entry.Key, entry.Value);
        }
      }
    }
  }
}

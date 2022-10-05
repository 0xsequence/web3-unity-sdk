using System;

namespace SequenceSharp
{
    public class JSExecutionException : Exception
    {
        public JSExecutionException(string err) : base(string.Format("Error while executing JS: {0}", err))
        {

        }
    }
}

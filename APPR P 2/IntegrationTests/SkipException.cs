using System;

namespace APPR_P_2.IntegrationTests
{
    public class SkipException : Exception
    {
        public SkipException(string message) : base(message) { }
    }
}

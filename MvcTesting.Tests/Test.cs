using NUnit.Framework;

namespace MvcTesting.Tests
{
    public static class Test
    {
        public static void WriteProgress(string line)
        {
            TestContext.Progress.WriteLine(line);
        }
    }
}

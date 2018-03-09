namespace MvcTesting.StubApp.Views.Stub
{
    public class SimpleFormModel
    {
        public int      Id      { get; set; }
        public string   Text    { get; set; }
        public string   Error   { get; set; }

        public SimpleFormModel SetError(string error)
        {
            Error = error;
            return this;
        }
    }
}

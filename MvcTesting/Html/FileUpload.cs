namespace MvcTesting.Html
{
    public class FileUpload
    {
        public FileUpload(string formName, string fileName, byte[] content)
        {
            FormName = formName;
            FileName = fileName;
            Content = content;
        }

        public string FormName  { get; }
        public string FileName  { get; }
        public byte[] Content   { get; }
    }
}

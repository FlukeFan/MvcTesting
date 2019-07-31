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

        public string FormName  { get; private set; }
        public string FileName  { get; private set; }
        public byte[] Content   { get; private set; }

        public FileUpload SetContent(string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
            return this;
        }
    }
}

namespace С4S.API.Features.GameStatisticReport.ViewModels
{
    public class FileViewModel
    {
        public byte[] Bytes { get; private set; }
        public string Name { get; private set; }
        public string MimeType { get; private set; }

        public FileViewModel(
            byte[] bytes,
            string name,
            string mimeType)
        {
            Bytes = bytes;
            Name = name;
            MimeType = mimeType;
        }
    }
}

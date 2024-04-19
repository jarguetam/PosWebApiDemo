using System;
namespace Pos.WebApi.Features.Common.Dto
{
    public class FileDownloadDto
    {
        public string Path { get; set; }
        public string MimeType { get; set; }
    }
}

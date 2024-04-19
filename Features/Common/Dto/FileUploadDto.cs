using System;
namespace Pos.WebApi.Features.Common.Dto
{
    public class FileUploadDto: FileUpload
    {
        public string Path { get; set; }
    }
}

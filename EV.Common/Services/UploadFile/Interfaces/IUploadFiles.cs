using Microsoft.AspNetCore.Http;

namespace EV.Common.Services.UploadFile.Interfaces
{
    public interface IUploadFiles
    {
        Task<IEnumerable<string>> ListFilesInAlbum(Guid? album, string type);

        Task<IEnumerable<string>> UploadFiles(string type, List<IFormFile> blobs, Guid album);

        Task<IEnumerable<string>> DownloadFiles(List<string> blobs);

        Task<string> DeleteFiles(List<string> blobs);

        bool TestConnection();
    }
}

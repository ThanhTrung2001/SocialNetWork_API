using EnVietSocialNetWorkAPI.Models;

namespace EnVietSocialNetWorkAPI.Services.Upload
{
    public interface IUploadFileService
    {
        Task<ResponseModel<IEnumerable<string>>> ListFilesInAlbum(Guid? album, string type);

        Task<ResponseModel<IEnumerable<string>>> UploadFiles(string type, List<IFormFile> blobs, Guid album);

        Task<ResponseModel<string>> DownloadFiles(List<string> blobs);

        Task<ResponseModel<string>> DeleteFiles(List<string> blobs);

        bool TestConnection();
    }
}

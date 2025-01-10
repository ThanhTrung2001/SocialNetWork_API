
namespace EnVietSocialNetWorkAPI.Services.Upload
{
    public class UploadFileService : IUploadFileService
    {


        public async Task DeleteFile(List<string> blobs)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> ListFilesInAlbum(Guid album)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> UploadFile(List<IFormFile> blobs, Guid album)
        {
            throw new NotImplementedException();
        }
    }
}

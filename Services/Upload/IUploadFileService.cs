namespace EnVietSocialNetWorkAPI.Services.Upload
{
    public interface IUploadFileService
    {
        Task<IEnumerable<string>> ListFilesInAlbum(Guid album);

        Task<IEnumerable<string>> UploadFile(List<IFormFile> blobs, Guid album);

        Task DeleteFile(List<string> blobs);
    }
}

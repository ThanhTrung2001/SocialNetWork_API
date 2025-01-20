using EnVietSocialNetWorkAPI.Models;
using Renci.SshNet;
using Renci.SshNet.Async;

namespace EnVietSocialNetWorkAPI.Services.Upload
{
    public class UploadFileService : IUploadFileService
    {
        //private readonly string _host;
        //private readonly int _port;
        //private readonly string _username;
        //private readonly string _password;
        //private readonly string _baseUrl;

        private readonly IConfiguration _configuration;

        public UploadFileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel<IEnumerable<string>>> ListFilesInAlbum(Guid album)
        {
            var files = new List<string>();
            try
            {
                using (var client = new SftpClient(_configuration["SFTP:host"], Int32.Parse(_configuration["SFTP:port"]), _configuration["SFTP:username"], _configuration["SFTP:password"]))
                {
                    client.Connect();
                    var fileList = client.ListDirectory(_configuration["SFTP:baseURL"] + "/" + album);
                    foreach (var file in fileList)
                    {
                        if (!file.IsDirectory) // Skip directories
                        {
                            files.Add(file.Name);
                        }
                    }
                    client.Disconnect();
                }
                return ResponseModel<IEnumerable<string>>.Success(files);
            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<string>>.Failure(ex.Message);

            }
        }

        public async Task<ResponseModel<IEnumerable<string>>> UploadFiles(string type, List<IFormFile> blobs, Guid album)
        {
            var fileUrls = new List<string>();
            try
            {
                using (var client = new SftpClient(_configuration["SFTP:host"], Int32.Parse(_configuration["SFTP:port"]), _configuration["SFTP:username"], _configuration["SFTP:password"]))
                {
                    client.Connect();
                    foreach (var file in blobs)
                    {
                        string fullPath = $"{type}/{album}/{file.FileName}";
                        //Upload
                        using (var stream = file.OpenReadStream())
                        {
                            await client.UploadAsync(stream, fullPath);
                            fileUrls.Add($"{_configuration["SFTP:baseURL"]}/{fullPath}");
                        }
                    }
                    client.Disconnect();
                }
                return ResponseModel<IEnumerable<string>>.Success(fileUrls);

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<string>>.Failure(ex.Message);

            }
        }

        public async Task<ResponseModel<string>> DownloadFiles(List<string> blobs)
        {
            try
            {
                using (var client = new SftpClient(_configuration["SFTP:host"], Int32.Parse(_configuration["SFTP:port"]), _configuration["SFTP:username"], _configuration["SFTP:password"]))
                {
                    client.Connect();
                    foreach (var file in blobs)
                    {
                        string fileName = Path.GetFileName(file);
                        string localPath = "D:/Download";
                        //ensure local folder exist
                        Directory.CreateDirectory(localPath);
                        using (var fileStream = new FileStream(localPath + "/" + fileName, FileMode.Create))
                        {
                            await client.DownloadAsync(file, fileStream);
                        }
                        client.Disconnect();
                    }
                }
                return ResponseModel<string>.Success("Download file successful.");
            }
            catch (Exception ex)
            {
                return ResponseModel<string>.Failure(ex.Message);
            }
        }

        public async Task<ResponseModel<string>> DeleteFiles(List<string> blobs)
        {
            try
            {
                using (var client = new SftpClient(_configuration["SFTP:host"], Int32.Parse(_configuration["SFTP:port"]), _configuration["SFTP:username"], _configuration["SFTP:password"]))
                {
                    client.Connect();
                    foreach (var file in blobs)
                    {
                        if (client.Exists(file))
                        {
                            client.Delete(file);
                            client.Disconnect();
                        }
                    }
                    client.Disconnect();
                }
                return ResponseModel<string>.Success("Delete file successful.");
            }
            catch (Exception ex)
            {
                return ResponseModel<string>.Failure(ex.Message);
            }
        }

    }
}

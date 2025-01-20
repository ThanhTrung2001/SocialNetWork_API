using EnVietSocialNetWorkAPI.Models;
using EnVietSocialNetWorkAPI.Services.Upload.Model;
using Renci.SshNet;
using Renci.SshNet.Async;

namespace EnVietSocialNetWorkAPI.Services.Upload
{
    public class UploadFileService : IUploadFileService
    {
        private readonly SFTPConfiguration _config;

        public UploadFileService(SFTPConfiguration config)
        {
            _config = config;
        }

        public async Task<ResponseModel<IEnumerable<string>>> ListFilesInAlbum(Guid? album)
        {
            var files = new List<string>();
            try
            {
                using (var client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password))
                {
                    client.Connect();
                    var fileList = client.ListDirectory(_config.BaseUrl + "/" + album ?? _config.BaseUrl + "/");
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
                using (var client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password))
                {
                    client.Connect();
                    foreach (var file in blobs)
                    {
                        string fullPath = $"{type}/{album}/{file.FileName}";
                        //Upload
                        using (var stream = file.OpenReadStream())
                        {
                            await client.UploadAsync(stream, fullPath);
                            fileUrls.Add($"{_config.BaseUrl}/{type}/{fullPath}");
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
                using (var client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password))
                {
                    client.Connect();
                    foreach (var file in blobs)
                    {
                        string fileName = Path.GetFileName(file);
                        string localPath = "D:/Download";
                        //ensure folder existed
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
                using (var client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password))
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

        public bool TestConnection()
        {
            try
            {
                // Create a connection to the SFTP server
                using (var sftp = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password))
                {
                    // Try connecting to the server
                    sftp.Connect();

                    // Check if connection is successful by listing the files in the root directory
                    var files = sftp.ListDirectory("/");

                    // If we successfully get the directory listing, the connection is working
                    Console.WriteLine("Successfully connected to SFTP server.");
                    foreach (var file in files)
                    {
                        Console.WriteLine(file.Name); // Print out file names in the root directory
                    }

                    sftp.Disconnect();
                }
                return true;
            }
            catch (Exception ex)
            {
                // If any exception occurs, the connection failed
                Console.WriteLine($"Error connecting to SFTP server: {ex.Message}");
                return false;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.ViewModels.Models;
using Xamarin.Essentials;

namespace IARA.Mobile.Pub.Helpers
{
    public static class CatchRecordFilesHelper
    {
        public const string CatchRecordDirectory = "CatchRecord";

        public static async Task<List<FileModel>> HandleAllFiles(List<CatchImageModel> files, string identifier)
        {
            string inspectionDirectory = Path.Combine(FileSystem.AppDataDirectory, CatchRecordDirectory + identifier);
            List<FileModel> resultFiles = new List<FileModel>(files.Count);

            if (!Directory.Exists(inspectionDirectory))
            {
                Directory.CreateDirectory(inspectionDirectory);
            }

            foreach (CatchImageModel inspectionFile in files)
            {
                if (inspectionFile.WasDeleted)
                {
                    if (inspectionFile.IsFileSavedLocally)
                    {
                        File.Delete(Path.Combine(inspectionDirectory, inspectionFile.Name));
                    }
                    else
                    {
                        resultFiles.Add(new FileModel
                        {
                            Id = inspectionFile.Id,
                            ContentType = inspectionFile.ContentType,
                            FullPath = inspectionFile.FullPath,
                            Name = inspectionFile.Name,
                            Size = inspectionFile.Size,
                            UploadedOn = DateTime.Now,
                            Deleted = true,
                        });
                    }
                }
                else
                {
                    string filePath;

                    if (inspectionFile.IsFileSavedLocally)
                    {
                        filePath = inspectionFile.FullPath;
                    }
                    else if (inspectionFile.WasAddedNow)
                    {
                        filePath = Path.Combine(inspectionDirectory, inspectionFile.Name);

                        using (FileStream stream = File.OpenRead(inspectionFile.FullPath))
                        using (FileStream newStream = File.OpenWrite(filePath))
                        {
                            await stream.CopyToAsync(newStream);
                        }

                        File.Delete(inspectionFile.FullPath);
                    }
                    else
                    {
                        filePath = null;
                    }

                    resultFiles.Add(new FileModel
                    {
                        Id = inspectionFile.Id,
                        ContentType = inspectionFile.ContentType,
                        FullPath = filePath,
                        Name = inspectionFile.Name,
                        Size = inspectionFile.Size,
                        UploadedOn = DateTime.Now,
                    });
                }
            }

            return resultFiles;
        }

        public static void DeleteFiles(string inspectionId)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, CatchRecordDirectory + inspectionId);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}

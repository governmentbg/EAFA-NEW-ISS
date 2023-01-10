using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Controls.ViewModels;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.Helpers
{
    /// <summary>
    /// Handles the files in the case they have to be saved offline.
    /// </summary>
    public static class InspectionFilesHelper
    {
        public const string InspectionDirectory = "Inspection";

        public static async Task<List<FileModel>> HandleAllFiles(List<InspectionFileViewModel> files, string inspectionId)
        {
            string inspectionDirectory = Path.Combine(FileSystem.AppDataDirectory, InspectionDirectory + inspectionId);
            List<FileModel> resultFiles = new List<FileModel>(files.Count);

            if (!Directory.Exists(inspectionDirectory))
            {
                Directory.CreateDirectory(inspectionDirectory);
            }

            foreach (InspectionFileViewModel inspectionFile in files)
            {
                if (inspectionFile.WasDeleted)
                {
                    if (inspectionFile.IsFileSavedLocally)
                    {
                        File.Delete(Path.Combine(inspectionDirectory, inspectionFile.File.FileName));
                    }
                    else
                    {
                        resultFiles.Add(new FileModel
                        {
                            Id = inspectionFile.Id,
                            ContentType = inspectionFile.File.ContentType,
                            Description = inspectionFile.Description,
                            FileTypeId = inspectionFile.FileType.Value,
                            FullPath = inspectionFile.File.FullPath,
                            Name = inspectionFile.File.FileName,
                            Size = inspectionFile.File.FileSize,
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
                        filePath = inspectionFile.File.FullPath;
                    }
                    else if (inspectionFile.WasAddedNow)
                    {
                        filePath = Path.Combine(inspectionDirectory, inspectionFile.File.FileName);

                        using (FileStream stream = File.OpenRead(inspectionFile.File.FullPath))
                        using (FileStream newStream = File.OpenWrite(filePath))
                        {
                            await stream.CopyToAsync(newStream);
                        }
                    }
                    else
                    {
                        filePath = null;
                    }

                    resultFiles.Add(new FileModel
                    {
                        Id = inspectionFile.Id,
                        ContentType = inspectionFile.File.ContentType,
                        Description = inspectionFile.Description,
                        FileTypeId = inspectionFile.FileType.Value,
                        FullPath = filePath,
                        Name = inspectionFile.File.FileName,
                        Size = inspectionFile.File.FileSize,
                        UploadedOn = DateTime.Now,
                    });
                }
            }

            return resultFiles;
        }

        public static void DeleteFiles(string inspectionId)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, InspectionDirectory + inspectionId);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}

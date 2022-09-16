using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using File = IARA.EntityModels.Entities.File;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class FileHelper
    {
        private const string JPEG_MIME_TYPE = "image/jpeg";

        public static List<FileInfoDTO> GetFiles<TFileEntity>(this IARADbContext db, DbSet<TFileEntity> dbSet, int recordId)
            where TFileEntity : class, ISoftDeletable, IFileEntity
        {
            return (from recordFile in dbSet
                    join file in db.Files on recordFile.FileId equals file.Id
                    where recordFile.RecordId == recordId
                         && recordFile.IsActive
                         && file.IsActive
                    select new FileInfoDTO
                    {
                        Id = file.Id,
                        FileTypeId = recordFile.FileTypeId,
                        Size = file.ContentLength,
                        Name = file.Name,
                        Description = file.Comments,
                        ContentType = file.MimeType,
                        UploadedOn = file.UploadedOn
                    }).ToList();
        }

        public static void DeleteFile<TFileEntity>(this IARADbContext db, int fileId)
            where TFileEntity : class, ISoftDeletable, IFileEntity
        {
            TFileEntity fileRefEntity = db.Set<TFileEntity>()
                .Include(x => x.File)
                .Where(x => x.FileId == fileId)
                .FirstOrDefault();

            if (fileRefEntity != null)
            {
                fileRefEntity.IsActive = false;
                fileRefEntity.File.ReferenceCounter--;
                if (fileRefEntity.File.ReferenceCounter == 0)
                {
                    fileRefEntity.File.IsActive = false;
                }
            }
        }

        public static void AddOrEditFile<TEntity, TFileEntity>(this IARADbContext db, TEntity record, ICollection<TFileEntity> recordFiles, FileInfoDTO file)
            where TEntity : class//, IBaseApplicationRegisterEntity
            where TFileEntity : class, ISoftDeletable, IFileEntity<TEntity>, new()
        {
            // file exists in this context
            if (file.Id.HasValue)
            {
                TFileEntity recordFile = (from recFile in recordFiles
                                          where recFile.FileId == file.Id.Value
                                                && recFile.FileTypeId == file.FileTypeId
                                          select recFile).SingleOrDefault();

                File oldFile = db.AddOrEditFile(file, increaseReferenceCounter: false);

                if (recordFile != null)
                {
                    recordFile.IsActive = !file.Deleted;
                }
                else
                {
                    recordFile = new TFileEntity
                    {
                        Record = record,
                        File = oldFile,
                        FileTypeId = file.FileTypeId
                    };

                    recordFiles.Add(recordFile);
                }
            }
            // file is new in this context
            else
            {
                File newFile = db.AddOrEditFile(file, increaseReferenceCounter: true);

                TFileEntity recordFile = null;

                if (newFile.Id != default)
                {
                    recordFile = (from recFile in recordFiles
                                  where recFile.FileId == newFile.Id
                                        && recFile.FileTypeId == file.FileTypeId
                                  select recFile).SingleOrDefault();
                }

                if (recordFile != null)
                {
                    recordFile.IsActive = true;
                }
                else
                {
                    recordFile = new TFileEntity
                    {
                        Record = record,
                        File = newFile,
                        FileTypeId = file.FileTypeId
                    };

                    recordFiles.Add(recordFile);
                }
            }
        }

        public static File AddOrEditFile(this IARADbContext db, FileInfoDTO fileInfo, bool increaseReferenceCounter = false)
        {
            if (fileInfo.Id != null)
            {
                if (fileInfo.Deleted)
                {
                    DeleteFile(db, fileInfo.Id.Value);
                    return null;
                }

                File result = db.Files.Where(x => x.Id == fileInfo.Id.Value).Single();
                result.Comments = fileInfo.Description;

                if (increaseReferenceCounter)
                {
                    ++result.ReferenceCounter;
                }

                result.IsActive = true;

                return result;
            }
            else
            {
                byte[] bytes = ReadFile(fileInfo.File, fileInfo.StoreOriginal, out bool isImage);
                string hash = ComputeFileContentHash(bytes);

                File result = db.Files.Where(x => x.ContentLength == bytes.Length && x.ContentHash == hash).SingleOrDefault();

                if (result != null)
                {
                    result.Comments = fileInfo.Description;

                    if (increaseReferenceCounter)
                    {
                        ++result.ReferenceCounter;
                    }
                    result.IsActive = true;
                }
                else
                {
                    result = new File
                    {
                        Name = fileInfo.File.FileName,
                        Content = bytes,
                        ContentHash = hash,
                        ContentLength = bytes.Length,
                        MimeType = isImage ? JPEG_MIME_TYPE : fileInfo.File.ContentType,
                        Comments = fileInfo.Description,
                        UploadedOn = fileInfo.UploadedOn,
                        ReferenceCounter = 1
                    };

                    db.Files.Add(result);
                }
                return result;
            }
        }

        public static File AddOrEditBase64Image(this IARADbContext db, string image, bool increaseReferenceCounter = false)
        {
            int idx = image.IndexOf("base64,") + 7;
            if (idx != -1)
            {
                image = image.Substring(idx);
            }

            byte[] bytes = Convert.FromBase64String(image);

            Image img = Image.Load(bytes);
            bytes = img.Resize(maxWidth: 1920, maxHeight: 1920);

            string hash = ComputeFileContentHash(bytes);

            File result = db.Files.Where(x => x.ContentLength == bytes.Length && x.ContentHash == hash).SingleOrDefault();

            if (result != null)
            {
                if (increaseReferenceCounter)
                {
                    ++result.ReferenceCounter;
                }
                result.IsActive = true;
            }
            else
            {
                result = new File
                {
                    Name = image.Substring(0, 10),
                    Content = bytes,
                    ContentHash = hash,
                    ContentLength = bytes.Length,
                    MimeType = JPEG_MIME_TYPE,
                    UploadedOn = DateTime.Now,
                    ReferenceCounter = 1
                };

                db.Files.Add(result);
            }
            return result;
        }

        public static int DeleteFile(this IARADbContext db, int fileId)
        {
            File file = db.Files.Where(x => x.Id == fileId).Single();
            if (file.ReferenceCounter > 0)
            {
                --file.ReferenceCounter;
            }
            if (file.ReferenceCounter == 0)
            {
                file.IsActive = false;
            }
            return file.ReferenceCounter;
        }

        public static byte[] Resize(this Image originalImage, float maxWidth, float maxHeight, int compressionRate = 75)
        {
            var maxResizeFactor = Math.Min(maxWidth / originalImage.Width, maxHeight / originalImage.Height);
            if (maxResizeFactor > 1)
            {
                //Image resolution is less than the required, compress the original size
                return originalImage.CompressImage(compressionRate);
            };

            var width = maxResizeFactor * originalImage.Width;
            var height = maxResizeFactor * originalImage.Height;

            originalImage.Mutate(i => i.Resize((int)width, (int)height));
            return originalImage.CompressImage(compressionRate); ;
        }

        private static byte[] CompressImage(this Image image, int compressionRate)
        {
            image.Metadata.ExifProfile = null;
            var memoryStream = new MemoryStream();
            image.SaveAsJpeg(memoryStream, new JpegEncoder { Quality = compressionRate });
            return memoryStream.ToArray();
        }

        private static string ComputeFileContentHash(byte[] bytes, HashAlgorithm hasher = null)
        {
            if (hasher == null)
            {
                hasher = SHA256.Create();
            }

            byte[] hashBytes = hasher.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        private static byte[] ReadFile(IFormFile file, bool storeOriginal, out bool isImage)
        {
            isImage = false;
            if (file != null)
            {
                using MemoryStream stream = new MemoryStream();
                file.CopyTo(stream);
                if (!storeOriginal && file.Length > 0 && file.ContentType.Contains("image"))
                {
                    Image image;
                    try
                    {
                        stream.Position = 0;
                        image = Image.Load(stream);
                        isImage = true;
                    }
                    catch
                    {
                        //Not a valid image
                        return stream.ToArray();
                    }
                    return image.Resize(maxWidth: 1920, maxHeight: 1920);
                }
                return stream.ToArray();
            }
            return null;
        }
    }
}

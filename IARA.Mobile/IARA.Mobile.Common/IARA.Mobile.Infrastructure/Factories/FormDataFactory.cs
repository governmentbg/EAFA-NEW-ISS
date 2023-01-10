using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Infrastructure.Factories
{
    public class FormDataFactory : IFormDataFactory
    {
        public HttpContent BuildFormData(object obj)
        {
            MultipartFormDataContent formData = new MultipartFormDataContent();

            if (obj != null)
            {
                CheckAndAddObjectToFormData(obj, obj?.GetType(), formData, null);
            }

            return formData;
        }

        private void CheckAndAddObjectToFormData(object val, Type propertyType, MultipartFormDataContent formData, string prefix)
        {
            if (propertyType.IsValueType || val is string)
            {
                AddValueTypeToFormData(val, formData, prefix);
            }
            else if (val is IEnumerable enumerable)
            {
                AddListToFormData(enumerable, formData, prefix);
            }
            else if (val is FileModel file)
            {
                AddFileToFormData(file, formData, prefix);
            }
            else
            {
                AddObjectToFormData(val, formData, prefix);
            }
        }

        private void AddValueTypeToFormData(object val, MultipartFormDataContent formData, string prefix)
        {
            string stringContent;

            if (val is DateTime date)
            {
                stringContent = date.ToUniversalTime().ToApiDateTime();
            }
            else if (val is decimal dec)
            {
                stringContent = dec.ToString().Replace(",", ".");
            }
            else if (val is double dob)
            {
                stringContent = dob.ToString().Replace(",", ".");
            }
            else
            {
                stringContent = val.ToString();
            }
            Debug.WriteLine($"{prefix} = {stringContent}");
            formData.Add(new StringContent(stringContent), prefix);
        }

        private void AddListToFormData(IEnumerable enumerable, MultipartFormDataContent formData, string prefix)
        {
            int index = 0;
            foreach (object item in enumerable)
            {
                CheckAndAddObjectToFormData(item, item.GetType(), formData, (prefix ?? string.Empty) + $"[{index}]");
                index++;
            }
        }

        private void AddFileToFormData(FileModel file, MultipartFormDataContent formData, string prefix)
        {
            if (file != null)
            {
                if (file.Id != null)
                {
                    formData.Add(new StringContent(file.Id.ToString()), CheckPrefix(prefix, nameof(FileModel.Id)));
                }

                if (file.Deleted)
                {
                    formData.Add(new StringContent(file.Deleted.ToString()), CheckPrefix(prefix, nameof(FileModel.Deleted)));
                }

                if (file.StoreOriginal)
                {
                    formData.Add(new StringContent(file.StoreOriginal.ToString()), CheckPrefix(prefix, nameof(FileModel.StoreOriginal)));
                }

                if (!string.IsNullOrEmpty(file.Description))
                {
                    formData.Add(new StringContent(file.Description), CheckPrefix(prefix, nameof(FileModel.Description)));
                }

                if (!string.IsNullOrEmpty(file.Name))
                {
                    formData.Add(new StringContent(file.Name), CheckPrefix(prefix, nameof(FileModel.Name)));
                }

                formData.Add(new StringContent(file.FileTypeId.ToString()), CheckPrefix(prefix, nameof(FileModel.FileTypeId)));
                formData.Add(new StringContent(file.Size.ToString()), CheckPrefix(prefix, nameof(FileModel.Size)));

                if (string.IsNullOrEmpty(file.FullPath))
                {
                    return;
                }

                StreamContent streamContent = new StreamContent(File.OpenRead(file.FullPath))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue(file.ContentType)
                    }
                };

                formData.Add(streamContent, CheckPrefix(prefix, nameof(File)), Path.GetFileName(file.FullPath));
                formData.Add(new StringContent(file.UploadedOn.ToUniversalTime().ToApiDateTime()), prefix + "." + nameof(FileModel.UploadedOn));
            }
        }

        private void AddObjectToFormData(object obj, MultipartFormDataContent formData, string prefix)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo prop = properties[i];
                object val = prop.GetValue(obj, null);

                if (val == null)
                {
                    continue;
                }

                CheckAndAddObjectToFormData(val,
                    prop.PropertyType,
                    formData,
                    CheckPrefix(prefix, prop.Name)
                );
            }
        }

        private string CheckPrefix(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix)
                ? name
                : prefix + '.' + name;
        }
    }
}

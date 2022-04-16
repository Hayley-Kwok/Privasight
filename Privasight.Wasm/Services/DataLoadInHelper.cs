﻿using System.IO.Compression;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Privasight.Model.Shared.Converters;
using Privasight.Model.Shared.DataStructures.Interfaces;

namespace Privasight.Wasm.Services
{
    public static class DataLoadInHelper
    {
        private const long MaxFileSize = 560000000; // ~512 MB

        public static async Task<(Dictionary<string, IFileWrapper> newData, DateTimeOffset generationDate)> TransformJsonToObj(InputFileChangeEventArgs e, Dictionary<string, Type> availableFileWrappers)
        {
            var newData = new Dictionary<string, IFileWrapper>();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new FbConverter());
            options.Converters.Add(new StringWrapperDbObjConverter());

            //TODO: figure out a better way to do this
            await using var stream = new MemoryStream();
            await e.File.OpenReadStream(MaxFileSize).CopyToAsync(stream);

            var generationDate = e.File.LastModified;

            using var archive = new ZipArchive(stream);
            foreach (var entry in archive.Entries)
            {
                if (!availableFileWrappers.TryGetValue(entry.FullName, out var wrapperType)) continue;
                var unzippedEntryStream = entry.Open();

                if (await JsonSerializer.DeserializeAsync(unzippedEntryStream, wrapperType, options) is IFileWrapper wrapperObj)
                {
                    newData.TryAdd(wrapperType.Name, wrapperObj);
                }
            }

            return (newData, generationDate);
		}

        public static void AddGenerationDate(Dictionary<string, IFileWrapper> newData, DateTimeOffset generationDate)
        {
            foreach (var wrapper in newData.Values)
            {
                switch (wrapper)
                {
                    case ISingleItemListFile:
                    {
                        //todo improve this
                        dynamic singleItemList = wrapper;
                        foreach (var item in singleItemList.Items)
                        {
                            item.GeneratedOn = generationDate;
                        }

                        break;
                    }
                }
            }
        }
    }
}
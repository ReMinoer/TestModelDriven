using System;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TestModelDriven.Framework.Application;

static public class FileTypeExtension
{
    static public bool SupportExtension(this IFileType fileType, string fileExtension)
    {
        return fileType.Extensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }

    static public CommonFileDialogFilter GetFileDialogFilter(this IFileType fileType)
    {
        return new CommonFileDialogFilter(fileType.DisplayName, string.Join(',', fileType.Extensions.Select(x => $".{x}")));
    }
}
﻿using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Abstractions.Models.FileImage;

[ExcludeFromCodeCoverage]
public class FileImageModel
{
    public string FieldName { get; set; }
    public string Value { get; set; }
}

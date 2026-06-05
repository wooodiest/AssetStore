namespace AssetStore.Dto.Files;

public class FileDownloadDto
{
    public Stream Stream { get; init; } = Stream.Null;

    public string ContentType { get; init; } = "application/octet-stream";

    public string FileName { get; init; } = string.Empty;
}


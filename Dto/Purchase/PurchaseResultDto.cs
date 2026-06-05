namespace AssetStore.Dto.Purchase;

public class PurchaseResultDto
{
    public bool CanDownload { get; init; }

    public string Message { get; init; } = string.Empty;

    public bool AlreadyOwned { get; init; }
}


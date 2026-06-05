using AssetStore.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Extensions;

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult<T>(this Controller controller, ServiceResult<T> result, Func<T, IActionResult> onSuccess)
    {
        if (result.Success && result.Data is not null)
        {
            return onSuccess(result.Data);
        }

        return ToErrorResult(controller, result.ErrorCode, result.ErrorMessage);
    }

    public static IActionResult ToActionResult(this Controller controller, ServiceResult result, Func<IActionResult> onSuccess)
    {
        if (result.Success)
        {
            return onSuccess();
        }

        return ToErrorResult(controller, result.ErrorCode, result.ErrorMessage);
    }

    private static IActionResult ToErrorResult(Controller controller, ServiceErrorCode errorCode, string? errorMessage)
    {
        controller.TempData["Error"] = errorMessage;

        return errorCode switch
        {
            ServiceErrorCode.NotFound => controller.NotFound(),
            ServiceErrorCode.Forbidden => controller.Forbid(),
            ServiceErrorCode.Conflict => controller.Conflict(),
            _ => controller.BadRequest()
        };
    }
}

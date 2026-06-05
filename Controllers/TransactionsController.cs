using AssetStore.Mappings;
using AssetStore.Services.Interfaces;
using AssetStore.ViewModels.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetStore.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ICurrentUserService _currentUserService;

    public TransactionsController(
        ITransactionService transactionService,
        ICurrentUserService currentUserService)
    {
        _transactionService = transactionService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = await _currentUserService.GetUserIdAsync();
        if (userId is null)
        {
            return Challenge();
        }

        var history = await _transactionService.GetUserHistoryAsync(userId, cancellationToken);
        return View(TransactionMappings.ToViewModel(history));
    }
}

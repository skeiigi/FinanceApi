using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceApi.Data;
using FinanceApi.Models;
using System.Security.Claims;

namespace FinanceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class FinanceController : ControllerBase
{
    private readonly ApiDbContext _context;

    public FinanceController(ApiDbContext context) => _context = context;

    // ============== TRANSACTIONS ==============

    [HttpGet("transactions")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    [HttpPost("transactions")]
    public async Task<ActionResult<Transaction>> AddTransaction(Transaction transaction)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        transaction.UserId = userId;
        transaction.Date = DateTime.UtcNow;

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(transaction);
    }

    // ============== GOALS ==============

    [HttpGet("goals")]
    public async Task<ActionResult<IEnumerable<Goal>>> GetGoals()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _context.Goals
            .Where(g => g.UserId == userId)
            .ToListAsync();
    }

    [HttpPost("goals")]
    public async Task<ActionResult<Goal>> AddGoal(Goal goal)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        goal.UserId = userId;
        goal.CreatedAt = DateTime.UtcNow;

        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();

        return Ok(goal);
    }

    [HttpPut("goals/{id}")]
    public async Task<IActionResult> UpdateGoal(int id, Goal goal)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingGoal = await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (existingGoal == null)
            return NotFound();

        existingGoal.Title = goal.Title;
        existingGoal.TargetAmount = goal.TargetAmount;
        existingGoal.CurrentAmount = goal.CurrentAmount;

        await _context.SaveChangesAsync();
        return Ok(existingGoal);
    }

    [HttpDelete("goals/{id}")]
    public async Task<IActionResult> DeleteGoal(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var goal = await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal == null)
            return NotFound();

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveInterceptor : SaveChangesInterceptor
{
    private readonly ILoggedInUserService _loggedInUserService;

    public AuditableEntitySaveInterceptor(ILoggedInUserService loggedInUserService)
    {
        _loggedInUserService = loggedInUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null) UpdateChangedEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context != null) UpdateChangedEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateChangedEntities(DbContext context)
    {
        foreach (var entityEntry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entityEntry.State is EntityState.Added)
            {
                entityEntry.Entity.CreatedAt = DateTime.Now;
                entityEntry.Entity.CreatedById = _loggedInUserService.UserId ?? string.Empty;
            }

            if (entityEntry.State is EntityState.Added or EntityState.Modified)
            {
                entityEntry.Entity.LastModifiedAt = DateTime.Now;
                entityEntry.Entity.LastModifiedById = _loggedInUserService.UserId ?? string.Empty;
            }

            foreach (var entityEntryReference in entityEntry.References)
                if (entityEntryReference.CurrentValue is AuditableEntity auditableEntity)
                {
                    auditableEntity.LastModifiedAt = DateTime.Now;
                    auditableEntity.LastModifiedById = _loggedInUserService.UserId ?? string.Empty;
                }
        }
    }
}
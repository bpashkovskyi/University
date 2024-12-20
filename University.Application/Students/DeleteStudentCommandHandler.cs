﻿using MediatR;

using Microsoft.Extensions.Caching.Distributed;

using University.Models;
using University.Persistence;

namespace University.Application.Students;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand>
{
    private readonly UniversityContext context;
    private readonly IDistributedCache cache;

    public DeleteStudentCommandHandler(UniversityContext context, IDistributedCache cache)
    {
        this.context = context;
        this.cache = cache;
    }

    public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FindAsync(request.Id, cancellationToken);
        if (student != null)
        {
            context.Students.Remove(student);
        }

        await context.SaveChangesAsync(cancellationToken);

        await this.InvalidateCache(student);
    }

    private async Task InvalidateCache(Student student)
    {
        var key = $"student-{student.Id}";
        await this.cache.RemoveAsync(key);
    }
}
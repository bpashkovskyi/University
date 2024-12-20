﻿using MediatR;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Caching.Distributed;

using University.Models;
using University.Persistence;

namespace University.Application.Students;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand>
{
    private readonly UniversityContext context;
    private readonly IDistributedCache cache;

    public UpdateStudentCommandHandler(UniversityContext context, IDistributedCache cache)
    {
        this.context = context;
        this.cache = cache;
    }

    public async Task Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = request.ToStudent();

        context.Update(student);

        await context.SaveChangesAsync(cancellationToken);

        await this.InvalidateCache(student);
    }

    private async Task InvalidateCache(Student student)
    {
        var key = $"student-{student.Id}";
        await this.cache.RemoveAsync(key);
    }
}
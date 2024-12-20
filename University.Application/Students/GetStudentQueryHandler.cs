﻿using System.Text.Json;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using University.Models;
using University.Persistence;

namespace University.Application.Students;

public class GetStudentQueryHandler : IRequestHandler<GetStudentQuery, Student>
{
    private readonly UniversityContext context;
    private readonly IDistributedCache cache;

    private TimeSpan threeMonths = new(0, 3, 0, 0, 0, 0);

    public GetStudentQueryHandler(UniversityContext context, IDistributedCache cache)
    {
        this.context = context;
        this.cache = cache;
    }

    public async Task<Student> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await GetStudentFromCache(request);
        if (student != null)
        {
            return student;
        }

        student = await GetStudentFromDatabase(request, cancellationToken);
        await SetStudentToCache(student);

        return student;
    }

    private async Task SetStudentToCache(Student student)
    {
        var key = $"student-{student.Id}";

        var studentAsSerializedJson = JsonSerializer.Serialize(student);

        await cache.SetStringAsync(key, studentAsSerializedJson, options: new DistributedCacheEntryOptions { SlidingExpiration = threeMonths });
    }

    private async Task<Student> GetStudentFromCache(GetStudentQuery request)
    {
        var key = $"student-{request.Id}";

        var studentAsSerializedJson = await cache.GetStringAsync(key);
        if (studentAsSerializedJson == null)
        {
            return null;
        }

        var student = JsonSerializer.Deserialize<Student>(studentAsSerializedJson);
        return student;
    }

    private async Task<Student> GetStudentFromDatabase(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        return student;
    }
}
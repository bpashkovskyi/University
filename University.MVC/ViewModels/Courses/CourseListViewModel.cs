﻿using System.ComponentModel.DataAnnotations;

using University.Models;

namespace University.MVC.ViewModels.Courses;

public class CourseDetailsViewModel
{
    public int Id { get; set; }

    public string Topic { get; set; }

    [Display(Name = "Number of hours")]
    public int NumberOfHours { get; set; }

    [Display(Name = "Start date")]
    public DateTime StartDate { get; set; }

    [Display(Name = "End date")]
    public DateTime EndDate { get; set; }

    public static CourseDetailsViewModel FromCourse(Course course)
    {
        var courseDetailsViewModel = new CourseDetailsViewModel
        {
            Id = course.Id,
            Topic = course.Topic,
            NumberOfHours = course.NumberOfHours,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
        };

        return courseDetailsViewModel;
    }
}
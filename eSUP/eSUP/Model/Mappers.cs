namespace eSUP.Model;
using eSUP.DTO;
using Microsoft.AspNetCore.Identity;

public static class Mappers
{
    public static UserInformationDto Map(this ApplicationUser user)
    {
        return new UserInformationDto
        {
            UserId = new Guid(user.Id),
            Email = user.Email ?? "-",
            FirstName = user.FirstName ?? "-",
            LastName = user.LastName ?? "-",
            Group = user.Group,
            IsAssigned = false
        };
    }

    public static PlannerDto Map(this Planner planner)
    {
        return new PlannerDto
        {
            Id = planner.Id,
            Title = planner.Title ?? "-",
            Exercises = planner.Exercises.OrderBy(e => e.Sequence).Select(e => e.Map()).ToList(),
        };
    }

    public static ExerciseDto Map(this Exercise exercise)
    {
        return new ExerciseDto
        {
            Id = exercise.Id,
            Sequence = exercise.Sequence,
            Title = exercise.Title ?? "-",
            Questions = exercise.Questions.OrderBy(q => q.Sequence).Select(p => p.Map()).ToList(),
        };
    }

    public static QuestionDto Map(this Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            Sequence = question.Sequence,
            Title = question.Title ?? "-",
            Parts = question.Parts.OrderBy(p => p.Sequence).Select(p => p.Map()).ToList(),
        };
    }

    public static QuestionDto Map(this Question question, ApplicationUser? user)
    {
        var parts = question.Parts.OrderBy(p => p.Sequence).Select(p => p.Map()).ToList();
        foreach (var part in parts)
        {
            if (user != null)
            {
                part.IsLevelBelow = user.Parts.Any(p => p.Id == part.Id && p.IsLevelBelow);
                part.IsLevelAt = user.Parts.Any(p => p.Id == part.Id && p.IsLevelAt);
                part.IsLevelAbove = user.Parts.Any(p => p.Id == part.Id && p.IsLevelAbove);
                part.IsCompleted = part.IsLevelBelow || part.IsLevelAt || part.IsLevelAbove;
            }
        }
        return new QuestionDto
        {
            Id = question.Id,
            Sequence = question.Sequence,
            Title = question.Title ?? "-",
            Parts = parts
        };
    }

    public static PartDto Map(this Part part)
    {
        return new PartDto
        {
            Id = part.Id,
            Sequence = part.Sequence,
            Title = part.Title ?? "-",
            IsLevelBelow = part.IsLevelBelow,
            IsLevelAt = part.IsLevelAt,
            IsLevelAbove = part.IsLevelAbove,
        };
    }
}

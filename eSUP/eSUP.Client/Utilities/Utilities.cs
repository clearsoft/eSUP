using eSUP.DTO;

namespace eSUP.Client;

public static class Utilities
{
    public static PlannerDto GenerateNewSUP(PlannerSpecificationDto specification)
    {
        var planner = new PlannerDto() { Title = specification.Title };
        for (int iExercise = 1; iExercise <= specification.NumberOfExercises; iExercise++)
        {
            var ex = new ExerciseDto
            {
                Title = MakeTitle(iExercise, specification.ExerciseTitleTemplate),
                Sequence = iExercise
            };
            for (int iQuestion = 1; iQuestion <= specification.MaximumQuestionsPerExercise; iQuestion++)
            {
                var q = new QuestionDto
                {
                    Title = MakeTitle(iQuestion, specification.QuestionTitleTemplate),
                    Sequence = iQuestion,
                    Parts = new List<PartDto>()
                };
                for (int iPart = 1; iPart <= specification.MaximumPartsPerQuestion; iPart++)
                {
                    var p = new PartDto
                    {
                        Title = MakeTitle(iPart, specification.PartTitleTemplate),
                        Sequence = iPart
                    };
                    q.Parts.Add(p);
                }
                ex.Questions.Add(q);
            }
            planner.Exercises.Add(ex);
        }
        return planner;
    }

    private static string MakeTitle(int n, string template)
    {
        return template.Replace("#", n.ToString()).Replace("$", n.ToLetter().ToUpper()).Replace("%", n.ToLetter());
    }

    private static string ToLetter(this int n)
    {
        if (n < 1 || n > 26)
            return "*";
        return ((char)('a' + n - 1)).ToString();
    }
}

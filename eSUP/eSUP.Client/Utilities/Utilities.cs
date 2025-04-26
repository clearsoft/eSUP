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
                    Sequence = iQuestion
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

    public static PlannerDto GenerateSUPFromTextDefinition(string fileName, string definition)
    {
        try
        {
            var planner = new PlannerDto() { Title = fileName };
            /*
             * Format:
             * EX3A
             *     Q1 a b c d e f
             *     Q2 a b c-i c-ii c-ii
             *     Q3
             * EX3B
             *     Q1 a b c d
             */
            // Split by line
            string[] lines = definition.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            // First line must start with a non-space character - this is the Ex level
            if (lines.Length == 0)
                throw new Exception("File is empty");
            if (char.IsWhiteSpace(lines[0], 0))
                throw new Exception($"Line 1 '{lines[0]}' must start with a non-space character");

            int iExerciseSequence = 1;
            int iQuestionSequence = 1;
            int iPartSequence = 1;
            ExerciseDto? exerciseDto = null;
            foreach (var item in lines)
            {
                if (!char.IsWhiteSpace(item, 0)) // Exercise 
                {
                    exerciseDto = new()
                    {
                        Title = item.Trim(),
                        Sequence = iExerciseSequence++
                    };
                    planner.Exercises.Add(exerciseDto);
                }
                else
                {
                    // Split the parts by space
                    string[] parts = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    bool firstPart = true;
                    QuestionDto questionDto = new();
                    foreach (var part in parts)
                    {
                        if (firstPart) // Question
                        {
                            questionDto.Title = part.Trim();
                            questionDto.Sequence = iQuestionSequence++;
                            firstPart = false;
                        }
                        else // Part
                        {
                            PartDto partDto = new();
                            partDto.Title = part.Trim();
                            partDto.Sequence = iPartSequence++;
                            questionDto.Parts.Add(partDto);
                        }
                    }
                    exerciseDto!.Questions.Add(questionDto);
                }
            }
            return planner;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in file '{fileName}': {ex.Message}");
        }
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

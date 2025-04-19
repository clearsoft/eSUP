namespace eSUP.DTO;

public class PlannerDto
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = "";
    public List<ExerciseDto> Exercises { get; set; } = [];
}

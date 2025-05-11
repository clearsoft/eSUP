namespace eSUP.DTO;

internal class DetailRequestDto
{
    public Guid ExerciseId { get; set; }
    public Guid StudentId { get; set; }
}

internal class DetailResponseDto
{
    public List<QuestionDto> Questions { get; set; } = [];
}

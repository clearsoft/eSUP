namespace eSUP.DTO
{
    public class ExerciseDto : CommonDto
    {
        public List<QuestionDto> Questions { get; set; } = [];
        public string LevelSelected { get; set; } = "0";
        public int PartCount { get; set; }
    }
}

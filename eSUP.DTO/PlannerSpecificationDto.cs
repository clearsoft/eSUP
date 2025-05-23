﻿namespace eSUP.DTO;

    public class PlannerSpecificationDto
    {
        public string Title = "New Planner: " + DateTime.Now.ToString("dd MMM yyyy hh:mm");
        public int NumberOfExercises { get; set; } = 12;
        public int MaximumQuestionsPerExercise { get; set; } = 10;
        public int MaximumPartsPerQuestion { get; set; } = 10;

        public string ExerciseTitleTemplate { get; set; } = "Ex3$";
        public string QuestionTitleTemplate { get; set; } = "Q#";
        public string PartTitleTemplate { get; set; } = "%";
    }

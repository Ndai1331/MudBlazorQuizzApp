using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAppBlazor.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add unique constraint to prevent duplicate scores within 5 minutes
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_Score_UserId_CorrectAnswers_Date 
                ON ""Score"" (""UserId"", ""CorrectAnswers"", ""Date"") 
                WHERE ""Date"" > NOW() - INTERVAL '5 minutes'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the unique index
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_Score_UserId_CorrectAnswers_Date");
        }
    }
}

using CledAcademy.Core.Domain;
using CledAcademy.DataAccess.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CledAcademy.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<TestAnswer> TestAnswers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentLesson> StudentLessons { get; set; }
        public DbSet<StudentTestAnswer> StudentTestAnswers { get; set; }
        public DbSet<StudentOpenTestAnswer> StudentOpenTestAnswers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<News> Newses { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Section> Topics { get; set; }
        public DbSet<HappyStudent> HappyStudents { get; set; }
        public DbSet<DataDictionary> DataDictionaries { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<AccountTopUp> AccountTopUps { get; set; }
        public DbSet<StudentOrder> StudentOrders { get; set; }
        public DbSet<ShoppingCard> ShoppingCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new CourseMapping(modelBuilder.Entity<Course>());
            new LessonMapping(modelBuilder.Entity<Lesson>());
            new TestMapping(modelBuilder.Entity<Test>());
            new AnswerMapping(modelBuilder.Entity<Answer>());
            new TestAnswerMapping(modelBuilder.Entity<TestAnswer>());
            new StudentMapping(modelBuilder.Entity<Student>());
            new StudentLessonMapping(modelBuilder.Entity<StudentLesson>());
            new StudentTestAnswerMapping(modelBuilder.Entity<StudentTestAnswer>());
            new StudentOpenTestAnswerMapping(modelBuilder.Entity<StudentOpenTestAnswer>());
            new PersonMapping(modelBuilder.Entity<Person>());
            new AdminMapping(modelBuilder.Entity<Admin>());
            new TeacherMapping(modelBuilder.Entity<Teacher>());
            new NewsMapping(modelBuilder.Entity<News>());
            new ContactMessageMapping(modelBuilder.Entity<ContactMessage>());
            new SectionMapping(modelBuilder.Entity<Section>());
            new HappyStudentMapping(modelBuilder.Entity<HappyStudent>());
            new DataDictionaryMapping(modelBuilder.Entity<DataDictionary>());
            new FaqMapping(modelBuilder.Entity<Faq>());
            new AccountTopUpMapping(modelBuilder.Entity<AccountTopUp>());
            new StudentOrderMapping(modelBuilder.Entity<StudentOrder>());
            new ShoppingCardMapping(modelBuilder.Entity<ShoppingCard>());

            base.OnModelCreating(modelBuilder);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PracticeDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LinqToSqlDataClassesDataContext dataContext;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["PracticeDB.Properties.Settings.PracticeDBConnectionString"].ConnectionString;
            dataContext = new LinqToSqlDataClassesDataContext(connectionString);

            //InsertUniversities();
            //InsertStudents();
            //InsertLectures();
            //InsertStudentLectureAssociations();
            //GetUniversityOfTony();
            //GetTonysLectures();
            //GetAllStudentsFromYale();
            //GetAllUniversityWithTransgender();
            //GetColoradoLectures();
            //UpdateTony();
            DeleteJames();
        }

        public void InsertUniversities()
        {
            dataContext.ExecuteCommand("Delete from University");

            University yale = new University();
            yale.Name = "Yale";
            dataContext.Universities.InsertOnSubmit(yale);

            University colorado = new University();
            colorado.Name = "Colorado";
            dataContext.Universities.InsertOnSubmit(colorado);

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Universities;
        }

        public void InsertStudents()
        {
            University yale = dataContext.Universities.First(un => un.Name.Equals("Yale"));
            University colorado = dataContext.Universities.First(un => un.Name.Equals("Colorado"));

            List<Student> students = new List<Student>();

            students.Add(new Student { Name = "Carla", Gender = "Female", UniversityId = yale.Id });
            students.Add(new Student { Name = "Tony", Gender = "Male", University = yale });
            students.Add(new Student { Name = "Leyle", Gender = "Female", University = colorado });
            students.Add(new Student { Name = "James", Gender = "Trans-gender", University = colorado });

            dataContext.Students.InsertAllOnSubmit(students);
            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        }

        public void InsertLectures()
        {
            

            dataContext.Lectures.InsertOnSubmit(new Lecture { Name = "Geology" });
            dataContext.Lectures.InsertOnSubmit(new Lecture { Name = "Math" });
            
            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Lectures;
        }

        public void InsertStudentLectureAssociations()
        {
            Student Carla = dataContext.Students.First(st => st.Name.Equals("Carla"));
            Student Tony = dataContext.Students.First(st => st.Name.Equals("Tony"));
            Student Leyle = dataContext.Students.First(st => st.Name.Equals("Leyle"));
            Student James = dataContext.Students.First(st => st.Name.Equals("James"));

            Lecture Geology = dataContext.Lectures.First(lc => lc.Name.Equals("Geology"));
            Lecture Math = dataContext.Lectures.First(lc => lc.Name.Equals("Math"));

            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Carla, Lecture = Geology });
            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Tony, Lecture = Geology });

            StudentLecture slTony = new StudentLecture();
            slTony.Student = Tony;

            slTony.LectureId = Math.Id;
            dataContext.StudentLectures.InsertOnSubmit(slTony);

            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Leyle, Lecture = Math });
            dataContext.SubmitChanges();
            MainDataGrid.ItemsSource = dataContext.StudentLectures;

        }

        public void GetUniversityOfTony()
        {
            Student Tony = dataContext.Students.First(st => st.Name.Equals("Tony"));
            University TonysUniversity = Tony.University;

            List<University> universities = new List<University>();
            universities.Add(TonysUniversity);

            MainDataGrid.ItemsSource = universities;
        }
         public void GetTonysLectures()
        {
            Student Tony = dataContext.Students.First(st => st.Name.Equals("Tony"));

            var tonysLectures = from sl in Tony.StudentLectures select sl.Lecture;
            MainDataGrid.ItemsSource = tonysLectures;            
        }
        public void GetAllStudentsFromYale()
        {


            var yaleStudents = from student in dataContext.Students
                               where student.University.Name == "Yale"
                               select student;

            MainDataGrid.ItemsSource = yaleStudents;

        }
        public void GetAllUniversityWithTransgender()
        {
            var transUniversities = from student in dataContext.Students
                                    join university in dataContext.Universities
                                    on student.University equals university
                                    where student.Gender == "Trans Gender"
                                    select university;

            MainDataGrid.ItemsSource = transUniversities;
        }

        public void GetColoradoLectures()
        {
            var coloradoLectures = from sl in dataContext.StudentLectures
                                   join student in dataContext.Students
                                   on sl.StudentId equals student.Id
                                   where student.University.Name == "Colorado"
                                   select sl.Lecture;
            MainDataGrid.ItemsSource = coloradoLectures;
        }

        public void UpdateTony()
        {
            Student Tony = dataContext.Students.FirstOrDefault(st => st.Name == "Tony");

            Tony.Name = "Tommy";

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        }

        public void DeleteJames()
        {
            Student James = dataContext.Students.FirstOrDefault(st => st.Name == "James");

            dataContext.Students.DeleteOnSubmit(James);
            dataContext.SubmitChanges();
            MainDataGrid.ItemsSource = dataContext.Students;
        }
    }
}

namespace StudentGradeManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStudent
{
    public string Name { get; set; }
    public int ID { get; set; }
    public Dictionary<string, Grade> Grades { get; }
    public void AddGrade(string course, string credit, string score);
    public void AddGrades(List<(string course, int credit, int score)> grades);
    public void RemoveGrade(string course);
    public void RemoveGrades(List<string> courses);
    public int GetTotalCredit();
    public double GetTotalGradePoint();
    public double GetGPA();
    public string ToString();
}

public class Student : IStudent
{
    // 请仅在此处实现接口，不要在此处以外的地方进行任何修改
    // 请尽可能周全地考虑鲁棒性
    // 这里基本上要求写一些结构的创建、运用等逻辑
    // 与情景关系不大，所以单纯写函数即可
    // 然而从应用分析的角度也可做一些分析

    public string Name { get; set; }
    public int ID { get; set; }
    public Dictionary<string, Grade> Grades { get; private set; }

    public Student(string name, string id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        if (!int.TryParse(id, out int idValue) || idValue <= 0)
            throw new ArgumentException("ID must be a positive integer.", nameof(id));

        Name = name;
        ID = idValue;
        Grades = new Dictionary<string, Grade>();
    }

    public void AddGrade(string course, string credit, string score)
    {
        // grades类中有限制credit、score都是int
        if (string.IsNullOrWhiteSpace(course))
            throw new ArgumentException("Course name cannot be null or whitespace.", nameof(course));

        if (!int.TryParse(credit, out int creditValue) || creditValue <= 0)
            throw new ArgumentException("Credit must be a positive integer.", nameof(credit));

        if (!int.TryParse(score, out int scoreValue) || scoreValue < 0 || scoreValue > 100)
            throw new ArgumentException("Score must be an integer between 0 and 100.", nameof(score));

        // 如果已存在，覆盖；不存在，新增
        var grade = new Grade(creditValue, scoreValue);
        Grades[course] = grade;
    }

    public void AddGrades(List<(string course, int credit, int score)> grades)
    {
        if (grades == null)
            throw new ArgumentNullException(nameof(grades));

        foreach (var (course, credit, score) in grades)
            AddGrade(course, credit.ToString(), score.ToString());
    }

    public void RemoveGrade(string course)
    {
        if (string.IsNullOrWhiteSpace(course))
            throw new ArgumentException("Course name cannot be null or whitespace.", nameof(course));

        // 尝试删除
        if (!Grades.Remove(course))
            throw new KeyNotFoundException($"Course '{course}' not found.");
    }

    public void RemoveGrades(List<string> courses)
    {
        if (courses == null)
            throw new ArgumentNullException(nameof(courses));

        foreach (var course in courses)
            RemoveGrade(course);
    }

    public int GetTotalCredit()
    {
        return Grades.Values.Sum(grade => grade.Credit);
    }

    public double GetTotalGradePoint()
    {   // 总绩点=学分×绩点的总和
        return Grades.Values.Sum(grade => grade.Credit * grade.GradePoint);
    }

    public double GetGPA()
    {   // 平均绩点
        int totalCredit = GetTotalCredit();
        if (totalCredit == 0)
            return 0.0;
        return GetTotalGradePoint() / totalCredit;
    }


    public override string ToString()
    {   // 重写ToString方法，打印学生信息
        StringBuilder sb = new();
        sb.AppendLine($"Student: {Name}, ID: {ID}");
        if (Grades.Count == 0)
            sb.AppendLine("No grades recorded.");
        else
        {
            sb.AppendLine("Courses:");
            foreach (var (course, grade) in Grades.OrderBy(g => g.Key))
                sb.AppendLine($"  {course}: Credit={grade.Credit}, Score={grade.Score}, Grade Point={grade.GradePoint:F2}");
        }
        sb.AppendLine($"Total Credit: {GetTotalCredit()}");
        sb.AppendLine($"GPA: {GetGPA():F2}");

        return sb.ToString();
    }
}
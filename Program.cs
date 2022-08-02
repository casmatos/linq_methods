using Linq_Methods.Common;
using Linq_Methods.Exemples;
using Linq_Methods.Expressions;
using Linq_Methods.Model;
using System.Linq.Expressions;
using System.Reflection;


#region == Test ==

Expression<Func<Student, bool>> isTeenAgerExpr = s => s.Age > 12 && s.Age < 20;

List<Student> lstStudent = new List<Student>
{
    new Student
    {
        StudentID = 1,
        Age = 12,
        StudentName = "John",
        CountryId = 1,
        Country = new Country
        {
            Id = 1,
            Name = "Portugal"
        }
    },
    new Student
    {
        StudentID = 2,
        Age = 20,
        StudentName = "Mike",
        CountryId = 2,
        Country = new Country
        {
            Id = 2,
            Name = "Spain"
        }
    },
};

Examples exemplos = new();
Type newType;

var resDirect = exemplos.Expression_Direct(lstStudent);

var resDirectOneFieldCustom = exemplos.Expression_DirectOneFieldCustom(lstStudent);

var resHolder = exemplos.Expression_SelectFieldDirect(lstStudent);

var resCustomType = exemplos.Expression_SelectFieldCustomFields<Student>(out newType, "StudentId", "Age", "StudentName");

var ExpressionCustomTypeAllFields = exemplos.Expression_SelectFieldCustomFields<Student>(out newType);

var resCustomTypeAllFields = lstStudent.Select(ExpressionCustomTypeAllFields).ToList();

var ExpressionCustomTypeCountry = exemplos.Expression_SelectFieldCustomFields<Student>(out newType, "Country");

var resCustomTypeCountry = lstStudent.Select(ExpressionCustomTypeCountry).ToList();

#endregion

var json = System.Text.Json.JsonSerializer.Serialize(resCustomTypeCountry);


var lst = lstStudent.Select(s => new { s.Country }).ToList();



public class Holder
{
    public int Item1 { get; set; }
    public string Item2 { get; set; }
    public int Item3 { get; set; }
}


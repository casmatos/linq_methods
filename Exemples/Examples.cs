using Linq_Methods.Expressions;
using Linq_Methods.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq_Methods.Exemples
{
    public class Examples
    {
        /// <summary>
        /// Build a Expression amd use Direct Expression, then select Age and StudentId
        /// </summary>
        /// <param name="lstStudent"></param>
        /// <returns></returns>
        public List<object> Expression_Direct(List<Student> lstStudent)
        {
            Expression<Func<Student, object>> expressionTreeStudent_Direct = s => new { s.Age, s.StudentID };

            var resTree_Direct = lstStudent.Select(expressionTreeStudent_Direct.Compile()).ToList();

            return resTree_Direct;
        }

        /// <summary>
        /// Build a Expression and create dynamic parameters (body) for selection, return a anonymous type
        /// </summary>
        /// <param name="lstStudent"></param>
        /// <returns></returns>
        public List<int> Expression_DirectOneFieldCustom(List<Student> lstStudent)
        {
            ParameterExpression pe = Expression.Parameter(typeof(Student), "s"); // s =>

            MemberExpression age = Expression.Property(pe, "Age"); // => s.Age
            var expressionTreeAge = Expression.Lambda<Func<Student, int>>(age, new[] { pe });

            MemberExpression Student = Expression.Property(pe, "StudentID"); // => s.StudentID
            var expressionTreeStudent = Expression.Lambda<Func<Student, int>>(Student, new[] { pe });

            var cc = expressionTreeAge.Compile();

            var resTree = lstStudent.Select(cc).ToList();

            return resTree;
        }

        /// <summary>
        /// Build a Expression and use template to selection struct
        /// That struct have some fields(Holder) of Student
        /// </summary>
        /// <param name="lstStudent"></param>
        /// <returns></returns>
        public List<Holder> Expression_SelectFieldDirect<T>(List<T> lstStudent)
        {
            ParameterExpression paramExp = Expression.Parameter(typeof(T));
            NewExpression newHolder = Expression.New(typeof(Holder));
            Type anonType = typeof(Holder);
            MemberInfo item1Member = anonType.GetMember("Item1")[0];
            MemberInfo item2Member = anonType.GetMember("Item2")[0];
            MemberInfo item3Member = anonType.GetMember("Item3")[0];

            // Create a MemberBinding object for each member 
            // that you want to initialize.
            MemberBinding item1MemberBinding =
            Expression.Bind(
                item1Member,
                Expression.PropertyOrField(paramExp, "StudentID"));
            MemberBinding item2MemberBinding =
            Expression.Bind(
                item2Member,
                Expression.PropertyOrField(paramExp, "StudentName"));
            MemberBinding item3MemberBinding =
            Expression.Bind(
                item3Member,
                Expression.PropertyOrField(paramExp, "Age"));

            // Create a MemberInitExpression that represents initializing 
            MemberInitExpression memberInitExpression =
                Expression.MemberInit(
                    newHolder,
                    item1MemberBinding,
                    item2MemberBinding,
                    item3MemberBinding);

            var lambda = Expression.Lambda<Func<T, Holder>>(memberInitExpression, paramExp);

            var comp = lambda.Compile();

            var resHolder = lstStudent.Select(comp).ToList();

            return resHolder;
        }

        public Func<T, object> Expression_SelectFieldCustomFields<T>(out Type newType, params string[] fields)
        {
            ExpressionLambda crt = new ExpressionLambda();

            Func<T, object> lambada = crt.CreateSelectLambda<T>(out newType, fields);

            return lambada;

        }

    }
}

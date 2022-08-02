using Linq_Methods.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq_Methods.Expressions
{
    public class ExpressionLambda
    {

        public Func<T, object> CreateSelectLambda<T>(out Type newType, params string[] fields)
        {
            ParameterExpression paramExp = Expression.Parameter(typeof(T));

            if (fields.Count() == 0)
            {
                List<string> addFields = new();

                foreach (var prop in typeof(T).GetProperties())
                {
                    addFields.Add(prop.Name);
                }

                fields = addFields.ToArray();
            }

            newType = TypesDinamic.CreateNewType($"{typeof(T).Assembly.GetName().Name}_{Guid.NewGuid().ToString("N")}", typeof(T).Name, typeof(T), fields)!;

            if (newType is null)
                throw new ArgumentNullException("newType");

            if (newType?.GetProperties().Count() == 0)
                throw new Exception("The new Type doesn't have propreties. Verify the custom properties you send!");

            NewExpression newTypeExpression = Expression.New(newType!);

            List<MemberBinding> listMembers = new List<MemberBinding>();

            foreach (var item in fields)
            {
                MemberInfo itemMember = newType!.GetMember(item)[0];

                MemberBinding itemMemberBinding = Expression.Bind(
                    itemMember,
                    Expression.PropertyOrField(paramExp, item));

                listMembers.Add(itemMemberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(
                newTypeExpression, listMembers);

            var lambda = Expression.Lambda<Func<T, object>>(memberInitExpression, paramExp);

            var comp = lambda.Compile();

            return comp;
        }
    }
}

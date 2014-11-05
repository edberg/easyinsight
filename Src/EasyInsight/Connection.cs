using System;
using System.Linq;
using System.Linq.Expressions;
using EasyInsight.Internal;

namespace EasyInsight
{
    public class Connection
    {
        public string SourceDataSource { get; private set; }
        public string SourceDataField { get; private set; }
        public string SourceCardinality { get; private set; }
        public string TargetDataSource { get; private set; }
        public string TargetDataField { get; private set; }
        public string TargetCardinality { get; private set; }

        internal Connection() { }

        public static Connection Create<Source, Target>(Expression<Func<Source, object>> source, Expression<Func<Target, object>> target, Cardinality cardinality = Cardinality.OneToOne)
        {
            var sourceDataSource = GetMemberInfo(source).Member.DeclaringType.GetDataSource();
            var sourceDataField = GetMemberInfo(source).Member.GetDataField();
            var targetDataSource = GetMemberInfo(target).Member.DeclaringType.GetDataSource();
            var targetDataField = GetMemberInfo(target).Member.GetDataField();

            if (sourceDataSource == null) throw new ArgumentException("Missing DataSourceAttribute", "source");
            if (sourceDataField == null) throw new ArgumentException("Missing DataFieldAttribute", "source");
            if (targetDataSource == null) throw new ArgumentException("Missing DataSourceAttribute", "target");
            if (targetDataField == null) throw new ArgumentException("Missing DataFieldAttribute", "target");

            return new Connection
            {
                SourceDataSource = sourceDataSource.name,
                SourceDataField = sourceDataField.name,
                SourceCardinality = cardinality.GetSourceCardinality(),
                TargetDataSource = targetDataSource.name,
                TargetDataField = targetDataField.name,
                TargetCardinality = cardinality.GetTargetCardinality(),
            };
        }

        internal static MemberExpression GetMemberInfo(Expression method)
        {
            var lambda = method as LambdaExpression;
            if (lambda == null) throw new ArgumentNullException("method");
            MemberExpression memberExpr = null;
            if (lambda.Body.NodeType == ExpressionType.Convert)
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                memberExpr = lambda.Body as MemberExpression;
            if (memberExpr == null) throw new ArgumentException("method");
            return memberExpr;
        }

    }
}

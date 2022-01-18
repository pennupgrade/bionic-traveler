namespace Framework
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    // From https://stackoverflow.com/a/41516621.

    /// <summary>
    /// Helper class to perform fast invocations of delegates with unknown signatures at runtime.
    /// </summary>
    public class FastMethodInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FastMethodInfo"/> class.
        /// </summary>
        /// <param name="methodInfo">The method.</param>
        public FastMethodInfo(MethodInfo methodInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            var argumentExpressions = new List<Expression>();
            var parameterInfos = methodInfo.GetParameters();
            for (var i = 0; i < parameterInfos.Length; ++i)
            {
                var parameterInfo = parameterInfos[i];
                argumentExpressions.Add(Expression.Convert(Expression.ArrayIndex(argumentsExpression, Expression.Constant(i)), parameterInfo.ParameterType));
            }

            var callExpression = Expression.Call(!methodInfo.IsStatic ? Expression.Convert(instanceExpression, methodInfo.ReflectedType) : null, methodInfo, argumentExpressions);
            if (callExpression.Type == typeof(void))
            {
                var voidDelegate = Expression.Lambda<VoidDelegate>(callExpression, instanceExpression, argumentsExpression).Compile();
                this.Delegate = (instance, arguments) =>
                {
                    voidDelegate(instance, arguments);
                    return null;
                };
            }
            else
            {
                this.Delegate = Expression.Lambda<ReturnValueDelegate>(Expression.Convert(callExpression, typeof(object)), instanceExpression, argumentsExpression).Compile();
            }
        }

        private delegate object ReturnValueDelegate(object instance, object[] arguments);

        private delegate void VoidDelegate(object instance, object[] arguments);

        private ReturnValueDelegate Delegate { get; }

        /// <summary>
        /// Invokes the delegate.
        /// </summary>
        /// <param name="instance">Object instance.</param>
        /// <param name="arguments">Array of arguments.</param>
        /// <returns>Return value.</returns>
        public object Invoke(object instance, params object[] arguments)
        {
            return this.Delegate(instance, arguments);
        }
    }
}
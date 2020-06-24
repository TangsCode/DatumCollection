using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using DatumCollection.Core.Middleware;
using DatumCollection.Infrastructure.Spider;

namespace DatumCollection.Core.Builder
{
    public static class UseMiddlewareExtension
    {
        internal const string InvokeMethodName = "Invoke";
        internal const string InvokeAsyncMethodName = "InvokeAsync";

        private static readonly MethodInfo GetServiceInfo = typeof(UseMiddlewareExtension).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static);
        private static object GetService(IServiceProvider sp,Type type,Type middleware)
        {
            var service = sp.GetService(type);
            if (service == null)
            {
                throw new InvalidOperationException(string.Format("service {0} is not registered.",nameof(type)));
            }

            return service;
        }

        public static IApplicationBuilder UseMiddleware<TMiddleware>(this IApplicationBuilder app, params object[] args)
        {
            return app.UseMiddleware(typeof(TMiddleware), args);
        }

        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder app, Type middlewareType,params object[] args)
        {
            if (typeof(IMiddleware).GetTypeInfo().IsAssignableFrom(middlewareType.GetTypeInfo()))
            {
                if (args.Length > 0)
                {
                    throw new NotSupportedException("IMiddleware doesn't support passing args directly since it's activated from the container");
                }

                return UseMiddlewareInterface(app, middlewareType);
            }

            var applicationServices = app.ApplicationServices;
            return app.Use(next =>
            {
                var methods = middlewareType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                var invokeMethods = methods.Where(m =>
                string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)
                || string.Equals(m.Name, InvokeAsyncMethodName, StringComparison.Ordinal)
                ).ToArray();

                if (invokeMethods.Length > 1)
                {
                    throw new InvalidOperationException(string.Format("Invoke method not found in {0}", nameof(middlewareType)));
                }

                var methodInfo = invokeMethods[0];
                if (typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
                {
                    throw new InvalidOperationException("Invoke method return type is wrong");
                }

                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(SpiderContext))
                {
                    throw new InvalidOperationException(string.Format("invoke method paramters is not {0}", nameof(SpiderContext)));
                }

                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(app.ApplicationServices, middlewareType, ctorArgs);
                if (parameters.Length == 1)
                {
                    return (PiplineDelegate)methodInfo.CreateDelegate(typeof(PiplineDelegate), instance);
                }

                var factory = Compile<object>(methodInfo, parameters);

                return context =>
                {
                    var serviceProvider = context.Services ?? applicationServices;
                    if (serviceProvider == null)
                    {
                        throw new InvalidOperationException("service provider not available");
                    }

                    return factory(instance, context, serviceProvider);
                };
            });
        }

        private static Func<T,SpiderContext,IServiceProvider,Task> Compile<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            var middleware = typeof(T);

            var spiderContextArg = Expression.Parameter(typeof(SpiderContext), "spiderContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middleware, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = spiderContextArg;
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    throw new NotSupportedException(string.Format("paramter type {0} is not supported", nameof(parameterType)));
                }

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType,typeof(Type)),
                    Expression.Constant(methodInfo.DeclaringType,typeof(Type))
                };

                var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression middlewareInstanceArg = instanceArg;
            if (methodInfo.DeclaringType != typeof(T))
            {
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodInfo.DeclaringType);
            }

            var body = Expression.Call(middlewareInstanceArg, methodInfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, SpiderContext, IServiceProvider, Task>>(body, instanceArg, spiderContextArg, providerArg);

            return lambda.Compile();
        }

        private static IApplicationBuilder UseMiddlewareInterface(IApplicationBuilder app, Type middlewareType)
        {
            return app.Use(next =>
            {
                return async context =>
                {
                    var middlewareFactory = (IMiddlewareFactory)context.Services.GetService(typeof(IMiddlewareFactory));
                    if (middlewareFactory == null)
                    {
                        throw new InvalidOperationException("no middleare factory");
                    }
                    var middleware = middlewareFactory.Create(middlewareType);
                    if (middleware == null)
                    {
                        throw new InvalidOperationException("the fatory return null,its a broken implementation");
                    }

                    try
                    {
                        await middleware.InvokeAsync(context, next);
                    }
                    finally
                    {
                        middlewareFactory.Release(middleware);
                    }
                };
            });
        }
    }
}

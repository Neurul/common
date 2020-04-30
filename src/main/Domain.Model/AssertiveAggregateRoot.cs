using CQRSlite.Domain;
using CQRSlite.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace neurUL.Common.Domain.Model
{
    public abstract class AssertiveAggregateRoot : AggregateRoot
    {
        private static readonly IList<string> cache = new List<string>();

        protected override void ApplyEvent(IEvent @event)
        {
            var aggregateType = this.GetType();
            var eventType = @event.GetType();
            var eventName = eventType.Name;
            var typeEventName = aggregateType.Name + eventName;
            if (!AssertiveAggregateRoot.cache.Contains(typeEventName))
            {
                // check if event is supported
                var argtypes = Helper.GetArgTypes(new object[] { @event });
                var m = Helper.GetMember(aggregateType, "Apply", argtypes);

                // actual fix should be in line 34 of https://github.com/gautema/CQRSlite/blob/master/Framework/CQRSlite/Infrastructure/DynamicInvoker.cs
                AssertionConcern.AssertStateTrue(m != null, $"'{eventName}' is not a recognized event of '{aggregateType.Name}'.");

                AssertiveAggregateRoot.cache.Add(typeEventName);
            }
            base.ApplyEvent(@event);
        }

        private void Apply(UnrecognizedEvent e) {}
    }

    internal static class Helper
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal static Type[] GetArgTypes(object[] args)
        {
            var argtypes = new Type[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var argtype = args[i].GetType();
                argtypes[i] = argtype;
            }
            return argtypes;
        }

        internal static MethodInfo GetMember(Type type, string name, Type[] argtypes)
        {
            while (true)
            {
                var methods = type.GetMethods(bindingFlags).Where(m => m.Name == name).ToArray();
                var member = methods.FirstOrDefault(m => m.GetParameters().Select(p => p.ParameterType).SequenceEqual(argtypes)) ??
                             methods.FirstOrDefault(m => m.GetParameters().Select(p => p.ParameterType).ToArray().Matches(argtypes));

                if (member != null)
                {
                    return member;
                }
                var t = type.GetTypeInfo().BaseType;
                if (t == null)
                {
                    return null;
                }
                type = t;
            }
        }

        internal static bool Matches(this Type[] arr, Type[] args)
        {
            if (arr.Length != args.Length) return false;
            for (var i = 0; i < args.Length; i++)
            {
                if (!arr[i].IsAssignableFrom(args[i]))
                    return false;
            }
            return true;
        }
    }
}

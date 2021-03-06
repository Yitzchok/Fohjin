using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace Fohjin.EventStore.Infrastructure
{
    public interface IAggregateRootFactory
    {
        TAggregateRoot Create<TAggregateRoot>();
        object Create(Type type);
    }

    public class AggregateRootFactory : IAggregateRootFactory
    {
        private readonly ICacheRegisteredEvents _cacheRegisteredEvents;
        private readonly ProxyGenerator _proxyGenerator;

        public AggregateRootFactory(ICacheRegisteredEvents cacheRegisteredEvents)
        {
            _cacheRegisteredEvents = cacheRegisteredEvents;
            _proxyGenerator = new ProxyGenerator();
        }

        public TAggregateRoot Create<TAggregateRoot>()
        {
            return (TAggregateRoot)Create(typeof(TAggregateRoot));
        }

        public object Create(Type type)
        {
            HasApplyMethod(type);
            HasRegiteredEventsMethod(type);

            var eventProvider = new EventProvider();
            var orginator = new Orginator();
            
            var proxy = CreateProxy(type, eventProvider, orginator);

            orginator.SetProxy(proxy);

            var cache = _cacheRegisteredEvents.Get(type);
            return cache == null
                ? SetProxyAndSaveRegisteredEventsInCache(eventProvider, proxy, type)
                : SetProxyAndRegisteredEventsFromCache(eventProvider, proxy, cache);
        }

        private static object SetProxyAndRegisteredEventsFromCache(EventProvider eventProvider, object proxy, Dictionary<Type, List<Action<object, object>>> cache)
        {
            eventProvider.SetProxy(proxy);
            eventProvider.SetRegisteredEventHandlers(cache);
            return proxy;
        }

        private object SetProxyAndSaveRegisteredEventsInCache(EventProvider eventProvider, object proxy, Type type)
        {
            eventProvider.SetProxy(proxy, type);
            _cacheRegisteredEvents.Add(type, eventProvider.GetRegisteredEventHandlers());
            return proxy;
        }

        private object CreateProxy(Type type, IInterceptor eventProvider, Orginator orginator)
        {
            var proxyGenerationOptions = new ProxyGenerationOptions();
            proxyGenerationOptions.AddMixinInstance(eventProvider);
            proxyGenerationOptions.AddMixinInstance(orginator);

            return _proxyGenerator.CreateClassProxy(
                    type, 
                    proxyGenerationOptions,
                    eventProvider
                );
        }

        private static void HasApplyMethod(Type type)
        {
            var applyMethod = type.GetMethod("Apply", BindingFlags.Instance | BindingFlags.NonPublic);

            if (applyMethod == null || applyMethod.ToString() != "Void Apply(System.Object)")
                throw new ProtectedApplyMethodMissingException(string.Format("Object '{0}' needs to have a 'proteced virtual void Apply(object @event)' method declared", type.FullName));
        }

        private static void HasRegiteredEventsMethod(Type type)
        {
            var regiteredEventsMethod = type.GetMethod("RegisteredEvents", BindingFlags.Instance | BindingFlags.NonPublic);

            if (regiteredEventsMethod == null || regiteredEventsMethod.ToString() != "System.Collections.Generic.IEnumerable`1[System.Type] RegisteredEvents()")
                throw new ProtectedRegisteredEventsMethodMissingException(string.Format("Object '{0}' needs to have a 'protected IEnumerable<Type> RegisteredEvents()' method declared", type.FullName));
        }
    }
}
// https://chris.charabaruk.com/2014/10/using-tinyioc-with-common-service-locator/
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace org.neurul.Common.Http
{
    public class TinyIoCServiceLocator : IServiceProvider
    {
        private TinyIoCContainer _container;
        private TinyIoCContainer _requestContainer;

        public TinyIoCServiceLocator(TinyIoCContainer container)
        {
            _container = container ?? TinyIoCContainer.Current;
            this._requestContainer = null;
        }

        public TinyIoCServiceLocator() : this(null) { }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            // return _container.ResolveAll(typeof(TService), true).Cast<TService>();
            return (IEnumerable<TService>)this.GetService(typeof(TService), (x, y) => x.ResolveAll(y, true).Cast<TService>());
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            // return _container.ResolveAll(serviceType, true);
            return (IEnumerable<object>) this.GetService(serviceType, (x, y) => x.ResolveAll(y, true));
        }

        public TService GetInstance<TService>(string key)
        {
            // return (TService)_container.Resolve(typeof(TService), key);
            return (TService) this.GetService(typeof(TService), (x, y) => x.Resolve(y, key));
        }

        private object GetService(Type resolveType, Func<TinyIoCContainer, Type, object> getter)
        {
            object result = null;

            if (this._container.CanResolve(resolveType))
                result = getter(this._container, resolveType);
            else
                result = getter(this._requestContainer, resolveType);

            return result;
        }

        public TService GetInstance<TService>()
        {
            // return (TService)_container.Resolve(typeof(TService));
            return (TService) this.GetService(typeof(TService), (x, y) => x.Resolve(y));
        }

        public object GetInstance(Type serviceType, string key)
        {
            // return _container.Resolve(serviceType, key);
            return this.GetService(serviceType, (x, y) => x.Resolve(y, key));
        }

        public object GetInstance(Type serviceType)
        {
            // return _container.Resolve(serviceType);
            return this.GetService(serviceType, (x, y) => x.Resolve(y));
        }

        public object GetService(Type serviceType)
        {
            // return _container.Resolve(serviceType);
            return this.GetService(serviceType, (x, y) => x.Resolve(y));
        }

        public T GetService<T>()
        {
            // return (T) _container.Resolve(typeof(T));
            return (T) this.GetService(typeof(T), (x, y) => x.Resolve(y));
        }

        public void SetRequestContainer(TinyIoCContainer requestContainer)
        {
            this._requestContainer = requestContainer;
        }
    }
}
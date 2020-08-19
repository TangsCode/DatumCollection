using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Core.Builder
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        private const string ApplicationServicesKey = "application.Services";
        private readonly IList<Func<PiplineDelegate, PiplineDelegate>> _components = new List<Func<PiplineDelegate, PiplineDelegate>>();

        public ApplicationBuilder(IServiceProvider serviceProvider)
        {
            Properties = new Dictionary<string, object>(StringComparer.Ordinal);
            ApplicationServices = serviceProvider;
        }

        private ApplicationBuilder(ApplicationBuilder builder)
        {
            Properties = new CopyOnWriteDictionary<string, object>(builder.Properties, StringComparer.Ordinal);
        }

        public IServiceProvider ApplicationServices
        {
            get
            {
                return GetProperty<IServiceProvider>(ApplicationServicesKey);
            }
            set
            {
                SetProperty<IServiceProvider>(ApplicationServicesKey, value);
            }
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        private T GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out object value) ? (T)value : default(T);
        }

        public IDictionary<string, object> Properties { get; }

        public PiplineDelegate Build()
        {
            PiplineDelegate app = context =>
            {
                return Task.CompletedTask;
            };

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }

        public IApplicationBuilder New()
        {
            return new ApplicationBuilder(this);
        }

        public IApplicationBuilder Use(Func<PiplineDelegate, PiplineDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }
    }
}

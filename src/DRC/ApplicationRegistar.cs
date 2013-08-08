namespace DRC
{
    using System;
    using System.Linq;

    public static class ApplicationRegistar
    {
        public static void ProcessRegistrations(TinyIoC.TinyIoCContainer container)
        {
            var applicationRegistrationImplementations = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetReferencedAssemblies().Contains(typeof(ApplicationRegistar).Assembly.GetName()) || a == typeof(ApplicationRegistar).Assembly).SelectMany(t => t.GetTypes()).Where(t => t.Implements(typeof(IApplicationRegistration))).ToList();
            
            container.RegisterMultiple<IApplicationRegistration>(applicationRegistrationImplementations);
            
            var applicationRegistrations = container.ResolveAll<IApplicationRegistration>().ToList();
            
            foreach (var typeRegistration in applicationRegistrations.SelectMany(ar => ar.TypeRegistrations))
            {
                container.Register(typeRegistration.RegistrationType, typeRegistration.InstanceType);
            }

            foreach (var instanceRegistration in applicationRegistrations.SelectMany(ar => ar.InstanceRegistrations))
            {
                container.Register(instanceRegistration.RegistrationType, instanceRegistration.Instance);
            }

            foreach (var collectionRegistration in applicationRegistrations.SelectMany(ar => ar.CollectionRegistration).GroupBy(cr => cr.RegistrationType))
            {
                container.RegisterMultiple(collectionRegistration.Key, collectionRegistration.SelectMany(c => c.InstanceTypes));
            }
        }
    }
}
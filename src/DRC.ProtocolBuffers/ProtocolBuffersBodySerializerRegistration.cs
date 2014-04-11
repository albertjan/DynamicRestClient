namespace DRC.ProtocolBuffers
{
    using System.Collections.Generic;
    using System.Linq;

    using DRC.Interfaces;

    public class ProtocolBuffersBodySerializerRegistration : IApplicationRegistration
    {
        public IEnumerable<TypeRegistration> TypeRegistrations { get { return Enumerable.Empty<TypeRegistration>(); } }

        public IEnumerable<InstanceRegistration> InstanceRegistrations { get { return Enumerable.Empty<InstanceRegistration>(); } }

        public IEnumerable<CollectionRegistration> CollectionRegistration
        {
            get
            {
                return new[]
                {
                    new CollectionRegistration()
                    {
                        RegistrationType = typeof (IBodySerializer),
                        InstanceTypes = new[] {typeof (ProtocolBuffersBodySerializer)}
                    }
                };
            } 
        }
    }
}
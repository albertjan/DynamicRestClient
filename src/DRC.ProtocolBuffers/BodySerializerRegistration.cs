﻿using System.Collections.Generic;
using System.Linq;
using DRC.BodySerializers;
using DRC.Interfaces;

namespace DRC.ProtocolBuffers
{
    public class BodySerializerRegistration : IApplicationRegistration
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
                        InstaceTypes = new[] {typeof (ProtocolBuffersBodySerializer)}
                    }
                };
            } 
        }
    }
}
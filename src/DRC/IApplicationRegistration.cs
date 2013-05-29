namespace DRC
{
    using System;
    using System.Collections.Generic;

    public interface IApplicationRegistration
    {
        IEnumerable<TypeRegistration> TypeRegistrations { get; }
        IEnumerable<InstanceRegistration> InstanceRegistrations { get; }
        IEnumerable<CollectionRegistration> CollectionRegistration { get; }
    }

    public class CollectionRegistration : Registration
    {
        public IEnumerable<Type> InstanceTypes { get; set; }
    }

    public abstract class Registration
    {
        public Type RegistrationType { get; set; }
    }

    public class TypeRegistration : Registration
    {
        public Type InstanceType { get; set; }        
    }

    public class InstanceRegistration : Registration
    {
        public object Instance { get; set; }
    }
}
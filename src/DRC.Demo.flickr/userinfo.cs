﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class rsp {
    
    private rspPerson[] personField;
    
    private string statField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("person", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public rspPerson[] person {
        get {
            return this.personField;
        }
        set {
            this.personField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string stat {
        get {
            return this.statField;
        }
        set {
            this.statField = value;
        }
    }

    public override string ToString()
    {
        return "Name: " + person[0].realname;
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class rspPerson {
    
    private string usernameField;
    
    private string realnameField;
    
    private string locationField;
    
    private string descriptionField;
    
    private string photosurlField;
    
    private string profileurlField;
    
    private string mobileurlField;
    
    private rspPersonTimezone[] timezoneField;
    
    private rspPersonPhotos[] photosField;
    
    private string idField;
    
    private string nsidField;
    
    private string isproField;
    
    private string iconserverField;
    
    private string iconfarmField;
    
    private string path_aliasField;
    
    private string datecreateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string username {
        get {
            return this.usernameField;
        }
        set {
            this.usernameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string realname {
        get {
            return this.realnameField;
        }
        set {
            this.realnameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string location {
        get {
            return this.locationField;
        }
        set {
            this.locationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string photosurl {
        get {
            return this.photosurlField;
        }
        set {
            this.photosurlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string profileurl {
        get {
            return this.profileurlField;
        }
        set {
            this.profileurlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string mobileurl {
        get {
            return this.mobileurlField;
        }
        set {
            this.mobileurlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("timezone", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public rspPersonTimezone[] timezone {
        get {
            return this.timezoneField;
        }
        set {
            this.timezoneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("photos", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public rspPersonPhotos[] photos {
        get {
            return this.photosField;
        }
        set {
            this.photosField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nsid {
        get {
            return this.nsidField;
        }
        set {
            this.nsidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ispro {
        get {
            return this.isproField;
        }
        set {
            this.isproField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string iconserver {
        get {
            return this.iconserverField;
        }
        set {
            this.iconserverField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string iconfarm {
        get {
            return this.iconfarmField;
        }
        set {
            this.iconfarmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string path_alias {
        get {
            return this.path_aliasField;
        }
        set {
            this.path_aliasField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string datecreate {
        get {
            return this.datecreateField;
        }
        set {
            this.datecreateField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class rspPersonTimezone {
    
    private string labelField;
    
    private string offsetField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string label {
        get {
            return this.labelField;
        }
        set {
            this.labelField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string offset {
        get {
            return this.offsetField;
        }
        set {
            this.offsetField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class rspPersonPhotos {
    
    private string firstdatetakenField;
    
    private string firstdateField;
    
    private string countField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string firstdatetaken {
        get {
            return this.firstdatetakenField;
        }
        set {
            this.firstdatetakenField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string firstdate {
        get {
            return this.firstdateField;
        }
        set {
            this.firstdateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string count {
        get {
            return this.countField;
        }
        set {
            this.countField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class NewDataSet {
    
    private rsp[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("rsp")]
    public rsp[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}
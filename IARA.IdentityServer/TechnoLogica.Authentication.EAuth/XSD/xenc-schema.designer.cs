// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.23471
//    <NameSpace>TechnoLogica.Authentication.EAuth.XSD.Enc</NameSpace><Collection>Array</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>True</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>True</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace TechnoLogica.Authentication.EAuth.XSD.Xenc
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("CipherData", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class CipherDataType {
        
        private object itemField;
        
        [System.Xml.Serialization.XmlElementAttribute("CipherReference", typeof(CipherReferenceType), Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("CipherValue", typeof(byte[]), DataType="base64Binary", Order=0)]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("CipherReference", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class CipherReferenceType {
        
        private TransformsType itemField;
        
        private string uRIField;
        
        [System.Xml.Serialization.XmlElementAttribute("Transforms", Order=0)]
        public TransformsType Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string URI {
            get {
                return this.uRIField;
            }
            set {
                this.uRIField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=true)]
    public partial class TransformsType {
        
        private TransformType[] transformField;
        
        [System.Xml.Serialization.XmlElementAttribute("Transform", Namespace="http://www.w3.org/2000/09/xmldsig#", Order=0)]
        public TransformType[] Transform {
            get {
                return this.transformField;
            }
            set {
                this.transformField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptedData", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class EncryptedDataType : EncryptedType {
    }
    
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EncryptedKeyType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EncryptedDataType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=true)]
    public abstract partial class EncryptedType {
        
        private EncryptionMethodType encryptionMethodField;
        
        private KeyInfoType keyInfoField;
        
        private CipherDataType cipherDataField;
        
        private EncryptionPropertiesType encryptionPropertiesField;
        
        private string idField;
        
        private string typeField;
        
        private string mimeTypeField;
        
        private string encodingField;
        
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public EncryptionMethodType EncryptionMethod {
            get {
                return this.encryptionMethodField;
            }
            set {
                this.encryptionMethodField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#", Order=1)]
        public KeyInfoType KeyInfo {
            get {
                return this.keyInfoField;
            }
            set {
                this.keyInfoField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public CipherDataType CipherData {
            get {
                return this.cipherDataField;
            }
            set {
                this.cipherDataField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public EncryptionPropertiesType EncryptionProperties {
            get {
                return this.encryptionPropertiesField;
            }
            set {
                this.encryptionPropertiesField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MimeType {
            get {
                return this.mimeTypeField;
            }
            set {
                this.mimeTypeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Encoding {
            get {
                return this.encodingField;
            }
            set {
                this.encodingField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=true)]
    public partial class EncryptionMethodType {
        
        private string keySizeField;
        
        private byte[] oAEPparamsField;
        
        private System.Xml.XmlNode[] anyField;
        
        private string algorithmField;
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer", Order=0)]
        public string KeySize {
            get {
                return this.keySizeField;
            }
            set {
                this.keySizeField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary", Order=1)]
        public byte[] OAEPparams {
            get {
                return this.oAEPparamsField;
            }
            set {
                this.oAEPparamsField = value;
            }
        }
        
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=2)]
        public System.Xml.XmlNode[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Algorithm {
            get {
                return this.algorithmField;
            }
            set {
                this.algorithmField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptionProperties", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class EncryptionPropertiesType {
        
        private EncryptionPropertyType[] encryptionPropertyField;
        
        private string idField;
        
        [System.Xml.Serialization.XmlElementAttribute("EncryptionProperty", Order=0)]
        public EncryptionPropertyType[] EncryptionProperty {
            get {
                return this.encryptionPropertyField;
            }
            set {
                this.encryptionPropertyField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptionProperty", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class EncryptionPropertyType {
        
        private System.Xml.XmlElement[] itemsField;
        
        private string[] textField;
        
        private string targetField;
        
        private string idField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=0)]
        public System.Xml.XmlElement[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Target {
            get {
                return this.targetField;
            }
            set {
                this.targetField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("EncryptedKey", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class EncryptedKeyType : EncryptedType {
        
        private ReferenceList referenceListField;
        
        private string carriedKeyNameField;
        
        private string recipientField;
        
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public ReferenceList ReferenceList {
            get {
                return this.referenceListField;
            }
            set {
                this.referenceListField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string CarriedKeyName {
            get {
                return this.carriedKeyNameField;
            }
            set {
                this.carriedKeyNameField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Recipient {
            get {
                return this.recipientField;
            }
            set {
                this.recipientField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class ReferenceList {
        
        private ReferenceType[] itemsField;
        
        private ItemsChoiceType3[] itemsElementNameField;
        
        [System.Xml.Serialization.XmlElementAttribute("DataReference", typeof(ReferenceType), Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("KeyReference", typeof(ReferenceType), Order=0)]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public ReferenceType[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName", Order=1)]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType3[] ItemsElementName {
            get {
                return this.itemsElementNameField;
            }
            set {
                this.itemsElementNameField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=true)]
    public partial class ReferenceType {
        
        private System.Xml.XmlElement[] anyField;
        
        private string uRIField;
        
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=0)]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string URI {
            get {
                return this.uRIField;
            }
            set {
                this.uRIField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#", IncludeInSchema=false)]
    public enum ItemsChoiceType3 {
        
        /// <remarks/>
        DataReference,
        
        /// <remarks/>
        KeyReference,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.4.0.30701")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [System.Xml.Serialization.XmlRootAttribute("AgreementMethod", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public partial class AgreementMethodType {
        
        private byte[] kANonceField;
        
        private System.Xml.XmlNode[] anyField;
        
        private KeyInfoType originatorKeyInfoField;
        
        private KeyInfoType recipientKeyInfoField;
        
        private string algorithmField;
        
        [System.Xml.Serialization.XmlElementAttribute("KA-Nonce", DataType="base64Binary", Order=0)]
        public byte[] KANonce {
            get {
                return this.kANonceField;
            }
            set {
                this.kANonceField = value;
            }
        }
        
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=1)]
        public System.Xml.XmlNode[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public KeyInfoType OriginatorKeyInfo {
            get {
                return this.originatorKeyInfoField;
            }
            set {
                this.originatorKeyInfoField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public KeyInfoType RecipientKeyInfo {
            get {
                return this.recipientKeyInfoField;
            }
            set {
                this.recipientKeyInfoField = value;
            }
        }
        
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
        public string Algorithm {
            get {
                return this.algorithmField;
            }
            set {
                this.algorithmField = value;
            }
        }
    }
}
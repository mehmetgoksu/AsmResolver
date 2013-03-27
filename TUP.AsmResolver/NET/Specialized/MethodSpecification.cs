﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TUP.AsmResolver.NET.Specialized
{
    public class MethodSpecification : MethodReference , ISpecification
    {
        MethodReference originalmethod;
        TypeReference[] genericArgs;
        GenericParameter[] genericParams;
        IGenericArgumentsProvider argProvider;
        IGenericParametersProvider paramProvider;

        internal MethodSpecification()
        {
        }
        public MethodSpecification(MethodReference methodRef)
        {
            OriginalMethod = methodRef;
            paramProvider = methodRef;
            argProvider = methodRef.DeclaringType;
            
        }

        public ISpecification TransformWith(IGenericParametersProvider paramProvider, IGenericArgumentsProvider argProvider)
        {
            if (this.IsGenericMethod)
            {
                MethodSpecification copy = new MethodSpecification(OriginalMethod);
                copy.paramProvider = paramProvider ;
                copy.argProvider = argProvider;
                copy.metadatarow = this.metadatarow;
                return copy;
            }
            return this;
        }

        public MethodReference OriginalMethod
        {
            get
            {
                if (originalmethod == null)
                    originalmethod = (MethodReference)netheader.TablesHeap.MethodDefOrRef.GetMember(Convert.ToInt32(metadatarow.parts[0]));
                return originalmethod;
                //if ( == 0)
                //    return null;
                //return (MethodReference)netheader.tableheap.tablereader.MethodDefOrRef.GetMember(Convert.ToInt32(metadatarow.parts[0]));
            }
            private set { originalmethod = value; }
        }

        public override TypeReference DeclaringType
        {
            get
            {
                if (originalmethod != null)
                    return originalmethod.DeclaringType;
                else
                    return null;
            }
        }
        public override bool IsDefinition
        {
            get
            {
                return false;
            }
        }
        public override string Name
        {
            get
            {
                if (originalmethod != null)
                    return originalmethod.Name;
                return null;
            }
        }
        public override bool IsGenericMethod
        {
            get
            {
                return true;
            }
        }

        public override GenericParameter[] GenericParameters
        {
            get
            {
                if (genericParams == null)
                {
                    genericParams = new GenericParameter[GenericArguments.Length];
                    for (int i = 0; i < genericParams.Length; i++)
                        genericParams[i] = new GenericParameter() { name = "!!" + i, owner = this };
                }
                return genericParams;
            }
        }

        public TypeReference[] GenericArguments
        {
            get
            {
                if (genericArgs == null)
                    genericArgs = netheader.BlobHeap.ReadGenericArgumentsSignature(SpecificationSignature, paramProvider, argProvider );
                return genericArgs;

            }
        }
         
        public override MethodSignature Signature
        {
            get
            {
                return OriginalMethod.Signature;
            }
        }

        public uint SpecificationSignature
        {
            get { return Convert.ToUInt32(metadatarow.parts[1]); }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(OriginalMethod.Signature.ReturnType.FullName);
            builder.Append(" ");
            builder.Append(OriginalMethod.DeclaringType.ToString());
            builder.Append("::");
            builder.Append(OriginalMethod.Name);
            if (GenericArguments != null && GenericArguments.Length > 0)
            {
                builder.Append("<");
                for (int i = 0;i < GenericArguments.Length; i++)
                    builder.Append(GenericArguments[i].FullName + (i == GenericArguments.Length-1 ? "": ", "));
                builder.Append(">");
            }
            builder.Append(OriginalMethod.Signature.GetParameterString());
            return builder.ToString();
        }
        //public uint Signature { get { return Convert.ToUInt32(metadatarow.parts[1]); } }

    }
}

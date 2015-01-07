﻿using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface ITypeMemberContainer : IDom, IContainer
    {
        RDomCollection<ITypeMemberAndDetail> MembersAll { get; }

        IEnumerable<ITypeMember> Members { get; }

        IEnumerable<IProperty> Properties { get; }

        IEnumerable<IMethod> Methods { get; }

        IEnumerable<IEvent> Events { get; }
    }
}
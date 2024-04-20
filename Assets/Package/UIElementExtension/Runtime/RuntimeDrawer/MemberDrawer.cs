using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(MemberInfo))]
    public class MemberDrawer : RuntimeDrawer<MemberInfo>
    {
        public override void RepaintDrawer()
        {
            throw new System.NotImplementedException();
            
        }

        protected override void CreateGUI()
        {
            throw new System.NotImplementedException();
        }
    }
}

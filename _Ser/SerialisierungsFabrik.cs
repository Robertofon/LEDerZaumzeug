using System;
using System.Collections.Generic;
using System.Text;

namespace LEDerZaumzeug
{
    public class SerialisierungsFabrik
    {
        public MusterPipeline ErstellePipeline(MusterNode programm)
        {
            if( programm is JoinNode )
            {
                var join = (JoinNode)programm;
                Type t = Type.GetType(join.TypeName);
                IJoin joinobj = (IJoin)Activator.CreateInstance(t);
                joinobj.Initialize();

                MusterPipeline
                join.TypeName
            }
        }


    }
}

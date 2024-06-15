types = {
    "string" : "String",
    "int" : "Int",
    "int[]" : "IntArray",
    "uint" : "UInt",
    "float" : "Float",
    "float[]" : "FloatArray",
    "Vector2" : "Vector2",
    "Vector3" : "Vector3",
    "AnimationCurve" : "Curve",
    "Object" : "UnityEngine.Object",
}

template = """using System;
using UnityEngine;

namespace HIAAC.BehaviorTree
{{
    [Serializable]
    public class {fullname} : BlackboardProperty<{type}>
    {{}}
}}
"""


for t in types:
    name = types[t]
    fullname = name+"BlackboardProperty"
    
    f = open(fullname+".cs", "w", encoding='utf-8')
    text = template.format(fullname=fullname, type=t)
    f.write(text)
    f.close()


Shader "Solid Color" {
 
Properties {
    _Color ("Main Color", Color) = (0.1,0.1,0.1,1)
}
 
SubShader {
    Color [_Color]
    Pass {Color(.3,.3,.3)}
} 
 
}
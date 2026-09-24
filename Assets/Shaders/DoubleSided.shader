// 双面渲染 shader：解决 Quad / Plane 从背面看不可见的问题
// 用法：新建材质 -> Shader 选 Custom/DoubleSided -> 赋给 Quad
// Cull Mode 保持 Off 即可双面可见；也可以暴露出参数在 Inspector 里切换。
Shader "Custom/DoubleSided"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // _Cull = 0 表示关闭背面剔除，正反两面都画
        Cull [_Cull]

        CGPROGRAM
        // 标准（内置管线）光照模型，物理光照
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = _EmissionColor.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    // 老式显卡的降级方案，同样双面
    FallBack "Legacy Shaders/Transparent/Cutout/VertexLit"
}

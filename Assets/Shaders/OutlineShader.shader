Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 1, 0, 1)
        _OutlineThickness ("Outline Thickness", Range (0.0, 0.03)) = 0.003
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "BASE"
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                // just make a copy of incoming vertex data but scaled according to normal direction
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 norm = mul((float3x3)unity_ObjectToWorld, v.normal);
                float3 scaled = norm * _OutlineThickness;
                o.pos.xy += scaled.xy;

                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

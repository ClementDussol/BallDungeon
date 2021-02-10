Shader "Sprites/DropShadow"
{
    Properties
    {
        _XOffset("Horizontal Offset", float) = 0.25
        _YOffset("Vertical Offset", float) = -0.25
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 0)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _ShadowColor;
            float _XOffset;
            float _YOffset;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvOffset = float2(_XOffset, _YOffset) * _MainTex_TexelSize.xy;
                fixed4 offset = tex2D(_MainTex, i.uv - uvOffset.xy);
                fixed4 color  = tex2D(_MainTex, i.uv);
                
                return offset.a > 0 && color.a == 0 ? _ShadowColor * offset.a : color;
            }
            ENDCG
        }
    }
}

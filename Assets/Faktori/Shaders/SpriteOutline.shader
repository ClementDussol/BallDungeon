// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        // Add values to determine if outlining is enabled and outline color.
    	_Outline ("Outline", Float) = 1
    	_OutlineColor("Outline Color", Color) = (1,1,1,1)
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

            fixed4 _Color;
            float _Outline;
            fixed4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                float4 offset = 0;
                // If outline is enabled and there is a pixel, try to draw an outline.
                if (_Outline > 0) {
                    // Get the neighbouring four pixels.
                    fixed4 pixelUp    = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y * _Outline));
                    fixed4 pixelDown  = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y * _Outline));
                    fixed4 pixelRight = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x * _Outline, 0));
                    fixed4 pixelLeft  = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x * _Outline, 0));
                    fixed4 pixelTopLeft      = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * .66 * _Outline);
                    fixed4 pixelTopRight     = tex2D(_MainTex, i.uv - fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y) * .66 * _Outline);
                    fixed4 pixelBottomRight  = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * .66 * _Outline);
                    fixed4 pixelBottomLeft   = tex2D(_MainTex, i.uv + fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y) * .66 * _Outline);

                    offset = pixelUp + pixelDown + pixelRight + pixelLeft + pixelBottomLeft + pixelBottomRight + pixelTopLeft + pixelTopRight;
                }

                return offset.a > 0 && color.a == 0 ? _OutlineColor * offset.a : color;
            }
            ENDCG
        }
    }
}
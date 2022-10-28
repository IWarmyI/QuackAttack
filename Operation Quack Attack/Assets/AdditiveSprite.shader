// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Additive"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
		// Blend SrcAlpha One
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color; // color to add

            // vertex shader
            v2f vert(appdata v)
            {
                v2f o;
                // transform position to clip space
                o.vertex = UnityObjectToClipPos(v.vertex);
                // just pass the texture coordinate and color
                o.texcoord = v.texcoord;
                o.color = v.color;// * _Color;

                // pixel snap
                #ifdef PIXELSNAP_ON
                    o.vertex = UnityPixelSnap(o.vertex);
                #endif

                return o;
            }

            sampler2D _MainTex; // texture to sample

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 color = tex2D(_MainTex, i.texcoord) * i.color;
                // add _color to the texture's color
                color.rgb = color.rgb + _Color.rgb;
                color.rgb *= color.a;
                return color;
            }
            ENDCG
        }
    }
}
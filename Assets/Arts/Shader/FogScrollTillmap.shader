Shader "UI/FogScrollUI_WithAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Vector) = (0.03, 0.01, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ScrollSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;         // <- important
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;         // <- on transmet la couleur
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;            // <- on la garde
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 scrolledUV = i.uv + _ScrollSpeed.xy * _Time.y;
                scrolledUV = frac(scrolledUV); // pour le looping
                fixed4 texColor = tex2D(_MainTex, scrolledUV);
                
                // Multiplie par la couleur du vertex (inclut alpha)
                return texColor * i.color;
            }
            ENDCG
        }
    }
}

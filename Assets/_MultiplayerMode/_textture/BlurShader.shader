Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lấy giá trị trung tâm của texture
                fixed4 color = tex2D(_MainTex, i.uv);

                // Lấy giá trị từ các điểm xung quanh để làm mờ
                color += tex2D(_MainTex, i.uv + float2(_BlurSize, 0));
                color += tex2D(_MainTex, i.uv - float2(_BlurSize, 0));
                color += tex2D(_MainTex, i.uv + float2(0, _BlurSize));
                color += tex2D(_MainTex, i.uv - float2(0, _BlurSize));

                // Trung bình giá trị
                color /= 5.0;

                return color;
            }
            ENDCG
        }
    }
}

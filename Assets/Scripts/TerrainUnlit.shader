Shader "Unlit/TerrainUnlit"
{
    Properties
    {
        _MainTex0 ("Main Texture 0", 2D) = "white" {}
        _MainTex1 ("Main Texture 1", 2D) = "white" {}
        _MainTex2 ("Main Texture 2", 2D) = "white" {}
        _MainTex3 ("Main Texture 3", 2D) = "white" {}
        _MainTex4 ("Main Texture 4", 2D) = "white" {}
        _MainTex5 ("Main Texture 5", 2D) = "white" {}
        _MainTex6 ("Main Texture 6", 2D) = "white" {}
        _MainTex7 ("Main Texture 7", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _Map0;
            sampler2D _Map1;

            // texture for each point
            sampler2D _MainTex0;
            sampler2D _MainTex1;
            sampler2D _MainTex2;
            sampler2D _MainTex3;
            sampler2D _MainTex4;
            sampler2D _MainTex5;
            sampler2D _MainTex6;
            sampler2D _MainTex7;

            // tile and offset for each texture
            float4 _MainTex0_ST;
            float4 _MainTex1_ST;
            float4 _MainTex2_ST;
            float4 _MainTex3_ST;
            float4 _MainTex4_ST;
            float4 _MainTex5_ST;
            float4 _MainTex6_ST;
            float4 _MainTex7_ST;

            int GetTextureIndex(int x, int y)
            {
                
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex0);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex0, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

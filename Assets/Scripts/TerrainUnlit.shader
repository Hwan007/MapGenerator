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
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uv4 : TEXCOORD4;
                float2 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
                float2 uv7 : TEXCOORD7;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uv4 : TEXCOORD4;
                float2 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
                float2 uv7 : TEXCOORD7;
                // float3 worldPos : TEXCOORD8;
                fixed4 colorLeft : TEXCOORD8;
                fixed4 colorRight : TEXCOORD9;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

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

            sampler2D _Map0;
            sampler2D _Map1;

            float mixScale;
            float mixPower;
            /*
            fixed4 GetCodeAround(sampler2D map, float2 uv, float radius)
            {
                fixed4 codeAdd = {0,0,0,0};
                for (int y = -1; y < 2; ++y)
                {
                    for (int x = -1; x < 2; ++x)
                    {
                        if (x == 0 && y == 0) continue;

                        float2 pos = {clamp(uv.x+x*radius,0,1), clamp(uv.y+y*radius,0,1)};
                        codeAdd += tex2Dlod(map, float4(pos.xy, 0, 0));
                    }
                }
                codeAdd /= 8;

                return codeAdd;
            }
            
            fixed4 GetTextureIndex(sampler2D map, float2 uv, float mixScale, float mixPower)
            {
                fixed4 code = tex2Dlod(map, float4(uv.xy, 0, 0));
                fixed4 codeAround = GetCodeAround(map, uv, mixScale);

                fixed4 codeRet = code + codeAround * mixPower;
                fixed4 index = {clamp(codeRet.r,0,1), clamp(codeRet.g,0,1), clamp(codeRet.b,0,1), clamp(codeRet.a,0,1)};

                return index;
            }
            */
            v2f vert (appdata v)
            {
                v2f o;
                float2 uv = { v.vertex.x, v.vertex.z };
                o.colorLeft = GetTextureIndex(_Map0, uv, mixScale, mixPower);
                o.colorRight = GetTextureIndex(_Map1, uv, mixScale, mixPower);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex0);
                o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex1);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex2);
                o.uv3 = TRANSFORM_TEX(v.uv3, _MainTex3);
                o.uv4 = TRANSFORM_TEX(v.uv4, _MainTex4);
                o.uv5 = TRANSFORM_TEX(v.uv5, _MainTex5);
                o.uv6 = TRANSFORM_TEX(v.uv6, _MainTex6);
                o.uv7 = TRANSFORM_TEX(v.uv7, _MainTex7);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = {0,0,0,0};
                col += tex2D(_MainTex0, i.uv0) * i.colorLeft.r;
                col += tex2D(_MainTex1, i.uv1) * i.colorLeft.g;
                col += tex2D(_MainTex2, i.uv2) * i.colorLeft.b;
                col += tex2D(_MainTex3, i.uv3) * i.colorLeft.w;
                col += tex2D(_MainTex4, i.uv4) * i.colorRight.r;
                col += tex2D(_MainTex5, i.uv5) * i.colorRight.g;
                col += tex2D(_MainTex6, i.uv6) * i.colorRight.b;
                col += tex2D(_MainTex7, i.uv7) * i.colorRight.w;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

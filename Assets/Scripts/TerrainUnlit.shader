Shader "Unlit/TerrainUnlit"
{
    Properties
    {
        _Map0 ("Map 0", 2D) = "white" {}
        _Map1 ("Map 1", 2D) = "white" {}
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
        Tags { "Queue" = "Geometry-100" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "False" "TerrainCompatible" = "True"}
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

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
                float3 normal : NORMAL;

                float4 uv01 : TEXCOORD0;
                float4 uv23 : TEXCOORD1;
                float4 uv45 : TEXCOORD2;
                float4 uv67 : TEXCOORD3;

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
            
            fixed4 GetTextureIndex(sampler2D map, float2 uv, float mixScale = 0, float mixPower = 0)
            {
                fixed4 code = tex2Dlod(map, float4(uv.xy, 0, 0));
                fixed4 codeRet;

                if (mixScale == 0 && mixPower == 0) {
                    codeRet = code;
                }
                else {
                    fixed4 codeAround = GetCodeAround(map, uv, mixScale);
                    codeRet = code + codeAround * mixPower;
                }

                fixed4 index = {clamp(codeRet.r,0,1), clamp(codeRet.g,0,1), clamp(codeRet.b,0,1), clamp(codeRet.a,0,1)};
                return index;
            }
            
            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv01 = float4(v.uv0.x * _MainTex0_ST.x + _MainTex0_ST.z, v.uv0.y * _MainTex0_ST.y + _MainTex0_ST.w,
                                v.uv1.x * _MainTex1_ST.x + _MainTex1_ST.z, v.uv1.y * _MainTex1_ST.y + _MainTex1_ST.w);

                o.uv23 = float4(v.uv2.x * _MainTex2_ST.x + _MainTex2_ST.z, v.uv2.y * _MainTex2_ST.y + _MainTex2_ST.w,
                                v.uv3.x * _MainTex3_ST.x + _MainTex3_ST.z, v.uv3.y * _MainTex3_ST.y + _MainTex3_ST.w);

                o.uv45 = float4(v.uv4.x * _MainTex4_ST.x + _MainTex4_ST.z, v.uv4.y * _MainTex4_ST.y + _MainTex4_ST.w,
                                v.uv5.x * _MainTex5_ST.x + _MainTex5_ST.z, v.uv5.y * _MainTex5_ST.y + _MainTex5_ST.w);

                o.uv67 = float4(v.uv6.x * _MainTex6_ST.x + _MainTex6_ST.z, v.uv6.y * _MainTex6_ST.y + _MainTex6_ST.w,
                                v.uv7.x * _MainTex7_ST.x + _MainTex7_ST.z, v.uv7.y * _MainTex7_ST.y + _MainTex7_ST.w);

                UNITY_TRANSFER_FOG(o,o.vertex);

                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // get value in map
                float4 map0 = tex2D(_Map0, i.vertex.xz);
                float4 map1 = tex2D(_Map1, i.vertex.xz);

                // sample the texture
                fixed4 col;
                col = lerp(fixed4(0,0,0,1), tex2D(_MainTex0, i.uv01.xy), map0.x);
                col = lerp(col, tex2D(_MainTex1, i.uv01.wz), map0.y);
                col = lerp(col, tex2D(_MainTex2, i.uv23.xy), map0.z);
                col = lerp(col, tex2D(_MainTex3, i.uv23.wz), map0.w);
                col = lerp(col, tex2D(_MainTex4, i.uv45.xy), map1.x);
                col = lerp(col, tex2D(_MainTex5, i.uv45.wz), map1.y);
                col = lerp(col, tex2D(_MainTex6, i.uv67.xy), map1.z);
                col = lerp(col, tex2D(_MainTex7, i.uv67.wz), map1.w);

                col = col * (dot(_WorldSpaceLightPos0, i.normal) * 0.6 + 0.4);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

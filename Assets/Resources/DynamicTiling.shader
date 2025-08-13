Shader "Custom/DynamicTiling"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset][Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", Range(0, 2)) = 1.0
        
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
        _Scale ("Tiling Scale", Range(0.1, 10)) = 1.0
        
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _Transparency ("Transparency", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent-100"
            "IgnoreProjector"="True"
            "ForceNoShadowCasting"="True"
        }
        LOD 200

        // Проход 1: Запись глубины для непрозрачных частей
        Pass
        {
            ZWrite On
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float2 _Offset;
            float _Scale;
            float _Transparency;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            float2 GetProjectedUV(float3 worldPos, float3 worldNormal)
            {
                float3 absNormal = abs(worldNormal);
                float2 uv = absNormal.x > absNormal.y 
                    ? (absNormal.x > absNormal.z ? worldPos.yz : worldPos.xy)
                    : (absNormal.y > absNormal.z ? worldPos.xz : worldPos.xy);
                return uv * _Scale + _Offset;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                float2 uv = GetProjectedUV(i.worldPos, i.worldNormal);
                return 0;
            }
            ENDCG
        }

        // Проход 2: Основной рендеринг с прозрачностью
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _BumpScale;
        half2 _Offset;
        half _Scale;
        half _Metallic;
        half _Glossiness;
        fixed4 _Color;
        half _Transparency;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        float2 GetProjectedUV(float3 worldPos, float3 worldNormal)
        {
            float3 absNormal = abs(worldNormal);
            float2 uv = absNormal.x > absNormal.y 
                ? (absNormal.x > absNormal.z ? worldPos.yz : worldPos.xy)
                : (absNormal.y > absNormal.z ? worldPos.xz : worldPos.xy);
            return uv * _Scale + _Offset;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = GetProjectedUV(IN.worldPos, WorldNormalVector(IN, float3(0,0,1)));
            
            fixed4 texColor = tex2D(_MainTex, uv) * _Color;
            half4 normalSample = tex2D(_BumpMap, uv);
            
            o.Albedo = texColor.rgb;
            o.Normal = UnpackScaleNormal(normalSample, _BumpScale);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = texColor.a * _Transparency;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}

#define M_PI 3.14159265358979323846
#define degToRad (M_PI * 2.0) / 360.0

float rand(float2 co)
{
    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}
float rand(float2 co, float l)
{
    return rand(float2(rand(co), l));
}
float rand(float2 co, float l, float t)
{
    return float2(float2(rand(co, l), t));
}

float perlin(float2 p, float dim, float time)
{
    float2 pos = floor(p * dim);
    float2 posx = pos + float2(1.0, 0.0);
    float2 posy = pos + float2(0.0, 1.0);
    float2 posxy = pos + float2(1.0, 1.0);

    float c = rand(pos, dim, time);
    float cx = rand(posx, dim, time);
    float cy = rand(posy, dim, time);
    float cxy = rand(posxy, dim, time);

    float2 d = frac(p * dim);
    d = -0.5 * cos(d * M_PI) + 0.5;

    float ccx = lerp(c, cx, d.x);
    float cycxy = lerp(cy, cxy, d.x);
    float center = lerp(ccx, cycxy, d.y);

    return center * 2.0 - 1.0;
}
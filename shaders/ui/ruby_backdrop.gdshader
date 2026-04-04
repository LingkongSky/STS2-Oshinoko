shader_type canvas_item;

uniform vec4 top_color : source_color = vec4(0.20, 0.05, 0.30, 1.0);
uniform vec4 bottom_color : source_color = vec4(0.02, 0.00, 0.05, 1.0);
uniform vec4 bokeh_color : source_color = vec4(1.0, 0.40, 0.80, 1.0);
uniform float bokeh_intensity : hint_range(0.0, 2.0) = 0.75;

float hash11(float p) {
	p = fract(p * 0.1031);
	p *= p + 33.33;
	p *= p + p;
	return fract(p);
}

vec2 hash21(float p) {
	return vec2(hash11(p + 1.0), hash11(p + 2.0));
}

float soft_circle(vec2 uv, vec2 center, float radius, float feather) {
	float d = length(uv - center);
	return smoothstep(radius, radius - feather, d);
}

void fragment() {
	vec2 uv = UV;

	// Base gradient (slightly skewed for depth).
	float gy = clamp(uv.y * 0.95 + uv.x * 0.05, 0.0, 1.0);
	vec3 col = mix(top_color.rgb, bottom_color.rgb, gy);

	// Vignette.
	vec2 p = uv * 2.0 - 1.0;
	float vig = smoothstep(1.15, 0.25, dot(p, p));
	col *= mix(0.75, 1.0, vig);

	// Subtle film grain.
	float g = hash11(dot(uv, vec2(12.9898, 78.233)) + TIME * 10.0);
	col += (g - 0.5) * 0.03;

	// Bokeh blobs (fixed-count for performance).
	vec3 b = vec3(0.0);
	for (int i = 0; i < 8; i++) {
		float fi = float(i);
		vec2 base = hash21(fi * 19.13);
		vec2 drift = vec2(sin(TIME * (0.12 + fi * 0.02)), cos(TIME * (0.10 + fi * 0.015))) * 0.02;
		vec2 c = fract(base + drift);

		float r = mix(0.06, 0.14, hash11(fi * 7.77));
		float a = soft_circle(uv, c, r, 0.03) * mix(0.08, 0.22, hash11(fi * 11.11));
		b += bokeh_color.rgb * a;
	}

	col += b * bokeh_intensity;

	COLOR = vec4(col, 1.0);
}


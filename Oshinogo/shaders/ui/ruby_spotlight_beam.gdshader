shader_type canvas_item;
render_mode blend_add;

uniform vec4 beam_color : source_color = vec4(1.0, 0.55, 0.90, 1.0);
uniform float intensity : hint_range(0.0, 3.0) = 1.0;

float hash11(float p) {
	p = fract(p * 0.1031);
	p *= p + 33.33;
	p *= p + p;
	return fract(p);
}

void fragment() {
	vec2 uv = UV;

	// Beam is narrow at top and wide at bottom.
	float w = mix(0.03, 0.60, uv.y);
	float edge = smoothstep(w, w - 0.08, abs(uv.x - 0.5));

	// Stronger near the origin, softer near the bottom.
	float falloff = pow(1.0 - uv.y, 0.35);

	// Flicker.
	float f = 0.85 + 0.15 * sin(TIME * 7.0 + uv.y * 6.0) + 0.05 * (hash11(floor(TIME * 12.0)) - 0.5);

	float a = edge * falloff * f * intensity;
	vec3 col = beam_color.rgb * a;

	COLOR = vec4(col, a);
}


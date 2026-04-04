shader_type canvas_item;

uniform vec4 floor_top : source_color = vec4(0.20, 0.02, 0.12, 1.0);
uniform vec4 floor_bottom : source_color = vec4(0.02, 0.00, 0.05, 1.0);
uniform vec4 shine_color : source_color = vec4(1.0, 0.35, 0.80, 1.0);
uniform float shine_intensity : hint_range(0.0, 2.0) = 0.55;

float hash11(float p) {
	p = fract(p * 0.1031);
	p *= p + 33.33;
	p *= p + p;
	return fract(p);
}

void fragment() {
	vec2 uv = UV;

	vec3 col = mix(floor_top.rgb, floor_bottom.rgb, smoothstep(0.0, 1.0, uv.y));

	// Perspective-ish planks/lines.
	float stripes = sin((uv.x + uv.y * 0.35) * 70.0) * 0.5 + 0.5;
	stripes = smoothstep(0.45, 0.55, stripes);
	col *= mix(0.92, 1.04, stripes * 0.35);

	// Central shine band.
	float band = exp(-pow((uv.x - 0.5) * 2.5, 2.0)) * smoothstep(0.15, 0.75, uv.y);
	col += shine_color.rgb * band * shine_intensity * 0.25;

	// Sparkly specks.
	float s = hash11(floor(uv.x * 220.0) + floor(uv.y * 120.0) * 71.0 + floor(TIME * 6.0) * 997.0);
	col += shine_color.rgb * step(0.995, s) * 0.35;

	COLOR = vec4(col, 1.0);
}


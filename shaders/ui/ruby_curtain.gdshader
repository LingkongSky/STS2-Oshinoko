shader_type canvas_item;

uniform vec4 base_color : source_color = vec4(0.35, 0.03, 0.18, 1.0);
uniform vec4 highlight_color : source_color = vec4(0.95, 0.35, 0.75, 1.0);
uniform float fold_strength : hint_range(0.0, 1.0) = 0.55;
uniform float speed : hint_range(0.0, 2.0) = 0.25;
uniform float shade : hint_range(0.0, 1.0) = 0.35;

void fragment() {
	vec2 uv = UV;

	// Vertical gradient (darker towards bottom).
	float v = smoothstep(0.0, 1.0, uv.y);

	// Curtain folds.
	float folds = sin((uv.x * 14.0 + TIME * speed) * 6.2831853) * 0.5 + 0.5;
	folds = pow(folds, 1.6);

	vec3 col = mix(base_color.rgb, highlight_color.rgb, folds * fold_strength);
	col *= mix(1.0, 1.0 - shade, v);

	// Side edge fade for softness.
	float edge = smoothstep(0.0, 0.05, uv.x) * smoothstep(1.0, 0.95, uv.x);
	col *= mix(0.6, 1.0, edge);

	COLOR = vec4(col, base_color.a);
}


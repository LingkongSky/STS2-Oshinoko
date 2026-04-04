shader_type canvas_item;
render_mode blend_add;

uniform vec4 glow_color : source_color = vec4(1.0, 0.40, 0.85, 1.0);
uniform float intensity : hint_range(0.0, 3.0) = 1.0;

void fragment() {
	vec2 p = UV * 2.0 - 1.0;
	float d = length(p);
	float a = smoothstep(1.0, 0.0, d);
	a = pow(a, 2.0) * intensity;
	COLOR = vec4(glow_color.rgb * a, a);
}


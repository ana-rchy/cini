shader_type canvas_item;

uniform vec3 color : source_color;

void fragment() {
	COLOR = texture(TEXTURE, UV);
	COLOR.rgb = vec3(1,1,1) - (vec3(1,1,1) - COLOR.rgb) * (vec3(1,1,1) - color);
}

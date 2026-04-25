shader_type canvas_item;

uniform float border_radius = 0.0;
uniform float spawn_radius = 0.0;
uniform vec2 camera_pos = vec2(0.0,0.0);
uniform vec2 camera_zoom = vec2(1.0,1.0);

void fragment() {
	vec2 resolution = vec2(1.0,1.0)/SCREEN_PIXEL_SIZE;
	vec2 position = UV * resolution - resolution * 0.5;
	
	position += camera_pos;
	position /= camera_zoom;
	
	float pos_length = length(position); 
	
	float outside = sign(border_radius - pos_length) * 0.5 + 0.5;
	
	float inside = sign(pos_length-spawn_radius) * 0.5 + 0.5;
	
	float value = min(outside, inside) * 0.1 + 0.1;
	
	COLOR = vec4(value, value, value, 1.0);
}
